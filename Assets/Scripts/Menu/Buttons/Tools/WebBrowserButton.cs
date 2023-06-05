using CellexalVR.General;
using CellexalVR.Interaction;
using System.Diagnostics;
using UnityEngine;

namespace CellexalVR.Menu.Buttons.Tools
{
    /// <summary>
    /// Toggle on/off the web browser tool. This also activates the laser on the right hand so that one can interact with the browser.
    /// </summary>
    public class WebBrowserButton : CellexalToolButton
    {
        private void Start()
        {
            SetButtonActivated(true);
            CellexalEvents.GraphsUnloaded.RemoveListener(TurnOff);
        }

        protected override string Description
        {
            get { return "Toggle Web Browser"; }
        }

        protected override ControllerModelSwitcher.Model ControllerModel
        {
            get { return ControllerModelSwitcher.Model.WebBrowser; }
        }

        public override void Click()
        {
            base.Click();
            referenceManager.multiuserMessageSender.SendMessageActivateBrowser(toolActivated);
            CellexalLog.Log("Web client should start now!");
            //Debug.Log("Web client should start now!");

        }


    }
}