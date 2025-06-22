namespace TwitchChaos
{
    internal static class DebugOutput
    {
        internal static bool ShowDebugMessages = true;

        internal enum DebugType
        {
            Normal,
            Warning,
            Error
        }

        internal static void Debug(string message, DebugType debugType = DebugType.Normal)
        {
            if (!ShowDebugMessages)
                return;

            switch (debugType)
            {
                case DebugType.Normal:  UnityEngine.Debug.Log       (message);  break;
                case DebugType.Warning: UnityEngine.Debug.LogWarning(message);  break;
                case DebugType.Error:   UnityEngine.Debug.LogError  (message);  break;
            }
        }
    }
}
