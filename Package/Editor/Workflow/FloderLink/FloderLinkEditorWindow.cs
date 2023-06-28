using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

/*
 * 使用 MKLink 对Assets、Package两个文件夹进行源文件引用传递，指向的是文件的引用 并不是克隆一份资源
 * 复制ProjectSettings到目标文件夹
 */

public class FloderLinkEditorWindow : EditorWindow
{
    private static string unityRootPath;
    private string[] platformOptions =
      {
        "Standalone",
        "Android",
        "IOS",
        "WebGL"
    };
    private int selectedPlatformIndex = 0;
    private string linkPath;
    private string projectSettingsSourcePath;

    [MenuItem("Tools/打开")]
    public static void Open()
    {
        unityRootPath = Application.dataPath.Replace("/Assets", "");
        FloderLinkEditorWindow linkWindow = GetWindow<FloderLinkEditorWindow>("FloderLinkEditorWindow");
        CenterWindow(linkWindow);
        linkWindow.Show();
    }
    private static void CenterWindow(EditorWindow window)
    {
        Vector2 screenSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        Vector2 maxSize = new Vector2(800, 600);
        Vector2 minSize = new Vector2(600, 400);
        float x = screenSize.x * 0.5f - maxSize.x * 0.5f;
        float y = screenSize.y * 0.5f - maxSize.y * 0.5f;
        window.position = new Rect(x, y, maxSize.x, maxSize.y);
        window.minSize = new Vector2(minSize.x, minSize.y);
    }

    private void OnGUI()
    {
        GUI.enabled = string.IsNullOrEmpty(linkPath);
        selectedPlatformIndex = EditorGUILayout.Popup("Select Platform", selectedPlatformIndex, platformOptions);
        string selecedPlatfrom = platformOptions[selectedPlatformIndex];
        //这里固定字符串拼接,其他平台的ProjectSettings格式为 ProjectSettings_平台名
        selecedPlatfrom = "ProjectSettings_" + selecedPlatfrom;

        GUILayout.Label($"会将以下文件进行链接:\n{unityRootPath}/Assets\n{unityRootPath}/Packages");
        string copyFromPath = unityRootPath + "/" + selecedPlatfrom;
        //看看是否存在对应平台的ProjectSettings如果不存在，就使用默认的
        bool hasProjectSettingFolder = Directory.Exists(copyFromPath);
        selecedPlatfrom = hasProjectSettingFolder ? selecedPlatfrom : "ProjectSettings";
        GUILayout.Label($"会将以下文件进行Copy:\nForm: {unityRootPath}/{selecedPlatfrom}");
        if (GUILayout.Button("选择链接文件的目录"))
        {
            if (!hasProjectSettingFolder)
            {
                bool isSelect = EditorUtility.DisplayDialog("", $"不存在{selecedPlatfrom},是否链接默认的ProjectSettings文件夹", "确定", "否");
                if (isSelect)
                    projectSettingsSourcePath = Path.Combine(unityRootPath, "ProjectSettings");
                else
                    return;
            }
            else
            {
                projectSettingsSourcePath = Path.Combine(unityRootPath, selecedPlatfrom);
            }
            //打开文件夹选择器 
            if (!string.IsNullOrEmpty(projectSettingsSourcePath))
                linkPath = FileEx.OpenFolderBrowser(null);
        }
        GUI.enabled = true;

        if (!string.IsNullOrEmpty(linkPath))
        {
            GUILayout.Label($"链接到:\n{linkPath}");
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("开始链接"))
            {
                bool isSelect = EditorUtility.DisplayDialog("", $"确定链接到:\n{linkPath}吗？", "确定", "再想想");
                if (isSelect)
                {
                    //复制Projecting文件夹到目标文件夹
                    string copyToPath = Path.Combine(linkPath, "ProjectSettings");
                    Debug.Log(projectSettingsSourcePath);
                    CopyFolderFromTo(projectSettingsSourcePath, copyToPath);
                    //开进程
                    Process process = StartProcess();
                    //执行MK命令
                    ExecuteMKLinkCommand(new List<MKLinkCommandStruct>()
                {
                   new MKLinkCommandStruct(process,linkPath,unityRootPath,"Assets"),
                   new MKLinkCommandStruct(process,linkPath,unityRootPath,"Packages")
                });
                    FileEx.OpenFolder(linkPath);
                    Close();
                    linkPath = "";
                    projectSettingsSourcePath = "";
                }
            }
            if (GUILayout.Button("取消"))
            {
                linkPath = "";
            }
            GUILayout.EndHorizontal();
        }
    }

    private void CopyFolderFromTo(string sourcePath, string copyToPath)
    {
        int isCopySucceed = FileEx.CopyFolder(sourcePath, copyToPath);
        //文件已经存在了就给他删了
        if (isCopySucceed == 1)
        {
            Directory.Delete(copyToPath, true);
            Debug.Log("删除文件夹成功:" + copyToPath);
            FileEx.CopyFolder(sourcePath, copyToPath);
        }
    }

    /// <summary>
    /// 开进程
    /// </summary>
    private Process StartProcess()
    {
        ProcessStartInfo info = new ProcessStartInfo()
        {
            FileName = "cmd.exe",
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        return Process.Start(info);
    }

    /// <summary>
    /// MKLink命令行字符处理
    /// //MKLink /J E:\NewTest\Assets D:\UnityProject\ProcessTest\Assets
    /// </summary>
    /// <param name="copyToPath"></param>
    /// <param name="sourcePath"></param>
    /// <param name="folderName"></param>
    /// <param name="isFinally"></param>
    /// <returns></returns>
    private string GetMKLinkCommandString(string copyToPath, string sourcePath, string folderName, bool isFinally)
    {
        string command_head = "MKLink /J";
        copyToPath = Path.Combine(copyToPath, folderName).Replace('/', '\\');
        sourcePath = Path.Combine(sourcePath, folderName).Replace('/', '\\');
        string command_LinkAssets = string.Format("{0} {1} {2}", command_head, copyToPath, sourcePath);
        if (isFinally)
        {
            //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
            //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令
            command_LinkAssets += "&exit";
        }
        return command_LinkAssets;
    }

    /// <summary>
    /// 执行cmd命令，MKLinkCommandStruct为每一条命令，执行到最后一条时自动关闭dataList的Process
    /// </summary>
    /// <param name="dataList"></param>
    private void ExecuteMKLinkCommand(List<MKLinkCommandStruct> dataList)
    {
        for (int i = 0; i < dataList.Count; i++)
        {
            MKLinkCommandStruct data = dataList[i];
            bool isFinally = i == dataList.Count - 1;
            string command_Link = GetMKLinkCommandString(data.copyToPath, data.sourcePath, data.folderName, isFinally);
            data.process.StandardInput.WriteLine(command_Link);
            if (isFinally)
            {
                data.process.WaitForExit();
                data.process.Close();
            }
            Debug.Log($"命令行程序执行成功\n{command_Link} \n关闭进程{isFinally}");
        }
    }

}
#if UNITY_EDITOR
public class MKLinkCommandStruct
{
    public MKLinkCommandStruct(Process process, string copyToPath, string sourcePath, string folderName)
    {
        this.process = process;
        this.copyToPath = copyToPath;
        this.sourcePath = sourcePath;
        this.folderName = folderName;
    }
    public Process process;
    public string copyToPath;
    public string sourcePath;
    public string folderName;
}
#endif