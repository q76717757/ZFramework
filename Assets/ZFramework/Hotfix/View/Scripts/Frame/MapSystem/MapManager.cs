using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景加载管理器
/// </summary>
public class MapManager : Singleton<MapManager>
{
    /// <summary>
    /// 同步加载场景
    /// 阻塞游戏进程
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="action">功能函数</param>
    public void LoadScene(string name,UnityAction action)
    {
        //完成场景加载后才触发功能函数
        SceneManager.LoadScene(name);
        {
            action();
        }
    }

    /// <summary>
    /// 异步加载场景的接口方法
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="time">时长</param>
    /// <param name="action">功能函数</param>
    public void IAsynLoadScene(string name, float time = 0, UnityAction action = null)
    {
        if (time < 0) return;           
        RealTimeManager.Instance.StartCoroutine(AsynLoadScene(name, time, action));
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="name">场景名</param>
    /// <param name="time">时长</param>
    /// <param name="action">功能函数</param>
    private IEnumerator AsynLoadScene(string name, float time, UnityAction action)
    {
        //展示用进度(用于进度条更新)
        float DisplayProgress = 0f;
        //实际进度(场景加载进度)
        float RealProgress = 0f;
        //定义异步事件
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        //禁用自动跳转
        operation.allowSceneActivation = false;
        /*
         * 当实际进度小于90%,且展示用进度小于实际进度时
         * 以预设速度更新展示用进度,并触发订阅了"场景加载进度条更新"事件的委托函数
         */
        while (RealProgress < 0.9f)
        {
            RealProgress = operation.progress;
            while (DisplayProgress < RealProgress)
            {
                DisplayProgress += 0.01f;
                EventManager.Instance.EventTrigger("场景加载进度条更新", DisplayProgress);
                yield return null;
            }
        }
        /*
         * 当真实进度不小于90%时,且展示用进度未满时
         * 以预设速度更新展示用进度,并触发订阅了"场景加载进度条更新"事件的委托函数
         */
        while (DisplayProgress < 1)
        {
            DisplayProgress += 0.01f;
            EventManager.Instance.EventTrigger("场景加载进度条更新", DisplayProgress);
            yield return null;
        }
        /*
         * 当场景加载未完成,且展示用进度已满时
         * 将展示用进度设置为满值,并触发订阅了"场景加载进度条更新"事件的委托函数
         */
        while (!operation.isDone)
        {
            EventManager.Instance.EventTrigger("场景加载进度条更新", 1f);
            //当且仅当展示用进度接近满时,允许场景自动跳转,避免进度条未满时跳转
            if (DisplayProgress >= 0.95f)
            {
                operation.allowSceneActivation = true;
            }
            if (time != 0)
                yield return new WaitForSeconds(time);
            else yield return null;
        }
        //场景加载完成后执行功能函数
        action?.Invoke();
    }
}
