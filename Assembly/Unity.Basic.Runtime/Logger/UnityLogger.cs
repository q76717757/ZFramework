using UnityEngine;

namespace ZFramework
{
    public class UnityLogger : ILogger
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
#endif
        static void Initialize()
        {
            Log.ILog = new UnityLogger();
        }

        void ILogger.Info(object obj)
        {
            Debug.Log(obj);
        }
        void ILogger.Warning(object obj)
        {
            Debug.LogWarning(obj);
        }
        void ILogger.Error(object obj)
        {
            Debug.LogError(obj);
        }
    }
}