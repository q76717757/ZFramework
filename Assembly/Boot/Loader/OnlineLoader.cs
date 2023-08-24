using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    public class OnlineLoader : IAssemblyLoader
    {
        public Type[] LoadAssembly(string[] assemblyNames)
        {
            if ((Defines.TargetRuntimePlatform & Defines.PlatformType.OnlineSupported) == 0)
            {
                throw new Exception("不支持的平台");
            }

            //获取持久化清单
            if (true)//清单有效  (不是第一次启动,第一次启动后会把清单复制到持久化目录下)
            {
                //对比文件
                if (true)//文件合法
                {
                    //补充元数据
                    //HybridCLR_API.LoadMetadataForAOTAssembly(assemblyNames);
                    //加载JIT.ab
                    //加载程序集
                }
            }

            if (true)//持久化清单无效   (第一次启动,这时候还没有持久化清单)
            {
                
            }
            else
            {
                //获取只读清单
                if (true)//只读清单有效
                {
                    //对比文件
                    if (true)//文件有效
                    {
                        //补充元数据
                        //加载JIT.ab
                        //加载程序集
                    }
                    else
                    {
                        //启动失败
                    }
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
