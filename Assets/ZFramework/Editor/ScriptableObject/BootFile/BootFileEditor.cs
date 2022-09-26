using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

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
            EditorGUILayout.LabelField("PlayerSetting");
            EditorGUI.BeginDisabledGroup(true);//HyCLRҪ�� IL2CPP��� �ر�����GC
            EditorGUILayout.LabelField("������");
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            ScriptingImplementation banckendType = PlayerSettings.GetScriptingBackend(buildTargetGroup);
            EditorGUILayout.EnumPopup(banckendType);
            EditorGUILayout.LabelField("C++����������");
            var com = PlayerSettings.GetIl2CppCompilerConfiguration(buildTargetGroup);
            EditorGUILayout.EnumPopup(com);
            EditorGUILayout.LabelField("����GC");
            EditorGUILayout.Toggle(PlayerSettings.gcIncremental);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(10);

            if (serializedObject == null) return;

            GUILayout.Label("������Ŀʵ���������ѡ��");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Ŀ��ƽ̨:");
            var platform = serializedObject.FindProperty("platform");
            platform.enumValueIndex = (int)(Platform)EditorGUILayout.EnumPopup((Platform)platform.enumValueIndex);
            var selectPlatform = (Platform)platform.enumValueIndex;
            EditorGUILayout.EndHorizontal();

            switch (selectPlatform)
            {
                case Platform.Win:
                case Platform.Mac:
                case Platform.Andorid:
                case Platform.IOS:
                    EditorGUILayout.HelpBox("֧���ȸ��µ�ƽ̨", MessageType.None);
                    break;
                case Platform.WebGL:
                case Platform.UWP:
                    EditorGUILayout.HelpBox("��֧���ȸ��µ�ƽ̨", MessageType.Warning);
                    break;
            }



            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("���·�ʽ");
            var network = serializedObject.FindProperty("network");
            network.enumValueIndex = (int)(Network)EditorGUILayout.EnumPopup((Network)network.enumValueIndex);
            var selectNetwork = (Network)network.enumValueIndex;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("��PCƽ̨�б���ģʽ,ֱ��ͨ�����ض�����,һЩ����PC��Ŀ", MessageType.None);//������������������� ��ô����Ҳ����ʵ�� ��ЩС��Ŀʱû�з���˵� ����Ҫ���ظ���

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("��������"); //֧������ = ����ɰ汾����   ��֧������ = ���뱣������
            EditorGUILayout.EndHorizontal();

            //֧�ָ���                   ��֧�ָ���
            //���ظ��� = ������           Զ�̸��� = ������
            if (serializedObject.ApplyModifiedProperties())
            {
                AssetDatabase.SaveAssetIfDirty(serializedObject.targetObject);
            }
        }


        protected override bool ShouldHideOpenButton() => true;
    }
}
