﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

namespace ZFramework
{
    [CustomEditor(typeof(BootStrap))]
    public class BootStrapInspector: Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("引导文件", GUILayout.Width(50));
            var obj = serializedObject.FindProperty("boot");
            obj.objectReferenceValue = EditorGUILayout.ObjectField(obj.objectReferenceValue, typeof(BootFile), false);
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(10);
            GUILayout.Label("Model");
            EditorGUI.BeginDisabledGroup(true);
            var modelDll = File.Exists($"{BuildAssemblieEditor.UnityTempDllPath}Model.dll");
            GUILayout.TextField(modelDll ? $"{BuildAssemblieEditor.UnityTempDllPath}Model.dll" : "Model.Dll Not Exists");
            EditorGUI.EndDisabledGroup();

            GUILayout.Label("Logic");
            EditorGUI.BeginDisabledGroup(true);
            if (Directory.Exists(BuildAssemblieEditor.UnityTempDllPath))
            {
                string[] logicFiles = Directory.GetFiles(BuildAssemblieEditor.UnityTempDllPath, "Logic_*.dll");
                if (logicFiles.Length > 0)
                {
                    foreach (string file in logicFiles)
                    {
                        GUILayout.TextField(file);
                    }
                }
                else
                {
                    GUILayout.TextField("Logic.Dll Not Exists");
                }
            }
            else
            {
                GUILayout.TextField("Logic.Dll Not Exists");
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling);
            if (!EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Compile Model + Logic"))
                {
                    BuildAssemblieEditor.CompileAssembly_Development();
                }
            }
            else
            {
                if (GUILayout.Button("逻辑热重载"))
                {
                    HotReload();
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.EndDisabledGroup();

        }


        async void HotReload()
        {
            var assemblyName = await BuildAssemblieEditor.CompileAssembly_Logic("", "");
            var dll = File.ReadAllBytes($"{BuildAssemblieEditor.UnityTempDllPath}{assemblyName}.dll");
            var pdb = File.ReadAllBytes($"{BuildAssemblieEditor.UnityTempDllPath}{assemblyName}.pdb");
            Assembly logic = Assembly.Load(dll,pdb);
            (target as BootStrap).entry.Reload(logic);

        }

        void backup()
        {
            //资源路径可以改成从配置文件获取
            var dll = AssetDatabase.LoadAssetAtPath<TextAsset>(BuildAssemblieEditor.AssetsSaveDllPath + "Code.dll.bytes");
            var pdb = AssetDatabase.LoadAssetAtPath<TextAsset>(BuildAssemblieEditor.AssetsSaveDllPath + "Code.pdb.bytes");

            EditorGUILayout.BeginHorizontal();
            if (dll == null || pdb == null)
            {
                EditorGUILayout.HelpBox("Assembly Not Exist", MessageType.Error);
            }
            else
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.ObjectField(dll, typeof(TextAsset), false);
                EditorGUILayout.ObjectField(pdb, typeof(TextAsset), false);
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            FileInfo fileInfo = new FileInfo(BuildAssemblieEditor.AssetsSaveDllPath + "Code.dll.bytes");
            if (fileInfo.Exists)
            {
                GUILayout.Label("最后编译时间: " + fileInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }

            bool disable = EditorApplication.isPlaying || EditorApplication.isCompiling;
            EditorGUI.BeginDisabledGroup(disable);
            if (GUILayout.Button("Compile Assembly"))
            {
                string projectName = "";
                BuildAssemblieEditor.CompileAssembly_Debug(projectName);
            }
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
            var obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (obj == null) return;

            if (obj.GetComponent<BootStrap>() != null)
            {
                GUIStyle style = new GUIStyle()
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
                if (style != null)
                {
                    GUI.Label(selectionRect, obj.name, style);//重叠的 底层原本的字还在
                }
            }
        }
    }
#endif
}
