using UnityEngine;
using UnityEngine.UI;

public class ModifyPanel : BasePanel
{
    public static readonly string path = "UI/ModifyPanel/ModifyPanel";
    public ModifyPanel() : base(new UI_Info(path)) { }

    public override void OnEnter()
    {
        base.OnEnter();
        #region -Initialize-
        UIManager.Instance.UI_GetGameObject("ModifyBox").transform.Find("Btn_Close").GetComponent<Button>().onClick.AddListener(() => { Btn_Close(); });
        UIManager.Instance.UI_GetGameObject("ModifyBox").transform.Find("Btn_Cancel").GetComponent<Button>().onClick.AddListener(() => { Btn_Cancel(); });
        UIManager.Instance.UI_GetGameObject("ModifyBox").transform.Find("Btn_Save").GetComponent<Button>().onClick.AddListener(() => { Btn_Save(); });
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
    /// ���水ť�¼�
    /// </summary>
    private void Btn_Save()
    {
        Debug.Log("�޸ĺ���ǳ�:" + UIManager.Instance.UI_GetGameObject("ModifyBox").transform.Find("InputField").Find("Text").GetComponent<Text>().text);
        Debug.Log("���水ť�¼�:���ٴ����");
        UIManager.Instance.Pop();
    }
}
