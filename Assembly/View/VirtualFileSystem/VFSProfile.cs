using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZFramework
{
    public class VFSProfile
    {
        public int version;
        public List<VFSTreeElement> elements;



        internal static bool IsExists
        {
            get
            {
                return File.Exists(FilePath);
            }
        }
        internal static string FilePath
        {
            get
            {
                return Path.Combine(Defines.BuildInAssetAPath, "Profiles", "VFSProfile.json");
            }
        }

        private static VFSProfile _instance;
        private bool isDirty = true;
        internal static VFSProfile GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            if (!IsExists)
            {
                Log.Error("VFSProfile.json Not Exists");
                return null;
            }
            return _instance = Json.ToObject<VFSProfile>(DiskFilesLoadingUtility.DownLoadText(FilePath));
        }
        internal void Save()
        {
            _instance = this;
            Directory.CreateDirectory(new FileInfo(FilePath).Directory.FullName);
            string json = Json.ToJson(this,false,true);
            File.WriteAllText(FilePath, json);
        }
        internal void SaveIfDirty()
        {
            if (isDirty)
            {
                Save();
                isDirty = false;
            }
        }
        internal void SetDirty()
        {
            isDirty = true;
        }
    }
}
