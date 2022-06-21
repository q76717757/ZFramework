using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace ZFramework
{
    [CustomEditor(typeof(ZFrameworkSetting))]
    public class ZFrameworkSettingInspector:Editor
    {

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);

            EditorGUILayout.LabelField("测试Project Setting");

            //base.OnInspectorGUI();

            EditorGUI.EndDisabledGroup();
        }

    }
}
