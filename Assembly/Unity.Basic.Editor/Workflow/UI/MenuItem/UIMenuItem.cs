using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ZFramework.Editor
{
    internal class UIMenuItem
    {
        [MenuItem("GameObject/ZUI/ZButton",priority = 8)]
        public static void CreateBtn()
        {
            GameObject g = new GameObject { name = "ZButton" };

            Transform parent = GetParentTransform();
            g.transform.SetParent(parent, false);
            g.AddComponent<ZImage>();
            g.AddComponent<ZButton>();
            Selection.activeGameObject = g;

            Undo.RegisterCreatedObjectUndo(g, "ZButton Created");
            EditorUtility.SetDirty(g);
        }
        [MenuItem("GameObject/ZUI/ZImage", priority = 8)]
        public static void CreateImage()
        {
            GameObject g = new GameObject { name = "ZImage" };

            Transform parent = GetParentTransform();
            g.transform.SetParent(parent, false);
            g.AddComponent<ZImage>();
            Selection.activeGameObject = g;

            Undo.RegisterCreatedObjectUndo(g, "ZImage Created");
            EditorUtility.SetDirty(g);
        }

        [MenuItem("GameObject/ZUI/ZRawImage", priority = 8)]
        public static void CreateRawImage()
        {
            GameObject g = new GameObject { name = "ZRawImage" };

            Transform parent = GetParentTransform();
            g.transform.SetParent(parent, false);
            g.AddComponent<ZImage>();
            Selection.activeGameObject = g;

            Undo.RegisterCreatedObjectUndo(g, "ZRawImage Created");
            EditorUtility.SetDirty(g);
        }
        //.....more


        private static Transform GetParentTransform()
        {
            Transform parent;
            if (Selection.activeGameObject != null &&
                Selection.activeGameObject.GetComponentInParent<Canvas>() != null)
            {
                parent = Selection.activeGameObject.transform;
            }
            else
            {
                Canvas c = GetCanvas();
                //AddAdditionalShaderChannelsToCanvas(c);
                parent = c.transform;
            }

            return parent;
        }
        private static Canvas GetCanvas()
        {
            StageHandle handle = StageUtility.GetCurrentStageHandle();
            if (!handle.FindComponentOfType<Canvas>())
            {
                EditorApplication.ExecuteMenuItem("GameObject/UI/Canvas");
            }

            Canvas c = handle.FindComponentOfType<Canvas>();
            return c;
        }
    }
}
