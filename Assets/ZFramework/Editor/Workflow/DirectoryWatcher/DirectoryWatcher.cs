using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor;

namespace ZFramework
{
    public class DirectoryWatcher
    {
        public static FileSystemWatcher watcher;

        //[InitializeOnLoadMethod]
        public static void Start()
        {
            DirectoryInfo dd = new DirectoryInfo("Codes");//hotfix脚本都放在这个文件夹下面 监听这个文件夹 可选自动/手动编译
            if (dd == null || !dd.Exists)
            {
                Debug.LogError("热更文件夹Codes不存在");
                return;
            }

            var watcher = new FileSystemWatcher(dd.FullName, "*.cs");
            watcher.NotifyFilter =// NotifyFilters.Attributes
                                  NotifyFilters.CreationTime
                                  //| NotifyFilters.DirectoryName
                                  | NotifyFilters.FileName
                                  //| NotifyFilters.LastAccess
                                  | NotifyFilters.LastWrite
                                  //| NotifyFilters.Security
                                  | NotifyFilters.Size;

            watcher.Created += OnCreated;
            watcher.Changed += OnChanged;

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            RebulidDLL();
        }
        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            RebulidDLL();
        }

        static void RebulidDLL()
        {
            Debug.Log("Rebulid DLL");
        }
    }
}
