using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZFramework
{
	//[CanEditMultipleObjects]
	[CustomEditor(typeof(References))]
	public class ReferencesInspector : Editor
	{
		private References references;
		string searchKey;

		private void OnEnable()
		{
			references = (References)target;
		}

        public override void OnInspectorGUI()
		{
			Undo.RecordObject(references, "Changed References");

			//搜索栏
			EditorGUILayout.BeginHorizontal();
			searchKey = EditorGUILayout.TextField(searchKey, new GUIStyle("ToolbarSeachTextField"));
			if (GUILayout.Button("clear", !string.IsNullOrEmpty(searchKey) ? new GUIStyle("ToolbarSeachCancelButton") : new GUIStyle("ToolbarSeachCancelButtonEmpty")))
			{
				searchKey = string.Empty;
				EditorGUI.FocusTextInControl(string.Empty);
			}
			EditorGUILayout.EndHorizontal();

			var serializedArray = serializedObject.FindProperty("data");

			GUILayout.BeginHorizontal();//公共按钮
			if (GUILayout.Button("排序(按名称)"))
			{
				references.data.Sort(new ReferenceDataComparer());
				EditorUtility.SetDirty(references);
			}
			if (GUILayout.Button("排序(按类型)"))
			{
				references.data.Sort(new ReferenceDataComparer2());
				EditorUtility.SetDirty(references);
			}
			if (GUILayout.Button("删除空引用"))
			{
				for (int i = serializedArray.arraySize - 1; i >= 0; i--)
				{
					if (serializedArray.GetArrayElementAtIndex(i).FindPropertyRelative("value").objectReferenceValue == null)
						serializedArray.DeleteArrayElementAtIndex(i);
				}
			}
			if (GUILayout.Button("全部删除"))
			{
				serializedArray.ClearArray();
			}
			
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

            if (serializedArray.arraySize > 0)
            {
				//画内容
				SerializedProperty property;
				for (int i = 0; i < serializedArray.arraySize; i++)
				{
					var key = serializedArray.GetArrayElementAtIndex(i).FindPropertyRelative("key").stringValue;
				    if (!string.IsNullOrEmpty(searchKey) && !string.IsNullOrEmpty(key) && !key.ToLower().Contains(searchKey.ToLower()))
				    {
						continue;
					}

					GUILayout.BeginHorizontal();
					property = serializedArray.GetArrayElementAtIndex(i).FindPropertyRelative("key");
					property.stringValue = EditorGUILayout.TextField(property.stringValue, GUILayout.Width(120));
					property = serializedArray.GetArrayElementAtIndex(i).FindPropertyRelative("value");
					property.objectReferenceValue = EditorGUILayout.ObjectField(property.objectReferenceValue, typeof(UnityEngine.Object), true/*, GUILayout.MinWidth(30), GUILayout.MaxWidth(150)*/);

					UnityEngine.Object targetObj = property.objectReferenceValue;//选组件
					bool isGameobjectOrComponent = targetObj is GameObject || targetObj is Component;
					EditorGUI.BeginDisabledGroup(!isGameobjectOrComponent);
					if (property.objectReferenceValue != null)
					{
				        if (isGameobjectOrComponent)
				        {
							Component[] componengtList = null;
							if (targetObj is GameObject)
								componengtList = ((GameObject)targetObj).GetComponents<Component>();
							if (targetObj is Component)
								componengtList = ((Component)targetObj).GetComponents<Component>();

							string[] showItem = new string[componengtList.Length + 1];
							showItem[0] = typeof(GameObject).Name;
							int componengtIndex = 0;
							for (int j = 1; j < showItem.Length; j++)
							{
				                if (componengtList[j - 1] != null)
				                {
									showItem[j] = componengtList[j - 1].GetType().Name;
									if (componengtList[j - 1].GetType() == targetObj.GetType())
									{
										componengtIndex = j;
									}
				                }
				                else
				                {
									showItem[j] = "无效组件";

								}
							}
							int selectIndex = EditorGUILayout.Popup(componengtIndex, showItem, GUILayout.MinWidth(100));
				            if (selectIndex != componengtIndex)
				            {
				                if (selectIndex == 0)
				                {
				                    if (targetObj is GameObject)
				                    {
										property.objectReferenceValue = targetObj;
				                    }
				                    else
				                    {
										property.objectReferenceValue = ((Component)targetObj).gameObject;
									}
								}
				                else
				                {
									property.objectReferenceValue = componengtList[selectIndex - 1];
				                }
							}
						}
				        else
				        {
							EditorGUILayout.Popup(0, new string[] { targetObj.GetType().Name }, GUILayout.MinWidth(100));
						}
				    }
				    else
				    {
						EditorGUILayout.Popup(0, new string[] { "Null" }, GUILayout.MinWidth(100));
					}

					EditorGUI.EndDisabledGroup();

					if (GUILayout.Button("删除", GUILayout.Width(50)))
					{
						serializedArray.DeleteArrayElementAtIndex(i);

						GUILayout.EndHorizontal();
						break;
					}
					GUILayout.EndHorizontal();
				}

				//
				EditorGUILayout.LabelField("待开发的功能,支持可编辑的数组,而不是当前的单个");
            }
            else
            {
				EditorGUILayout.HelpBox("当前没有保存任何引用,拖拽可以添加引用,如果不需要这个脚本可以点击下面的按钮把它移除", MessageType.Info);

				EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                if (GUILayout.Button("点此移除本组件"))
                {
					DestroyImmediate(references);
					return;
				}
				EditorGUI.EndDisabledGroup();
			}

			//拖拽事件
			var eventType = UnityEngine.Event.current.type;
			if (eventType == EventType.DragUpdated || eventType == EventType.DragPerform)
			{
				//var rect = new Rect();
				//if (rect.Contains(UnityEngine.Event.current.mousePosition))//不知道怎么拿ins的区域
				//{
				DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
					if (eventType == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();
						foreach (var o in DragAndDrop.objectReferences)
						{
							AddReference(serializedArray, o.name, o);
						}
					}
					UnityEngine.Event.current.Use();
				//}
			}
			serializedObject.ApplyModifiedProperties();
			serializedObject.UpdateIfRequiredOrScript();
		}

		private void AddReference(SerializedProperty dataProperty, string key, UnityEngine.Object value)
		{
			int index = dataProperty.arraySize;
			dataProperty.InsertArrayElementAtIndex(index);
			var element = dataProperty.GetArrayElementAtIndex(index);
			element.FindPropertyRelative("key").stringValue = key;
			element.FindPropertyRelative("value").objectReferenceValue = value;
		}

	}

	//按名称排列
	internal class ReferenceDataComparer : IComparer<ReferenceData>
	{
		public int Compare(ReferenceData x, ReferenceData y)
		{
			return string.Compare(x.key, y.key, StringComparison.Ordinal);
		}
	}
	//按类型排列
	internal class ReferenceDataComparer2 : IComparer<ReferenceData>
	{
		public int Compare(ReferenceData x, ReferenceData y)
		{
			return string.Compare(x.value.GetType().Name, y.value.GetType().Name, StringComparison.Ordinal);
		}
	}
}