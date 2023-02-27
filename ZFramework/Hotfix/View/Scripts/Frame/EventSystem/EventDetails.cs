using UnityEngine.Events;

/// <summary>
/// 事件接口
/// </summary>
public interface IEventDetails { }

/// <summary>
/// 事件
/// </summary>
/// <typeparam name="T">参数类型</typeparam>
public class EventDetails<T> : IEventDetails
{
    /// <summary>
    /// 事件内容
    /// </summary>
    public UnityAction<T> Actions;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="action">事件</param>
    public EventDetails(UnityAction<T> action)
    {
        Actions += action;
    }
}

/// <summary>
/// 事件(无参)
/// </summary>
public class EventDetails : IEventDetails
{
    /// <summary>
    /// 事件内容
    /// </summary>
    public UnityAction Actions;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="action">事件</param>
    public EventDetails(UnityAction action)
    {
        Actions += action;
    }
}
