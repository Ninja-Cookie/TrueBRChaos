using Reptile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueBRChaos.Patches;
using UnityEngine;

namespace TrueBRChaos.Events
{
    internal class Event_Death : ChaosEvent
    {
        public override string          EventName           => "Dead";
        public override float           EventTime           => EventTimes.OnTrigger;
        public override EventRarities   EventRarity         => EventRarities.Rare;
        public override bool            EventEndsOnTrigger  => true;
        public override bool            EventStatePass      => !EventStates.AbilityInUse && Commons.Player != null && !Commons.PlayerInSequence();

        Player      player;
        DieAbility  dieAbility;

        private bool eventActive = false;

        public override void OnEventAwake()
        {
            player      = Commons.Player;
            dieAbility  = player?.GetValue<DieAbility>("dieAbility");

            if (player == null || dieAbility == null)
            {
                Kill();
                return;
            }

            eventActive = true;
        }

        public override void OnEventStart()
        {
            if (eventActive)
            {
                EventPatch_Death.event_death = true;
                player.ActivateAbility(dieAbility);

                if (player.isActiveAndEnabled)
                {
                    player.StartCoroutine(UpdateDeath());
                }
                else
                {
                    player.StopCurrentAbility();
                    Kill();
                }
            }
        }

        public override void OnEventKill()
        {
            EventPatch_Death.event_death = false;
            eventActive = false;
        }

        private IEnumerator UpdateDeath()
        {
            float timer = 0f;

            while (timer <= 0.5f && eventActive)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (eventActive)
            {
                CameraClearFlags    currentCamFlags = default;
                Color               currentEffects  = default;
                int                 currentMask     = 0;
                bool                uiState         = true;

                if (ChaosManager.Random.Range(0, 3) == 1)
                {
                    EventPatch_Death.event_death = false;
                    dieAbility?.OnStartAbility();
                }
                else
                {
                    Commons.UIManager?  .PopAllMenusInstant();
                    Commons.Phone?      .TurnOff(false);

                    if (Commons.GameplayUI != null)
                    {
                        uiState = Commons.GameplayUI.enabled;
                        Commons.GameplayUI.TurnOn(false);
                    }

                    if (Commons.PlayerCameraCam != null)
                    {
                        currentCamFlags = Commons.PlayerCameraCam.clearFlags;
                        currentEffects  = Commons.PlayerCameraCam.backgroundColor;
                        currentMask     = Commons.PlayerCameraCam.cullingMask;

                        Commons.PlayerCameraCam.clearFlags        = CameraClearFlags.Color;
                        Commons.PlayerCameraCam.backgroundColor   = EffectsUI.niceBlack;
                        Commons.PlayerCameraCam.cullingMask       = 32768;
                    }

                    Commons.BaseModule?.PauseGame(PauseType.GameOver);

                    if (Commons.Animator != null)
                        Commons.Animator.speed = 1f;

                    timer = 0f;
                    while (timer <= 1f && eventActive)
                    {
                        timer += Time.deltaTime;
                        yield return null;
                    }

                    if (eventActive && player != null)
                    {
                        player.StopCurrentAbility();
                        player.ResetHP();
                    }

                    if (Commons.PlayerCameraCam != null)
                    {
                        Commons.PlayerCameraCam.clearFlags = currentCamFlags;
                        Commons.PlayerCameraCam.backgroundColor = currentEffects;
                        Commons.PlayerCameraCam.cullingMask = currentMask;
                    }

                    Commons.GameplayUI?.TurnOn(uiState);
                    Commons.BaseModule?.UnPauseGame(PauseType.GameOver);
                }
            }

            Kill();
        }
    }
}
