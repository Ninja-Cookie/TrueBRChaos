using Reptile;
using Reptile.Phone;
using Rewired;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace TrueBRChaos
{
    internal partial class Plugin
    {
        public void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
                RunTest();

            if (DebugMode)
            {
                //if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
                //    RunTest();

                if (UnityEngine.Input.GetKeyDown(KeyCode.F3))
                {
                    foreach (var chaosEvent in ChaosManager.ActiveEvents)
                        ChaosManager.ForceEndEvent(chaosEvent);
                }

                if (UnityEngine.Input.GetKeyDown(KeyCode.F4))
                {
                    //TestUI.openGUI = !TestUI.openGUI;
                    //ChaosManager.CreateChaosEvent(typeof(Events.Event_FlingProps));
                }

                if (UnityEngine.Input.GetKeyDown(KeyCode.F5))
                {
                    ChaosAssetHandler.DebugGetGameObjects();
                }
            }
        }

        private void RunTest()
        {
            TwitchControl.SetTwitchControl(!TwitchControl.HasTwitchControl);
            //ChaosManager.CreateChaosEvent(typeof(Events.Event_RandomCharacter));
        }
    }
}
