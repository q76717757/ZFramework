using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    public class OfflineLoader : IAssemblyLoader
    {
        public Type[] LoadAssembly(string[] assemblyNames)
        {
            if ((Defines.TargetRuntimePlatform & Defines.PlatformType.OfflineSupported) == 0)
            {
                throw new Exception("不支持的平台");
            }

            //先判断持久化清单
            if (Directory.Exists(Defines.PersistenceDataAPath))
            {
                Log.Info("持久化目录存在->启动");
            }
            else//第一次启动,持久化目录还不存在
            {

            }

            //查找持久化清单
            if (true)//持久化清单存在
            {
                //对比文件  (离线模式的资产是全量的)
                if (true)//文件完整
                {
                    //补充元数据
                    //HybridCLR_API.LoadMetadataForAOTAssembly(assemblyNames);
                    //加载JIT.ab
                    //加载程序集
                }
                else
                {
                    //文件损坏 启动失败
                }
            }
            else//第一次启动持久化目录还不存在
            {
                if (true)//查找随包资源
                {
                    //将随包资源移动到持久化目录
                }
                else//
                {
                    //文件损坏 启动失败
                }
            }

            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assemblyNames.Contains(assembly.GetName().Name))
                {
                    allTypes.AddRange(assembly.GetTypes());
                }
            }
            return allTypes.ToArray();
        }



    }
}
