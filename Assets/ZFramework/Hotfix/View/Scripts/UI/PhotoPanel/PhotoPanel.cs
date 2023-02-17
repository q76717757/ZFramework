using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 相片预览面板
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
    /// 返回按钮事件
    /// </summary>
    private void Btn_Back()
    {
        Debug.Log("返回按钮事件:弹出摄像机面板");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new CameraPanel());
    }

    /// <summary>
    /// 保存按钮事件
    /// </summary>
    private void Btn_Save()
    {
        Debug.Log("保存按钮事件:保存此照片");
        Debug.Log("保存按钮事件:弹出游戏面板");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new GamePanel());
    }
}
