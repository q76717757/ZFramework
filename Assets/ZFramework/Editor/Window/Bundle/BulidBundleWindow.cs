using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

namespace ZFramework
{
    public class BulidBundleWindow : EditorWindow
    {
        [MenuItem("ZFramework/打包工具")]
        static void Open() {
            var window = GetWindow<BulidBundleWindow>("打包工具");
            window.minSize = new Vector2(200, 200);
            window.Show();
        }

        string outPath;
        BuildTarget target;
        BuildAssetBundleOptions option;

        bool copyToStreamingAsset = true;
        bool copyToRucieHSF = false;

        private void OnEnable()
        {
            outPath = Path.Combine(Application.dataPath, "_Bundles").Replace("\\", "/");
            target = EditorUserBuildSettings.activeBuildTarget;
            option = BuildAssetBundleOptions.None;
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.TextField("输出路径", outPath);
            if (GUILayout.Button("选择路径",GUILayout.Width(100)))
            {
                var newPath = EditorUtility.OpenFolderPanel("输出路径", Application.dataPath, "_Bundles");
                if (!string.IsNullOrEmpty(newPath))
                {
                    outPath = newPath;
                    EditorGUI.FocusTextInControl("");
                }
            }
            if (GUILayout.Button("打开", GUILayout.Width(100)))
            {
                WindowsEx.OpenFolder(outPath);
            }
            GUILayout.EndHorizontal();

            target = (BuildTarget)EditorGUILayout.EnumPopup("目标平台", target);
            option = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("选项", option);

            EditorGUILayout.LabelField("ChunkBasedCompression	创建 AssetBundle 时使用基于语块的 LZ4 压缩。");
            EditorGUILayout.LabelField("DryRunBuild 进行预演构建。(对 AssetBundle 进行预演构建，但不实际构建它们。仍将返回 AssetBundleManifest 对象)");

            copyToStreamingAsset = GUILayout.Toggle(copyToStreamingAsset, "复制到StreamingAsset");
            copyToRucieHSF = GUILayout.Toggle(copyToRucieHSF, "复制到Rucie.HFS");

            EditorGUI.BeginDisabledGroup(!Directory.Exists(outPath));
            if (GUILayout.Button("构建AB包"))
            {
                var abm = BuildPipeline.BuildAssetBundles(outPath, option, target);
                BuildSuccess(abm);
            }
            EditorGUI.EndDisabledGroup();
        }

        void BuildSuccess(AssetBundleManifest abm) {
            if (abm == null) return;
            //var abs = abm.GetAllAssetBundles();
            //if (abs != null)
            //{
            //    foreach (var item in abs)
            //    {
            //        Debug.Log(" -> " + item);
            //        foreach (var ad in abm.GetAllDependencies(item))
            //        {
            //            Debug.Log("依赖 -> " + ad);
            //        }
            //    }
            //}
            //Debug.Log("Build Bundle Success");

            DirectoryInfo directoryInfo = new DirectoryInfo(outPath);
            var files = directoryInfo.GetFiles("*.unity3d");
            StringBuilder sb = new StringBuilder();
            foreach (var file in files)
            {
                var md5 = MD5Helper.FileMD5(file.FullName);
                sb.AppendLine($"{file.Name}:{md5}");
            }
            File.WriteAllText(Path.Combine(outPath, "md5.ini"), sb.ToString());

            if (copyToStreamingAsset)
            {
                DirectoryInfo s = new DirectoryInfo(Application.streamingAssetsPath + "/Bundle");
                var abs = abm.GetAllAssetBundles();
                foreach (var bundle in abs)
                {
                    File.Copy(outPath + "/" + bundle, s.FullName + "/" + bundle, true);
                }
                File.Copy(outPath + "/_Bundles", s.FullName + "/_Bundles", true);
                File.Copy(outPath + "/md5.ini", s.FullName + "/md5.ini", true);
            }

            if (copyToRucieHSF)
            {
                string hsfPath = @"D:\Users\Desktop\Http File Server 2.3i Build 297\HFS Using Files\Downloads";
                var abs = abm.GetAllAssetBundles();
                foreach (var bundle in abs)
                {
                    File.Copy(outPath + "/" + bundle, hsfPath + "/" + bundle, true);
                }
                File.Copy(outPath + "/_Bundles", hsfPath + "/_Bundles", true);
                File.Copy(outPath + "/md5.ini", hsfPath + "/md5.ini", true);
            }

            AssetDatabase.Refresh();
            Debug.Log("Build Bundle Success");
        }

    }
}