using UnityEngine;
using UnityEngine.UI;

public class FriendPanel : BasePanel
{
    public static readonly string path = "UI/FriendPanel/FriendPanel";
    public FriendPanel() : base(new UI_Info(path)) { }

    public override void OnEnter()
    {
        base.OnEnter();
        #region -Initialize-
        UIManager.Instance.UI_GetGameObject("FriendBox").transform.Find("Btn_Close").GetComponent<Button>().onClick.AddListener(() => { Btn_Close(); });
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
