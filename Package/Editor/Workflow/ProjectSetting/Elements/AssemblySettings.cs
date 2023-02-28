using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

#if ENABLE_HYBRIDCLR
using HybridCLR.Editor;
using HybridCLR.Editor.Installer;
#endif

namespace ZFramework.Editor
{
    public class AssemblySettings : ZFrameworkSettingsProviderElement<AssemblySettings>
    {
        [SettingsProvider] public static SettingsProvider Register() => GetInstance();
        GUILayoutOption titleWidth = GUILayout.Width(100);

        SerializedProperty hotUpdateType;
        SerializedProperty assemblyNames;
        SerializedProperty aotMetaAssemblyNames;

        public override void OnEnable()
        {
            hotUpdateType = RuntimeSettings.FindProperty("hotUpdateType");
            assemblyNames = RuntimeSettings.FindProperty("assemblyNames");
            aotMetaAssemblyNames = RuntimeSettings.FindProperty("aotMetaAssemblyNames");
        }
        public override void OnDisable()
        {
            hotUpdateType.Dispose();
            assemblyNames.Dispose();
            aotMetaAssemblyNames.Dispose();
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("�����ȸ��·���: HybridCLR", EditorStyles.boldLabel);
            if (GUILayout.Button("�ٷ��ĵ�:https://focus-creative-games.github.io/hybridclr/"))
            {
                Application.OpenURL("https://focus-creative-games.github.io/hybridclr/");
            }
            EditorGUILayout.Space();

            HotUpdateType type = EnumPopup();
            switch (type)
            {
                case HotUpdateType.Online:
                    ShowModInfo("���߸�����ҪĿ��ƽ̨֧���ȸ�,��Ҫ�����绷��,��Ҫ�����ȸ�����,��汾��ѯ,�������ص�");
                    Remote();
                    break;
                case HotUpdateType.Offline:
                    ShowModInfo("���߸��½�֧��Windows,����Ҫ���绷�����ȸ�����,Windows�����ļ�����,ֱ�ӿ�������ȥ");
                    Local();
                    break;
                case HotUpdateType.Not:
                    ShowModInfo("��Բ��߱��ȸ���������,������ʱ��������Ŀ�������ȸ�����");
                    Not();
                    break;
            }

            if (RuntimeSettings.ApplyModifiedProperties())
            {
                AssetDatabase.SaveAssets();
            }
        }
        HotUpdateType EnumPopup()
        {
            string[] displayNames = new string[]
            {
                "���߸���",
                "���߸���",
                "������",
            };
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("�ȸ���ģʽ:", EditorStyles.boldLabel, titleWidth);
            hotUpdateType.enumValueIndex = EditorGUILayout.Popup(hotUpdateType.enumValueIndex, displayNames/* loadType.enumDisplayNames*/);
            EditorGUILayout.EndHorizontal();

            return (HotUpdateType)hotUpdateType.enumValueIndex;
        }
        void ShowModInfo(string info)
        {
            EditorGUILayout.HelpBox(info, MessageType.Info);
            EditorGUILayout.Space();
        }

        void Remote()
        {
            if (CheckHybridCLR())
            {
                CheckAssemblySetting();
            }
        }
        void Local()
        {
            if (CheckHybridCLR())
            {
                CheckAssemblySetting();
            }
        }
        void Not()
        {
            if (!Defines.HybridCLRPackageIsInstalled || IsDisable())
            {
                DrawAssembly(assemblyNames);
                if (GUILayout.Button("���õ�Ĭ��"))
                {
                    ResetAssembly(assemblyNames, Defines.DefaultAssemblyNames);
                }
            }
            
        }



