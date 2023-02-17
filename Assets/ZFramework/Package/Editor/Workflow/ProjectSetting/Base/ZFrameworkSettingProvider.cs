using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
    public class ZFrameworkSettingProvider : SettingsProvider
    {
        public static string Path => "Project/ZFramework";
        public ZFrameworkSettingProvider() : base(Path, SettingsScope.Project) { }
        [SettingsProvider] public static SettingsProvider Register() => new ZFrameworkSettingProvider();
        public static void OpenSettings() => SettingsService.OpenProjectSettings(Path);

        Vector2 pos;
        public override void OnGUI(string searchContext)
        {
            if (ZFrameworkRuntimeSettingsEditor.CheckExistsAndDrawCreateBtnIfNull())
            {
                var readme = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/ZFramework/readme.txt");
                if (readme == null) return;

                var text = readme.text;
                pos = EditorGUILayout.BeginScrollView(pos);

                using (StringReader sr = new StringReader(text))
                {
                    while (true)
                    {
                        var line = sr.ReadLine();
                        if (line == null)
                        {
                            break;
                        }
                        EditorGUILayout.LabelField(line);
                    }
                }

                EditorGUILayout.EndScrollView();

            }
        }
    }

}
