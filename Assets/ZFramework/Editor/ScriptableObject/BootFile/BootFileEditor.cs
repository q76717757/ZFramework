using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZFramework
{
    [CustomEditor(typeof(BootFile))]
    public class BootFileEditor : Editor
    {
        BootFile Boot => target as BootFile;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

        }
    }
}
