using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZFramework
{
    public static class ZFrameworkSettingProvider
    {
        [SettingsProvider]
        public static SettingsProvider AA()
        {
            var provider = new SettingsProvider("Project/ZFramework", SettingsScope.Project);
            provider.label = "ZFramework";
            provider.guiHandler += OnG;
            return provider;
        }
        [SettingsProvider]
        public static SettingsProvider AA2()
        {
            var provider = new SettingsProvider("Project/ZFramework/Bundle", SettingsScope.Project);
            provider.label = "Bundle";
            provider.guiHandler += OnG2;
            return provider;
        }
        static void OnG(string srt)
        {
            EditorGUILayout.HelpBox("测试顶菜单", MessageType.Info);
        }

        static void OnG2(string srt)
        {
            EditorGUILayout.HelpBox("测试子菜单" + srt, MessageType.Info);
        }
    }
}
