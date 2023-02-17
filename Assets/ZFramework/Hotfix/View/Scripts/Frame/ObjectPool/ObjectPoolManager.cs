using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ع�����
/// </summary>
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    /// <summary>
    /// �����
    /// </summary>
    private Dictionary<string, List<GameObject>> ObjectPool;

    /// <summary>
    /// ���캯��
    /// </summary>
    public ObjectPoolManager()
    {
        //��ʼ�������
        ObjectPool = new Dictionary<string, List<GameObject>>();
    }

    /// <summary>
    /// ��ȡ����
    /// </summary>
    /// <param name="path">����·��</param>
    /// <returns>�ö���</returns>
    public GameObject GetObject(string path, string name = null, Vector3 position = default)
    {
        GameObject obj = null;
        name ??= path.Substring(path.LastIndexOf('/') + 1);
        //��������д��ڴ˶�����б����б�Ϊ��,ȡ���˶���
        if (ObjectPool.ContainsKey(name) && ObjectPool[name].Count > 0) 
        {
            obj = ObjectPool[name][0];
            ObjectPool[name].RemoveAt(0);
        }
        //����ʵ����������
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
    /// ��ȡ���󲢸��ӵ���������
    /// </summary>
    /// <param name="path">����·��</param>
    /// <param name="parent">������</param>
    /// <returns>�ö���</returns>
    public GameObject GetObject(string path, Transform parent, string name = null, Vector3 position = default)
    {
        GameObject obj = null;
        name ??= path.Substring(path.LastIndexOf('/') + 1);
        //��������д��ڴ˶�����б����б�Ϊ��,ȡ���˶���
        if (ObjectPool.ContainsKey(name) && ObjectPool[name].Count > 0)
        {
            obj = ObjectPool[name][0];
            ObjectPool[name].RemoveAt(0);
        }
        //����ʵ����������,��������������
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
    /// ���ն���
    /// </summary>
    /// <param name="obj">�����յĶ���</param>
    public void RecycleObject(GameObject obj)
    {
        //��Ϊ��,����
        if(obj == null) return;
        
        obj.SetActive(false);
        //��������д��ڴ�������б�,���������Ӧ�б�
        if (ObjectPool.ContainsKey(obj.name))
        {
            ObjectPool[obj.name].Add(obj);
        }
        //�������½��б�����
        else
        {
            ObjectPool.Add(obj.name, new List<GameObject> { obj });
        }
    }

    /// <summary>
    /// ��ն����
    /// </summary>
    public void Clear()
    {
        ObjectPool.Clear();
    }
}
