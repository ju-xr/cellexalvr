using CellexalVR.General;
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
        [Header("The prefab for all browser window instances")]
        [SerializeField] GameObject browserWindowPrefab;

        public SimpleWebBrowser.WebBrowser webBrowser;
        public TMPro.TextMeshPro output;
        public bool isVisible;
        public ReferenceManager referenceManager;

        private bool firstActivated;
        private XRGrabInteractable interactable;

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

            SetVisible(false);
            interactable = GetComponent<XRGrabInteractable>();
        }

        /// <summary>
        /// Creates a new browser window relative to the window creating it
        /// </summary>
        public GameObject CreateNewWindow(Transform browserTransform, string url)
        {
            Vector3 newPos = browserTransform.position;
            newPos.z += 0.01f;
            GameObject newWindow = Instantiate(browserWindowPrefab, newPos, browserTransform.rotation);

            // set up the initial url to see if it works without loading
            newWindow.GetNamedChild("CanvasMainWindow").GetComponent<CanvasWebViewPrefab>().InitialUrl = url;

            // need to set the camera of the canvas object for this window
            newWindow.GetComponent<Canvas>().worldCamera = Camera.main;

            return newWindow;

        } // end CreateNewWindow

        private void Update()
        {
            // Open XR
            if (interactable.isSelected)
            {
                referenceManager.multiuserMessageSender.SendMessageMoveBrowser(transform.localPosition, transform.localRotation, transform.localScale);
            }
        }

        public void EnterKey()
        {
            print("Navigate to - " + output.text);
            // If url field does not contain '.' then may not be a url so google the output instead
            if (!output.text.Contains('.'))
            {
                output.text = "www.google.com/search?q=" + output.text;
            }
            webBrowser.OnNavigate(output.text);
            referenceManager.multiuserMessageSender.SendMessageBrowserEnter();
        }

        public void SetBrowserActive(bool active)
        {
            if (!firstActivated && !webBrowser.gameObject.activeInHierarchy)
            {
                webBrowser.gameObject.SetActive(active);
                GetComponentInChildren<ReportListGenerator>().GenerateList();

                firstActivated = true;
            }
            SetVisible(active);
        }

        public void SetVisible(bool visible)
        {
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.enabled = visible;
            }
            foreach (Collider c in GetComponentsInChildren<Collider>())
            {
                c.enabled = visible;
            }
            isVisible = visible;
            webBrowser.enabled = visible;
        }

    }
}