using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 个人信息面板
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
    /// 返回按钮事件
    /// </summary>
    private void Btn_Back()
    {
        Debug.Log("返回按钮事件:弹出游戏面板");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new GamePanel());
    }

    /// <summary>
    /// 修改按钮事件
    /// </summary>
    private void Btn_Modify()
    {
        Debug.Log("修改按钮事件:弹出修改面板");
        UIManager.Instance.Push(new ModifyPanel());
    }
}
