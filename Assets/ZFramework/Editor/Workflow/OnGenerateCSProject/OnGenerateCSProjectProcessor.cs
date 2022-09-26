using UnityEditor;
using System.Xml;
using System.IO;
using UnityEngine;

namespace ZFramework
{
    public class OnGenerateCSProjectProcessor : AssetPostprocessor
    {
        public static string OnGeneratedCSProject(string path, string content)
        {
            Debug.Log(path);
            //热更程序集的名称
            string[] csprojNames = new string[]
            {
                "Data","Func","View",
            };
            string emptyCSPath = @"Assets\ZFramework\Hotfix\";
            foreach (var name in csprojNames)
            {
                if (path.EndsWith($"Unity.Hotfix.{name}.csproj"))
                {
                    //程序集的位置
                    content = content.Replace($"<Compile Include=\"{emptyCSPath}{name}\\Empty.cs\" />", string.Empty);
                    content = content.Replace($"<None Include=\"{emptyCSPath}{name}\\Unity.Hotfix.{name}.asmdef\" />", string.Empty);

                    //实际代码的位置
                    return IncludeCustom(content, $"Assets\\ZFramework\\.Code\\{name}\\**\\*.cs");
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
