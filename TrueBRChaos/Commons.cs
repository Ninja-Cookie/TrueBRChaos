using UnityEngine;
using Reptile;
using Reptile.Phone;
using System.Collections.Generic;
using System.Linq;

namespace TrueBRChaos
{
    public static class Commons
    {
        public static float Delta       => Time.deltaTime / Time.timeScale;
        public static float DeltaFixed  => Time.fixedDeltaTime / Time.timeScale;

        public static bool ForceChaosDisabled = false;
        public static bool ChaosShouldRun
        {
            get
            {
                if (!ForceChaosDisabled && Core != null && BaseModule != null && StageManager != null && Player != null)
                {
                    bool flag =
                    Core.IsCorePaused                       ||
                    BaseModule.IsInGamePaused               ||
                    BaseModule.IsLoading                    ||
                    StageManager.IsExtendingLoadingScreen   ||
                    BaseModule.CurrentStage == Stage.NONE   ||
                    Player.IsDead();

                    return !flag;
                }
                return false;
            }
        }

        public static bool PlayerInSequence()
        {
            return Player != null ? !Player.isActiveAndEnabled || Player.GetValue<SequenceState>("sequenceState") != SequenceState.NONE : true;
        }

        public static void BetterSetOutfit(int outfit)
        {
            Player player = Player;
            CharacterVisual characterVisual = player.GetValue<CharacterVisual>("characterVisual");

            if (player != null && !PlayerInSequence() && characterVisual != null && characterVisual.GetComponentInChildren<SkinnedMeshRenderer>() != null)
                player.SetOutfit(outfit);
        }

    // Core
    // ---------------------
        public static Core                  Core                    => Core.Instance;
        public static UIManager             UIManager               => Core?.UIManager;
        public static GameInput             GameInput               => Core?.GameInput;
        public static GameVersion           GameVersion             => Core?.GameVersion;
        public static Reptile.Logger        Logger                  => Core?.Logger;
        public static AudioManager          AudioManager            => Core?.AudioManager;
        public static Assets                Assets                  => Core?.Assets;

        // BaseModule
        public static BaseModule            BaseModule              => Core?.BaseModule;
        public static StageManager          StageManager            => BaseModule?.StageManager;

        // SaveManager
        public static SaveManager           SaveManager             => Core?.SaveManager;
        public static PlayerStats           PlayerStats             => SaveManager?.PlayerStats;
        public static AchievementsCache     AchievementsCache       => SaveManager?.AchievementsCache;
        public static SettingsData          Settings                => SaveManager?.Settings;
        public static SaveSlotData          CurrentSaveSlot         => SaveManager?.CurrentSaveSlot;

        // Platform
        public static APlatform             Platform                => Core?.Platform;
        public static Achievements          Achievements            => Platform?.Achievements;
        public static AErrorHandler         ErrorHandler            => Platform?.ErrorHandler;
        public static AStorage              Storage                 => Platform?.Storage;
        public static ADownloadableContent  DownloadableContent     => Platform?.DownloadableContent;

        // User
        public static AUser                 User                    => Platform?.User;
        public static VideoSettingsManager  VideoSettingsManager    => User?.VideoSettingsManager;
    // ---------------------


    // WorldHandler
    // ---------------------
        public static WorldHandler          WorldHandler            => WorldHandler.instance;
        public static Camera                CurrentCamera           => WorldHandler?.CurrentCamera;
        public static StoryManager          StoryManager            => WorldHandler?.StoryManager;
        public static SceneObjectsRegister  SceneObjectsRegister    => WorldHandler?.SceneObjectsRegister;
        public static Player                Player                  => WorldHandler?.GetCurrentPlayer();

        // Player
        public static Phone                 Phone                   => Player?.GetValue<Phone>("phone");
        public static GameplayCamera        PlayerCamera            => Player?.GetValue<GameplayCamera>("cam");
        public static GameplayUI            GameplayUI              => Player?.GetValue<GameplayUI>("ui");
        public static Animator              Animator                => Player?.GetValue<Animator>("anim");

        // PlayerCamera
        public static Camera                PlayerCameraCam         => PlayerCamera?.GetValue<Camera>("cam");
    // ---------------------

        // WantedManager
        // ---------------------
        public static WantedManager         WantedManager           => WantedManager.instance;
    // ---------------------

    // Mapcontroller
    // ---------------------
        public static Mapcontroller         Mapcontroller           => Mapcontroller.Instance;
    // ---------------------

    // SequenceHandler
    // ---------------------
        public static SequenceHandler       SequenceHandler         => SequenceHandler.instance;
    // ---------------------

    // PlayerPhoneCameras
    // ---------------------
        public static PlayerPhoneCameras    PlayerPhoneCameras      => PlayerPhoneCameras.Instance;
    // ---------------------
    }
}
