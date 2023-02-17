using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ƬԤ�����
/// </summary>
public class PhotoPanel : BasePanel
{
    public static readonly string path = "UI/PhotoPanel/PhotoPanel";
    public PhotoPanel() : base(new UI_Info(path)) { }

    public override void OnEnter()
    {
        base.OnEnter();
        UIManager.Instance.UI_GetGameObject("Btn_Back").GetComponent<Button>().onClick.AddListener(() => { Btn_Back(); });
        UIManager.Instance.UI_GetGameObject("Btn_Save").GetComponent<Button>().onClick.AddListener(() => { Btn_Save(); });
    }

    /// <summary>
    /// ���ذ�ť�¼�
    /// </summary>
    private void Btn_Back()
    {
        Debug.Log("���ذ�ť�¼�:������������");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new CameraPanel());
    }

    /// <summary>
    /// ���水ť�¼�
    /// </summary>
    private void Btn_Save()
    {
        Debug.Log("���水ť�¼�:�������Ƭ");
        Debug.Log("���水ť�¼�:������Ϸ���");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new GamePanel());
    }
}
