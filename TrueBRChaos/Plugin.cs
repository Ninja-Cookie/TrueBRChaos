using BepInEx;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace TrueBRChaos
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public partial class Plugin : BaseUnityPlugin
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
            Core.OnCoreInitialized += LoadChaos;
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
    }
}
