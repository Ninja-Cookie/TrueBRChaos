using Json.Net;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrueBRChaos;
using TrueBRChaos.Events;
using static TwitchChaos.TwitchWebReply;
using static TwitchChaos.TwitchWebReply.SocketReply.Payload.PollEvent;
using static TwitchChaos.TwitchWebSocket;

namespace TwitchChaos
{
    internal static class Twitch
    {
        private static string ActivePollID = string.Empty;

        [Serializable]
        private class Poll
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

        internal static void StartWebSocket(string clientID, string OAuth)
        {
            if (CurrentSocketState == SocketState.Disconnected)
            {
                OnConnectedToSocket += WaitForGame;
                OnPollEnd           += HandlePollResult;
                ConnectToWebSocket(clientID, OAuth);
            }
        }

        internal static void EndWebSocket()
        {
            DisconnectFromWebSocket();
        }

        private async static void WaitForGame()
        {
            OnConnectedToSocket -= WaitForGame;

            while (!TwitchControl.EventCanBeCreated) { await Task.Delay(TimeSpan.FromSeconds(0.1f)); }
            if (CurrentSocketState == SocketState.Connected)
                RunVote();
        }

        private async static void RunVote()
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

        private async static Task StartPoll(Poll poll)
        {
            if (CurrentSocketState != SocketState.Connected || CurrentTwitchUserInfo == null)
                return;

            var body = new
            {
                broadcaster_id = CurrentTwitchUserInfo.id,
                title       = poll.Title,
                choices     = poll.Options,
                duration    = poll.Duration
            };

            if (body.choices == null || body.choices.Length == 0)
                return;

            string reply = await SendWebRequest(URL_API_POLL, RequestType.POST, JsonNet.Serialize(body));

            if (!string.IsNullOrEmpty(reply))
            {
                API_PollStart_Reply pollReply = JsonNet.Deserialize<API_PollStart_Reply>(reply);
                if (pollReply?.data?.FirstOrDefault()?.id != null)
                    ActivePollID = pollReply.data.FirstOrDefault().id;
            }
        }

        private async static void HandlePollResult(string id, string winner)
        {
            bool isOurPoll = !string.IsNullOrEmpty(ActivePollID) && id == ActivePollID;
            ActivePollID = string.Empty;

            if (isOurPoll)
            {
                ChaosEvent chaosEvent = TwitchControl.ChaosEvents.FirstOrDefault(x => x.EventName == winner);
                if (!(chaosEvent is null))
                    await TwitchControl.WaitAndCreateEvent(chaosEvent);

                RunVote();
            }
        }
    }
}
