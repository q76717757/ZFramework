using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 提示面板
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
    /// 关闭按钮事件
    /// </summary>
    private void Btn_Close()
    {
        Debug.Log("关闭按钮事件:销毁此面板");
        UIManager.Instance.Pop();
    }

    /// <summary>
    /// 取消按钮事件
    /// </summary>
    private void Btn_Cancel()
    {
        Debug.Log("取消按钮事件:销毁此面板");
        UIManager.Instance.Pop();
    }

    /// <summary>
    /// 确认按钮事件
    /// </summary>
    private void Btn_Confirm()
    {
        if (isQuit)
        {
            Debug.Log("游戏已经退出!");
            Application.Quit();
        }
        else
        {
            Debug.Log("确认按钮事件:销毁此面板");
            UIManager.Instance.Pop();
        }
    }
}
