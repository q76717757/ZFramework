using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace ZFramework.Editor
{
    public class ECSTreeViewWindow : EditorWindow
    {
        [MenuItem("ZFramework/ECSTreeViewWindow")]
        static void ShowNewHierarchy()
        {
            GetWindow<ECSTreeViewWindow>("ECS TreeView");
        }

        ECSTreeView view;
        Vector2 p = default;

        void OnGUI()
        {
            if (Application.isPlaying)
            {
                DrawGameInstance();
            }
            else
            {
                EditorGUILayout.LabelField("Runtime下生效");
            }
        }

        ECSTreeView GetView()
        {
            if (view == null)
            {
                return default;
            }
            else
            {
                return view;
            }
        }

        //想要把服务器那边也画出来,那么服务器那边需要将ecs的内容序列化之后将内容传过来,这边反序列化成对象,然后创建ViewItem进行绑定
        //这样才可以绘制出Server那边的树结构,客户端和服务端共用,所以客户端也需要将树先序列化
        void DrawGameInstance()
        {
            Rect rect = this.rootVisualElement.contentRect;
            EditorGUILayout.LabelField(rect.ToString());

            ECSTreeView view = GetView();

            p = EditorGUILayout.BeginScrollView(p);
            if (view != null)
            {
                view.OnGUI(GUILayoutUtility.GetRect(rect.width, view.totalHeight));
            }
            for (int i = 0; i < 30; i++)
            {
                EditorGUILayout.LabelField(i.ToString());
            }
            EditorGUILayout.EndScrollView();
        }

    }
}
