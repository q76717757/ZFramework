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
                EditorGUILayout.LabelField("Runtime����Ч");
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

        //��Ҫ�ѷ������Ǳ�Ҳ������,��ô�������Ǳ���Ҫ��ecs���������л�֮�����ݴ�����,��߷����л��ɶ���,Ȼ�󴴽�ViewItem���а�
        //�����ſ��Ի��Ƴ�Server�Ǳߵ����ṹ,�ͻ��˺ͷ���˹���,���Կͻ���Ҳ��Ҫ���������л�
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
