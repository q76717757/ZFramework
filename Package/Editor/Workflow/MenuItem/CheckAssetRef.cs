using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace ZFramework.Editor
{
    public class CheckAssetRef
    {
        [MenuItem("Assets/ZFramework/查找资产依赖项",true)]
        static bool A()
        {
            if (Selection.objects.Length == 1 && AssetDatabase.Contains(Selection.activeObject))
            {
                //排除掉文件夹本身
                if (AssetDatabase.IsForeignAsset(Selection.activeObject) || AssetDatabase.IsNativeAsset(Selection.activeObject))
                {
                    return true;
                }
            }
            return false;
        }

        [MenuItem("Assets/ZFramework/查找资产依赖项")]
        static void OnClick()
        {
            var s = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(Selection.activeObject),false);
            foreach (var item in s)//打印出来的包括自己
            {
                Debug.Log(item);
            }
        }

    }
}
