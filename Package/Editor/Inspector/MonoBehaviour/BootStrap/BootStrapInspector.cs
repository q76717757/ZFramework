using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ZFramework.Editor
{
    [CustomEditor(typeof(BootStrap))]
    public class BootStrapInspector: UnityEditor.Editor
    {
        List<FadeFoldout> fadeFoldouts;

        public override void OnInspectorGUI()
        {
            DrawElements();
        }

        void DrawElements()
        {
            if (fadeFoldouts == null)
            {
                fadeFoldouts = new List<FadeFoldout>();
                new NormalFadeFoldout().JoinGroup(fadeFoldouts);
                new HybridCLRFoldout().JoinGroup(fadeFoldouts);
                new BuildFadeFoldout().JoinGroup(fadeFoldouts);
                new ToolBoxFadeFoldout().JoinGroup(fadeFoldouts);
                new TencentCOSFoldout().JoinGroup(fadeFoldouts);

                foreach (FadeFoldout item in fadeFoldouts)
                {
                    item.OnValueChange.AddListener(Repaint);
                }
            }

            foreach (FadeFoldout item in fadeFoldouts)
            {
                item.OnGui();
            }
        }

    }

}
