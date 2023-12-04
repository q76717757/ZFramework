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

        //ReorderableList _assemblyList;

        protected override void OnGUI()
        {
            ShowDoc();

        }

        void ShowDoc()
        {
            EditorGUILayout.BeginVertical("GroupBox", GUILayout.ExpandWidth(true));

            EditorGUILayout.LabelField("引导流程图", EditorStyles.centeredGreyMiniLabel);

            string docURL = "https://www.processon.com/view/link/64b9f2dda554064ccf306779";
            Rect linkRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, EditorStyles.linkLabel, GUILayout.ExpandWidth(true));
            EditorGUIUtility.AddCursorRect(linkRect, MouseCursor.Link);
            if (GUI.Button(linkRect, docURL, EditorStyles.linkLabel))
            {
                Application.OpenURL(docURL);
            }

            EditorGUILayout.EndVertical();
        }
    }
}
