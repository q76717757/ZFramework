using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置面板
/// </summary>
public class OptionPanel : BasePanel
{
    public static readonly string path = "UI/OptionPanel/OptionPanel";
    public OptionPanel() : base(new UI_Info(path)) { }

    public override void OnEnter()
    {
        base.OnEnter();
        #region -Initialize-
        UIManager.Instance.UI_GetGameObject("Btn_Close").GetComponent<Button>().onClick.AddListener(() => { Btn_Close(); });
        UIManager.Instance.UI_GetGameObject("Fuctions").transform.Find("Btn_Fuction_1").GetComponent<Button>().onClick.AddListener(() => { Btn_Fuction_1(); });
        UIManager.Instance.UI_GetGameObject("Fuctions").transform.Find("Btn_Fuction_2").GetComponent<Button>().onClick.AddListener(() => { Btn_Fuction_2(); });
        UIManager.Instance.UI_GetGameObject("Fuctions").transform.Find("Btn_Fuction_3").GetComponent<Button>().onClick.AddListener(() => { Btn_Fuction_3(); });
        UIManager.Instance.UI_GetGameObject("Fuctions").transform.Find("Btn_Fuction_4").GetComponent<Button>().onClick.AddListener(() => { Btn_Fuction_4(); });
        UIManager.Instance.UI_GetGameObject("Fuctions").transform.Find("Btn_Fuction_5").GetComponent<Button>().onClick.AddListener(() => { Btn_Fuction_5(); });
        UIManager.Instance.UI_GetGameObject("Fuctions").transform.Find("Btn_Fuction_6").GetComponent<Button>().onClick.AddListener(() => { Btn_Fuction_6(); });
        UIManager.Instance.UI_GetGameObject("Fuctions").transform.Find("Btn_Fuction_7").GetComponent<Button>().onClick.AddListener(() => { Btn_Fuction_7(); });
        UIManager.Instance.UI_GetGameObject("MusicBar").GetComponent<Slider>().onValueChanged.AddListener((float value) => { Slider_MusicBar(value); });
        UIManager.Instance.UI_GetGameObject("MusicBar").GetComponent<Slider>().value = PlayerPrefs.GetFloat("BGM_Volume", 100f);
        Slider_MusicBar(PlayerPrefs.GetFloat("BGM_Volume", 100f));
        //UIManager.Instance.UI_GetGameObject("MusicEffectBar").GetComponent<Slider>().onValueChanged.AddListener((float value) => { Slider_MusicEffectBar(value); });
        #endregion
    }

    /// <summary>
    /// 关闭按钮事件
    /// </summary>
    private void Btn_Close()
    {
        Debug.Log("关闭按钮事件:销毁此面板");
        UIManager.Instance.Pop();
    }

    /// <summary>
    /// 功能按钮_1
    /// </summary>
    private void Btn_Fuction_1()
    {
        Debug.Log("功能按钮事件:弹出订单面板");
        UIManager.Instance.Push(new BillsPanel());
    }

    /// <summary>
    /// 功能按钮_2
    /// </summary>
    private void Btn_Fuction_2()
    {
        Debug.Log("功能按钮事件:弹出隐私协议面板");
        UIManager.Instance.Push(new ProtocolPanel());
        UIManager.Instance.UI_GetGameObject("Title").GetComponent<TMP_Text>().text = "隐私协议";
        UIManager.Instance.UI_GetGameObject("Text").GetComponent<Text>().text = "";
    }

    /// <summary>
    /// 功能按钮_3
    /// </summary>
    private void Btn_Fuction_3()
    {
        Debug.Log("功能按钮事件:弹出用户协议面板");
        UIManager.Instance.Push(new ProtocolPanel());
        UIManager.Instance.UI_GetGameObject("Title").GetComponent<TMP_Text>().text = "用户协议";
        UIManager.Instance.UI_GetGameObject("Text").GetComponent<Text>().text = "";
    }

    /// <summary>
    /// 功能按钮_4
    /// </summary>
    private void Btn_Fuction_4()
    {
        Debug.Log("功能按钮事件:弹出外部网站面板");
        var _UrlTipsPanel = UIManager.Instance.Push<UrlTipsPanel>(new UrlTipsPanel());
        _UrlTipsPanel.url = "";
    }

    /// <summary>
    /// 功能按钮_5
    /// </summary>
    private void Btn_Fuction_5()
    {
        Debug.Log("功能按钮事件:弹出外部网站面板");
        var _UrlTipsPanel = UIManager.Instance.Push<UrlTipsPanel>(new UrlTipsPanel());
        _UrlTipsPanel.url = "";
    }

    /// <summary>
    /// 功能按钮_6
    /// </summary>
    private void Btn_Fuction_6()
    {
        Debug.Log("功能按钮事件:弹出外部网站面板");
        var _UrlTipsPanel = UIManager.Instance.Push<UrlTipsPanel>(new UrlTipsPanel());
        _UrlTipsPanel.url = "";
    }

    /// <summary>
    /// 功能按钮_7
    /// </summary>
    private void Btn_Fuction_7()
    {
        Debug.Log("功能按钮事件:弹出提示面板");
        var _TipsPanel = UIManager.Instance.Push<TipsPanel>(new TipsPanel());
        _TipsPanel.isQuit = true;
        UIManager.Instance.UI_GetGameObject("Tips").GetComponent<TMP_Text>().text = "确定要退出吗?";
    }

    /// <summary>
    /// 背景音乐滑动条
    /// </summary>
    private void Slider_MusicBar(float value)
    {
        Debug.Log(value);
        UIManager.Instance.UI_GetGameObject("MusicBar").transform.Find("Percent").GetComponent<TMP_Text>().text = Math.Truncate(value * 100) + "%";
        var BGM_Player = GameObject.Find("BGM_Player");
        BGM_Player.GetComponent<AudioSource>().volume = value;
        PlayerPrefs.SetFloat("BGM_Volume", value);
    }

    /// <summary>
    /// 音效滑动条
    /// </summary>
    private void Slider_MusicEffectBar(float value)
    {
        Debug.Log(value);
        UIManager.Instance.UI_GetGameObject("MusicEffectBar").transform.Find("Percent").GetComponent<TMP_Text>().text = Math.Truncate(value * 100) + "%";
        //var BGM_Player = GameObject.Find("BGM_Player");
        //BGM_Player.GetComponent<AudioSource>().volume = value;
    }
}

