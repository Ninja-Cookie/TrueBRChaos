using System;

namespace TwitchChaos
{
    internal static partial class Twitch
    {
        [Serializable]
        private class API_Reply_Users
        {
            public UserData[] data { get; set; }
        }

        [Serializable]
        private class UserData
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

        [Serializable]
        private class SocketReply
        {
            public Metadata metadata    { get; set; }
            public Payload  payload     { get; set; }
        }

        [Serializable]
        private class Payload
        {
            public Session      session { get; set; }
            public PollEvent    Event   { get; set; }
        }

        [Serializable]
        private class Metadata
        {
            public string message_type      { get; set; }
            public string subscription_type { get; set; }
        }

        [Serializable]
        private class Session
        {
            public string   id                          { get; set; }
            public string   status                      { get; set; }
            public string   connected_at                { get; set; }
            public int      keepalive_timeout_seconds   { get; set; }
            public string   reconnect_url               { get; set; }
            public string   recovery_url                { get; set; }
        }

        [Serializable]
        private class PollEvent
        {
            public string       title   { get; set; }
            public Choices[]    choices { get; set; }
            public string       status  { get; set; }
        }

        [Serializable]
        internal class Choices
        {
            public string   title { get; set; }
            public int      votes { get; set; }
        }
    }
}
