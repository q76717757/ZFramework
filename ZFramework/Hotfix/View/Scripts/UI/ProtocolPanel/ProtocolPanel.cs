using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 协议面板
/// </summary>
public class ProtocolPanel : BasePanel
{
    public static readonly string path = "UI/ProtocolPanel/ProtocolPanel";
    public ProtocolPanel() : base(new UI_Info(path)) { }

    public override void OnEnter()
    {
        base.OnEnter();
        #region -Initialize-
        UIManager.Instance.UI_GetGameObject("Btn_Cancel").GetComponent<Button>().onClick.AddListener(() => { Btn_Cancel(); });
        UIManager.Instance.UI_GetGameObject("Btn_Confirm").GetComponent<Button>().onClick.AddListener(() => { Btn_Confirm(); });
        #endregion
    }

    /// <summary>
    /// 拒绝按钮事件
    /// </summary>
    private void Btn_Cancel()
    {
        var title = UIManager.Instance.UI_GetGameObject("Title").GetComponent<TMP_Text>().text;
        if (title == "隐私协议") 
        {
            LoginPanel.AgreePrivacy = false;
        }
        else if (title == "用户协议")
        {
            LoginPanel.AgreeUser = false;
        }
        Debug.Log("拒绝按钮事件:销毁此面板");
        UIManager.Instance.Pop();
    }

    /// <summary>
    /// 同意按钮事件
    /// </summary>
    private void Btn_Confirm()
    {
        var title = UIManager.Instance.UI_GetGameObject("Title").GetComponent<TMP_Text>().text;
        if (title == "隐私协议")
        {
            LoginPanel.AgreePrivacy = true;
        }
        else if (title == "用户协议")
        {
            LoginPanel.AgreeUser = true;
        }
        Debug.Log("同意按钮事件:销毁此面板");
        UIManager.Instance.Pop();
    }
}
