using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZFramework
{
    [CustomEditor(typeof(BootFile))]
    public class BootFileEditor : Editor
    {
        BootFile boot => target as BootFile;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            base.OnInspectorGUI();

            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("´ò¿ªÅäÖÃ´°¿Ú"))
            {
                BootFileEditorWindow.Open(boot);
            }
        }
    }
}
