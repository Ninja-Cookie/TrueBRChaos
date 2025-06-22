using BepInEx;
using HarmonyLib;
using Reptile;

namespace TrueBRChaos
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    internal partial class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid      = "ninjacookie.brc.truebrchaos";
        public const string pluginName      = "TrueBRChaos";
        public const string pluginVersion   = "0.1.0";

        public static bool DebugMode = true;

        public void Awake()
        {
            var harmony = new Harmony(pluginGuid);
            harmony.PatchAll();

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
