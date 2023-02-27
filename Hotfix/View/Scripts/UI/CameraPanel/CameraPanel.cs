using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��������
/// </summary>
public class CameraPanel : BasePanel
{
    public static readonly string path = "UI/CameraPanel/CameraPanel";
    public CameraPanel() : base(new UI_Info(path)) { }

    public override void OnEnter()
    {
        base.OnEnter();
        UIManager.Instance.UI_GetGameObject("Btn_Back").GetComponent<Button>().onClick.AddListener(() => { Btn_Back(); });
        UIManager.Instance.UI_GetGameObject("Btn_Shutter").GetComponent<Button>().onClick.AddListener(() => { Btn_Shutter(); });
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
    /// ���Ű�ť�¼�
    /// </summary>
    private void Btn_Shutter()
    {
        Debug.Log("���Ű�ť�¼�:������Ƭ");
        Debug.Log("���Ű�ť�¼�:������ƬԤ�����");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new PhotoPanel());
    }
}