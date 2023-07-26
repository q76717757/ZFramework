using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

namespace ZFramework.Editor
{
    //当工程是多平台的复合工程的时候,StreamingAssets下会存放包括其他目标平台的一些文件,如配置,资源等
    //将这些文件和文件夹添加.前缀,并移到Assets/下  (.开头的会被unity排除,认为是隐藏文件)
    //等构建完成后再给移回来,这样不是目标平台的文件就不会被打包出去了
    public class IgnoreStreamingAssets : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private class MoveJob
        {
            public string sourceDir;
            public string sourceMeta;
            public string movetoDir;
            public string movetoMeta;

            public MoveJob(Defines.PlatformType platform)
            {
                var directory = new DirectoryInfo(Defines.GetBuildInAssetsAPath(platform));
                var meta = new FileInfo(AssetDatabase.GetTextMetaFilePathFromAssetPath(directory.FullName));

                sourceDir = directory.FullName;
                sourceMeta = meta.FullName;
                movetoDir = new DirectoryInfo(Path.Combine(Application.dataPath, "." + directory.Name)).FullName;
                movetoMeta = new FileInfo(Path.Combine(Application.dataPath, "." + meta.Name)).FullName;
            }

            public void Do()
            {
                DirectoryMove(sourceDir, movetoDir);
                FileMove(sourceMeta, movetoMeta);
            }
            public void Undo()
            {
                DirectoryMove(movetoDir, sourceDir);
                FileMove(movetoMeta, sourceMeta);
            }

            void DirectoryMove(string a, string b)
            {
                if (Directory.Exists(a))
                {
                    if (Directory.Exists(b))
                    {
                        throw new Exception("已存在的文件夹,移动失败:" + movetoDir);
                    }
                    Directory.Move(a, b);
                }
            }
            void FileMove(string a, string b)
            {
                if (File.Exists(a))
                {
                    if (File.Exists(b))
                    {
                        throw new Exception("已存在文件,移动失败:" + movetoMeta);
                    }
                    File.Move(a, b);
                }
            }
        }

        public int callbackOrder => 0;
        List<MoveJob> jobs;
        public void OnPreprocessBuild(BuildReport report)
        {
            jobs = new List<MoveJob>();
            foreach (Defines.PlatformType platform in Enum.GetValues(typeof(Defines.PlatformType)))
            {
                if (platform != Defines.TargetRuntimePlatform)
                {
                    jobs.Add(new MoveJob(platform));
                }
            }
            foreach (MoveJob job in jobs)
            {
                job.Do();
            }
            AssetDatabase.Refresh();
        }
        public void OnPostprocessBuild(BuildReport report)
        {
            foreach (MoveJob job in jobs)
            {
                job.Undo();
            }
            jobs = null;
            AssetDatabase.Refresh();
        }

    }
}
