//----------------------------
//弹窗
//----------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Reflection;

public class MyEditor2 : EditorWindow
{
}
public class MyEditor : EditorWindow
{
    [MenuItem("ZFramework/backup/测试窗口")]
    public static void AddWindow()
    {
        if (!EditorUtility.DisplayDialog("123123", "321321", "cc", "BB"))
        {
            return;
        }

        //创建窗口
        Rect wr = new Rect(0, 0, 500, 500);

        MyEditor window = (MyEditor)EditorWindow.GetWindowWithRect<MyEditor>(wr, true, "固定窗口");
        window.Show();
        var windows2 = GetWindow<MyEditor2>("伸缩窗口");//可吸附

        window.ProgressBar();//进度条
    }

    //float t = 0;
    double t2;
    double dt = 0;
    void ProgressBar()
    { //进度条
        t2 = EditorApplication.timeSinceStartup;
        EditorApplication.update += delegate ()
        {
            if (dt < 10)
            {
                if (EditorUtility.DisplayCancelableProgressBar("3", "4", (float)(dt * 0.1)))
                {
                    EditorUtility.ClearProgressBar();
                    EditorApplication.update = null;
                }
                else
                {
                    dt = EditorApplication.timeSinceStartup - t2;
                }
            }
            else
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
            }
        };

    }

    //输入文字的内容
    private string text;
    //选择贴图的对象
    private Texture texture;

    public void Awake()
    {
        //在资源中读取一张贴图
        texture = Resources.Load("紫外线测试图") as Texture;
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
    }

    private void OnDisable()
    {
        Debug.Log("OnDisable");
    }
    private void OnValidate()
    {
        Debug.Log("OnValidate");
    }

    Gradient tempGra = new Gradient();
    float value = 0;
    //绘制窗口时调用
    void OnGUI()
    {
        //输入框控件
        text = EditorGUILayout.TextField("输入文字:", text);
        value = EditorGUILayout.FloatField("输入Float", value);
        EditorGUILayout.GradientField(tempGra);
        if (GUILayout.Button("打开通知", GUILayout.Width(200)))
        {
            //打开一个通知栏

            GUIContent temp = new GUIContent() { image = texture, text = "内容11111", tooltip = "提示" };
            this.ShowNotification(temp);
        }

        if (GUILayout.Button("关闭通知", GUILayout.Width(200)))
        {
            //关闭通知栏
            this.RemoveNotification();
        }
        //文本框显示鼠标在窗口的位置  //相对于窗口0,0点(左上角)
        EditorGUILayout.LabelField("鼠标在窗口的位置", Event.current.mousePosition.ToString());
        value = EditorGUILayout.Slider(new GUIContent("GUIContent", texture, "tips"), value, 0, 2);
        EditorGUILayout.Slider("label", value, 0, 2);
        //选择贴图
        texture = EditorGUILayout.ObjectField("添加贴图", texture, typeof(Texture), true) as Texture;

        if (GUILayout.Button("关闭窗口", GUILayout.Width(200)))
        {
            //关闭窗口
            this.Close();
        }
        GUILayout.TextArea("TextArea" + text, 200);
        GUILayout.TextField("TextField" + text, 110);
    }
    private void Update()
    {
        Debug.Log("update");
    }

    void OnFocus()
    {
        Debug.Log("获得焦点");
    }

    void OnLostFocus()
    {
        Debug.Log("失去焦点");
    }

    void OnHierarchyChange()
    {
        Debug.Log("当Hierarchy视图中的任何对象发生改变时调用一次");
    }

    void OnProjectChange()
    {
        Debug.Log("当Project视图中的资源发生改变时调用一次");
    }

    void OnInspectorUpdate()
    {
        //Debug.Log("窗口面板的更新");
        //这里开启窗口的重绘，不然窗口信息不会刷新
        this.Repaint();
    }

    void OnSelectionChange()
    {
        //当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
        foreach (Transform t in Selection.transforms)
        {
            //有可能是多选，这里开启一个循环打印选中游戏对象的名称
            Debug.Log("OnSelectionChange" + t.name);
        }
    }

    void OnDestroy()
    {
        Debug.Log("当窗口关闭时调用");
    }
}

public class NodeEditor : EditorWindow
{

    Rect window1;

    Rect window2;

