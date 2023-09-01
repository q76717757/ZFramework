using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

namespace ZFramework.Editor
{
    [CustomEditor(typeof(BootStrap))]
    public class BootStrapInspector: UnityEditor.Editor
    {
        List<FadeFoldout> fadeFoldouts= new List<FadeFoldout>();

        private void OnEnable()
        {
            fadeFoldouts.Add(new NormalFadeFoldout());
            fadeFoldouts.Add(new HybridCLRFoldout());
            fadeFoldouts.Add(new BuildFadeFoldout());

            //new NormalFadeFoldout().JoinGroup(fadeFoldouts);
            //new AssemblyFadeGroup().JoinGroup(fadeFoldouts); 
            //new HotfixFadeFoldout().JoinGroup(fadeFoldouts);

            foreach (var item in fadeFoldouts)
            {
                item.OnValueChange.AddListener(Repaint);
            }
        }

        public override void OnInspectorGUI()
        {
            if (!BootProfile.IsExists)
            {
                BootProfileUtility.DrawCreateButton();
            }
            else
            {
                foreach (var item in fadeFoldouts)
                {
                    item.OnGui();
                }
            }
        }
    }

}
