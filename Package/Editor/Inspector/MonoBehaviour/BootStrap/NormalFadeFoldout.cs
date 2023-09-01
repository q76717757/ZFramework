using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
    public class NormalFadeFoldout : FadeFoldout
    {
        public override string Title => "基本设置";

        protected override void OnGUI()
        {
            ShowDoc();

            EditorGUILayout.LabelField("目标平台:" + Defines.TargetRuntimePlatform.ToString());
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
    }
}
