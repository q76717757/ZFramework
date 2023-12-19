using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
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
}
