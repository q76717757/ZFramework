using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

/// <summary>
/// ֡���¹�����
/// ��δ�̳�MonoBehaviour�����ṩ֡���·����ĸ���ӿ�
/// </summary>
public class RealTimeManager : Singleton<RealTimeManager>
{
    /// <summary>
    /// ֡���¿���������
    /// </summary>
    private RealTimeController controller;

    /// <summary>
    /// ���캯��
    /// </summary>
    public RealTimeManager()
    {
        //��֤��MonoController�����Ψһ��
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<RealTimeController>();
    }

    /// <summary>
    /// ����֡�����¼��ӿ�
    /// </summary>
    /// <param name="Action">�¼�</param>
    public void AddUpdateListener(UnityAction Action)
    {
        controller.AddUpdateListener(Action);
    }

    /// <summary>
    /// �Ƴ�֡�����¼��ӿ�
    /// </summary>
    /// <param name="Action">�¼�</param>
    public void RemoveUpdateListener(UnityAction Action)
    {
        controller.RemoveUpdateListener(Action);
    }

    /// <summary>
    /// ��ʼЭ�̽ӿ�
    /// </summary>
    /// <param name="coroutine">Э��</param>
    /// <returns></returns>
    public Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return controller.StartCoroutine(coroutine);
    }

    /// <summary>
    /// ��ʼЭ�̽ӿ�
    /// </summary>
    /// <param name="Name">������</param>
    /// <param name="value">����</param>
    /// <returns></returns>
    public Coroutine StartCoroutine(string Name, [DefaultValue("null")] object value)
    {
        return controller.StartCoroutine(Name, value);
    }

    /// <summary>
    /// ��ʼЭ�̽ӿ�
    /// </summary>
    /// <param name="Name">������</param>
    /// <returns></returns>
    public Coroutine StartCoroutine(string Name)
    {
        return controller.StartCoroutine(Name);
    }
}
