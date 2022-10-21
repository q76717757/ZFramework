using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Text.RegularExpressions;
using System.Reflection;

namespace ZFramework
{
    [CustomEditor(typeof(BootFile))]
    public class BootFileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Draw(serializedObject);
        }
        public static void Draw(SerializedObject serializedObject)
        {
            if (serializedObject == null)
            {
                return;
            }
            var titleWidth = GUILayout.Width(80);
          
            EditorGUILayout.BeginVertical("FrameBox");


            EditorGUILayout.BeginHorizontal();
            var temp = GUI.contentColor;
            GUI.contentColor = Color.green;
            var projectCode = serializedObject.FindProperty("projectCode");
            EditorGUILayout.LabelField("项目代号:", titleWidth);
            projectCode.stringValue = GetNumberAlpha(EditorGUILayout.DelayedTextField(projectCode.stringValue));
            GUI.contentColor = temp;
            EditorGUILayout.EndHorizontal();
            bool notCode = string.IsNullOrEmpty(projectCode.stringValue);
            if (notCode)
            {
                EditorGUILayout.HelpBox("需要填项目代号,项目代号将作为Dll命名的一部分", MessageType.Error);
            }
            string GetNumberAlpha(string source)//只保留数字和字母
            {
                string pattern = "[A-Za-z0-9]";
                string strRet = "";
                MatchCollection results = Regex.Matches(source, pattern);
                foreach (var v in results)
                {
                    strRet += v.ToString();
                }
                return strRet;
            }


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("项目名称:", titleWidth);
            var projectName = serializedObject.FindProperty("projectName");
            projectName.stringValue = EditorGUILayout.DelayedTextField(projectName.stringValue);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            var versionString = serializedObject.FindProperty("projectVersion");
            EditorGUILayout.LabelField("程序版本:", titleWidth);
            if (!VersionInfo.TryParse(versionString.stringValue,out VersionInfo ver))
            {
                versionString.stringValue = ver.ToString();
            }
            if (GUILayout.Button(ver.X.ToString()))
            {
                ver.X++;
                ver.Y = 0;
                ver.Z = 0;
                ver.W = 0;
                versionString.stringValue = ver.ToString();
            }
            if (GUILayout.Button(ver.Y.ToString()))
            {
                ver.Y++;
                ver.Z = 0;
                ver.W = 0;
                versionString.stringValue = ver.ToString();
            }
            if (GUILayout.Button(ver.Z.ToString()))
            {
                ver.Z++;
                ver.W = 0;
                versionString.stringValue = ver.ToString();
            }
            GUILayout.Label("编译版本:" + ver.W);

            ver.S = GUILayout.TextField(ver.S);
            versionString.stringValue = ver.ToString();

            if (GUILayout.Button("Reset", GUILayout.Width(50)))
            {
                versionString.stringValue = new VersionInfo().ToString();
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("目标平台:", titleWidth);
            var platform = serializedObject.FindProperty("platform");
            platform.enumValueIndex = (int)(Platform)EditorGUILayout.EnumPopup((Platform)platform.enumValueIndex);
            EditorGUILayout.EndHorizontal();

            var selectPlatform = (Platform)platform.enumValueIndex;
            bool supportHotfix = false;
            switch (selectPlatform)
            {
                case Platform.Win:
                case Platform.Mac:
                case Platform.Andorid:
                case Platform.IOS:
                    supportHotfix = true;
                    break;
                case Platform.WebGL:
                case Platform.UWP:
                    supportHotfix = false;
                    break;
            }
            if (!supportHotfix)
            {
                EditorGUILayout.HelpBox("不支持热更新的平台,程序集编译完需要跟工程一起发布,以插件的形式加载", MessageType.Warning);
            }


            //EditorGUILayout.BeginHorizontal();
            //GUILayout.Label("更新方式:", titleWidth);
            //var network = serializedObject.FindProperty("network");
            //network.enumValueIndex = (int)(Network)EditorGUILayout.EnumPopup((Network)network.enumValueIndex);
            //var selectNetwork = (Network)network.enumValueIndex;
            //EditorGUILayout.EndHorizontal();
            //仅PC平台有本地模式,直接通过本地读补丁,一些内网PC项目,macPC很少  先针对Win来设计

            //

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("允许离线:", titleWidth);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.Space(20);
           
            EditorGUI.BeginDisabledGroup(EditorApplication.isCompiling || EditorApplication.isPlaying || notCode);
            bool ReCompile = GUILayout.Button("重新编译", GUILayout.Height(50));
            if (ReCompile)
            {
                ver.W++;
                versionString.stringValue = ver.ToString();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndVertical();

            if (serializedObject.ApplyModifiedProperties())
            {
                AssetDatabase.SaveAssetIfDirty(serializedObject.targetObject);
            }

            if (ReCompile)
            {
                string assemblyName = (serializedObject.targetObject as BootFile).GetDllName();
                BuildAssemblieEditor.CompileAssembly_Development(assemblyName, UnityEditor.Compilation.CodeOptimization.Debug);
            }
        }

        protected override bool ShouldHideOpenButton() => true;
    }
}
