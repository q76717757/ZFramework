using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEditor.IMGUI.Controls;
using UnityEngine.UIElements;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;

namespace ZFramework.Editor
{
    [CustomEditor(typeof(BootStrap))]
    public class BootStrapInspector: UnityEditor.Editor
    {
        AssemblyFadeGroup a;

        private void OnEnable()
        {
            a = new AssemblyFadeGroup();
            a.OnValueChange.AddListener(Repaint);
        }

        public override void OnInspectorGUI()
        {
            if (!BootProfile.IsExists)
            {
                BootProfileUtility.DrawCreateButton();
            }
            else
            {



                DrawBootProfile();
            }
        }

        ReorderableList list;
        void DrawBootProfile()
        {
            a.OnGui();

            //if (BeginFadeGroup(0, "基本设置"))
            //{
            //    EditorGUILayout.LabelField("目标平台:" + Defines.TargetRuntimePlatform.ToString());
            //    EditorGUILayout.LabelField("强同步:" + profile.IsMustSync);
            //}
            //EndFadeGroup();


            //if (BeginFadeGroup(1, "程序集设置"))
            //{
            //    if (list == null)
            //    {
            //        list = new ReorderableList(profile.AotMetaAssemblyNames, null, true, false, true, true);
            //        list.drawElementCallback += A;
            //    }
            //    list.DoLayoutList();
            //}
            //EndFadeGroup();
            //if (BeginFadeGroup(2, "热更新设置"))
            //{
            //    EditorGUILayout.LabelField("代码热更新方案: HybridCLR",EditorStyles.centeredGreyMiniLabel);
            //    if (GUILayout.Button("官方文档:https://hybridclr.doc.code-philosophy.com/docs/intro"))
            //    {
            //        Application.OpenURL("https://hybridclr.doc.code-philosophy.com/docs/intro");
            //    }
            //    EditorGUILayout.Space();

            //    switch (EnumPopup())
            //    {
            //        case Defines.UpdateType.Not:
            //            //Info("针对不具备热更新条件的,或者临时开发的项目不想走热更流程");
            //            //Not();
            //            break;
            //        case Defines.UpdateType.Online:
            //            //Info("在线更新需要目标平台支持热更,需要有网络环境,需要部署热更服务,如版本查询,补丁下载等");
            //            //Remote();
            //            break;
            //        case Defines.UpdateType.Offline:
            //            //Info("离线更新仅支持Windows,不需要网络环境和热更服务,Windows操作文件方便,直接拷补丁进去");
            //            //Local();
            //            break;
            //    }
            //}
            //EndFadeGroup();
        }
        Defines.UpdateType EnumPopup()
        {
            BootProfile profile = BootProfile.GetInstance();
            string[] displayNames = new string[]
            {
                "不更新",
                "在线更新",
                "离线更新",
            };
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("热更新模式:", EditorStyles.boldLabel, GUILayout.Width(100));

            profile.SerLoadMod((Defines.UpdateType)EditorGUILayout.EnumPopup(profile.AssemblyLoadType));
            EditorGUILayout.EndHorizontal();

            return BootProfile.GetInstance().AssemblyLoadType;
        }

    }

}
