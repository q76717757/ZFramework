using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZFramework
{
    [CustomEditor(typeof(GlobalConfig))]
    public class GlobalConfigEditor : Editor
    {
        GlobalConfig Config => target as GlobalConfig;


        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);



            base.OnInspectorGUI();

            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("�����ô���"))
            {
                GlobalConfigEditorWindow.Open(Config);
            }
        }
    }
}
