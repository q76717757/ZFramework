﻿using UnityEditor;
using System.Xml;
using System.IO;

namespace ZFramework
{
    public class OnGenerateCSProjectProcessor : AssetPostprocessor
    {
        public static string OnGeneratedCSProject(string path, string content)
        {
            foreach (var name in Define.csprojNames)
            {
                if (path.EndsWith($"Unity.{name}.csproj"))
                {
                    content = content.Replace($"<Compile Include=\"Assets\\ZFramework\\Hotfix\\{name}\\Empty.cs\" />", string.Empty);
                    content = content.Replace($"<None Include=\"Assets\\ZFramework\\Hotfix\\{name}\\Unity.{name}.asmdef\" />", string.Empty);
                    return IncludeCustom(content, $"Codes\\{name}\\**\\*.cs");
                }
            }
            return content;
        }

        private static string IncludeCustom(string content, string codesPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var newDoc = doc.Clone() as XmlDocument;

            var compile = newDoc.CreateElement("Compile", newDoc.DocumentElement.NamespaceURI);
            compile.SetAttribute("Include", codesPath);

            var itemGroup = newDoc.CreateElement("ItemGroup", newDoc.DocumentElement.NamespaceURI);
            itemGroup.AppendChild(compile);

            var rootNode = newDoc.GetElementsByTagName("Project")[0];
            rootNode.AppendChild(itemGroup);

            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter tw = new XmlTextWriter(sw))
                {
                    tw.Formatting = Formatting.Indented;
                    newDoc.WriteTo(tw);
                    tw.Flush();
                    return sw.GetStringBuilder().ToString();
                }
            }
        }

    }
}
