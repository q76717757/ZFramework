/** Header
 * UnityIconView.cs
 * 
 **/

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZFramework;

class UnityIconView : EditorWindow
{
    static string[] text;
    [MenuItem("ZFramework/内置图标")]
    public static void ShowWindow()
    {
        var textAsset = Resources.Load<TextAsset>("InnerIconList");
        text = (textAsset as TextAsset).text.Replace("\r\n", "\n").Split('\n');
        Resources.UnloadAsset(textAsset);
        EditorWindow.GetWindow(typeof(UnityIconView));
    }

    public Vector2 scrollPosition;
    void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        //鼠标放在按钮上的样式
        foreach (MouseCursor item in Enum.GetValues(typeof(MouseCursor)))
        {
            GUILayout.Button(Enum.GetName(typeof(MouseCursor), item));
            EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), item);
            GUILayout.Space(10);
        }

        //内置图标
        for (int i = 0; i < text.Length; i += 8)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < 8; j++)
            {
                int index = i + j;
                if (index < text.Length)
                {
                    try
                    {
                        var con = EditorGUIUtility.IconContent(text[index]);
                        if (con != null)
                        {
                            if (GUILayout.Button(EditorGUIUtility.IconContent(text[index]), GUILayout.Width(50), GUILayout.Height(30)))
                            {
                                Log.Info(text[index]);
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            GUILayout.EndHorizontal();
        }




        GUILayout.EndScrollView();
    }
}
