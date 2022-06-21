using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace ZFramework
{
    public class ZFrameworkSettingWindow : EditorWindow
    {
        public static void Open()
        {
            var window = GetWindow<ZFrameworkSettingWindow>("设置选项");
            window.minSize = new Vector2(200, 200);
            window.Show();
        }


        private void OnGUI()
        {
            GUILayout.Label("123123123");
        }
    }
}
