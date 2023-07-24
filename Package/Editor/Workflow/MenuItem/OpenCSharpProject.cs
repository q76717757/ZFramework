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
            string url = Path.GetFullPath("Assets/ZFramework/.Server/ZFramework.sln");
            if (File.Exists(url))
            {
                Application.OpenURL(Path.GetFullPath("Assets/ZFramework/.Server/ZFramework.sln"));
            }
            else
            {
                EditorUtility.DisplayDialog("Open C# Project","文件路径错误,请确认框架目录在Assets/ZFramework","OK");
            }
        }

    }
}
