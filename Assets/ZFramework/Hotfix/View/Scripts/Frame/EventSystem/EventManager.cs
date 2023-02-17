using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �¼�������
/// </summary>
public class EventManager : Singleton<EventManager>
{
    /// <summary>
    /// �¼�����
    /// Key:�¼���
    /// Value:��������¼������ɸ�ί�к���
    /// </summary>
    private Dictionary<string, IEventDetails> DicEvent;

    /// <summary>
    /// ���캯��
    /// </summary>
    public EventManager()
    {
        DicEvent = new Dictionary<string, IEventDetails>();
    }

    /// <summary>
    /// ����µ��¼�����
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="name">�¼���</param>
    /// <param name="action">ί�к���</param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        //�ж϶�Ӧ���¼������Ƿ��Ѿ�����
        if (DicEvent.ContainsKey(name))
        {
            (DicEvent[name] as EventDetails<T>).Actions += action;
        }
        else
        {
            DicEvent.Add(name, new EventDetails<T>(action));
        }
    }

    /// <summary>
    /// ����µ��¼�����(�޲�)
    /// </summary>
    /// <param name="name">�¼���</param>
    /// <param name="action">ί�к���</param>
    public void AddEventListener(string name, UnityAction action)
    {
        //�ж϶�Ӧ���¼������Ƿ��Ѿ�����
        if (DicEvent.ContainsKey(name))
        {
            (DicEvent[name] as EventDetails).Actions += action;
        }
        else
        {
            DicEvent.Add(name, new EventDetails(action));
        }
    }

    /// <summary>
    /// �Ƴ��¼�����
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="name">�¼���</param>
    /// <param name="action">��Ҫ�Ƴ���ĳ��ί�к���</param>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (DicEvent.ContainsKey(name))
            (DicEvent[name] as EventDetails<T>).Actions -= action;
    }

    /// <summary>
    /// �Ƴ��¼�����(�޲�)
    /// </summary>
    /// <param name="name">�¼���</param>
    /// <param name="action">��Ҫ�Ƴ���ĳ��ί�к���</param>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (DicEvent.ContainsKey(name))
            (DicEvent[name] as EventDetails).Actions -= action;
    }

    /// <summary>
    /// �¼�������
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="name">�¼���</param>
    /// <param name="info">��Ҫ���ݵĲ�������</param>
    public void EventTrigger<T>(string name,T info)
    {
        //���¼����ĺ��ж�Ӧ�¼��Ҽ����¼���ί�к�����Ϊ��,�򴥷�����ί�к���
        if (DicEvent.ContainsKey(name))
        {
            if ((DicEvent[name] as EventDetails<T>).Actions != null)
                (DicEvent[name] as EventDetails<T>).Actions.Invoke(info);
        }
    }

    /// <summary>
    /// �¼�������(�޲�)
    /// </summary>
    /// <param name="name">�¼���</param>
    public void EventTrigger(string name)
    {
        //���¼����ĺ��ж�Ӧ�¼��Ҽ����¼���ί�к�����Ϊ��,�򴥷�����ί�к���
        if (DicEvent.ContainsKey(name))
        {
            if ((DicEvent[name] as EventDetails).Actions != null)
                (DicEvent[name] as EventDetails).Actions.Invoke();
        }
    }

    /// <summary>
    /// ����¼�����
    /// Tips:�л�����ʱ������¼�����
    /// </summary>
    public void Clear()
    {
        DicEvent.Clear();
    }
}
