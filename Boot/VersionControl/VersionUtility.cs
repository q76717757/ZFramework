using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZFramework
{
    public static class VersionUtility
    {
        //boot层随包,如果boot层有修改,则增加boot层的版本号
        //客户端更新时如果发现boot层版本不一致,将直接拒接热更,
        public static VersionNum BootVersion => new VersionNum(0, 1, 0);


        public static VersionNum GetCodeVersion()
        {
            var remoteFilePath = Application.streamingAssetsPath;
            return default;
        }

        public static VersionNum GetAssetsVersion()
        {
            return default;
        }
    }
}
