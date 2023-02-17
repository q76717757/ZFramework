using UnityEngine;
using UnityEngine.UI;

public class UrlTipsPanel : BasePanel
{
    public static readonly string path = "UI/UrlTipsPanel/UrlTipsPanel";
    public UrlTipsPanel() : base(new UI_Info(path)) { }
    /// <summary>
    /// �ⲿ����
    /// </summary>
    public string url = null;

    public override void OnEnter()
    {
        base.OnEnter();
        #region -Initialize-
        UIManager.Instance.UI_GetGameObject("Btn_Close").GetComponent<Button>().onClick.AddListener(() => { Btn_Close(); });
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
}
