using CellexalVR.General;
using System.Collections.Generic;
using System.Linq;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Vuplex.WebView;

namespace CellexalVR.Interaction
{
    /// <summary>
    /// Keyboard for web browser. Enter key sends output to navigate function in web browser.
    /// </summary>
    public class WebManager : MonoBehaviour
    {
        public const string default_url = "https://mdv.molbiol.ox.ac.uk/projects";
        //"https://datashare.molbiol.ox.ac.uk/public/project/Wellcome_Discovery/sergeant/pbmc1k";
        [Header("The prefab for all browser window instances")]
        [SerializeField] GameObject browserWindowPrefab;
        [SerializeField] GameObject popoutWindowPrefab;

        public TMPro.TextMeshPro output;
        public bool isVisible;
        public ReferenceManager referenceManager;

        private Dictionary<int, GameObject> browserWindows;
        private int lastBrowserID = 0;

        private void OnValidate()
        {
            if (gameObject.scene.IsValid())
            {
                referenceManager = GameObject.Find("InputReader").GetComponent<ReferenceManager>();
            }
        }

        // Use this for initialization
        void Start()
        {
            // Use a desktop User-Agent to request the desktop versions of websites.
            // https://developer.vuplex.com/webview/Web#SetUserAgent
            Web.SetUserAgent(false);

            //SetVisible(false);

            // set up a list to store the browser windows
            // TODO - will need to have the browsers remove themselves with a method
            browserWindows = new Dictionary<int, GameObject>();
        }

        /// <summary>
        /// Creates a new browser window relative to the window creating it
        /// </summary>
        public GameObject CreateNewWindow(Transform browserTransform, string url)
        {
            Vector3 newPos = browserTransform.position;
            newPos.z -= 0.01f;
            GameObject newWindow = Instantiate(browserWindowPrefab, newPos, browserTransform.rotation);

            // set up the initial url to see if it works without loading
            newWindow.GetNamedChild("CanvasMainWindow").GetComponent<CanvasWebViewPrefab>().InitialUrl = url;

            // need to set the camera of the canvas object for this window
            newWindow.GetComponent<Canvas>().worldCamera = Camera.main;

            newWindow.GetComponent<FullCanvasWebBrowserManager>().browserID = lastBrowserID;
            browserWindows.Add(lastBrowserID++, newWindow);
            return newWindow;

        } // end CreateNewWindow

        /// <summary>
        /// Creates a new browser window relative to the window creating it
        /// </summary>
        public GameObject CreatePopOutWindow(Transform browserTransform, IWebView webView)
        {
            // set the position to be just in front of the last main window
            Vector3 newPos = browserTransform.position;
            newPos.z -= 0.01f;
            GameObject newWindow = Instantiate(popoutWindowPrefab, newPos, browserTransform.rotation);

            // need to set the camera of the canvas object for this window
            newWindow.GetComponent<Canvas>().worldCamera = Camera.main;

            // set up the pop out to look back at the previous web view
            CanvasWebViewPrefab popupPrefab =
                newWindow.GetNamedChild("CanvasMainWindow").GetComponent<CanvasWebViewPrefab>();
            popupPrefab.SetWebViewForInitialization(webView);

            // store the object so it can be set to invisible if the browser is turned off
            newWindow.GetComponent<FullCanvasWebBrowserManager>().browserID = lastBrowserID;
            browserWindows.Add(lastBrowserID++, newWindow);
            return newWindow;

        } // end CreateNewWindow

        /// <summary>
        /// Removes the browser window from the list of game objects being tracked by the browser manager
        /// </summary>
        /// <param name="browserWindowToRemove">The browser window to remove from the scene</param>
        public void RemoveBrowserWindowFromScene(GameObject browserWindowToRemove)
        {
            // remove the game object from browser list
            browserWindows.Remove(browserWindowToRemove.GetComponent<FullCanvasWebBrowserManager>().browserID);

            // Destroy the parent browser object which will destroy all the child objects as well
            Destroy(browserWindowToRemove);

        } // end RemoveBrowserWindowFromScene

        // TODO: Update this to use a different output text and use 3D Web View keyboard
        public void EnterKey()
        {
            print("Navigate to - " + output.text);
            // If url field does not contain '.' then may not be a url so google the output instead
            if (!output.text.Contains('.'))
            {
                output.text = "www.google.com/search?q=" + output.text;
            }
            //webBrowser.OnNavigate(output.text);
            referenceManager.multiuserMessageSender.SendMessageBrowserEnter();
        }

        public void SetBrowserActive(bool active)
        {
            if (active && (browserWindows.Count <= 0))
            {
                // create a new window here
                CreateNewWindow(gameObject.transform, default_url);
                //GetComponentInChildren<ReportListGenerator>().GenerateList();
            }

            //SetVisible(active);

        } // end SetBrowserActive

        public void SetVisible(bool visible)
        {
            /*foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.enabled = visible;
            }
            foreach (Collider c in GetComponentsInChildren<Collider>())
            {
                c.enabled = visible;
            }*/

            isVisible = visible;
        }

    }
}