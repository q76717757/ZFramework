using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 激活UI系统
/// </summary>
public class UserInterface : MonoSingleton<UserInterface>
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    #region 淡入淡出功能
    /// <summary>
    /// 淡入淡出效果接口方法
    /// </summary>
    /// <param name="panel">启用效果的面板</param>
    /// <param name="Panel">淡出后启用的面板</param>
    /// <param name="INorOUT">淡入或者淡出(true:淡入  false:淡出)</param>
    public void Fade(BasePanel panel, BasePanel Panel = null, bool INorOUT = true, float TimeScale = 0.75f)
    {
        if (INorOUT)
            StartCoroutine(FadeIN(panel, TimeScale));
        else if (!INorOUT)
        {
            StartCoroutine(FadeOUT(panel, Panel, TimeScale));
        }
                 
    }

    /// <summary>
    /// 淡入淡出效果接口方法
    /// </summary>
    /// <param name="panel">启用效果的面板</param>
    /// <param name="Panel">淡出后启用的面板</param>
    /// <param name="time">此面板存在的时间</param>
    public void Fade(BasePanel panel,BasePanel Panel, float time, float TimeScale = 0.75f)
    {
        StartCoroutine(Faded(panel, Panel, time, TimeScale));
    }

    /// <summary>
    /// 淡入效果
    /// </summary>
    /// <param name="panel">淡入的面板</param>
    /// <returns></returns>
    public IEnumerator FadeIN(BasePanel panel, float TimeScale = 0.75f)
    {
        if (UIManager.Instance.DicUI.ContainsKey(panel.Info))
        {
            GameObject Curtain;
            if (!UIManager.Instance.DicUI[panel.Info].transform.Find("Curtain"))
            {
                Curtain = GameObject.Instantiate(Resources.Load("UI/Misc/Curtain"), UIManager.Instance.DicUI[panel.Info].transform) as GameObject;
                Curtain.name = "Curtain";
            }
            else
                Curtain = UIManager.Instance.DicUI[panel.Info].transform.Find("Curtain").gameObject;
            Curtain.SetActive(true);
            var alpha = 1.0f;
            while (alpha > 0.01f)
            {
                alpha -= Time.deltaTime * TimeScale;
                Curtain.GetComponent<Image>().color = new Vector4(0, 0, 0, alpha);
                yield return null;
            }
            Curtain.GetComponent<Image>().color = Color.clear;
            Curtain.SetActive(false);
        }
        else Debug.LogError("!!!Can't Find Curtain!!!");
    }

    /// <summary>
    /// 淡出效果
    /// </summary>
    /// <param name="panel">淡出的面板</param>
    /// <returns></returns>
    public IEnumerator FadeOUT(BasePanel panel, BasePanel Panel = null, float TimeScale = 0.75f)
    {
        if (UIManager.Instance.DicUI.ContainsKey(panel.Info))
        {
            GameObject Curtain;
            if (!UIManager.Instance.DicUI[panel.Info].transform.Find("Curtain"))
            {
                Curtain = GameObject.Instantiate(Resources.Load("UI/Misc/Curtain"), UIManager.Instance.DicUI[panel.Info].transform) as GameObject;
                Curtain.name = "Curtain";
            }
            else
                Curtain = UIManager.Instance.DicUI[panel.Info].transform.Find("Curtain").gameObject;
            Curtain.SetActive(true);
            var alpha = 0.0f;
            while (alpha < 1.0f)
            {
                alpha += Time.deltaTime * TimeScale;
                Curtain.GetComponent<Image>().color = new Vector4(0, 0, 0, alpha);
                yield return null;
            }
            Curtain.GetComponent<Image>().color = Color.black;
            Curtain.SetActive(false);
            if (Panel != null)
            {
                UIManager.Instance.Pop();
                UIManager.Instance.Push(Panel);
            }                
        }
        else Debug.LogError("!!!Can't Find Curtain!!!");
    }

    /// <summary>
    /// 淡入淡出效果
    /// </summary>
    /// <param name="panel">实现效果的面板</param>
    /// <param name="Panel">下一个面板</param>
    /// <param name="duration">持续时间</param>
    /// <returns></returns>
    public IEnumerator Faded(BasePanel panel, BasePanel Panel, float duration, float TimeScale = 0.75f)
    {
        yield return FadeIN(panel, TimeScale);
        yield return new WaitForSeconds(duration);
        yield return FadeOUT(panel, null, TimeScale);
        UIManager.Instance.Pop();
        UIManager.Instance.Push(Panel);
    }
    #endregion

    #region 消息弹窗功能
    /// <summary>
    /// 消息弹窗功能接口方法
    /// </summary>
    /// <param name="panel">弹出的消息面板</param>
    /// <param name="text">消息内容</param>
    /// <param name="speed">显隐速度</param>
    public void ShowMessage(BasePanel panel, string text, float speed = 1.0f)
    {
        var msg = UIManager.Instance.GetUI(panel.Info);
        msg.transform.Find("Content").GetComponent<Text>().text = text;
        panel.OnEnter();
        StartCoroutine(Message(panel, speed));
    }

    public IEnumerator Message(BasePanel panel, float speed)
    {
        var Alpha = UIManager.Instance.DicUI[panel.Info].GetComponent<CanvasGroup>().alpha;
        while (Alpha < 1)
        {
            Alpha += Time.deltaTime * speed;
            UIManager.Instance.DicUI[panel.Info].GetComponent<CanvasGroup>().alpha = Alpha;
            yield return null;
        }
        yield return new WaitForSeconds(2.0f);
        while (Alpha > 0) 
        {
            Alpha -= Time.deltaTime * speed;
            UIManager.Instance.DicUI[panel.Info].GetComponent<CanvasGroup>().alpha = Alpha;
            yield return null;
        }
        UIManager.Instance.DestoryUI(panel.Info);
    }
    #endregion


}
#region 层级功能
public class Hierarchy
{
    public Hierarchy _Parent = null;
    public Hierarchy _Children;
}
#endregion
