using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Э�����
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
    /// �ܾ���ť�¼�
    /// </summary>
    private void Btn_Cancel()
    {
        var title = UIManager.Instance.UI_GetGameObject("Title").GetComponent<TMP_Text>().text;
        if (title == "��˽Э��") 
        {
            LoginPanel.AgreePrivacy = false;
        }
        else if (title == "�û�Э��")
        {
            LoginPanel.AgreeUser = false;
        }
        Debug.Log("�ܾ���ť�¼�:���ٴ����");
        UIManager.Instance.Pop();
    }

    /// <summary>
    /// ͬ�ⰴť�¼�
    /// </summary>
    private void Btn_Confirm()
    {
        var title = UIManager.Instance.UI_GetGameObject("Title").GetComponent<TMP_Text>().text;
        if (title == "��˽Э��")
        {
            LoginPanel.AgreePrivacy = true;
        }
        else if (title == "�û�Э��")
        {
            LoginPanel.AgreeUser = true;
        }
        Debug.Log("ͬ�ⰴť�¼�:���ٴ����");
        UIManager.Instance.Pop();
    }
}
