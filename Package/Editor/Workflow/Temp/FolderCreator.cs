using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class FolderCreator : EditorWindow
{
    static string[] assetsFolderNames = { "Scenes", "Art", "UI", "Scripts", "Localization", "Media", "Bundles", "Config", "ThirdParty", "Prefabs", "Temp", "Resources", "StreamingAssets", "URP", "XR", "XRI", "ZFramework", "Bulid" ,"Editor"};

    static string[] scenesFolderNames = { "Loading", "Login"};

    static string[] artFolderNames = { "Characters", "Effects", "Environments", "Props"};

    static string[] uIFolderNames = { "Loading", "Login" };


    private static List<string> folderNamesList;
    [MenuItem("Tools/初始化文件夹")]
    private static void CreateWindow()
    {
        InitializeFolder("Assets" , assetsFolderNames);
        InitializeFolder("Assets/Scenes", scenesFolderNames);
        InitializeFolder("Assets/Art", artFolderNames);
        InitializeFolder("Assets/UI", uIFolderNames);
    }

    private static void InitializeFolder(string path, string[] folderNames)
    {
        //InitFolderNameList(folderNames);
        //初始化文件夹名称列表
        folderNamesList = new List<string>(folderNames);
        List<string> newFolderNameList = ExcludeTheSameFolder(path, folderNamesList);

        for (int i = 0; i < newFolderNameList.Count; i++)
        {
            AssetDatabase.CreateFolder(path, newFolderNameList[i]);
        }
        AssetDatabase.Refresh();

    }

    //防止新建同名文件夹
    private static List<string> ExcludeTheSameFolder(string path, List<string> folderNames)
    {
        string[] folders = AssetDatabase.GetSubFolders(path);
        foreach (string folder in folders)
        {
            string folderName = Path.GetFileName(folder);
            for (int i = 0; i < folderNames.Count; i++)
            {
                if (folderName == folderNames[i])
                {
                    folderNames.Remove(folderNames[i]);
                }
            }
        }
        return folderNames;
    }
}