using UnityEngine;
using UnityEngine.UI;

public class UrlTipsPanel : BasePanel
{
    public static readonly string path = "UI/UrlTipsPanel/UrlTipsPanel";
    public UrlTipsPanel() : base(new UI_Info(path)) { }
    /// <summary>
    /// 外部链接
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
    /// 关闭按钮事件
    /// </summary>
    private void Btn_Close()
    {
        Debug.Log("关闭按钮事件:销毁此面板");
        UIManager.Instance.Pop();
    }
}
