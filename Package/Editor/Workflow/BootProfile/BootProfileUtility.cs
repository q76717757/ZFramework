using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.Editor
{
    public static class BootProfileUtility
    {






        public static void DrawCreateButton()
        {
            if (GUILayout.Button("Create BootConfig", GUILayout.Height(50)))
            {
                CreateBootProfile();
            }
        }

        static void CreateBootProfile()
        {
            new BootProfile().Save();
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}
