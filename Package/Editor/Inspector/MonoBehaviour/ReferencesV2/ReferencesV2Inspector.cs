using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZFramework.Editor
{
    [CustomEditor(typeof(ReferencesV2))]
    public class ReferencesV2Inspector : UnityEditor.Editor
    {
        private ReferencesV2 references;
        string searchKey;

        private void OnEnable()
        {
            references = (ReferencesV2)target;
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(references, "Changed References");

            //������
            EditorGUILayout.BeginHorizontal();
            searchKey = EditorGUILayout.TextField(searchKey, new GUIStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("clear", !string.IsNullOrEmpty(searchKey) ? new GUIStyle("ToolbarSeachCancelButton") : new GUIStyle("ToolbarSeachCancelButtonEmpty")))
            {
                searchKey = string.Empty;
                EditorGUI.FocusTextInControl(string.Empty);
            }
            EditorGUILayout.EndHorizontal();

            var serializedArray = serializedObject.FindProperty("data");

            GUILayout.BeginHorizontal();//������ť
            if (GUILayout.Button("����"))
            {
                references.data.Sort(new ReferenceDataComparer2V2<RFDV>());
                EditorUtility.SetDirty(references);
            }
            if (GUILayout.Button("ɾ��������"))
            {
                for (int i = serializedArray.arraySize - 1; i >= 0; i--)
                {
                    if (serializedArray.GetArrayElementAtIndex(i).FindPropertyRelative("value").objectReferenceValue == null)
                        serializedArray.DeleteArrayElementAtIndex(i);
                }
            }
            if (GUILayout.Button("ȫ��ɾ��"))
            {
                serializedArray.ClearArray();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (serializedArray.arraySize > 0)
            {
                //������
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

                    UnityEngine.Object targetObj = property.objectReferenceValue;//ѡ���
                    bool isGameobjectOrComponent = targetObj is UnityEngine.GameObject || targetObj is UnityEngine.Component;
                    EditorGUI.BeginDisabledGroup(!isGameobjectOrComponent);
                    if (property.objectReferenceValue != null)
                    {
                        if (isGameobjectOrComponent)
                        {
                            UnityEngine.Component[] componengtList = null;
                            if (targetObj is GameObject)
                                componengtList = ((UnityEngine.GameObject)targetObj).GetComponents<UnityEngine.Component>();
                            if (targetObj is UnityEngine.Component)
                                componengtList = ((UnityEngine.Component)targetObj).GetComponents<UnityEngine.Component>();

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
                                    showItem[j] = "��Ч���";

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
                                        property.objectReferenceValue = ((UnityEngine.Component)targetObj).gameObject;
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

                    if (GUILayout.Button("ɾ��", GUILayout.Width(50)))
                    {
                        serializedArray.DeleteArrayElementAtIndex(i);

                        GUILayout.EndHorizontal();
                        break;
                    }
                    GUILayout.EndHorizontal();
                }

                //
                EditorGUILayout.LabelField("�������Ĺ���,֧�ֿɱ༭������,�����ǵ�ǰ�ĵ���");

            }
            else
            {
                EditorGUILayout.HelpBox("��ǰû�б����κ�����,��ק�����������,�������Ҫ����ű����Ե������İ�ť�����Ƴ�", MessageType.Info);
                EditorGUILayout.Space(50);
            }
            //else
            //{
            //    EditorGUILayout.HelpBox("��ǰû�б����κ�����,��ק�����������,�������Ҫ����ű����Ե������İ�ť�����Ƴ�", MessageType.Info);

            //    EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            //    if (GUILayout.Button("����Ƴ������"))
            //    {
            //        DestroyImmediate(references);
            //        return;
            //    }
            //    EditorGUI.EndDisabledGroup();
            //}

            //��ק�¼�
            var eventType = UnityEngine.Event.current.type;
            if (eventType == UnityEngine.EventType.DragUpdated || eventType == UnityEngine.EventType.DragPerform)
            {
                //var rect = new Rect();
                //if (rect.Contains(UnityEngine.Event.current.mousePosition))//��֪����ô��ins������
                //{
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                if (eventType == UnityEngine.EventType.DragPerform)
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


            var rect = GUILayoutUtility.GetRect(new GUIContent("Show"), EditorStyles.toolbarButton);
            if (GUI.Button(rect, new GUIContent("Show"), EditorStyles.toolbarButton))
            {
                var dropdown = new WeekdaysDropdown(new AdvancedDropdownState());
                dropdown.Show(rect);
            }

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

    //����������
    internal class ReferenceDataComparer2V2<T> : IComparer<RFDV>
    {
        public int Compare(RFDV x, RFDV y)
        {
            return string.Compare(x.GetType().Name, y.GetType().Name, StringComparison.Ordinal);
        }
    }


}
