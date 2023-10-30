using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
    public class ToolBoxFadeFoldout : FadeFoldout
    {
        public override string Title => "π§æﬂœ‰";

        protected override void OnGUI()
        {
            if (GUILayout.Button("VFS"))
            {
                VirtualFileSystemBrowser.OpenWindow();
            }
        }
    }
}
