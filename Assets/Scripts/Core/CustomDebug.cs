#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define USE_CUSTOM_DEBUG
#endif
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Core
{
    public static class CustomDebug
    {
        [Conditional("USE_CUSTOM_DEBUG")]
        public static void Log(object message, UnityEngine.Object context = null)
        {
            Debug.Log(message, context);
        }
        
        [Conditional("USE_CUSTOM_DEBUG")]
        public static void LogWarning(object message, UnityEngine.Object context = null)
        {
            Debug.LogWarning(message, context);
        }
        
        [Conditional("USE_CUSTOM_DEBUG")]
        public static void LogError(object message, UnityEngine.Object context  = null)
        {
            Debug.LogError(message, context);
        }
        
        [Conditional("USE_CUSTOM_DEBUG")]
        public static void LogException(System.Exception exception, UnityEngine.Object context = null)
        {
            Debug.LogException(exception, context);
        }
    }
}