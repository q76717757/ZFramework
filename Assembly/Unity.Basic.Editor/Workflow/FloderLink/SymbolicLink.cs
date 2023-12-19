using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Linq;
using static UnityEditor.Progress;

namespace ZFramework.Editor
{
    public class SymbolicLink : EditorWindow
    {
        [MenuItem("ZFramework/工程镜像工具(多开)")]
        static void Open() => GetWindow<SymbolicLink>("SymbolicLink", true);

        readonly string[] includes = new string[]
        {
            "Assets",
            "Packages",
            "ProjectSettings",
        };
        readonly string[] ignores = new string[]
        {
            "Library",
            "Logs",
            "obj",
            "Temp",
            "ProjectSettings"//特殊处理 
        };

        string targetAPath;//当前工程的根目录
        string linkAPath;//联接工程的根目录

        bool copyProjectSettings;
        string copyProjectSettingsName;
        string targetProjectSettings;
        Dictionary<string, bool> optionalFloder;//可选的文件夹

        private void OnEnable()
        {
            targetAPath = Path.GetFullPath(".");
            linkAPath = $"{targetAPath}_Clone";
            targetProjectSettings = new DirectoryInfo("./ProjectSettings").FullName;
            copyProjectSettings = true;
            copyProjectSettingsName = "ProjectSettings_Clone";
            optionalFloder = new Dictionary<string, bool>();

            var subDirectories = new DirectoryInfo(".").GetDirectories();
            foreach (var directory in subDirectories)
            {
                string dirName = directory.Name;
                if (includes.Contains(dirName) || ignores.Contains(dirName))
                {
                    continue;
                }
                optionalFloder.Add(dirName, false);
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("本工具使用mklink命令将工程的关键目录进行联接,从而实现工程镜像,而实际资源文件只占一份硬盘空间");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("1.选择要联接的新工程路径",EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("当前工程路径:", GUILayout.Width(80));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextArea(targetAPath);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("联接的新路径:", GUILayout.Width(80));
            linkAPath = EditorGUILayout.TextArea(linkAPath);
            if (GUILayout.Button("浏览", GUILayout.Width(80)))
            {
                var newAPath = EditorUtility.OpenFolderPanel("选择你要联接的路径", "", "").Replace("/", "\\");
                if (!string.IsNullOrEmpty(newAPath))
                {
                    linkAPath = newAPath;
                    GUI.FocusControl(null);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("联接路径请选择一个空文件夹或者输入绝对路径(文件夹不存在时将自动创建)", MessageType.None);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("2.勾选要联接的目录", EditorStyles.boldLabel);
            foreach (string dirName in includes)
            {
                DrawFolder(dirName, true);
            }
            foreach (string dirName in optionalFloder.Keys.ToArray())
            {
                DrawFolder(dirName, false);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("3.设置\"ProjectSettings\"", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("物理文件夹路径:", GUILayout.Width(100));
            targetProjectSettings = EditorGUILayout.TextField(targetProjectSettings);
            if (GUILayout.Button("浏览", GUILayout.Width(80)))
            {
                var newAPath = EditorUtility.OpenFolderPanel("选择你要联接的路径", "", "").Replace("/", "\\");
                if (!string.IsNullOrEmpty(newAPath))
                {
                    targetProjectSettings = newAPath;
                    GUI.FocusControl(null);
                }
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("联接到复制文件:",GUILayout.Width(100));
            copyProjectSettings = EditorGUILayout.Toggle(copyProjectSettings, GUILayout.Width(20));
            EditorGUI.BeginDisabledGroup(!copyProjectSettings);
            EditorGUILayout.LabelField("复制名称:", GUILayout.Width(60));
            copyProjectSettingsName = EditorGUILayout.TextField(copyProjectSettingsName);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(
                "ProjectSettings文件夹特殊处理,将对文件夹在根目录下进行物理复制,并对复制的文件夹建立软连接而不是直接联接原文件夹,这么做的原因:\r\n" +
                "一.为了让镜像工程的差异设置得以保留\r\n" +
                "二.将镜像工程的设置留在物理工程内,方便进行版本控制"
                , MessageType.None);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("4.点击\"开始执行\"按钮", EditorStyles.boldLabel);
            if (GUILayout.Button("开始执行",GUILayout.Height(50)))
            {
                Directory.CreateDirectory(linkAPath);

                List<string> copy = new List<string>();
                copy.Add(new DirectoryInfo($"{linkAPath}/Assets").FullName);
                copy.Add(new DirectoryInfo($"{linkAPath}/Packages").FullName);
                copy.Add(new DirectoryInfo($"{linkAPath}/ProjectSettings").FullName);

                List<string> source = new List<string>();
                source.Add(new DirectoryInfo("Assets").FullName);
                source.Add(new DirectoryInfo("Packages").FullName);
                if (copyProjectSettings)
                {
                    var path = new DirectoryInfo(copyProjectSettingsName).FullName;
                    CopyFolder(new DirectoryInfo(targetProjectSettings).FullName, path);
                    source.Add(path);
                }
                else
                {
                    source.Add(new DirectoryInfo(targetProjectSettings).FullName);
                }

                foreach (var item in optionalFloder)
                {
                    if (item.Value)
                    {
                        copy.Add(new DirectoryInfo($"{linkAPath}/{item.Key}").FullName);
                        source.Add(new DirectoryInfo($"./{item.Key}").FullName);
                    }
                }

                string[] copyToPath = copy.ToArray();;
                string[] sourcePath = source.ToArray();
                MKLink(copyToPath, sourcePath);

                Application.OpenURL(linkAPath);
            }
        }



        void CopyFolder(string sourceFolderPath, string destinationFolderPath)
        {
            try
            {
                // 创建目标文件夹
                Directory.CreateDirectory(destinationFolderPath);

                // 获取源文件夹中的所有文件和子文件夹
                string[] files = Directory.GetFiles(sourceFolderPath);
                string[] subDirectories = Directory.GetDirectories(sourceFolderPath);

                // 拷贝文件
                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileName(filePath);
                    string destinationFilePath = Path.Combine(destinationFolderPath, fileName);
                    File.Copy(filePath, destinationFilePath, true);
                }

                // 递归拷贝子文件夹
                foreach (string subDirectoryPath in subDirectories)
                {
                    string directoryName = Path.GetFileName(subDirectoryPath);
                    string destinationSubFolderPath = Path.Combine(destinationFolderPath, directoryName);
                    CopyFolder(subDirectoryPath, destinationSubFolderPath);
                }
                Debug.Log("Copy ProjectSettings Succeed");
            }
            catch (IOException e)
            {
                Debug.LogError("Copy ProjectSettings Failed:" + e.Message);
            }
        }

        void DrawFolder(string directoryName,bool isInclude)
        {
            using (var horizonal = new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(isInclude);
                if (isInclude)
                {
                    EditorGUILayout.ToggleLeft("", true, GUILayout.Width(20));
                }
                else
                {
                    optionalFloder[directoryName] = EditorGUILayout.ToggleLeft("", optionalFloder[directoryName], GUILayout.Width(20));
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.LabelField(EditorGUIUtility.IconContent("Folder Icon"), GUILayout.Width(20));
                if (isInclude)
                {
                    EditorGUILayout.LabelField($"{directoryName}*");
                }
                else
                {
                    EditorGUILayout.LabelField(directoryName);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        //检查联接路径是否合规
        string CheckPath(string path)
        {
            return "";
        }

        //执行mklink命令
        void MKLink(string[] copyToPath, string[] sourcePath)
        {
            string command = "MKLink /J";

            string[] commands = new string[copyToPath.Length];
            for (int i = 0; i < commands.Length; i++)
            {
                commands[i] = string.Format("{0} {1} {2}", command, copyToPath[i].Replace('/', '\\'), sourcePath[i].Replace('/', '\\'));
                Debug.Log(commands[i]);
            }
            CommandUtility.Start(commands);
        }
    }
}
