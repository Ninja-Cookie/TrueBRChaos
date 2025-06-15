using BepInEx;
using HarmonyLib;
using Reptile;
using System.Collections;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.SceneManagement;

namespace TrueBRChaos
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid      = "ninjacookie.brc.truebrchaos";
        public const string pluginName      = "TrueBRChaos";
        public const string pluginVersion   = "0.1.0";

        public static bool DebugMode = false;

        public void Awake()
        {
            

            var harmony = new Harmony(pluginGuid);
            harmony.PatchAll();

            if (DebugMode)
            {
                GameObject assetAssistant = new GameObject("AssetAssistant", typeof(TestUI));
                DontDestroyOnLoad(assetAssistant);
            }

            LoadResources();
            Core.OnCoreInitialized          += LoadChaos;
            SceneManager.activeSceneChanged += SetLogo;
        }

        private void LoadChaos()
        {
            Core.OnCoreInitialized -= LoadChaos;
            ChaosAssetHandler   .LoadAssetBundle();
            ChaosManager        .Init();
        }

        private void LoadResources()
        {
            ChaosAudioHandler   .LoadAllResourceAudio();
            ChaosMaterialHandler.LoadAllResourceMaterials();
        }

        private void SetLogo(Scene previousScene, Scene activeScene)
        {
            /*
            if (activeScene.name == "mainMenu")
                StartCoroutine(WaitAndSetLogo());
            */
        }

        private IEnumerator WaitAndSetLogo()
        {
            while (true)
            {
                /*
                GameObject logoObject = FindObjectsOfType<GameObject>().FirstOrDefault(x => x.name == "GraffitiMiddleImage");
                if (logoObject != null)
                {
                    Texture2D texture = ChaosMaterialHandler.GetMaterial(Properties.Resources.logo).mainTexture as Texture2D;
                    logoObject.GetComponent<UnityEngine.UI.Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    break;
                }
                */
                yield return null;
            }
        }

        public void Update()
        {
            if (DebugMode)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
                    RunTest();

                if (UnityEngine.Input.GetKeyDown(KeyCode.F3))
                {
                    foreach (var chaosEvent in ChaosManager.ActiveEvents)
                        ChaosManager.ForceEndEvent(chaosEvent);
                }

                if (UnityEngine.Input.GetKeyDown(KeyCode.F4))
                {
                    TestUI.openGUI = !TestUI.openGUI;
                }

                if (UnityEngine.Input.GetKeyDown(KeyCode.F5))
                {
                    ChaosAssetHandler.DebugGetGameObjects();
                }
            }
        }

        private void RunTest()
        {
            ChaosManager.CreateChaosEvent(typeof(Events.Event_Freeze));
        }
    }
}
