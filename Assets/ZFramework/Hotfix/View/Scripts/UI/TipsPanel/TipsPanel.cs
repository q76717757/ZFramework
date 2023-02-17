using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ʾ���
/// </summary>
public class TipsPanel : BasePanel
{
    public static readonly string path = "UI/TipsPanel/TipsPanel";
    public TipsPanel() : base(new UI_Info(path)) { }
    public bool isQuit = false;

    public override void OnEnter()
    {
        base.OnEnter();
        #region -Initialize-
        UIManager.Instance.UI_GetGameObject("Btn_Close").GetComponent<Button>().onClick.AddListener(() => { Btn_Close(); });
        UIManager.Instance.UI_GetGameObject("Btn_Cancel").GetComponent<Button>().onClick.AddListener(() => { Btn_Cancel(); });
        UIManager.Instance.UI_GetGameObject("Btn_Confirm").GetComponent<Button>().onClick.AddListener(() => { Btn_Confirm(); });
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
    /// ȡ����ť�¼�
    /// </summary>
    private void Btn_Cancel()
    {
        Debug.Log("ȡ����ť�¼�:���ٴ����");
        UIManager.Instance.Pop();
    }

    /// <summary>
    /// ȷ�ϰ�ť�¼�
    /// </summary>
    private void Btn_Confirm()
    {
        if (isQuit)
        {
            Debug.Log("��Ϸ�Ѿ��˳�!");
            Application.Quit();
        }
        else
        {
            Debug.Log("ȷ�ϰ�ť�¼�:���ٴ����");
            UIManager.Instance.Pop();
        }
    }
}
