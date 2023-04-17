using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace ZFramework.Editor
{
    [CustomEditor(typeof(ZFrameworkRuntimeSettings))]
    public class ZFrameworkRuntimeSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Editor"))
            {
                ZFrameworkSettingProvider.OpenSettings();
            }
        }
        public static bool CheckExistsAndDrawCreateBtnIfNull()
        {
            if (ZFrameworkRuntimeSettings.Get() == null)
            {
                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                EditorGUILayout.HelpBox("缺少配置文件,点击按钮创建", MessageType.Error);
                if (GUILayout.Button("Create"))
                {
                    CreateAsset();
                    EditorGUIUtility.PingObject(ZFrameworkRuntimeSettings.Get());
                    ZFrameworkSettingProvider.OpenSettings();
                }
                EditorGUI.EndDisabledGroup();
                return false;
            }
            else
            {
                return true;
            }
        }
        public static void CreateAsset()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            AssetDatabase.CreateAsset(CreateInstance<ZFrameworkRuntimeSettings>(), $"Assets/Resources/{ZFrameworkRuntimeSettings.AssetName}.asset");
        }
        protected override bool ShouldHideOpenButton() => true;
    }
}
