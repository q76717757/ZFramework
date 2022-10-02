using UnityEngine;
using UnityEditor;

namespace ZFramework
{
    [CustomEditor(typeof(BootStrap))]
    public class BootStrapInspector: Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("引导文件", GUILayout.Width(50));
            var obj = serializedObject.FindProperty("boot");
            obj.objectReferenceValue = (BootFile)EditorGUILayout.ObjectField(obj.objectReferenceValue, typeof(BootFile), false);
            EditorGUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();

            if (obj.objectReferenceValue != null)
            {
                BootFileEditor.Draw(new SerializedObject(obj.objectReferenceValue));
            }
            else
            {
                EditorGUILayout.HelpBox("需要一个引导文件", MessageType.Error);
            }
        }
    }



#if UNITY_2020_3_OR_NEWER
    [InitializeOnLoad]
    public class BootStrapHierarchyColor
    {
        static BootStrapHierarchyColor()
        {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
        }

        private static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is GameObject obj && obj != null)
            {
                var Style = new GUIStyle()
                {
                    padding =
                    {
                        left =EditorStyles.label.padding.left + 17,
                        top = EditorStyles.label.padding.top
                    },
                    normal =
                    {
                        textColor = Color.green
                    }
                };

                if (obj.GetComponent<BootStrap>() != null && Style != null)
                {
                    GUI.Label(selectionRect, obj.name, Style);//重叠的 底层原本的字还在
                }
            }
        }
    }
#endif
}
