using Json.Net;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static TwitchChaos.DebugOutput;

namespace TwitchChaos
{
    internal static partial class Twitch
    {
        private delegate void PollEnd(string winner);
        private static event PollEnd OnPollEnd;

        private delegate void WebhookConnected();
        private static event WebhookConnected OnWebhookConnected;

        private static SocketStatus _currentSocketStatus = SocketStatus.Disconnected;
        private static SocketStatus CurrentSocketStatus
        {
            get => _currentSocketStatus;
            set
            {
                switch (value)
                {
                    case SocketStatus.Connected:
                        if (_currentSocketStatus != SocketStatus.Connected)
                        {
                            _currentSocketStatus = value;
                            OnWebhookConnected?.Invoke();
                        }
                    break;

                    case SocketStatus.Disconnected:
                        CurrentUserData     = null;
                        OnPollEnd           = null;
                        OnWebhookConnected  = null;
                    break;
                }
                _currentSocketStatus = value;
            }
        }

        private enum SocketStatus
        {
            Disconnected,
            Disconnecting,
            Connecting,
            Connected
        }

        private enum MessageType
        {
            session_welcome,
            notification
        }

        private enum PollStatus
        {
            completed
        }

        private enum RequestType
        {
            POST,
            GET,
            PATCH
        }

        private static string ClientID  { get; set; } = string.Empty;
        private static string App_OAuth { get; set; } = string.Empty;

        internal static void StartWebSocket(string clientID, string OAuth)
        {
            if (CurrentSocketStatus != SocketStatus.Disconnected)
                return;

            CurrentSocketStatus = SocketStatus.Connecting;

            ClientID    = clientID;
            App_OAuth   = OAuth;

            ConnectWebSocket();
        }

        private static async void ConnectWebSocket()
        {
            string response = await SendWebRequest(API_User, ClientID, App_OAuth, RequestType.GET, ignoreSocket: true);
            if (!string.IsNullOrEmpty(response))
            {
                API_Reply_Users broadcaster = Json.Net.JsonNet.Deserialize<API_Reply_Users>(response);
                CurrentUserData = broadcaster?.data?.FirstOrDefault();
            }

            if (CurrentUserData == null)
            {
                CurrentSocketStatus = SocketStatus.Disconnected;
                return;
            }

            using (ClientWebSocket socket = new ClientWebSocket())
            {
                await socket.ConnectAsync(URI_EventSub, CancellationToken.None);
                if (socket.State != WebSocketState.Open)
                {
                    CurrentSocketStatus = SocketStatus.Disconnected;
                    return;
                }

                CurrentSocketStatus = SocketStatus.Connected;
                WebSocketReceiveResult socketReply = null;
                byte[] buffer = new byte[2048];

                while (CurrentSocketStatus == SocketStatus.Connected && socket.State == WebSocketState.Open)
                {
                    // Add cancellation token
                    socketReply = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (socketReply == null)
                        continue;

                    string message = Encoding.UTF8.GetString(buffer, 0, socketReply.Count);
                    if (string.IsNullOrEmpty(message))
                        continue;

                    SocketReply reply = JsonNet.Deserialize<SocketReply>(message);
                    if (reply != null)
                        await HandleReply(reply);

                    // Debug
                    // -----------------------------------
                    Debug("-----", DebugType.Warning);
                    Debug(message, DebugType.Warning);
                    Debug(reply?.metadata?.message_type, DebugType.Warning);
                    Debug("-----", DebugType.Warning);
                    // -----------------------------------
                }
                CurrentSocketStatus = SocketStatus.Disconnected;
            }
        }

        private async static Task HandleReply(SocketReply reply)
        {
            if (reply?.metadata?.message_type == null)
                return;

            if (Enum.TryParse<MessageType>(reply.metadata.message_type, true, out var messageType))
            {
                switch (messageType)
                {
                    case MessageType.session_welcome:
                        await CreateSession(reply);
                    break;

                    case MessageType.notification:
                        if (reply.metadata.subscription_type == EventSubs.OnPollEnd)
                            HandlePollEnd(reply.payload?.Event);
                    break;
                }
            }
        }

        private static void HandlePollEnd(PollEvent pollEvent)
        {
            if (pollEvent == null || pollEvent.choices == null || pollEvent.choices.Length == 0 || pollEvent.status != PollStatus.completed.ToString())
                return;

            int[] votes = new int[pollEvent.choices.Length];
            for (int i = 0; i < pollEvent.choices.Length; i++) { votes[i] = pollEvent.choices[i].votes; }

            OnPollEnd?.Invoke(pollEvent.choices.First(x => x.votes == votes.Max()).title);
        }

        private async static Task CreateSession(SocketReply reply)
        {
            if (CurrentSessionData != null)
                return;
            
            if (reply?.payload?.session != null)
            {
                CurrentSessionData = reply.payload.session;
                if (CurrentSessionData?.id != null)
                    await Subscribe(EventSubs.OnPollEnd, CurrentUserData.id, CurrentSessionData.id, ClientID, App_OAuth);
            }
        }

        private async static Task Subscribe(string subEvent, string broadcasterID, string sessionID, string clientID, string OAuth)
        {
            var body = new
            {
                type        = subEvent,
                version     = "1",
                condition   = new { broadcaster_user_id = broadcasterID },
                transport   = new
                {
                    method      = "websocket",
                    session_id  = sessionID
                }
            };

            await SendWebRequest(API_Sub, clientID, OAuth, RequestType.POST, JsonNet.Serialize(body));
        }

        private async static Task<string> SendWebRequest(string URL, string clientID, string OAuth, RequestType requestType, string body = null, bool ignoreSocket = false)
        {
            if (!ignoreSocket && CurrentSocketStatus != SocketStatus.Connected)
                return string.Empty;

            string finalResponse = string.Empty;

            try
            {
                var request     = (HttpWebRequest)WebRequest.Create(URL);
                request.Method  = requestType.ToString();
                request.Headers.Add("Client-Id", clientID);
                request.Headers.Add("Authorization", $"Bearer {OAuth}");

                if (body != null)
                {
                    request.ContentType = "application/json";
                    using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
                    {
                        streamWriter.Write(body);
                    }
                }

                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (var stream   = response.GetResponseStream())
                using (var reader   = new StreamReader(stream))
                {
                    finalResponse = reader?.ReadToEnd()?.Trim();
                }
            } catch (Exception ex) { UnityEngine.Debug.LogError(ex); }

            return finalResponse ?? string.Empty;
        }
    }
}
