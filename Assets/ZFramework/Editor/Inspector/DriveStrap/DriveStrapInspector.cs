using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ZFramework
{
    [CustomEditor(typeof(DriveStrap))]
    public class DriveStrapInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying && EditorApplication.isCompiling);

            if (GUILayout.Button("»»÷ÿ‘ÿ",GUILayout.Height(50)))
            {

            }


            EditorGUI.EndDisabledGroup();
        }

        async void HotReload()
        {
            var assemblyName = await BuildAssemblieEditor.CompileAssembly_Logic("", "");
            var dll = File.ReadAllBytes($"{AssemblyLoader.TempDllPath}{assemblyName}.dll");
            var pdb = File.ReadAllBytes($"{AssemblyLoader.TempDllPath}{assemblyName}.pdb");
            Assembly logic = Assembly.Load(dll, pdb);



            (target as DriveStrap) .entry.Reload(logic);

        }
    }
}
