using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ZFramework.Editor
{
    public class OpenCSharpProject
    {
        [MenuItem("ZFramework/Open C# Project",priority = 10)]
        static void OnClick()
        {
            string[] files = Directory.GetFiles("Assets", "ZFramework.sln", SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                Application.OpenURL(Path.GetFullPath(files[0]));
            }
            else
            {
                EditorUtility.DisplayDialog("Open C# Project", "找不到解决方案", "OK");
            }
        }

    }
}
