using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 帧更新控制器
/// 向未继承MonoBehaviour的类提供帧更新方法
/// </summary>
public class RealTimeController : MonoBehaviour
{
    /// <summary>
    /// 帧更新事件集
    /// </summary>
    private event UnityAction FrameUpdateEvent;

    /// <summary>
    /// 初始化
    /// </summary>
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// 帧更新方法
    /// </summary>
    void Update()
    {
        if (FrameUpdateEvent != null)
            FrameUpdateEvent();
    }

    /// <summary>
    /// 新增帧更新事件
    /// </summary>
    /// <param name="Action">事件</param>
    public void AddUpdateListener(UnityAction Action)
    {
        FrameUpdateEvent += Action;
    }

    /// <summary>
    /// 移除帧更新事件
    /// </summary>
    /// <param name="Action">事件</param>
    public void RemoveUpdateListener(UnityAction Action)
    {
        FrameUpdateEvent -= Action;
    }

}
