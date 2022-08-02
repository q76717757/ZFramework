using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZFramework
{
    public class GlobalConfigEditorWindow : EditorWindow
    {
        static Vector2 windowMinSize = new Vector2(800, 500);
        [UnityEditor.Callbacks.OnOpenAsset(0)]//Open重定向  双击也可以打开面板
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var instance = EditorUtility.InstanceIDToObject(instanceID);
            if (instance != null && instance is GlobalConfig)
            {
                Open(instance as GlobalConfig);
                return true;
            }
            return false;
        }

        internal static void Open(GlobalConfig library)
        {
            var window = EditorWindow.GetWindow<GlobalConfigEditorWindow>("GlobalConfig", true);
            if (window == null) return;
            window.minSize = windowMinSize;
            window.Show();
        }


    }
}
