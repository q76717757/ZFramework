using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 摄像机面板
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
    /// 返回按钮事件
    /// </summary>
    private void Btn_Back()
    {
        Debug.Log("返回按钮事件:弹出游戏面板");
        UIManager.Instance.Pop(); 
        UIManager.Instance.Push(new GamePanel());
    }

    /// <summary>
    /// 快门按钮事件
    /// </summary>
    private void Btn_Shutter()
    {
        Debug.Log("快门按钮事件:拍摄照片");
        Debug.Log("快门按钮事件:弹出相片预览面板");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new PhotoPanel());
    }
}