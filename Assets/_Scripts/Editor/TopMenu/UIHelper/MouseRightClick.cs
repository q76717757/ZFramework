using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

namespace ZFramework
{
    public static class MouseRightClick
    {
        [MenuItem("GameObject/����UI����", true)]
        public static bool Init2()
        {
            return Selection.objects.Length == 1;
        }
        [MenuItem("GameObject/����UI����",false,50)]
        public static void Init()//ÿ��go���һ�� ��ô������??
        {
            Bulid(Selection.objects[0] as GameObject);
        }

        static void Bulid(GameObject hierarchyGO)
        {
            if (hierarchyGO == null) return;
            if (hierarchyGO.GetComponent<Canvas>() == null) return;

            switch (PrefabUtility.GetPrefabInstanceStatus(hierarchyGO))
            {
                case PrefabInstanceStatus.Connected://�Ѿ���Ԥ����
                    var preInstance = PrefabUtility.GetOutermostPrefabInstanceRoot(hierarchyGO);
                    SetBundleName(preInstance);
                    break;
                case PrefabInstanceStatus.Disconnected:
                case PrefabInstanceStatus.MissingAsset:
                case PrefabInstanceStatus.NotAPrefab:
                    BulidPrefab(hierarchyGO);
                    break;
                default:
                    return;
            }
            string viewName = hierarchyGO.name;

            CreateUIType(viewName);
            CreateComponentCS(viewName);
            CreateSystemCS(viewName);
        }

        static void SetBundleName(GameObject prefabInstance)
        {
            if (prefabInstance == null) return;
            var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefabInstance);
            AssetImporter prefab = AssetImporter.GetAtPath(assetPath);
            prefab.assetBundleName = prefabInstance.name.ToLower();
            prefab.assetBundleVariant = "unity3d";
        }

        static void BulidPrefab(GameObject hierarchyGO)
        {
            string savePath = $"Assets/_Prefabs/UI/{hierarchyGO.name}.prefab";

            var im = AssetImporter.GetAtPath(savePath);
            if (im != null)
            {
                Debug.LogError("�Ѿ�����������Ԥ����:" + savePath);
                return;
            }
            //savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);//ȷ�����Ʋ��ظ�
            if (PrefabUtility.GetPrefabInstanceStatus(hierarchyGO) == PrefabInstanceStatus.MissingAsset)
                PrefabUtility.UnpackPrefabInstance(hierarchyGO, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);

            var prefabGO = PrefabUtility.SaveAsPrefabAssetAndConnect(hierarchyGO, savePath, InteractionMode.AutomatedAction);
            SetBundleName(prefabGO);
        }

        static void CreateUIType(string scriptName)
        {
            string UITypeFilePath = Path.Combine(Application.dataPath, "../../Unity.Model/_Components/UIComponent/UIType.cs");
            if (File.Exists(UITypeFilePath))
            {
                string fileText = File.ReadAllText(UITypeFilePath);

                if (fileText.IndexOf(scriptName) > 0)
                {
                    Debug.Log("�Ѵ���ͬ��UIType");
                    return;
                }
                var index = fileText.LastIndexOf(";");
                fileText = fileText.Insert(index + 1, System.Environment.NewLine + $"        public const string {scriptName} = \"{scriptName}\";");
                File.WriteAllText(UITypeFilePath, fileText);
            }
        }
        static void CreateComponentCS(string scriptName)
        {
            string componentPath = Path.Combine(Application.dataPath, "../../Unity.Model/_Components/UICanvasComponent", scriptName);
            DirectoryInfo componentDirInfo = new DirectoryInfo(componentPath);
            if (!componentDirInfo.Exists)
            {
                componentDirInfo.Create();
            }

            string scriptFileName = $"{scriptName}_Component.cs";
            string scriptFileText = UIComponentTemp.Replace("#ScriptName#", scriptName);
            string scriptSavePath = Path.Combine(componentDirInfo.FullName, scriptFileName);

            if (!File.Exists(scriptSavePath))
            {
                File.WriteAllText(scriptSavePath, scriptFileText, Encoding.UTF8);
            }
            else
            {
                Debug.LogError($"����{scriptFileName}");
            }
            
        }

        static void CreateSystemCS(string scriptName)
        {
            string systemPath = Path.Combine(Application.dataPath, "../../Unity.Hotfix/_Systems/UICanvasSystem", scriptName);
            DirectoryInfo systemDirInfo = new DirectoryInfo(systemPath);
            if (!systemDirInfo.Exists)
            { 
                systemDirInfo.Create();
            }

            string scriptFileName = $"{scriptName}_System.cs";
            string scriptFileText = UISystemTemp.Replace("#ScriptName#", scriptName);
            string scriptSavePath = Path.Combine(systemDirInfo.FullName, scriptFileName);

            if (!File.Exists(scriptSavePath))
            {
                File.WriteAllText(scriptSavePath, scriptFileText, Encoding.UTF8);
            }
            else
            {
                Debug.LogError($"����{scriptFileName}");
            }
        }

























        static string UIComponentTemp = @"
using UnityEngine;
using System;

namespace ZFramework
{
    [UIType(UIType.#ScriptName#)]
    public class #ScriptName#_Component:UICanvasComponent
    {

    }
}
";

        static string UISystemTemp = @"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    //��������
    public class #ScriptName#Awake : AwakeSystem<#ScriptName#_Component>
    {
        public override void Awake(#ScriptName#_Component component)
        {

        }
    }
    public class #ScriptName#Update : UpdateSystem<#ScriptName#_Component>
    {
        public override void Update(#ScriptName#_Component component)
        {
        }
    }

    public class #ScriptName#_Show : UIShowSystem<#ScriptName#_Component>
    {
        public override void OnShow(#ScriptName#_Component canvas)
        {
        }
    }

    public class #ScriptName#_Hide : UIHideSystem<#ScriptName#_Component>
    {
        public override void OnHide(#ScriptName#_Component canvas)
        {
        }
    }

    //�߼�
    public static class #ScriptName#_System
    {

    }
}
";
    }
}
