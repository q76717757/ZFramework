using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池管理器
/// </summary>
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    /// <summary>
    /// 对象池
    /// </summary>
    private Dictionary<string, List<GameObject>> ObjectPool;

    /// <summary>
    /// 构造函数
    /// </summary>
    public ObjectPoolManager()
    {
        //初始化对象池
        ObjectPool = new Dictionary<string, List<GameObject>>();
    }

    /// <summary>
    /// 获取对象
    /// </summary>
    /// <param name="path">对象路径</param>
    /// <returns>该对象</returns>
    public GameObject GetObject(string path, string name = null, Vector3 position = default)
    {
        GameObject obj = null;
        name ??= path.Substring(path.LastIndexOf('/') + 1);
        //若对象池中存在此对象的列表且列表不为空,取出此对象
        if (ObjectPool.ContainsKey(name) && ObjectPool[name].Count > 0) 
        {
            obj = ObjectPool[name][0];
            ObjectPool[name].RemoveAt(0);
        }
        //否则实例化此物体
        else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(path));
            obj.name = name;
            obj.transform.position = position;
        }
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 获取对象并附加到父物体上
    /// </summary>
    /// <param name="path">对象路径</param>
    /// <param name="parent">父物体</param>
    /// <returns>该对象</returns>
    public GameObject GetObject(string path, Transform parent, string name = null, Vector3 position = default)
    {
        GameObject obj = null;
        name ??= path.Substring(path.LastIndexOf('/') + 1);
        //若对象池中存在此对象的列表且列表不为空,取出此对象
        if (ObjectPool.ContainsKey(name) && ObjectPool[name].Count > 0)
        {
            obj = ObjectPool[name][0];
            ObjectPool[name].RemoveAt(0);
        }
        //否则实例化此物体,并附加至父物体
        else
        {
            obj = GameObject.Instantiate(Resources.Load<GameObject>(path), parent);
            obj.name = name;
            obj.transform.localPosition = position;
        }
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="obj">将回收的对象</param>
    public void RecycleObject(GameObject obj)
    {
        //若为空,返回
        if(obj == null) return;
        
        obj.SetActive(false);
        //若对象池中存在此物体的列表,则回收至对应列表
        if (ObjectPool.ContainsKey(obj.name))
        {
            ObjectPool[obj.name].Add(obj);
        }
        //若无则新建列表后回收
        else
        {
            ObjectPool.Add(obj.name, new List<GameObject> { obj });
        }
    }

    /// <summary>
    /// 清空对象池
    /// </summary>
    public void Clear()
    {
        ObjectPool.Clear();
    }
}
