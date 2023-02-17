using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ֡���¿�����
/// ��δ�̳�MonoBehaviour�����ṩ֡���·���
/// </summary>
public class RealTimeController : MonoBehaviour
{
    /// <summary>
    /// ֡�����¼���
    /// </summary>
    private event UnityAction FrameUpdateEvent;

    /// <summary>
    /// ��ʼ��
    /// </summary>
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// ֡���·���
    /// </summary>
    void Update()
    {
        if (FrameUpdateEvent != null)
            FrameUpdateEvent();
    }

    /// <summary>
    /// ����֡�����¼�
    /// </summary>
    /// <param name="Action">�¼�</param>
    public void AddUpdateListener(UnityAction Action)
    {
        FrameUpdateEvent += Action;
    }

    /// <summary>
    /// �Ƴ�֡�����¼�
    /// </summary>
    /// <param name="Action">�¼�</param>
    public void RemoveUpdateListener(UnityAction Action)
    {
        FrameUpdateEvent -= Action;
    }

}
