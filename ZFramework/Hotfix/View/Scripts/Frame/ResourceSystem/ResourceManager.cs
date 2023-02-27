using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源管理器
/// </summary>
public class ResourceManager : Singleton<ResourceManager>
{
    /// <summary>
    /// 同步资源加载
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <returns></returns>
    public T LoadResource<T>(string path, string name = null, Vector3 position = default) where T : Object
    {
        T resource = Resources.Load<T>(path);
        GameObject _object = null;
        if (resource is GameObject)
        {
            _object = Object.Instantiate(resource) as GameObject;
            if (name != null)
                _object.name = name;
            else _object.name = path.Substring(path.LastIndexOf('/') + 1);
            _object.transform.position = position;
            return _object as T;
        }            
        else
            return resource;
    }

    /// <summary>
    /// 异步资源加载接口方法
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="name">资源命名</param>
    /// <param name="callback">回调函数</param>
    public void IAsyncLoadResource<T>(string path, string name = null, Vector3 position = default, UnityAction<T> callback = null) where T : Object
    {
        RealTimeManager.Instance.StartCoroutine(AsyncLoadResource(path, name, position, callback));
    }

    /// <summary>
    /// 异步资源加载
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="name">资源命名</param>
    /// <param name="callback">回调函数</param>
    /// <returns></returns>
    private IEnumerator AsyncLoadResource<T>(string path, string name, Vector3 position, UnityAction<T> callback) where T : Object
    {
        ResourceRequest resource = Resources.LoadAsync<T>(path);
        GameObject _object = null;
        yield return resource;

        if(callback != null)
        {
            if (resource.asset is GameObject)
            {
                _object = Object.Instantiate(resource.asset) as GameObject;
                if(name != null)
                    _object.name = name;
                else _object.name = path.Substring(path.LastIndexOf('/') + 1);
                _object.transform.position = position;
                callback(_object as T);
            }                
            else
                callback(resource.asset as T);
        }
        else
        {
            if (resource.asset is GameObject)
                _object = Object.Instantiate(resource.asset) as GameObject;
            if (name != null)
                _object.name = name;
            else _object.name = path.Substring(path.LastIndexOf('/') + 1);
            _object.transform.position = position;
        }
    }
}
