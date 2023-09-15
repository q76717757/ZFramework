using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ZFramework.Editor
{
    public class NormalFadeFoldout : FadeFoldout
    {
        public override string Title => "基本设置";

        ReorderableList _assemblyList;

        BootProfile Profile
        {
            get
            {
                return BootProfile.GetInstance();
            }
        }
        ReorderableList AssemblyList
        {
            get
            {
                if (_assemblyList == null)
                {
                    _assemblyList = new ReorderableList(Profile.assemblyNames, typeof(string), true, false, true, true);
                    _assemblyList.drawElementCallback += OnDrawAotMetaElement;
                    _assemblyList.onChangedCallback += OnListChange;
                }
                return _assemblyList;
            }
        }

        protected override void OnGUI()
        {
            ShowDoc();

            var rect = EditorGUILayout.BeginVertical("GroupBox", GUILayout.ExpandWidth(true));

            EditorGUILayout.LabelField("框架加载程序集", EditorStyles.centeredGreyMiniLabel);
            AssemblyList.DoLayoutList();

            EditorGUILayout.EndVertical();

            if (GUI.Button(new Rect(rect.x + rect.width - 43, rect.y + 3, 40, 15), "重置", EditorStyles.miniButton))
            {
                GUI.FocusControl(null);
                Profile.assemblyNames.Clear();
                Profile.assemblyNames.AddRange(new BootProfile().assemblyNames);
                OnListChange();
            }
        }

        void ShowDoc()
        {
            string docURL = "https://www.processon.com/view/link/64b9f2dda554064ccf306779";
            if (GUILayout.Button($"引导流程图:https://www.processon.com/view/link/"))
            {
                Application.OpenURL(docURL);
            }
            EditorGUILayout.Space();
        }

        void OnDrawAotMetaElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            string next = EditorGUI.DelayedTextField(rect, Profile.assemblyNames[index]);
            if (string.IsNullOrEmpty(next))
            {
                EditorGUI.LabelField(rect, "不能为空", EditorStyles.centeredGreyMiniLabel);
            }
            if (next != Profile.assemblyNames[index])
            {
                Profile.assemblyNames[index] = next;
                OnListChange();
            }
        }
        void OnListChange(ReorderableList list = default)
        {
            Profile.SetDirty();
        }

    }
}
