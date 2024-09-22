using UnityEngine;
using System;

namespace WorldBuild.Mod
{
    public static class Debugger
    {
        public static bool IsDebugEnabled = true;

        public static void Log(object message, bool overrideDE = false)
        {
            if (!IsDebugEnabled && !overrideDE) return;

            Debug.Log(message);
        }

        public static void LogException(Exception ex, bool overrideDE = false)
        {
            if (!IsDebugEnabled && !overrideDE) return;

            Debug.LogException(ex);
        }

        public static void LogError(object message, bool overrideDE = false)
        {
            if (!IsDebugEnabled && !overrideDE) return;

            Debug.LogError(message);
        }

        public static void LogWarning(object message, bool overrideDE = false)
        {
            if(!IsDebugEnabled && !overrideDE) return;

            Debug.LogWarning(message);
        }
    }
}
