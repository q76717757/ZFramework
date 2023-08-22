using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine.UIElements;

namespace ZFramework.Editor
{
    [CustomEditor(typeof(BootStrap))]
    public class BootStrapInspector: UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (BootProfile.IsExists)
            {
                BootProfile.GetInstance();
                DrawProjectSettings();
            }
            else
            {
                BootProfileUtility.DrawCreateButton();
            }
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
        static GUIStyle style;
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
                    if (style == null)
                    {
                        style = new GUIStyle()
                        {
                            padding =
                            {
                                left = EditorStyles.label.padding.left + 17,
                                top = EditorStyles.label.padding.top
                            },
                            normal =
                            {
                                textColor = Color.green//BootConfig.IsExists? Color.green:Color.red,
                            }
                        };
                    }
                    GUI.Label(selectionRect, obj.name, style);
                }
            }
        }
    }
#endif
}
