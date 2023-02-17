using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ��Դ������
/// </summary>
public class ResourceManager : Singleton<ResourceManager>
{
    /// <summary>
    /// ͬ����Դ����
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
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
    /// �첽��Դ���ؽӿڷ���
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
    /// <param name="name">��Դ����</param>
    /// <param name="callback">�ص�����</param>
    public void IAsyncLoadResource<T>(string path, string name = null, Vector3 position = default, UnityAction<T> callback = null) where T : Object
    {
        RealTimeManager.Instance.StartCoroutine(AsyncLoadResource(path, name, position, callback));
    }

    /// <summary>
    /// �첽��Դ����
    /// </summary>
    /// <typeparam name="T">��Դ����</typeparam>
    /// <param name="path">��Դ·��</param>
    /// <param name="name">��Դ����</param>
    /// <param name="callback">�ص�����</param>
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
