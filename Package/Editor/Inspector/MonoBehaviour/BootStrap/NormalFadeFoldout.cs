using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace ZFramework.Editor
{
    public class NormalFadeFoldout : FadeFoldout
    {
        public override string Title => "基本设置";

        protected override void OnGUI()
        {
            var profile = BootProfile.GetInstance();
            EditorGUILayout.LabelField("目标平台:" + Defines.TargetRuntimePlatform.ToString());
            EditorGUILayout.LabelField("强同步:" + profile.IsMustSync);
        }
    }
}
