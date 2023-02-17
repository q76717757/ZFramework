using System.Collections.Generic;
using UnityEngine;
using ZFramework;
using Component = UnityEngine.Component;

/// <summary>
/// UI管理器
/// </summary>
public class UIManager : Singleton<UIManager>
{
    /// <summary>
    /// UI库
    /// Key:UI信息
    /// Value:UI实体
    /// </summary>
    public Dictionary<UI_Info, GameObject> DicUI { get; private set; }
    /// <summary>
    /// UI栈
    /// Parameter:基础面板
    /// </summary>
    private Stack<BasePanel> Panels;
    /// <summary>
    /// 当前激活的UI
    /// </summary>
    private GameObject CurrentUI = null;

    /// <summary>
    /// 构造函数
    /// </summary>
    public UIManager()
    {
        DicUI = new Dictionary<UI_Info, GameObject>();
        Panels = new Stack<BasePanel>();
    }

    #region UI库 相关方法
    /// <summary>
    /// 获取UI实体
    /// </summary>
    /// <param name="info">UI信息</param>
    /// <returns>UI实体</returns>
    public GameObject GetUI(UI_Info info)
    {
        //获取UI基底,若不存在则报错并返回空值

        Transform uiRoot = ClientProcess.userInterface.transform;

        //若UI库中含有此UI则返回该UI实体
        if (DicUI.ContainsKey(info))
            return DicUI[info];
        //若无则实例化该UI并添加至UI库
        GameObject UI = GameObject.Instantiate(Resources.Load<GameObject>(info.Path), uiRoot);
        UI.name = info.Name;
        DicUI.Add(info, UI);
        return UI;
    }

    /// <summary>
    /// 销毁UI实体
    /// </summary>
    /// <param name="info">UI信息</param>
    public void DestoryUI(UI_Info info)
    {
        //若UI库中含有此实体,则摧毁并移出UI库
        if (DicUI.ContainsKey(info))
        {
            GameObject.Destroy(DicUI[info]);            
            DicUI.Remove(info);
        }
        else Debug.LogError("This UI doesn't exist!");
    }
    #endregion

    #region UI栈 相关方法
    /// <summary>
    /// UI入栈方法
    /// UI入栈后将显示此UI
    /// </summary>
    /// <param name="panel">UI面板</param>
    public void Push(BasePanel panel, bool Pause = true)
    {
        if (Panels.Count > 0 && Pause)  
        {
            //获取栈顶UI,并暂停使用该UI
            Panels.Peek().OnPause();
        }
        //UI入栈,并触发该UI激活时的方法函数
        Panels.Push(panel);
        CurrentUI = GetUI(panel.Info);           
        panel.OnEnter();
    }

    /// <summary>
    /// UI入栈方法
    /// UI入栈后将显示此UI
    /// </summary>
    /// <typeparam name="T">UI面板类型</typeparam>
    /// <param name="panel">UI面板</param>
    /// <returns>当前UI面板类型</returns>
    public T Push<T>(BasePanel panel, bool Pause = true) where T : BasePanel
    {
        if (Panels.Count > 0 && Pause)
        {
            //获取栈顶UI,并暂停使用该UI
            Panels.Peek().OnPause();
        }
        //UI入栈,并触发该UI激活时的方法函数
        Panels.Push(panel);
        CurrentUI = GetUI(panel.Info);
        panel.OnEnter();
        return panel as T;
    }

    /// <summary>
    /// UI出栈方法
    /// UI出栈后将关闭此UI
    /// </summary>
    /// <param name="Resume">恢复指示</param>
    public void Pop()
    {
        //将UI栈的栈顶UI实体摧毁,并移出栈
        if (Panels.Count > 0)
        {
            Panels.Peek().OnDestroy();
            Panels.Pop();
        }
        //再次激活更新后栈顶的UI实体
        if (Panels.Count > 0)
        {
            CurrentUI = GetUI(Panels.Peek().Info);
            Panels.Peek().OnResume();            
        }        
    }
    #endregion

    #region UI工具 相关方法
    /// <summary>
    /// 设置当前激活的UI对象
    /// </summary>
    /// <param name="UI">UI对象</param>
    public void SetCurrentUI(GameObject UI)
    {
        CurrentUI = UI;
    }
    public GameObject GetCurrentUI()
    { 
        return CurrentUI;
    }

    /// <summary>
    /// 从当前激活的UI对象中获取(添加)组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>组件</returns>
    public T UI_GetComponent<T>() where T : Component
    {
        //若未正确获取到当前UI,报错并返回空值
        if (CurrentUI == null)
        {
            Debug.LogError("The current UI isn't set!");
            return null;
        }
        //若不存在此组件,则为当前UI附加此组件
        if(CurrentUI.GetComponent<T>() == null)
            CurrentUI.AddComponent<T>();
        //返回此组件
        return CurrentUI.GetComponent<T>();            
    }

    /// <summary>
    /// 查找当前UI对象中的子物体
    /// </summary>
    /// <param name="name">子物体名称</param>
    /// <returns>子物体</returns>
    public GameObject UI_GetGameObject(string name)
    {
        //以数组存储当前UI对象中所有的子物体
        Transform[] transforms = CurrentUI.GetComponentsInChildren<Transform>();
        //遍历子物体,查找是否有对应名称的子物体
        foreach (var item in transforms)
        {
            if (item.name == name)
                return item.gameObject;
        }
        //若无,则报错并返回空值
        Debug.LogWarning($"Cannot find {name} in {CurrentUI.name}!");
        return null;
    }
    #endregion
}
