using Json.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static TwitchChaos.DebugOutput;
using static TwitchChaos.TwitchWebReply;

namespace TwitchChaos
{
    internal sealed class TwitchWebSocket
    {
        internal delegate void  ConnectedToSocket();
        internal event          ConnectedToSocket OnConnectedToSocket;

        internal delegate void  DisconnectedFromSocket();
        internal event          DisconnectedFromSocket OnDisconnectedFromSocket;

        internal delegate void  PollEnd(string id, string winner);
        internal event          PollEnd OnPollEnd;

        private     SocketState _currentSocketState = SocketState.Disconnected;
        internal    SocketState CurrentSocketState
        {
            get => _currentSocketState;

            private set
            {
                switch (value)
                {
                    case SocketState.Connected:
                        if (_currentSocketState != value)
                        {
                            _currentSocketState = value;
                            CancelToken = new CancellationTokenSource();
                            OnConnectedToSocket?.Invoke();
                            return;
                        }
                    break;

                    case SocketState.Disconnected:
                        if (_currentSocketState != value)
                        {
                            CurrentTwitchUserInfo   = null;
                            CancelToken?.Dispose();
                            CancelToken             = null;
                            OnConnectedToSocket     = null;
                            OnPollEnd               = null;

                            OnDisconnectedFromSocket?.Invoke();
                        }
                    break;
                }

                _currentSocketState = value;
            }
        }

        internal enum SocketState
        {
            Disconnected,
            Disconnecting,
            Connecting,
            Connected
        }

        internal enum RequestType
        {
            POST,
            GET
        }

        internal API_Users.Data  CurrentTwitchUserInfo;
        private ClientInfo       CurrentClientInfo;
        private class ClientInfo
        {
            internal string ClientID    { get; private set; }
            internal string OAuth       { get; private set; }

            internal ClientInfo(string ClientID, string OAuth)
            {
                this.ClientID   = ClientID;
                this.OAuth      = OAuth;
            }
        }

        private SocketReply.Payload.Session CurrentSessionInfo;

        private CancellationTokenSource CancelToken;

        internal void ConnectToWebSocket(string clientID, string OAuth)
        {
            if (CurrentSocketState == SocketState.Disconnected)
            {
                CurrentClientInfo = new ClientInfo(clientID, OAuth);
                Connect();
            }
        }

        internal void DisconnectFromWebSocket()
        {
            if (CurrentSocketState == SocketState.Disconnected || CurrentSocketState == SocketState.Disconnecting)
                return;

            CurrentSocketState = SocketState.Disconnecting;
            CancelToken?.Cancel();
        }

        private async void Connect()
        {
            CurrentSocketState = SocketState.Connecting;
            if (await CreateUserInfo() == null)
            {
                CurrentSocketState = SocketState.Disconnected;
                return;
            }

            await RunWebSocket();
            CurrentSocketState = SocketState.Disconnected;
        }

        private async Task RunWebSocket()
        {
            using (var socket = new ClientWebSocket())
            {
                bool connected = false;

                try
                {
                    await socket.ConnectAsync(new Uri(WSS_EventSub), CancellationToken.None);
                    connected = socket.State == WebSocketState.Open;
                }
                catch
                {
                    connected = false;
                }

                if (!connected)
                {
                    CurrentSocketState = SocketState.Disconnected;
                    return;
                }

                CurrentSocketState = SocketState.Connected;

                while (CurrentSocketState == SocketState.Connected && socket.State == WebSocketState.Open)
                {
                    byte[] buffer = new byte[2048];
                    WebSocketReceiveResult socketReply = null;

                    try { socketReply = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancelToken.Token); } catch { break; }
                    if (socketReply == null)
                        continue;

                    string message = Encoding.UTF8.GetString(buffer, 0, socketReply.Count);
                    if (string.IsNullOrEmpty(message))
                        continue;

                    SocketReply reply = JsonNet.Deserialize<SocketReply>(message);
                    if (reply != null)
                        await HandleReply(reply);
                }
            }
        }

        private async Task HandleReply(SocketReply reply)
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

        private async Task CreateSession(SocketReply reply)
        {
            if (CurrentSessionInfo != null || CurrentTwitchUserInfo == null || reply?.payload?.session == null)
                return;

            CurrentSessionInfo = reply.payload.session;
            if (CurrentSessionInfo?.id != null)
                await Subscribe(EventSubs.OnPollEnd, CurrentTwitchUserInfo.id, CurrentSessionInfo.id);
        }

        private async Task Subscribe(string subEvent, string broadcasterID, string sessionID)
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

            await SendWebRequest(URL_API_SUB, RequestType.POST, JsonNet.Serialize(body));
        }

        private void HandlePollEnd(SocketReply.Payload.PollEvent pollEvent)
        {
            if (pollEvent == null || pollEvent.choices == null || pollEvent.choices.Length == 0 || pollEvent.status != PollStatus.completed.ToString())
                return;

            int[] votes = new int[pollEvent.choices.Length];
            for (int i = 0; i < pollEvent.choices.Length; i++) { votes[i] = pollEvent.choices[i].votes; }

            OnPollEnd?.Invoke(pollEvent.id, pollEvent.choices.First(x => x.votes == votes.Max()).title);
        }

        private async Task<API_Users.Data> CreateUserInfo()
        {
            string response = await SendWebRequest(URL_API_USERS, RequestType.GET);
            if (response == null)
                return null;

            return CurrentTwitchUserInfo = JsonNet.Deserialize<API_Users>(response)?.data?.FirstOrDefault();
        }

        internal async Task<string> SendWebRequest(string URL, RequestType requestType, string body = null)
        {
            if (CurrentClientInfo == null)
                return string.Empty;

            string requestResponse = string.Empty;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(URL);

                request.Method = requestType.ToString();
                request.Headers.Add("Client-Id", CurrentClientInfo.ClientID);
                request.Headers.Add("Authorization", $"Bearer {CurrentClientInfo.OAuth}");

                if (body != null)
                {
                    request.ContentType = "application/json";
                    using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync())) { streamWriter.Write(body); }
                }

                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (var reader   = new StreamReader(response.GetResponseStream()))
                {
                    requestResponse = reader?.ReadToEnd()?.Trim();
                }
            }
            catch (Exception ex)
            {
                Debug($"{ex}", DebugType.Error);
            }

            return requestResponse ?? string.Empty;
        }
    }
}
