using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZFramework
{
    public static class ZFrameworkSettingProvider
    {
        static BootFile bootFile;

        //[SettingsProvider]
        public static SettingsProvider RegisterZFramework()
        {
            var provider = new SettingsProvider("Project/ZFramework", SettingsScope.Project);
            provider.label = "ZFramework";
            provider.guiHandler += OnGUI;
            return provider;
        }

        static void OnGUI(string srt)
        {
            EditorGUILayout.BeginHorizontal();

            bootFile = (BootFile)EditorGUILayout.ObjectField(bootFile, typeof(BootFile), false);
            if (bootFile == null)
            {
                if (GUILayout.Button("New",GUILayout.Width(50)))
                {
                    var s = AssetDatabase.CreateFolder("Assets", "Resources");
                    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<BootFile>(), "Assets/Resources/Boot.asset");
                    bootFile = Resources.Load<BootFile>("Boot");
                }
            }
            EditorGUILayout.EndHorizontal();

            if (bootFile == null) return;
            {

            }

            //var boot = Resources.Load<BootFile>("BootFile");
            //EditorGUILayout.BeginVertical("Box");
            //SerializedObject se = new SerializedObject(boot);
            //BootFileEditor.Draw(se);
            //EditorGUILayout.EndVertical();
        }
    }
}
