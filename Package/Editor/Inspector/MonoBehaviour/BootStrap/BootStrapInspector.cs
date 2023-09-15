﻿using UnityEngine;
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
        List<FadeFoldout> fadeFoldouts;

        public override void OnInspectorGUI()
        {
            if (!BootProfile.IsExists)
            {
                BootProfileUtility.DrawCreateButton();
            }
            else
            {
                DrawElements();
            }
        }

        void DrawElements()
        {
            if (fadeFoldouts == null)
            {
                fadeFoldouts = new List<FadeFoldout>
                {
                    new NormalFadeFoldout(),
                    new HybridCLRFoldout(),
                    new BuildFadeFoldout(),
                    new ToolBoxFadeFoldout()
                };

                foreach (var item in fadeFoldouts)
                {
                    item.OnValueChange.AddListener(Repaint);
                }
            }

            foreach (var item in fadeFoldouts)
            {
                item.OnGui();
            }
        }

    }

}