    [MenuItem("ZFramework/Node editor")]
    static void ShowEditor()
    {
        NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();
        editor.Init();
    }
    public void Init()
    {
        window1 = new Rect(10, 10, 100, 100);
        window2 = new Rect(210, 210, 100, 100);
    }
    void OnGUI()
    {
        DrawNodeCurve(window1, window2); // Here the curve is drawn under the windows
        BeginWindows();
        window1 = GUI.Window(1, window1, DrawNodeWindow, "Window 1");   // Updates the Rect's when these are dragged
        window2 = GUI.Window(2, window2, DrawNodeWindow, "Window 2");
        EndWindows();
    }
    void DrawNodeWindow(int id)
    {
        GUI.DragWindow();
    }
    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);
        for (int i = 0; i < 3; i++) // Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.green, null, 5);
    }
}

#if UNITY_EDITOR
public class ZDebugRedirect//重定向相关
{
    const string logCSName = "/Log.cs:";
    static bool isLock = false;

    //反射堆栈信息 
    private static string GetStackTrace()
    {
        Assembly editorWindowAssembly = typeof(EditorWindow).Assembly;

        Type consoleWindowType = editorWindowAssembly.GetType("UnityEditor.ConsoleWindow");

        FieldInfo consoleWindowFieldInfo = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);

        EditorWindow consoleWindow = consoleWindowFieldInfo.GetValue(null) as EditorWindow;

        if (consoleWindow != EditorWindow.focusedWindow) return null;

        var activeTextFieldInfo = consoleWindowType.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
        //核心代码 m_ActiveText字段存有堆栈信息 网上的普遍流传的是反射堆栈容器再读值的方式会报错 现版本已不适用
        //没有必要拿到LogEntry，因为ConsoleWindow中的m_ActiveText就是StackTrace字符串 反射LogEntry对象的方式反而会报断言失败 试过延迟反射并不管用
        //Assertion failed on expression: 'm_CurrentEntriesPtr != NULL && m_IsGettingEntries'

        return (string)activeTextFieldInfo.GetValue(consoleWindow);
    }

    //字符串操作 提取脚本名和行数
    private static string GetRowCount(ref int line)
    {
        string condition = GetStackTrace();
        int index = condition.IndexOf(logCSName, StringComparison.Ordinal);
        if (index < 0)
        {
            return null;
        }
        int lineIndex = condition.IndexOf(")", index, StringComparison.Ordinal);
        condition = condition.Substring(lineIndex + 2);//去掉一个换行符和右括号
        index = condition.IndexOf(".cs:", StringComparison.Ordinal);
        if (index >= 0)
        {
            int lineEndIndex = condition.IndexOf(")", index, StringComparison.Ordinal);
            string _line = condition.Substring(index + 4, lineEndIndex - index - 4);
            int.TryParse(_line, out line);
            condition = condition.Substring(0, index);
            int startIndex = condition.LastIndexOf("/", StringComparison.Ordinal);
            string fileName = condition.Substring(startIndex + 1);
            fileName += ".cs";
            return fileName;
        }
        return null;
    }

    [OnOpenAsset(0)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        //Debug.Log("-->" + instanceID + "-->" + line);
        if (!EditorWindow.focusedWindow.titleContent.text.Equals("Console")) return false;//只对从控制台开启的重定向
        //if (EditorUtility.InstanceIDToObject(instanceID) is MonoScript) return false;//只对脚本重定向
        if (isLock) return false;//离开递归

        string fileName = GetRowCount(ref line);
        if (string.IsNullOrEmpty(fileName) || !fileName.EndsWith(".cs", StringComparison.Ordinal))
            return false;

        fileName = fileName.Substring(0, fileName.Length - 3);//去后缀

        string[] searchPaths = AssetDatabase.FindAssets(fileName + " t:MonoScript");
        for (int i = 0; i < searchPaths.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(searchPaths[i]);

            string checkName =System.IO.Path.GetFileNameWithoutExtension(path);

            if (checkName == fileName)
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript));
                isLock = true;
                AssetDatabase.OpenAsset(obj, line);//将引起递归 
                isLock = false;
                //Debug.Log(obj.ToString() + ":" + line);
                return true;//取代默认方式
            }
        }
        return false;
    }
}
#endif
