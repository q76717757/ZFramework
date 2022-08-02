using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public class BundleComponent : Entity
    {
        public Dictionary<string, BundleInfo> BundleInfos;//所有AB包

    }

    public class BundleInfo//单个AB包的依赖信息
    {
        public Dictionary<string, BundleInfo> Dependencies;//直接依赖

        public BundleInfo[] depens;
    }
}
