using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 事件管理器
/// </summary>
public class EventManager : Singleton<EventManager>
{
    /// <summary>
    /// 事件中心
    /// Key:事件名
    /// Value:监听这个事件的若干个委托函数
    /// </summary>
    private Dictionary<string, IEventDetails> DicEvent;

    /// <summary>
    /// 构造函数
    /// </summary>
    public EventManager()
    {
        DicEvent = new Dictionary<string, IEventDetails>();
    }

    /// <summary>
    /// 添加新的事件监听
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="name">事件名</param>
    /// <param name="action">委托函数</param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        //判断对应的事件监听是否已经存在
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
    /// 添加新的事件监听(无参)
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="action">委托函数</param>
    public void AddEventListener(string name, UnityAction action)
    {
        //判断对应的事件监听是否已经存在
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
    /// 移除事件监听
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="name">事件名</param>
    /// <param name="action">需要移除的某个委托函数</param>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (DicEvent.ContainsKey(name))
            (DicEvent[name] as EventDetails<T>).Actions -= action;
    }

    /// <summary>
    /// 移除事件监听(无参)
    /// </summary>
    /// <param name="name">事件名</param>
    /// <param name="action">需要移除的某个委托函数</param>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (DicEvent.ContainsKey(name))
            (DicEvent[name] as EventDetails).Actions -= action;
    }

    /// <summary>
    /// 事件触发器
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="name">事件名</param>
    /// <param name="info">需要传递的参数内容</param>
    public void EventTrigger<T>(string name,T info)
    {
        //若事件中心含有对应事件且监听事件的委托函数不为空,则触发所有委托函数
        if (DicEvent.ContainsKey(name))
        {
            if ((DicEvent[name] as EventDetails<T>).Actions != null)
                (DicEvent[name] as EventDetails<T>).Actions.Invoke(info);
        }
    }

    /// <summary>
    /// 事件触发器(无参)
    /// </summary>
    /// <param name="name">事件名</param>
    public void EventTrigger(string name)
    {
        //若事件中心含有对应事件且监听事件的委托函数不为空,则触发所有委托函数
        if (DicEvent.ContainsKey(name))
        {
            if ((DicEvent[name] as EventDetails).Actions != null)
                (DicEvent[name] as EventDetails).Actions.Invoke();
        }
    }

    /// <summary>
    /// 清空事件中心
    /// Tips:切换场景时请清空事件中心
    /// </summary>
    public void Clear()
    {
        DicEvent.Clear();
    }
}
