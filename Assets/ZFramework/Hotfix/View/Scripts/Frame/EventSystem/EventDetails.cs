using UnityEngine.Events;

/// <summary>
/// �¼��ӿ�
/// </summary>
public interface IEventDetails { }

/// <summary>
/// �¼�
/// </summary>
/// <typeparam name="T">��������</typeparam>
public class EventDetails<T> : IEventDetails
{
    /// <summary>
    /// �¼�����
    /// </summary>
    public UnityAction<T> Actions;

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="action">�¼�</param>
    public EventDetails(UnityAction<T> action)
    {
        Actions += action;
    }
}

/// <summary>
/// �¼�(�޲�)
/// </summary>
public class EventDetails : IEventDetails
{
    /// <summary>
    /// �¼�����
    /// </summary>
    public UnityAction Actions;

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="action">�¼�</param>
    public EventDetails(UnityAction action)
    {
        Actions += action;
    }
}
