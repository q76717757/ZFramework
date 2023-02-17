using UnityEngine;
using UnityEngine.UI;

public class BillsPanel : BasePanel
{
    public static readonly string path = "UI/BillsPanel/BillsPanel";
    public BillsPanel() : base(new UI_Info(path)) { }

    public override void OnEnter()
    {
        base.OnEnter();
        #region -Initialize-
        UIManager.Instance.UI_GetGameObject("BillsBox").transform.Find("Btn_Close").GetComponent<Button>().onClick.AddListener(() => { Btn_Close(); });
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
