using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Profiling.Memory.Experimental;
using static System.Collections.Specialized.BitVector32;

namespace TwitchChaos
{
    internal static class TwitchWebReply
    {
        internal const string WSS_EventSub  = "wss://eventsub.wss.twitch.tv/ws";
        internal const string URL_API_USERS = "https://api.twitch.tv/helix/users";
        internal const string URL_API_SUB   = "https://api.twitch.tv/helix/eventsub/subscriptions";
        internal const string URL_API_POLL  = "https://api.twitch.tv/helix/polls";

        internal enum MessageType
        {
            session_welcome,
            notification
        }

        internal enum PollStatus
        {
            completed,
            ACTIVE
        }

        internal class EventSubs
        {
            internal const string OnPollBegin       = "channel.poll.begin";
            internal const string OnPollProgress    = "channel.poll.progress";
            internal const string OnPollEnd         = "channel.poll.end";
        }

        [Serializable]
        internal class API_Users
        {
            public Data[] data { get; set; }

            [Serializable]
            public class Data
            {
                public string   id                  { get; set; }
                public string   login               { get; set; }
                public string   display_name        { get; set; }
                public string   type                { get; set; }
                public string   broadcaster_type    { get; set; }
                public string   description         { get; set; }
                public string   profile_image_url   { get; set; }
                public string   offline_image_url   { get; set; }
                public int      view_count          { get; set; }
                public string   created_at          { get; set; }
            }
        }

        [Serializable]
        internal class SocketReply
        {
            public Metadata metadata    { get; set; }
            public Payload  payload     { get; set; }

            [Serializable]
            public class Payload
            {
                public Session      session { get; set; }
                public PollEvent    Event   { get; set; }

                [Serializable]
                public class Session
                {
                    public string   id                          { get; set; }
                    public string   status                      { get; set; }
                    public string   connected_at                { get; set; }
                    public int      keepalive_timeout_seconds   { get; set; }
                    public string   reconnect_url               { get; set; }
                    public string   recovery_url                { get; set; }
                }

                [Serializable]
                public class PollEvent
                {
                    public string       id      { get; set; }
                    public string       title   { get; set; }
                    public Choices[]    choices { get; set; }
                    public string       status  { get; set; }

                    [Serializable]
                    public class Choices
                    {
                        public string   title { get; set; }
                        public int      votes { get; set; }
                    }
                }
            }

            [Serializable]
            public class Metadata
            {
                public string message_type      { get; set; }
                public string subscription_type { get; set; }
            }
        }

        [Serializable]
        internal class API_PollStart_Reply
        {
            public PollStartData[] data { get; set; }

            [Serializable]
            public class PollStartData
            {
                public string id        { get; set; }
                public string status    { get; set; }
            }
        }
    }
}
