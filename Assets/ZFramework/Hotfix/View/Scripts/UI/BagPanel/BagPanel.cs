using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 背包面板
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
    /// 关闭按钮事件
    /// </summary>
    private void Btn_Close()
    {
        Debug.Log("关闭按钮事件:弹出游戏面板");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new GamePanel());
    }
}
