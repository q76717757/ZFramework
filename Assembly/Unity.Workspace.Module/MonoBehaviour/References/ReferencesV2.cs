using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ZFramework
{

    public abstract class RFDV
    {
        public string key;
    }

    [Serializable]
    public class ReferenceDataV2<T> : RFDV
    {
        public T value;
    }


    [AddComponentMenu("ZFramework/ReferencesV2")]
    [DisallowMultipleComponent]
    public class ReferencesV2 : MonoBehaviour, ISerializationCallbackReceiver
    {
        public List<ReferenceDataV2<UnityEngine.Object>> data = new List<ReferenceDataV2<UnityEngine.Object>>();
     
        private Dictionary<string, UnityEngine.Object> dict;
        private Dictionary<string, Color> dicColor;
        private Dictionary<string, int> dicInt;
        private Dictionary<string, float> dicFloat;
        private Dictionary<string, double> dicDouble;
        private Dictionary<string, bool> dicBool;
        //Vector2、Vector3、Vector4、Rect、Quaternion、Matrix4x4、Color、Color32、LayerMask、AnimationCurve、Gradient、RectOffset、GUIStyle

        public T Get<T>(string key) where T : UnityEngine.Object
        {
            if (dict.TryGetValue(key, out UnityEngine.Object value))
            {
#if UNITY_EDITOR
                if (value is T)
                    return value as T;
                else
                    throw new ArgumentException($"[References]:Get[{key}]发生错误,请求的类型是{typeof(T)},但[{key}]的实际类型是{value.GetType()}");
#else
				return value as T;
#endif
            }
            return null;
        }

        public void OnBeforeSerialize()
        {

        }
        public void OnAfterDeserialize()//比AWAKE执行更早
        {
            dict = new Dictionary<string, UnityEngine.Object>();
            //foreach (ReferenceDataV2 referenceCollectorData in data)
            //{
            //    if (!dict.ContainsKey(referenceCollectorData.key))
            //    {
            //        dict.Add(referenceCollectorData.key, referenceCollectorData.value);
            //    }
            //}
        }




    }

}
