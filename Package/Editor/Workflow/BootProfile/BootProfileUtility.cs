using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZFramework.Editor
{
    public static class BootProfileUtility
    {
        public static void DrawCreateButton()
        {
            if (GUILayout.Button("创建配置文件", GUILayout.Height(50)))
            {
                new BootProfile().Save();
                AssetDatabase.Refresh();
            }
        }
    }
}
