using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace ZFramework
{
    [CustomEditor(typeof(BootFile))]
    public class BootFileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.FindProperty("useHotfix").intValue = EditorGUILayout.Popup(serializedObject.FindProperty("useHotfix").intValue, new string[] { "不应用", "应用" });
            

            

            if (serializedObject.FindProperty("useHotfix").intValue == 1)
            {
                serializedObject.FindProperty("hotfixType").intValue = EditorGUILayout.Popup(serializedObject.FindProperty("hotfixType").intValue, new string[] { "本地", "远程" });
            }
            else
            {

            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }
    }
}
