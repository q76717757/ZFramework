using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace ZFramework.Editor
{
    [CustomEditor(typeof(BootStrap))]
    public class BootStrapInspector: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawBanner();
            if (ZFrameworkRuntimeSettingsEditor.CheckExistsAndDrawCreateBtnIfNull())
            {
                DrawProjectSettings();
            }

            base.OnInspectorGUI();
        }

        void DrawBanner()
        {
            EditorGUILayout.BeginVertical("FrameBox");
            EditorGUILayout.LabelField("--------------");
            EditorGUILayout.LabelField("--广告位招租--");
            EditorGUILayout.LabelField("--------------");
            EditorGUILayout.EndVertical();
        }

        void DrawProjectSettings()
        {
            if (GUILayout.Button("Project Settings"))
            {
                ZFrameworkSettingProvider.OpenSettings();
            }
        }
    }

    


#if UNITY_2020_3_OR_NEWER
    [InitializeOnLoad]
    public class BootStrapHierarchyColor
    {
        static BootStrapHierarchyColor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }
        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is GameObject obj && obj != null)
            {   
                if (obj.TryGetComponent<BootStrap>(out _))
                {
                    var Style = new GUIStyle()
                    {
                        padding =
                        {
                            left = EditorStyles.label.padding.left + 17,
                            top = EditorStyles.label.padding.top
                        },
                        normal =
                        {
                             textColor = ZFrameworkRuntimeSettings.Get() != null? Color.green :Color.red,
                        }
                    };
                    GUI.Label(selectionRect, obj.name, Style);
                }
            }
        }
    }
#endif
}
