using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace ZFramework
{
    public class DirectoryWatcher : MonoBehaviour
    {
        private void Start()
        {
            return;
            var modelPath = Path.Combine("../Unity.Model");
            var hotfixPath = Path.Combine("../Unity.Hotfix");

            DirectoryInfo d = new DirectoryInfo(modelPath);
            
            Log.Info(d.FullName);

            using var watcher = new FileSystemWatcher(modelPath);
            watcher.NotifyFilter =// NotifyFilters.Attributes
                                  //|
                                  NotifyFilters.CreationTime
                                  //| NotifyFilters.DirectoryName
                                  | NotifyFilters.FileName
                                 //| NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite;
            //| NotifyFilters.Security
            //| NotifyFilters.Size;

            watcher.Changed += OnChanged;
            watcher.Created += OnCreated;
            watcher.Deleted += OnDeleted;
            watcher.Renamed += OnRenamed;
            watcher.Error += OnError;

            watcher.Filter = "*.cs";
            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            Log.Info($"Changed: {e.FullPath}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"Created: {e.FullPath}";
            Log.Info(value);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Log.Info($"Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Log.Info($"Renamed:");
            Log.Info($"    Old: {e.OldFullPath}");
            Log.Info($"    New: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Log.Info($"Message: {ex.Message}");
                Log.Info("Stacktrace:");
                Log.Info(ex.StackTrace);
                PrintException(ex.InnerException);
            }
        }

    }
}
