using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IncludeMissingObjects : EditorWindow
{
    private static List<int> missingScriptObject = new List<int>();
    static int index = -1;
    private GameObject prefab;

    [MenuItem("Tools/查找丢失物体")]
    private static void OpenWindow()
    {
        GetWindow<IncludeMissingObjects>("查询丢失脚本物体");
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("高亮丢失脚本物体",GUILayout.MinWidth(50), GUILayout.MaxHeight(30)))
        {
            FindObjects();
            index = -1;
            SelectObject();
        }
        if (GUILayout.Button("取消高亮效果", GUILayout.MinWidth(50), GUILayout.MaxHeight(30)))
        {
            missingScriptObject.Clear();
            index = -1;
            SelectObject();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("选择下一个丢失脚本物体",GUILayout.MinWidth(50), GUILayout.MaxHeight(30)))
        {
            SelectObject();
        }
        GUILayout.Space(30);

        GUILayout.BeginHorizontal("HelpBox");

        //预制体输入框
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false) as GameObject;
        GUILayout.Space(10);
        
        if (GUILayout.Button("检查"))
        {
            CheckPrefab();
        }
        GUILayout.EndHorizontal();

    }

    private void FindObjects()
    {
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        missingScriptObject.Clear();

        foreach (GameObject rootObject in rootObjects)
        {
            FindObjectsRecursive(rootObject);
        }
    }

    /// <summary>
    /// 判断是否存在组件已遗失
    /// </summary>
    /// <param name="gameObject"></param>
    private void FindObjectsRecursive(GameObject gameObject)
    {
        Component[] components = gameObject.GetComponents<Component>();

        foreach (Component component in components)
        {
            if (component == null)
            {
                missingScriptObject.Add(gameObject.GetInstanceID());
            }
        }

        //寻找子物体中的的文件
        Transform transform = gameObject.transform;
        int childCount = transform.childCount;
        if (childCount > 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                Transform childTransform = transform.GetChild(i);
                FindObjectsRecursive(childTransform.gameObject);
            }
        }
    }

    [InitializeOnLoadMethod]
    private static void RegisterHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyWindowItemOnGUI;
    }
    private static void UnRegisterHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyWindowItemOnGUI;
    }
    private static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (gameObject != null && ArrayContainsGameObjectID(gameObject.GetInstanceID()))
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.normal.textColor =new Color(0.6f,0.4f,0.3f,1);
            EditorGUI.DrawRect(selectionRect, new Color(0.9f, 0.9f, 0.9f, 1f));
            EditorGUI.LabelField(selectionRect, gameObject.name, style);
        }
    }

    private static bool ArrayContainsGameObjectID(int ID)
    {
        foreach (int gameObjectID in missingScriptObject)
        {
            if (ID == gameObjectID)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 选择下一个物体
    /// </summary>
    private static void SelectObject()
    {
        if (missingScriptObject.Count == 0) return;
        if (index < missingScriptObject.Count - 1)
            index++;
        else index = 0;
        // 查找目标物体
        GameObject targetObject = EditorUtility.InstanceIDToObject(missingScriptObject[index]) as GameObject;
            if (targetObject != null)
            {
                // 滚动并选中目标物体
                EditorGUIUtility.PingObject(targetObject);
                Selection.activeGameObject = targetObject;
            }
    }



    /// <summary>
    /// 检查预制体是否丢失代码
    /// </summary>
    private void CheckPrefab()
    {
        if (prefab == null)
        {
            EditorUtility.DisplayDialog("提示", "请添加要检查的预制体", "ok");
            return;
        }

        Component[] components = prefab.GetComponents<Component>();

        foreach (Component component in components)
        {
            if (component == null)
            {
                EditorUtility.DisplayDialog("错误提示","该预制体存在代码丢失","ok");
                Debug.Log("预制体丢失代码: " + prefab.gameObject.name);
                return;
            }
        }
        Debug.Log("预制体正常: " + prefab.gameObject.name);

    }

}
