using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class GameManager : MonoBehaviour
{
    // constants for this class
    private const float TIME_FOR_BUTTON_PRESS = 0.5f;

    [Header("Controller data for changing controller method")]
    [SerializeField] GameObject leftRayController;
    [SerializeField] GameObject leftDirectController;
    [SerializeField] GameObject rightRayController;
    [SerializeField] GameObject rightDirectController;

    // private variables used by this script
    private float pressTimer = 0;
    private bool buttonPressed;
    private bool directControllersActive;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Update is called once per frame to check if the user wants to change the controller method
    /// </summary>
    void Update()
    {
        // only allow a button press after a little while from the last one (a kluge fix for now)
        if (!buttonPressed && (pressTimer > TIME_FOR_BUTTON_PRESS))
        {
            // reset the timer
            pressTimer = 0;
            buttonPressed = true;

            // switch input method based on the right hand primary button
            bool switchInputMethod = false;

            if (rightRayController.activeSelf)
            {
                rightRayController.GetComponent<XRController>().inputDevice.IsPressed(InputHelpers.Button.PrimaryButton, out switchInputMethod);
            }
            else if (rightDirectController.activeSelf)
            {
                rightDirectController.GetComponent<XRController>().inputDevice.IsPressed(InputHelpers.Button.PrimaryButton, out switchInputMethod);
            }

            // check to see if the change controller button was pressed
            if (switchInputMethod)
            {
                if (directControllersActive)
                {
                    leftRayController.SetActive(true);
                    leftDirectController.SetActive(false);
                    rightRayController.SetActive(true);
                    rightDirectController.SetActive(false);
                    directControllersActive = false;
                }
                else
                {
                    leftRayController.SetActive(false);
                    leftDirectController.SetActive(true);
                    rightRayController.SetActive(false);
                    rightDirectController.SetActive(true);
                    directControllersActive = true;
                }
            }
        }
        else
        {
            buttonPressed = false;
            pressTimer += Time.deltaTime;
        }
        
    } // end Update
}
