using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace ZFramework.Editor
{
    public static class FolderCreator
    {
        private static List<(string, int)> folderStruct = new List<(string, int)>()
        {
              ("Scenes",0),
                  ("Loading",1),
                  ("Login",1),
              ("Art",0),
                  ("Characters",1),
                  ("Effects",1),
                  ("Environments",1),
                  ("Props",1),
              ("UI",0),
                  ("Loading",1),
                  ("Login",1),
              ("Scripts",0),
              ("Localization",0),
              ("Media",0),
              ("Bundles",0),
              ("Config",0),
              ("ThirdParty",0),
              ("Prefabs",0),
              ("Temp",0),
              ("Resources",-1),
              ("StreamingAssets",-1),
              ("URP",-1),
              ("XR",-1),
              ("XRI",-1),
      };

        [MenuItem("ZFramework/初始化工程文件目录结构")]
        private static void Creat()
        {
            if (EditorUtility.DisplayDialog("初始化工程文件目录结构", "自动生成工程文件目录结构", "确定", "取消"))
            {
                pathStack.Clear();
                lastDepath = 0;
                headIndex = 0;
                GenerateFolderByStruct();
            }
        }

        private static void GenerateFolderByStruct()
        {
            for (int i = 0; i < folderStruct.Count; i++)
            {
                pathStack.Clear();
                string path = folderStruct[i].Item1;
                int depath = folderStruct[i].Item2;
                pathStack.Push(path);
                if (depath == 0)
                {
                    headIndex++;
                }
                RecursiveFolderName(i);
                string creatPath = Application.dataPath;
                bool isFirst = true;
                while (pathStack.Count > 0)
                {
                    string folderItemPath = pathStack.Pop();
                    if (isFirst)
                    {
                        isFirst = false;
                        folderItemPath = depath >= 0 ? string.Format("{0}{1}{2}", headIndex.ToString("D2"), ".", folderItemPath) : folderItemPath;
                    }
                    creatPath = Path.Combine(creatPath, folderItemPath);
                }
                if (CreatFolder(creatPath))
                {
                    Debug.Log("CreatFolderSucceed: " + creatPath);
                }
                else
                {
                    Debug.Log("已存在同名文件夹: " + creatPath);
                }
            }
            AssetDatabase.Refresh();
        }
        static Stack<string> pathStack = new Stack<string>();
        static int lastDepath;
        static int headIndex = 0;
        //往上递归，直到找到0为止
        private static void RecursiveFolderName(int currentIndex)
        {
            //当前index的深度
            int currentDepath = folderStruct[currentIndex].Item2;
            //记录一下
            lastDepath = currentDepath;
            //如果当前的深度>0就往上找,直到深度<=0为止
            while (currentDepath > 0)
            {
                //当前的index的Depath如果>=上一次记录过的深度,就--Index
                while (folderStruct[--currentIndex].Item2 >= lastDepath)
                {

                }
                lastDepath = folderStruct[currentIndex].Item2;
                pathStack.Push(folderStruct[currentIndex].Item1);
                RecursiveFolderName(currentIndex);
                return;
            }
        }

        private static bool CreatFolder(string fullPath)
        {
            if (Directory.Exists(fullPath))
                return false;
            Directory.CreateDirectory(fullPath);
            return true;
        }
    }
}