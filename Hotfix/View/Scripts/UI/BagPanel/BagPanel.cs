using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������
/// </summary>
public class BagPanel : BasePanel
{
    public static readonly string path = "UI/BagPanel/BagPanel";
    public BagPanel() : base(new UI_Info(path)) { }

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
        Debug.Log("�رհ�ť�¼�:������Ϸ���");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new GamePanel());
    }
}
