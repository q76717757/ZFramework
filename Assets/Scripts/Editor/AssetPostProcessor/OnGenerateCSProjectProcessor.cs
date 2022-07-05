using UnityEditor;
using System.Xml;
using System.IO;

namespace ZFramework
{
    public class OnGenerateCSProjectProcessor : AssetPostprocessor
    {
        //增删cs文件的时候触发   修改不会触发
        public static string OnGeneratedCSProject(string path, string content)
        {
            string[] csprojKeys = new string[]
            {
                "Model","Logic","ViewModel","ViewLogic",
            };
            foreach (var key in csprojKeys)
            {
                if (path.EndsWith($"Unity.{key}.csproj"))
                {
                    content = content.Replace($"<Compile Include=\"Assets\\Scripts\\Hotfix\\{key}\\Empty.cs\" />", string.Empty);
                    content = content.Replace($"<None Include=\"Assets\\Scripts\\Hotfix\\{key}\\Unity.{key}.asmdef\" />", string.Empty);

                    return GenerateCustomProject(path, content, $"Codes\\{key}\\**\\*.cs");
                }
            }
            return content;
        }

        private static string GenerateCustomProject(string path, string content, string codesPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(content);

            var newDoc = doc.Clone() as XmlDocument;

            var rootNode = newDoc.GetElementsByTagName("Project")[0];

            var itemGroup = newDoc.CreateElement("ItemGroup", newDoc.DocumentElement.NamespaceURI);
            var compile = newDoc.CreateElement("Compile", newDoc.DocumentElement.NamespaceURI);

            compile.SetAttribute("Include", codesPath);
            itemGroup.AppendChild(compile);

            //var projectReference = newDoc.CreateElement("ProjectReference", newDoc.DocumentElement.NamespaceURI);
            //projectReference.SetAttribute("Include", @"..\Share\Analyzer\Share.Analyzer.csproj");
            //projectReference.SetAttribute("OutputItemType", @"Analyzer");
            //projectReference.SetAttribute("ReferenceOutputAssembly", @"false");

            //var project = newDoc.CreateElement("Project", newDoc.DocumentElement.NamespaceURI);
            //project.InnerText = @"{d1f2986b-b296-4a2d-8f12-be9f470014c3}";
            //projectReference.AppendChild(project);

            //var name = newDoc.CreateElement("Name", newDoc.DocumentElement.NamespaceURI);
            //name.InnerText = "Analyzer";
            //projectReference.AppendChild(project);

            //itemGroup.AppendChild(projectReference);

            rootNode.AppendChild(itemGroup);

            using (StringWriter sw = new StringWriter())
            {

                using (XmlTextWriter tx = new XmlTextWriter(sw))
                {
                    tx.Formatting = Formatting.Indented;
                    newDoc.WriteTo(tx);
                    tx.Flush();
                    return sw.GetStringBuilder().ToString();
                }
            }
        }

    }
}
