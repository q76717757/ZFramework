using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ZFramework
{
    [CustomEditor(typeof(BootStrap))]
    public class BootStrapInspector: Editor
    {
        public override void OnInspectorGUI()
        {
            //EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("引导文件", GUILayout.Width(50));
            var obj = serializedObject.FindProperty("boot");
            obj.objectReferenceValue = (BootFile)EditorGUILayout.ObjectField(obj.objectReferenceValue, typeof(BootFile), false);
            EditorGUILayout.EndHorizontal();


            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();


            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling || EditorApplication.isPlaying);
            if (GUILayout.Button("Compile", GUILayout.Height(50)))
            {
                BuildAssemblieEditor.CompileAssembly_Development("ProjectCode", UnityEditor.Compilation.CodeOptimization.Debug);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.EndDisabledGroup();
        }
    }




#if UNITY_2020_3_OR_NEWER
    [InitializeOnLoad]
    public class BootStrapHierarchyColor
    {
        static BootStrapHierarchyColor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
        }

        private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is GameObject obj && obj != null)
            {
                var Style = new GUIStyle()
                {
                    padding =
                    {
                        left =EditorStyles.label.padding.left + 17,
                        top = EditorStyles.label.padding.top
                    },
                    normal =
                    {
                        textColor = Color.green
                    }
                };

                if (obj.GetComponent<BootStrap>() != null && Style != null)
                {
                    GUI.Label(selectionRect, obj.name, Style);//重叠的 底层原本的字还在
                }
            }
        }
    }
#endif
}
