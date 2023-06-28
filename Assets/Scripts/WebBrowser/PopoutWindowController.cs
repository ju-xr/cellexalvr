using CellexalVR.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellexalVR.General;
using UnityEngine.XR.Interaction.Toolkit;

public class PopoutWindowController : FullCanvasWebBrowserPrefab
{
    // This class only uses a small subset of the FullCanvasWebBrowserPrefab
    //  - the controls for clicking on the main window of the popout

    // Start is called before the first frame update
    void Start()
    {
        // grab the reference manager
        referenceManager = GameObject.Find("InputReader").GetComponent<ReferenceManager>();

        // find the web manager script for adding other windows
        webManagerScript = GameObject.Find("WebManager").GetComponent<WebManager>();

        // grab the interactable script from this prefab for sending messages to other clients on position
        interactable = GetComponent<XRGrabInteractable>();

        // testing to see if I can get input to work manually
        CellexalEvents.RightTriggerClick.AddListener(OnTriggerClick);
        CellexalEvents.RightTriggerUp.AddListener(OnTriggerUp);

    }
}
