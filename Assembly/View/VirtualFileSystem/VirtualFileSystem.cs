using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using System;
using Object = UnityEngine.Object;
using System.Linq;

namespace ZFramework
{
    /// <summary>
    /// 虚拟文件系统    
    /// 模拟一个运行时磁盘  要获取某些资源的时候  想从本地磁盘读文件一样  从系统中读文件出来,IO操作由文件系统在底层全部处理完 如远程下载 版本对比 加载依赖等
    /// 用法参照Resources.Load    Resources怎么用  VFS就怎么用
    /// </summary>
    public static class VirtualFileSystem
    {
        static TreeModel<VFSTreeElement> treeModel;
        static Dictionary<string, AssetBundle> loadingBundles = new Dictionary<string, AssetBundle>();//已经加载的ab包 //封装一个壳 登记引用计数
        static VFSProfile profile;
        static IFileServer fileServer;

        public static void Init(IFileServer fileServer)
        {
            profile = VFSProfile.GetInstance();
            treeModel = new TreeModel<VFSTreeElement>(profile.elements);
            VirtualFileSystem.fileServer = fileServer;
        }
        private static VFSMetaData GetMetaData(string path)
        {
            var element = treeModel.GetElement(path);
            if (element != null)
            {
                return element.data;
            }
            else
            {
                return null;
            }
        }

        //预下载所有的包
        public static async ATask PreDownloadAllBundles()
        {
            var bundleInfos = profile.manifest.GetBundles();
            foreach (var item in bundleInfos)
            {
                await DownloadBundle(item);
            }
        }
        //预下载某个资源所属的包(递归)
        public static async ATask PreDownloadAssets(string path)
        {
            VFSMetaData metaData = GetMetaData(path);
            string assetGuid = metaData.guid;
            BundleInfo bundleInfo = profile.manifest.GetBundleInfoByAssetGuid(assetGuid);//所属的包
            await PreDownloadBundle(bundleInfo.bundleName);
        }
        //预下载某个包(递归)
        public static async ATask PreDownloadBundle(string bundleName)
        {
            var bundleNames = profile.manifest.GetAllDependencies(bundleName);
            foreach (var item in bundleNames)
            {
                await DownloadBundle(profile.manifest.GetBundleInfoByBundleName(item));
            }
        }
        //下载某个包(不递归)
        private static async ATask DownloadBundle(BundleInfo bundleInfo)
        {
            var remoteFilePath = $"{BootStrap.projectCode}/{Defines.TargetRuntimePlatform}/VFS/{bundleInfo.FileName}";
            Directory.CreateDirectory($"{Defines.PersistenceDataAPath}/VFS");
            var saveFilePath = $"{Defines.PersistenceDataAPath}/VFS/{bundleInfo.FileName}";
            Log.Info($"下载AB包-->{bundleInfo.bundleName}");
#if !UNITY_EDITOR
            await ATask.CompletedTask;
#else
            if (IsReady(bundleInfo))
            {
                Log.Info("已经下载过的包,跳过");
                await ATask.CompletedTask;
            }
            else
            {
                using (FileStream fs = new FileStream(saveFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    await fileServer.DownloadFileRange(remoteFilePath, fs);
                }
            }
#endif
        }

        //某个资源是否准备就绪(指已经下载好了)
        private static bool IsReady(BundleInfo bundleInfo)
        {
            var filePath = $"{Defines.PersistenceDataAPath}/VFS/{bundleInfo.FileName}";//持久路径
            var filePath2 = $"{Defines.BuildInAssetAPath}/VFS/{bundleInfo.FileName}";//内置路径
            return File.Exists(filePath) || File.Exists(filePath2);
        }




        //Asset相关
        public static async ATask<T> LoadAsync<T>(string path) where T : Object
        {
            return (T)await LoadAsync(path, typeof(T));
        }
        public static async ATask<Object> LoadAsync(string path, Type assetType)
        {
            VFSMetaData metaData = GetMetaData(path);
            if (metaData == null || !metaData.IsAsset)
            {
                return null;
            }
            string guid = metaData.guid;
#if UNITY_EDITOR  //区分Runtime和Editor
            var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            return UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, assetType);
#endif
            //先通过guid找到资源所在的包
            Log.Info($"加载资源->{metaData.guid}:{metaData.path}");
            BundleInfo bundleInfo = profile.manifest.GetBundleInfoByAssetGuid(metaData.guid);
            Log.Info($"所属ab包->{bundleInfo.bundleName}");


            ////从ab包加载资源
            AssetBundle bundle = await LoadAssetBundle(bundleInfo.bundleName);
            Object asset = bundle.LoadAsset(metaData.path, assetType);
            return asset;
        }
        public static async ATask<T[]> LoadAllAsync<T>(string path) where T : Object
        {
            return (await LoadAllAsync(path, typeof(T))).Cast<T>().ToArray();
        }
        public static ATask<Object[]> LoadAllAsync(string path, Type type) => throw new NotImplementedException();
        public static void UnloadAsset(string path) => throw new NotImplementedException();
        public static void UnloadAsset(Object asset) => throw new NotImplementedException();
        public static void UnloadUnusedAssets() => throw new NotImplementedException();





