using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace WorldBuild.Mod
{
    public static class Debugger
    {
        public const bool IsDebugEnabled = true;

        private static object FormatMessage(object msg)
        {
            var frame = new StackTrace().GetFrame(2);
            return msg + "\n                     Calling Method: " + frame.GetMethod().ReflectedType?.FullName + ":" + frame.GetMethod().Name;
        }
        
        public static void Log(object message)
        {
            if (!IsDebugEnabled) return;

            Debug.Log(FormatMessage(message));
        }

        public static void Exception(Exception ex, bool overrideDE = false)
        {
            Debug.LogException(ex);
        }

        public static void Error(object message, bool overrideDE = false)
        {
            Debug.LogError(message + "\n" + new StackTrace());
        }

        public static void Warning(object message, bool overrideDE = false)
        {

            Debug.LogWarning(FormatMessage(message));
        }
    }
}
