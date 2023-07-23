using UnityEngine;

namespace ZFramework
{
    public class UnityLogger : ILog
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

        void ILog.Info(object obj)
        {
            Debug.Log(obj);
        }
        void ILog.Warning(object obj)
        {
            Debug.LogWarning(obj);
        }
        void ILog.Error(object obj)
        {
            Debug.LogError(obj);
        }
    }
}