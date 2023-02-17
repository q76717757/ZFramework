using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

/// <summary>
/// 帧更新管理器
/// 向未继承MonoBehaviour的类提供帧更新方法的各类接口
/// </summary>
public class RealTimeManager : Singleton<RealTimeManager>
{
    /// <summary>
    /// 帧更新控制器对象
    /// </summary>
    private RealTimeController controller;

    /// <summary>
    /// 构造函数
    /// </summary>
    public RealTimeManager()
    {
        //保证了MonoController对象的唯一性
        GameObject obj = new GameObject("MonoController");
        controller = obj.AddComponent<RealTimeController>();
    }

    /// <summary>
    /// 新增帧更新事件接口
    /// </summary>
    /// <param name="Action">事件</param>
    public void AddUpdateListener(UnityAction Action)
    {
        controller.AddUpdateListener(Action);
    }

    /// <summary>
    /// 移除帧更新事件接口
    /// </summary>
    /// <param name="Action">事件</param>
    public void RemoveUpdateListener(UnityAction Action)
    {
        controller.RemoveUpdateListener(Action);
    }

    /// <summary>
    /// 开始协程接口
    /// </summary>
    /// <param name="coroutine">协程</param>
    /// <returns></returns>
    public Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return controller.StartCoroutine(coroutine);
    }

    /// <summary>
    /// 开始协程接口
    /// </summary>
    /// <param name="Name">方法名</param>
    /// <param name="value">参数</param>
    /// <returns></returns>
    public Coroutine StartCoroutine(string Name, [DefaultValue("null")] object value)
    {
        return controller.StartCoroutine(Name, value);
    }

    /// <summary>
    /// 开始协程接口
    /// </summary>
    /// <param name="Name">方法名</param>
    /// <returns></returns>
    public Coroutine StartCoroutine(string Name)
    {
        return controller.StartCoroutine(Name);
    }
}
