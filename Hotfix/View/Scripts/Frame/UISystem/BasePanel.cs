using UnityEngine;
using ZFramework;

/// <summary>
/// UI父类
/// </summary>
public class BasePanel
{
    /// <summary>
    /// UI信息
    /// </summary>
    public UI_Info Info { get; private set; }

    public BasePanel(UI_Info info)
    {
        Info = info;
    }

    public References Refs => UIManager.Instance.GetUI(this.Info).GetComponent<References>();


    /// <summary>
    /// 创建UI时激活的方法函数
    /// </summary>
    public virtual void OnEnter()
    {
        UIManager.Instance.SetCurrentUI(UIManager.Instance.DicUI[Info]);
    }

    /// <summary>
    /// 当UI被暂停使用时激活的方法函数
    /// </summary>
    public virtual void OnPause()
    {
        UIManager.Instance.UI_GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    /// <summary>
    /// 当UI被再次启用时激活的方法函数
    /// </summary>
    public virtual void OnResume()
    {
        UIManager.Instance.UI_GetComponent<CanvasGroup>().blocksRaycasts = true;
        
    }

    /// <summary>
    /// 当UI被销毁时激活的方法函数
    /// </summary>
    public virtual void OnDestroy()
    {
        UIManager.Instance.DestoryUI(Info);
    }
}
