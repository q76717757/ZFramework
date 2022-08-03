using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZFramework
{
    public class BootFileEditorWindow : EditorWindow
    {
        static Vector2 windowMinSize = new Vector2(800, 500);
        [UnityEditor.Callbacks.OnOpenAsset(0)]//Open�ض���  ˫��Ҳ���Դ����
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var instance = EditorUtility.InstanceIDToObject(instanceID);
            if (instance != null && instance is BootFile)
            {
                Open(instance as BootFile);
                return true;
            }
            return false;
        }

        internal static void Open(BootFile library)
        {
            var window = EditorWindow.GetWindow<BootFileEditorWindow>("GlobalConfig", true);
            if (window == null) return;
            window.minSize = windowMinSize;
            window.Show();
        }


    }
}
