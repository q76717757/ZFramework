using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������Ϣ���
/// </summary>
public class PersonalInformationPanel : BasePanel
{
    public static readonly string path = "UI/PersonalInformationPanel/PersonalInformationPanel";
    public PersonalInformationPanel() : base(new UI_Info(path)) { }

    public override void OnEnter()
    {
        base.OnEnter();
        #region -Initialize-
        UIManager.Instance.UI_GetGameObject("Btn_Back").GetComponent<Button>().onClick.AddListener(() => { Btn_Back(); });
        UIManager.Instance.UI_GetGameObject("NameBox").transform.Find("Btn_Modify").GetComponent<Button>().onClick.AddListener(() => { Btn_Modify(); });
        #endregion
    }

    /// <summary>
    /// ���ذ�ť�¼�
    /// </summary>
    private void Btn_Back()
    {
        Debug.Log("���ذ�ť�¼�:������Ϸ���");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new GamePanel());
    }

    /// <summary>
    /// �޸İ�ť�¼�
    /// </summary>
    private void Btn_Modify()
    {
        Debug.Log("�޸İ�ť�¼�:�����޸����");
        UIManager.Instance.Push(new ModifyPanel());
    }
}
