using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    [CreateAssetMenu(fileName = "BootFile", menuName = "ZFramework/引导文件", order = 0)]
    public class BootFile : ScriptableObject
    {
        /// <summary> 项目代号 </summary>
        public string ProjectCode;

        public string 内置路径;
        public string 缓存路径;
        public string 下载路径;//资源的路径  具体这个是什么路径 则通过globalconfig来定义了  


        public Platform platform;//这些移到globalconfig上
        public Network network;

        public bool allowOffline;

        //为了实现一个工程  能启动做个工程出的包  环境一样的前提下
        //boot定义了资源的具体路径 一般按照项目名称来定义,   程序集明明则使用DATA(程序集类型)_PROJECTNAME(项目代号)_TICK(编译时间戳) 这样的方式来命名
        //存在一个场景切换到另一个场景  又切换回来旧的场景   这时候程序集其实可以不需要重复加载,   在程序集加载器中以字典形式缓存程序集(反正不能写在 直接占死在内存中)
        //在加载场景之前先判断一下在不在缓存中  在的话直接启动,启动后在执行更新流程,如果成功执行更新流程,则重新加载新版本程序集,并把名字覆盖掉
    }
}
