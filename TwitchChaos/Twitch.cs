using Json.Net;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading.Tasks;
using TrueBRChaos;
using TrueBRChaos.Events;
using UnityEngine;
using static TwitchChaos.DebugOutput;
using static TwitchChaos.Twitch;

namespace TwitchChaos
{
    internal static partial class Twitch
    {
        private static readonly Uri     URI_EventSub    = new Uri("wss://eventsub.wss.twitch.tv/ws");
        private const string            API_User        = "https://api.twitch.tv/helix/users";
        private const string            API_Sub         = "https://api.twitch.tv/helix/eventsub/subscriptions";
        private const string            API_Poll        = "https://api.twitch.tv/helix/polls";

        private class EventSubs
        {
            internal const string OnPollBegin       = "channel.poll.begin";
            internal const string OnPollProgress    = "channel.poll.progress";
            internal const string OnPollEnd         = "channel.poll.end";
        }

        private static UserData CurrentUserData     { get; set; } = null;
        private static Session  CurrentSessionData  { get; set; } = null;

        [Serializable]
        internal class Poll
        {
            internal string     Title       { get; }
            internal int        Duration    { get; }
            internal Choices[]  Options     { get; }

            internal Poll(string title, int duration, params string[] options)
            {
                this.Title      = LimitTitle(title);
                this.Duration   = UnityEngine.Mathf.Clamp(duration, 15, 1800);

                Choices[] choices = new Choices[options.Length];
                for (int i = 0; i < choices.Length; i++)
                {
                    choices[i] = new Choices() { title = LimitTitle(options[i]) };
                }

                this.Options = choices ?? Array.Empty<Choices>();
            }

            private string LimitTitle(string title)
            {
                if (string.IsNullOrEmpty(title) || title.Length == 0)
                    return "<Empty Name>";

                return title.Length > 60 ? title.Substring(0, 60) : title;
            }
        }

        internal static void Init(string clientID, string OAuth)
        {
            if (CurrentSocketStatus != SocketStatus.Disconnected)
                return;

            OnPollEnd           += HandlePollResult;
            OnWebhookConnected  += WaitForGame;
            StartWebSocket(clientID, OAuth);
        }

        private async static void WaitForGame()
        {
            OnWebhookConnected -= WaitForGame;

            while (!TwitchControl.EventCanBeCreated) { await Task.Delay(TimeSpan.FromSeconds(0.1f)); }
            if (CurrentSocketStatus == SocketStatus.Connected) RunVote(); else OnWebhookConnected += WaitForGame;
        }

        internal async static void RunVote()
        {
            string[] choices = new string[5];

            for (int i = 0; i < choices.Length; i++)
            {
                ChaosEvent[] possibleEvents = TwitchControl.ChaosEvents.Where(x => !choices.Contains(x.EventName) && !TwitchControl.ActiveChaosEvents.Contains(x)).ToArray();
                if (possibleEvents.Length == 0)
                    return;

                ChaosEvent chaosEvent = possibleEvents[UnityEngine.Random.Range(0, possibleEvents.Length)];
                choices[i] = chaosEvent.EventName;
            }

            Poll poll = new Poll
            (
                "Next Chaos Event",
                15,
                choices
            );

            await StartPoll(poll);
        }

        internal async static Task StartPoll(Poll poll)
        {
            if (CurrentSocketStatus != SocketStatus.Connected)
                return;

            var body = new
            {
                broadcaster_id  = CurrentUserData.id,
                title           = poll.Title,
                choices         = poll.Options,
                duration        = poll.Duration
            };

            if (body.choices == null || body.choices.Length == 0)
                return;

            await SendWebRequest(API_Poll, ClientID, App_OAuth, RequestType.POST, JsonNet.Serialize(body));
        }

        private async static void HandlePollResult(string winner)
        {
            // Verify this is the poll created by chaos
            Debug($"The winner was: {winner}", DebugType.Error);
            ChaosEvent chaosEvent = TwitchControl.ChaosEvents.FirstOrDefault(x => x.EventName == winner);
            if (!(chaosEvent is null))
                await TwitchControl.WaitAndCreateEvent(chaosEvent);

            RunVote();
        }
    }
}
