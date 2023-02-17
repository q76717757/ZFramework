using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������
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
    /// �رհ�ť�¼�
    /// </summary>
    private void Btn_Close()
    {
        Debug.Log("�رհ�ť�¼�:���ٴ����");
        UIManager.Instance.Pop();
    }

    /// <summary>
    /// ���ܰ�ť_1
    /// </summary>
    private void Btn_Fuction_1()
    {
        Debug.Log("���ܰ�ť�¼�:�����������");
        UIManager.Instance.Push(new BillsPanel());
    }

    /// <summary>
    /// ���ܰ�ť_2
    /// </summary>
    private void Btn_Fuction_2()
    {
        Debug.Log("���ܰ�ť�¼�:������˽Э�����");
        UIManager.Instance.Push(new ProtocolPanel());
        UIManager.Instance.UI_GetGameObject("Title").GetComponent<TMP_Text>().text = "��˽Э��";
        UIManager.Instance.UI_GetGameObject("Text").GetComponent<Text>().text = "";
    }

    /// <summary>
    /// ���ܰ�ť_3
    /// </summary>
    private void Btn_Fuction_3()
    {
        Debug.Log("���ܰ�ť�¼�:�����û�Э�����");
        UIManager.Instance.Push(new ProtocolPanel());
        UIManager.Instance.UI_GetGameObject("Title").GetComponent<TMP_Text>().text = "�û�Э��";
        UIManager.Instance.UI_GetGameObject("Text").GetComponent<Text>().text = "";
    }

    /// <summary>
    /// ���ܰ�ť_4
    /// </summary>
    private void Btn_Fuction_4()
    {
        Debug.Log("���ܰ�ť�¼�:�����ⲿ��վ���");
        var _UrlTipsPanel = UIManager.Instance.Push<UrlTipsPanel>(new UrlTipsPanel());
        _UrlTipsPanel.url = "";
    }

    /// <summary>
    /// ���ܰ�ť_5
    /// </summary>
    private void Btn_Fuction_5()
    {
        Debug.Log("���ܰ�ť�¼�:�����ⲿ��վ���");
        var _UrlTipsPanel = UIManager.Instance.Push<UrlTipsPanel>(new UrlTipsPanel());
        _UrlTipsPanel.url = "";
    }

    /// <summary>
    /// ���ܰ�ť_6
    /// </summary>
    private void Btn_Fuction_6()
    {
        Debug.Log("���ܰ�ť�¼�:�����ⲿ��վ���");
        var _UrlTipsPanel = UIManager.Instance.Push<UrlTipsPanel>(new UrlTipsPanel());
        _UrlTipsPanel.url = "";
    }

    /// <summary>
    /// ���ܰ�ť_7
    /// </summary>
    private void Btn_Fuction_7()
    {
        Debug.Log("���ܰ�ť�¼�:������ʾ���");
        var _TipsPanel = UIManager.Instance.Push<TipsPanel>(new TipsPanel());
        _TipsPanel.isQuit = true;
        UIManager.Instance.UI_GetGameObject("Tips").GetComponent<TMP_Text>().text = "ȷ��Ҫ�˳���?";
    }

    /// <summary>
    /// �������ֻ�����
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
    /// ��Ч������
    /// </summary>
    private void Slider_MusicEffectBar(float value)
    {
        Debug.Log(value);
        UIManager.Instance.UI_GetGameObject("MusicEffectBar").transform.Find("Percent").GetComponent<TMP_Text>().text = Math.Truncate(value * 100) + "%";
        //var BGM_Player = GameObject.Find("BGM_Player");
        //BGM_Player.GetComponent<AudioSource>().volume = value;
    }
}