        bool CheckHybridCLR()
        {
            return CheckPackage() && IsFullInstall() && IsEnable() && CheckTargetPlatfrom() && CheckOtherSettings();//�ж�����˳���
        }
        bool CheckPackage()
        {
            if (Defines.HybridCLRPackageIsInstalled)
            {
                return true;
            }
            else
            {
                EditorGUILayout.LabelField("HybridCLR Package(δ��װ)",EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("����ͨ��Package Manager��Add package from git URL��װ", MessageType.Error);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("URL:", GUILayout.Width(50));
                EditorGUILayout.TextField("git@gitee.com:focus-creative-games/hybridclr_unity.git");
                EditorGUILayout.EndHorizontal();

                return false;
            }
        }
        bool IsFullInstall()
        {
            var isFull = new InstallerController().HasInstalledHybridCLR();
            if (!isFull)//δ������װ
            {
                EditorGUILayout.HelpBox("HybridCLR��Ϊ3����\r\n1.Package(�Ѱ�װ)\r\n2.HybrldCLR(δ��װ)\r\n3.Il2CPP Plus(δ��װ)", MessageType.Error);
                if (GUILayout.Button("��HybridCLR Installer������ɰ�װ"))
                {
                    InstallerWindow window = EditorWindow.GetWindow<InstallerWindow>("HybridCLR Installer", true);
                    window.minSize = new Vector2(800f, 500f);
                }
            }
            return isFull;
        }
        bool IsEnable()
        {
            if (SettingsUtil.Enable)
            {
                return true;
            }
            else
            {
                EditorGUILayout.HelpBox("HybridCLRδ����,��ѡ���ȸ�ģʽ����Ҫ����HybridCLR", MessageType.Error);
                if (GUILayout.Button("�������"))
                {
                    SettingsUtil.Enable = true;
                }
                return false;
            }
        }
        bool IsDisable()
        {
            if (!SettingsUtil.Enable)
            {
                return true;
            }
            else
            {
                EditorGUILayout.HelpBox("HybridCLR�ѿ���,��ѡ���ȸ�ģʽ����Ҫ�ر�HybridCLR", MessageType.Error);
                if (GUILayout.Button("����ر�"))
                {
                    SettingsUtil.Enable = false;
                }
                return false;
            }
            
        }
        bool CheckTargetPlatfrom()
        {
            var target = Defines.TargetRuntimePlatform;
            var select =  (HotUpdateType)hotUpdateType.enumValueIndex;
            switch (select)
            {
                case HotUpdateType.Online:
                    if (target == (target & PlatformType.OnlineSupported))
                    {
                        return true;
                    }
                    else
                    {
                        Show(target.ToString());
                        EditorGUILayout.HelpBox("Ŀ��ƽ̨��֧�����߸���ģʽ,Ĭ��ֻ֧��Windows/Android/iOS", MessageType.Error);
                        if (GUILayout.Button("�ر��ȸ��¹���"))
                        {
                            hotUpdateType.enumValueIndex = (int)HotUpdateType.Not;
                            SettingsUtil.Enable = false;
                        }
                    }
                    break;
                case HotUpdateType.Offline:
                    if (target == (target & PlatformType.OfflineSupported))
                    {
                        return true;
                    }
                    else
                    {
                        Show(target.ToString());
                        EditorGUILayout.HelpBox("Ŀ��ƽ̨��֧�����߸���,Ĭ��ֻ֧��Windows", MessageType.Error);
                        if (GUILayout.Button("�ر��ȸ��¹���"))
                        {
                            hotUpdateType.enumValueIndex = (int)HotUpdateType.Not;
                            SettingsUtil.Enable = false;
                        }
                    }
                    break;
                case HotUpdateType.Not:
                    return true;
            }
            return false;

            void Show(string label)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Ŀ��ƽ̨:", EditorStyles.boldLabel, titleWidth);
                EditorGUILayout.LabelField(label);
                EditorGUILayout.EndHorizontal();
            }
        }
        bool CheckOtherSettings()//���HybridҪ�������
        {
            CheckGC();
            CheckIL2CPP();
            //CheckAPILevel();
            return true;
        }
        bool CheckGC()
        {
            var gc = PlayerSettings.gcIncremental;
            if (gc)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("����GC:", EditorStyles.boldLabel, titleWidth);
                EditorGUILayout.LabelField("�ѿ���");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("HybridCLR�ݲ�֧������GC,��Ҫ�ر�����GC����", MessageType.Error);
                if (GUILayout.Button("�ر�����GC����"))
                {
                    PlayerSettings.gcIncremental = false;
                }
            }
            return !gc;
        }
        bool CheckIL2CPP()
        {
            var il2cpp = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (il2cpp != ScriptingImplementation.IL2CPP)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("������:", EditorStyles.boldLabel, titleWidth);
                EditorGUILayout.LabelField(il2cpp.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("HybridCLR�ǻ���IL2CPP��,��Ҫ�������˸�ΪIL2CPP", MessageType.Error);
                if (GUILayout.Button("�л���IL2CPP"))
                {
                    PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.IL2CPP);
                }
            }
            return il2cpp == ScriptingImplementation.IL2CPP;
        }
        //void CheckAPILevel()
        //{
        //    var apiLevel = PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup);
        //    if (apiLevel != ApiCompatibilityLevel.NET_4_6)
        //    {
        //        EditorGUILayout.BeginHorizontal();
        //        EditorGUILayout.LabelField("Ŀ����:", EditorStyles.boldLabel, titleWidth);
        //        EditorGUILayout.LabelField(apiLevel.ToString());
        //        EditorGUILayout.EndHorizontal();

        //        EditorGUILayout.HelpBox("�����̿���ѡ��Standard 2.0����4.x,�������ȸ����򼯱���ʹ��4.x,���ޱ�Ҫ����ͳһ��4.x", MessageType.Warning);
        //    }
        //}

        void CheckAssemblySetting()
        {
            DrawAssembly(assemblyNames);
            CheckAssembly();
            DrawAssembly(aotMetaAssemblyNames);
            if (GUILayout.Button("���õ�Ĭ��"))
            {
                ResetAssembly(assemblyNames, Defines.DefaultAssemblyNames);
                ResetAssembly(aotMetaAssemblyNames, Defines.DefaultAOTMetaAssemblyNames);
            }
        }
        void DrawAssembly(SerializedProperty assembly)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(assembly);
            if (EditorGUI.EndChangeCheck())
            {
                AssetDatabase.SaveAssets();
            }
        }
        void CheckAssembly()
        {
            string[] assemblyNameValues = new string[assemblyNames.arraySize];
            for (int i = 0; i < assemblyNameValues.Length; i++)
            {
                assemblyNameValues[i] = assemblyNames.GetArrayElementAtIndex(i).stringValue;
            }
            List<string> clr = new List<string>();
            if (HybridCLRSettings.Instance.hotUpdateAssemblies != null)
                clr.AddRange(HybridCLRSettings.Instance.hotUpdateAssemblies);
            if (HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions != null)
                clr.AddRange(HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions.Where((a) => a != null).Select((a) => a.name));


            if (!ComparisonArray(clr, assemblyNameValues))
            {
                EditorGUILayout.HelpBox("�ȸ������б���HybridCLR��һ��", MessageType.Error);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("������HybridCLR"))
                {
                    HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions = new UnityEditorInternal.AssemblyDefinitionAsset[0];
                    HybridCLRSettings.Instance.hotUpdateAssemblies = assemblyNameValues;
                    HybridCLRSettings.Save();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        void ResetAssembly(SerializedProperty assembly, string[] defVals)
        {
            assembly.ClearArray();
            for (int i = 0; i < defVals.Length; i++)
            {
                assembly.InsertArrayElementAtIndex(i);
                assembly.GetArrayElementAtIndex(i).stringValue = defVals[i];
            }
        }

        void AssemblyBytesLoadPath()
        {

        }




        bool ComparisonArray(IEnumerable<string> item1, IEnumerable<string> item2)
        {
            if (item1 == item2) return true;
            if (item1 == null || item2 == null) return false;
            if (item1.Count() != item2.Count()) return false;

            var enum1 = item1.GetEnumerator();
            var enum2 = item2.GetEnumerator();
            while (true)
            {
                if (enum1.MoveNext() && enum2.MoveNext())
                {
                    if (enum1.Current != enum2.Current)
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
               
            }
        }
    }
}
