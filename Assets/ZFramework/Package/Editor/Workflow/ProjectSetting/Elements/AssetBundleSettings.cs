using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
    public class AssetBundleSettings : ZFrameworkSettingsProviderElement<AssetBundleSettings>
    {
        [SettingsProvider] public static SettingsProvider Register() => GetInstance();


        public override void OnGUI()
        {
            SerializedProperty hotUpdateScenes = EdtiorSettings.FindProperty("hotUpdateScenes");
            EditorGUILayout.HelpBox("�ȸ�ģʽ��,�������������,������BootStrap�ĳ���\r\n���ȸ�ģʽ��,����Ҫ��������Ҫ�ĳ�������ӽ���", MessageType.Info);

            int len = hotUpdateScenes.arraySize;
            EditorGUI.BeginChangeCheck();//ͨ����ק������ķ�ʽ�������Ԫ�� ��鲻���仯 ��¼���������������BUG
            EditorGUILayout.PropertyField(hotUpdateScenes);
            if (EditorGUI.EndChangeCheck() || len != hotUpdateScenes.arraySize)
            {
                ZFrameworkEditorSettings.Save();
            }

            if (GUILayout.Button("��ʱ���AB����StreamingAssets"))
            {
                BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
            }
        }

    }
}
