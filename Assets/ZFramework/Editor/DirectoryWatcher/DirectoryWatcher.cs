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

        //[InitializeOnLoadMethod]   �ڴ�й¶??
        public static void Start()
        {
            DirectoryInfo dd = new DirectoryInfo("Codes");//hotfix�ű�����������ļ������� ��������ļ��� ��ѡ�Զ�/�ֶ�����
            if (dd == null || !dd.Exists)
            {
                Log.Error("�ȸ��ļ���Codes������,����ʧ��");
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
            Log.Info("Rebulid DLL");
        }
    }
}
