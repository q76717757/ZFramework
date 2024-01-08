using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

namespace ZFramework
{
    internal class VFSProfile
    {
        internal string hash;
        internal List<VFSTreeModelItem> elements;
        internal BundleManifest manifest;


        internal static bool IsExists
        {
            get
            {
                return GetInstance() != null;
            }
        }
        internal static string PersistencePath
        {
            get
            {
                return Path.Combine(Defines.PersistenceDataAPath, VFSUtility.FOLDER, VFSUtility.FILENAME);
            }
        }
        internal static string BuildInPath
        {
            get
            {
                return Path.Combine(Defines.BuildInAssetAPath, VFSUtility.FOLDER, VFSUtility.FILENAME);
            }
        }

        private static VFSProfile _instance;
        private bool isDirty = true;

        internal VFSProfile()
        {
            elements = new List<VFSTreeModelItem>
            {
                new VFSTreeModelItem(-1, -1, new VFSMetaData(VFSMetaData.MetaType.VirtualFolder)),
            };
        }

        //先从持久目录找 没有再从内建目录找
        internal static VFSProfile GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            string path;
            if (File.Exists(PersistencePath))
            {
                path = PersistencePath;
            }
            else if(File.Exists(BuildInPath))
            {
                path = BuildInPath;
            }
            else
            {
                return null;
            }
            return _instance = Load(path);
        }
        static VFSProfile Load(string path)
        {
            var profile = new VFSProfile();
            var xmlString = DiskFilesLoadingUtility.DownLoadText(path);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            profile.hash = xmlDoc.SelectSingleNode("VFS/Hash").Attributes["Hash"].Value;

            var node = xmlDoc.SelectSingleNode("VFS/Elements").ChildNodes;
            int id = 0;//id并不重要 只是编辑器在用  每次打开随便赋个自增ID即可
            for (int i = 0; i < node.Count; i++)
            {
                var item = node.Item(i);
                int depth = int.Parse(item.Attributes["Depth"].Value);
                var type = (VFSMetaData.MetaType)int.Parse(item.Attributes["Type"].Value);
                var guid = item.Attributes["Guid"].Value;
                profile.elements.Add(new VFSTreeModelItem(id++, depth, new VFSMetaData(guid, type)
                {
                    name = item.Attributes["Name"].Value,
                    path = item.Attributes["Path"].Value,
                    toggle = item.Attributes["Guid"].Value == "1",
                }));
            }
            var ManifestNodes = xmlDoc.SelectSingleNode("VFS/Manifest");
            if (ManifestNodes != null)
            {
                profile.manifest = new BundleManifest();
                var bundleNodes = ManifestNodes.ChildNodes;
                for (int i = 0; i < bundleNodes.Count; i++)
                {
                    var bundle = bundleNodes.Item(i);

                    var assetGuids = new List<string>();
                    var assetNodes = bundle.SelectNodes("Asset");
                    for (int j = 0; j < assetNodes.Count; j++)
                    {
                        assetGuids.Add(assetNodes.Item(j).Attributes["Guid"].Value);
                    }

                    var dependencies = new List<string>();
                    var dependenceNodes = bundle.SelectNodes("Dependence");
                    for (int k = 0; k < dependenceNodes.Count; k++)
                    {
                        dependencies.Add(dependenceNodes.Item(k).Attributes["Name"].Value);
                    }

                    BundleInfo info = new BundleInfo()
                    {
                        bundleName = bundle.Attributes["Name"].Value,
                        hash = bundle.Attributes["Hash"].Value,
                        assetGuids = assetGuids,
                        dependencies = dependencies
                    };
                    profile.manifest.AddBundleInfo(info);
                }
            }
            return profile;
        }


        internal void Save()
        {
            _instance = this;
            Directory.CreateDirectory(new FileInfo(BuildInPath).Directory.FullName);

            XmlDocument xmlDoc = new XmlDocument();
            XmlDeclaration dec = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(dec);
            var root = xmlDoc.CreateElement("VFS");
            xmlDoc.AppendChild(root);

            var hash = xmlDoc.CreateElement("Hash");
            root.AppendChild(hash);

            var elements = xmlDoc.CreateElement("Elements");
            root.AppendChild(elements);
            for (int i = 1; i < this.elements.Count; i++)
            {
                var item = this.elements[i];
                var xmlItem = xmlDoc.CreateElement("Item");
                xmlItem.SetAttribute("Depth", item.Depth.ToString());
                xmlItem.SetAttribute("Toggle", item.Data.toggle ? "1" : "0");
                xmlItem.SetAttribute("Type", ((int)item.Data.type).ToString());
                xmlItem.SetAttribute("Name", item.Data.name);
                xmlItem.SetAttribute("Guid", item.Data.Guid);
                xmlItem.SetAttribute("Path", item.Data.path);
                elements.AppendChild(xmlItem);
            }

            if (this.manifest != null)
            {
                var manifest = xmlDoc.CreateElement("Manifest");
                root.AppendChild(manifest);
                foreach (var item in this.manifest.GetBundles())
                {
                    var xmlItem = xmlDoc.CreateElement("Bundle");//包信息
                    xmlItem.SetAttribute("Name", item.bundleName);
                    xmlItem.SetAttribute("Hash", item.hash);
                    manifest.AppendChild(xmlItem);

                    foreach (var asset in item.assetGuids)//内容
                    {
                        var bundleItem = xmlDoc.CreateElement("Asset");
                        bundleItem.SetAttribute("Guid", asset);
                        xmlItem.AppendChild(bundleItem);
                    }
                    foreach (var asset in item.dependencies)//依赖项
                    {
                        var bundleItem = xmlDoc.CreateElement("Dependence");
                        bundleItem.SetAttribute("Name", asset);
                        xmlItem.AppendChild(bundleItem);
                    }
                }
            }
            string xmlStr = xmlDoc.FormatString();
            var md5 = MD5Helper.BytesMD5(new UTF8Encoding(false).GetBytes(xmlStr));
            hash.SetAttribute("Hash", md5);

            File.WriteAllText(BuildInPath, xmlDoc.FormatString(), new UTF8Encoding(false));
            isDirty = false;
        }
        internal void SaveIfDirty()
        {
            if (isDirty)
            {
                Save();
            }
        }
        internal void SetDirty()
        {
            isDirty = true;
        }
    }
}
