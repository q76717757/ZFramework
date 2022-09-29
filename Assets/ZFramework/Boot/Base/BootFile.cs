using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
 
namespace ZFramework
{
    public enum Network
    {
        None,
        Exist,
        Unknow,
    }
    public enum Platform
    {
        Win,
        Mac,
        Andorid,
        IOS,
        WebGL,
        UWP
    }

    /// <summary>
    /// 项目特定的属性  
    /// </summary>
    [CreateAssetMenu(fileName = "BootFile", menuName = "ZFramework/引导文件", order = 0)]
    public class BootFile : ScriptableObject
    {
        public string ProjectCode;//项目代号
        public string ProjectName;//项目名称
        public string AssemblyVersion = new VersionInfo().ToString();//程序集版本号

        public Platform platform;//目标平台
        public Network network;//网络环境
        public bool allowOffline;//允许离线

        public string buildInPath;
        public string cachePath;
        public string downloadPath;





        public RenderPipelineAsset renderPipelineAsset;
    }
}
