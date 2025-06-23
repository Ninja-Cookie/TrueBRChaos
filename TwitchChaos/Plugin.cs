using BepInEx;
using BepInEx.Configuration;
using System.IO;
using TrueBRChaos;

namespace TwitchChaos
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    internal partial class Plugin : BaseUnityPlugin
    {
        public const string pluginGuid      = "ninjacookie.brc.twitchchaos";
        public const string pluginName      = "TwitchChaos";
        public const string pluginVersion   = "0.0.1";

        private const string    DefaultClient   = "zdx1ezslw56t22nng56jyezslgbzqo";
        private string          ClientAppID     = string.Empty;
        private string          OAuthID         = string.Empty;

        private const string ConfigSection      = "Authentication";
        private const string ConfigClientKey    = "Client App ID";
        private const string ConfigOAuthKey     = "OAuth Code";

        private static readonly string  ConfigFilePath  = Path.Combine(BepInEx.Paths.ConfigPath, "BRChaos", "Twitch.cfg");
        private readonly ConfigFile     ConfigFile      = new ConfigFile(ConfigFilePath, true);

        public void Awake()
        {
            ConfigFile.Bind(ConfigSection, ConfigClientKey, DefaultClient,  "ID for the Twitch application");
            ConfigFile.Bind(ConfigSection, ConfigOAuthKey,  string.Empty,   "Authentication code for the app on your Twitch client (Do Not Share)");

            if (ConfigFile.TryGetEntry<string>(ConfigSection, ConfigClientKey, out var clientEntry))
                ClientAppID = clientEntry.Value;

            if (ConfigFile.TryGetEntry<string>(ConfigSection, ConfigOAuthKey, out var OAuthEntry))
                OAuthID = OAuthEntry.Value;

            TwitchControl.ControlStatusChanged += OnStatusChanged;
            StartTwitchControl();
        }

        private void OnStatusChanged(bool status)
        {
            if (status)
                StartTwitchControl();
            else if (!status)
                Twitch.EndWebSocket();
        }

        private void StartTwitchControl()
        {
            if (!string.IsNullOrEmpty(ClientAppID) && !string.IsNullOrEmpty(OAuthID) && TwitchControl.HasTwitchControl)
                Twitch.StartWebSocket(ClientAppID, OAuthID);
        }

        public void OnDestroy()
        {
            TwitchControl.ControlStatusChanged -= OnStatusChanged;
        }
    }
}
