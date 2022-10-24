using UnityEngine;

namespace ZFramework
{
    public class UnityLogger : ILog
    {
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