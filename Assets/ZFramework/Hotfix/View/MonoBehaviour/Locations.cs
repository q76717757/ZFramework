using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    /// <summary>
    /// 帮助直接拿到场景中一个预设物体  tag和gameobject 一对一 是个全局单例
    /// </summary>

    [AddComponentMenu("ZFramework/Locations")]
    [DisallowMultipleComponent]
    public class Locations : MonoBehaviour
    {
        [SerializeField]private string locationTag;
        private static Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();

        public string LocationTag => locationTag;

        void Awake()
        {
            if(string.IsNullOrEmpty(locationTag))
            {
                Destroy(this);
                return;
            }
            else
            {
                if (cache.ContainsKey(locationTag))
                {
                    Debug.Log($"存在重复的LocationTag-><color=red>{locationTag}</color>");
                }
                else
                {
                    cache.Add(locationTag, gameObject);
                }
            }
        }
        private void OnDestroy()
        {
            if (string.IsNullOrEmpty(locationTag))
            {
                return;
            }
            if (cache.TryGetValue(locationTag,out GameObject value))
            {
                if (value == null || value == gameObject)
                {
                    cache.Remove(locationTag);
                }
            }
        }

        public static GameObject Get(string tag)
        {
            if (cache.TryGetValue(tag, out GameObject value))
            {
                return value;
            }
            return null;
        }
    }
}
