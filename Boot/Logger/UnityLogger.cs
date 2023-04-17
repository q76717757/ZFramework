using UnityEngine;

namespace ZFramework
{
    public class UnityLogger : ILog
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void Initialize()
        {
            Log.ILog = new UnityLogger();
        }

        public void Info(object obj)
        {
            Debug.Log(obj);
        }
        public void Warning(object obj)
        {
            Debug.LogWarning(obj);
        }
        public void Error(object obj)
        {
            Debug.LogError(obj);
        }
    }
}