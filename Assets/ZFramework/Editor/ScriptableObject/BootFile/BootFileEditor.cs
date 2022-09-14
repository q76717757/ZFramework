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
            var boot = (target as BootFile);

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

            //Ŀ��ƽ̨  win android ios  webgl uwp
            //         [     �ȸ�     ] [ ���ȸ� ]
            var targetPlatform = (Platform)serializedObject.FindProperty("platform").enumValueIndex;
            serializedObject.FindProperty("platform").enumValueIndex = (int)(Platform)EditorGUILayout.EnumPopup(targetPlatform);

            serializedObject.FindProperty("useHotfix").intValue = EditorGUILayout.Popup(serializedObject.FindProperty("useHotfix").intValue, new string[] { "��Ӧ��", "Ӧ��" });

            if (serializedObject.FindProperty("useHotfix").intValue == 1)
            {
                serializedObject.FindProperty("hotfixType").intValue = EditorGUILayout.Popup(serializedObject.FindProperty("hotfixType").intValue, new string[] { "����", "Զ��" });
            }
            else
            {

            }

            //��ʱ��
            if (true)//ǰհ����Ŀ winƽ̨ mono���  not HyCLY
            {

            }

            //core����unity������  ��winƽ̨��  ���Ժ�il2cppһ��������  �������core����  (��Ҫ����)?
            //����������ƽ̨Ҫ����core  ��ҪHyCLR��֧��
            //����Core�����湤�� ���Ҳ��ܸ���

            if (true)//֧���ȸ���ƽ̨
            {
                if (true)//��Ҫ�ȸ���
                {
                    if (true)//�Ѿ����HyCLR������
                    {
                        if (true)//��Զ�̻���
                        {

                        }
                        else//û��Զ�̻��� ���ظ���
                        {

                        }
                    }
                    else//Ϊ�����ȸ�����
                    {

                    }
                }
                else//����Ҫ�ȸ���
                {

                }
            }
            else//��֧�ֵ�ƽ̨
            {

            }


            if (serializedObject.ApplyModifiedProperties())
            {
                AssetDatabase.SaveAssetIfDirty(serializedObject.targetObject);
            }
        }


        protected override bool ShouldHideOpenButton() => true;

    }
}
