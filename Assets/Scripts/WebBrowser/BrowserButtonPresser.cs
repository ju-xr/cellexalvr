using CellexalVR.General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrowserButtonPresser : MonoBehaviour
{
    //public BoxCollider collider;
    //[SerializeField] private GameObject note;
    //[SerializeField] private Color color;

    // Open XR 
    //private ReferenceManager referenceManager;
    //private SteamVR_Controller.Device device;
    //private UnityEngine.XR.Interaction.Toolkit.ActionBasedController rightController;
    //private SteamVR_Controller.Device device;
    //private UnityEngine.XR.InputDevice device;
    private bool controllerInside;
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();

        /*if (!referenceManager)
        {
            referenceManager = GameObject.Find("InputReader").GetComponent<ReferenceManager>();
        }

        rightController = referenceManager.rightController;*/
        //collider.size = new Vector3(70, 30, 1);
        //collider.center = new Vector3(0, -15, 0);

        CellexalEvents.RightTriggerClick.AddListener(OnTriggerPressed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Controller"))
        {
            controllerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Controller"))
        {
            controllerInside = false;
        }
    }

    private void OnTriggerPressed()
    {
        // Open XR
        if (controllerInside)
        {
            button.onClick.Invoke();

        }
    }
}

