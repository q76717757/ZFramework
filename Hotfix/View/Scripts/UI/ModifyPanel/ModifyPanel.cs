using UnityEngine;
using UnityEngine.UI;

public class ModifyPanel : BasePanel
{
    public static readonly string path = "UI/ModifyPanel/ModifyPanel";
    public ModifyPanel() : base(new UI_Info(path)) { }

    public override void OnEnter()
    {
        base.OnEnter();
        #region -Initialize-
        UIManager.Instance.UI_GetGameObject("ModifyBox").transform.Find("Btn_Close").GetComponent<Button>().onClick.AddListener(() => { Btn_Close(); });
        UIManager.Instance.UI_GetGameObject("ModifyBox").transform.Find("Btn_Cancel").GetComponent<Button>().onClick.AddListener(() => { Btn_Cancel(); });
        UIManager.Instance.UI_GetGameObject("ModifyBox").transform.Find("Btn_Save").GetComponent<Button>().onClick.AddListener(() => { Btn_Save(); });
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
    /// 保存按钮事件
    /// </summary>
    private void Btn_Save()
    {
        Debug.Log("修改后的昵称:" + UIManager.Instance.UI_GetGameObject("ModifyBox").transform.Find("InputField").Find("Text").GetComponent<Text>().text);
        Debug.Log("保存按钮事件:销毁此面板");
        UIManager.Instance.Pop();
    }
}