        //Scene相关
        public static async ATask LoadSceneBundle(string scenePath)
        {
            VFSMetaData metaData = GetMetaData(scenePath);
            if (metaData == null || !metaData.IsAsset)
            {
                return;
            }
            BundleInfo bundleInfo = profile.manifest.GetBundleInfoByAssetGuid(metaData.guid);
            var ab = await LoadAssetBundle(bundleInfo.bundleName);//场景包
            var scenePaths = ab.GetAllScenePaths();
            Log.Info("场景包?->" + ab.isStreamedSceneAssetBundle);
            foreach (var item in scenePaths)
            {
                Log.Info("ScenePath->" + item);
            }
            SceneManager.LoadScene(scenePaths[0]);
        }





        //Bundle相关
        static async ATask<AssetBundle> LoadAssetBundle(string bundleName)
        {
            BundleInfo bundleInfo = profile.manifest.GetBundleInfoByBundleName(bundleName);
            Log.Info("加载AB包-->" + bundleInfo.bundleName);
            //已加载的ab包
            if (loadingBundles.TryGetValue(bundleInfo.bundleName, out AssetBundle bundle))
            {
                return bundle;
            }
            else
            {
                //尝试下载
                await DownloadBundle(bundleInfo);

                var dependence = bundleInfo.dependencies;
                foreach (var item in dependence)
                {
                    await LoadAssetBundle(item);//递归加载依赖包
                }
                Stream stream = await GetFileStreamFromBundleName(bundleInfo.bundleName);
                AssetBundle b = AssetBundle.LoadFromStream(stream);
                stream.Close();
                loadingBundles.Add(bundleName, b);
                return b;
            }
        }
        static async ATask<Stream> GetFileStreamFromBundleName(string bundleName)
        {
            BundleInfo bundleInfo = profile.manifest.GetBundleInfoByBundleName(bundleName);
            var pathA = $"{Defines.PersistenceDataAPath}/VFS/{bundleInfo.FileName}";//持久目录的路径
            if (File.Exists(pathA))
            {
                return new FileStream(pathA, FileMode.Open, FileAccess.Read);
            }
            var pathB = $"{Defines.BuildInAssetAPath}/VFS/{bundleInfo.FileName}";//内建目录的路径
            if (File.Exists(pathB))
            {
                return new FileStream(pathB, FileMode.Open, FileAccess.Read);
            }
            var pathC = $"{BootStrap.projectCode}/{Defines.TargetRuntimePlatform}/VFS/{bundleInfo.FileName}";//COS的路径
            using (FileStream fs = new FileStream(pathA, FileMode.OpenOrCreate, FileAccess.Write))
            {
                await fileServer.DownloadFileRange(pathC, fs);
            }
            return new FileStream(pathA, FileMode.Open, FileAccess.Read);
        }





        //待实现API清单
        static void AssetBundleAPI()
        {
            //不对外暴露AssetBundle对象
            //只对外返回加载出来的对象

            ////从AB包加载资源
            //AssetBundle ab;
            //ab.isStreamedSceneAssetBundle
            //ab.GetAllAssetNames();
            //ab.GetAllScenePaths();
            //ab.LoadAllAssets();
            //ab.LoadAllAssets<T>();
            //ab.LoadAllAssetsAsync();
            //ab.LoadAllAssetsAsync<T>;
            //ab.Unload();
            //ab.UnloadAsync();
        }
    }
}
