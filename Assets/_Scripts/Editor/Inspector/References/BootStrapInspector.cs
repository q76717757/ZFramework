using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace ZFramework
{
    [CustomEditor(typeof(BootStrap))]
    public class BootStrapInspector: Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(20);


            switch ((target as BootStrap).mode)
            {
                case RunMode.HotReload_EditorRuntime:
                    {
                        if (!EditorApplication.isPlaying)
                        {
                            if (GUILayout.Button("编译热重载程序集"))
                            {
                                BuildAssemblieEditor.BuildHotReloadAssembly();
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("运行时热重载(Editor Runtime)"))
                            {
                                HotReload();
                            }
                        }
                    }
                    break;
                case RunMode.Mono_RealMachine:
                    {
                        EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);

                        if (GUILayout.Button("编译assembly到Assets/_Bundles/Code"))
                        {
                            BuildAssemblieEditor.BuildCodeDebug();
                        }
                        EditorGUI.EndDisabledGroup();
                    }
                    break;
                default:
                    break;
            }
        }

        async void HotReload() {
            await BuildAssemblieEditor.BuildHotfix();
            var boot = (target as BootStrap);
            AssemblyLoader.ReloadHotfixAssembly();
            boot.entry.Reload();
        }
    }
}
