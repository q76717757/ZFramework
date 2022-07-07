using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
	[Serializable]
	public class ReferenceData
	{
		public string key;
		public UnityEngine.Object value;
	}

	[DisallowMultipleComponent]
	public class References : MonoBehaviour, ISerializationCallbackReceiver
	{
		public List<ReferenceData> data = new List<ReferenceData>();

		private readonly Dictionary<string, UnityEngine.Object> dict = new Dictionary<string, UnityEngine.Object>();

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

		public void OnBeforeSerialize() { }
		public void OnAfterDeserialize()//比AWAKE执行更早
		{
			//ZLog.Info("OnAfterDeserialize");
			dict.Clear();
			foreach (ReferenceData referenceCollectorData in data)
			{
				if (!dict.ContainsKey(referenceCollectorData.key))
				{
					dict.Add(referenceCollectorData.key, referenceCollectorData.value);
				}
			}
		}
	}

}
