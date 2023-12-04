using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
    public class BuildParameters
    {
        public string OutputFolder { get; set; }
        public BuildTarget Target { get; set; }
        public BuildTargetGroup Group { get; set; }
    }
}
