using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZFramework;

/// <summary>
/// 角色选择界面
/// </summary>
public class RoleSelectPanel : BasePanel
{
    public static readonly string path = "UI/RoleSelectPanel/RoleSelectPanel";
    public RoleSelectPanel(int localSelect = -1) : base(new UI_Info(path))
    {
        this.localSelect = localSelect;
    }
    int localSelect;

    public override void OnEnter()
    {
        base.OnEnter();
        //淡入动画
        UserInterface.Instance.Fade(this);
        #region -Initialize-
        SetModelDisplayer();
        UIManager.Instance.UI_GetGameObject("Btn_Back").GetComponent<Button>().onClick.AddListener(() => { Btn_Back(); });
        UIManager.Instance.UI_GetGameObject("Btn_Quit").GetComponent<Button>().onClick.AddListener(() => { Btn_Quit(); });
        UIManager.Instance.UI_GetGameObject("Btn_Start").GetComponent<Button>().onClick.AddListener(() => { Btn_Start(); });
        UIManager.Instance.UI_GetGameObject("T_Role_1").transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { RoleSelect(1); });
        UIManager.Instance.UI_GetGameObject("T_Role_2").transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { RoleSelect(2); });
        UIManager.Instance.UI_GetGameObject("T_Role_3").transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { RoleSelect(3); });
        #endregion
    }

    /// <summary>
    /// 初始化模型展示容器
    /// </summary>
    private void SetModelDisplayer()
    {
        #region -初始化模型展示容器-
        var Displayer_1 = GameObject.Instantiate(Resources.Load("UI/Misc/Model Displayer"), UIManager.Instance.DicUI[this.Info].transform) as GameObject;
        Displayer_1.name = "Model Displayer_1";
        Displayer_1.transform.position = new Vector3(-(Screen.width / 2), -(Screen.height / 2) - 100, 0);
        var Displayer_2 = GameObject.Instantiate(Resources.Load("UI/Misc/Model Displayer"), UIManager.Instance.DicUI[this.Info].transform) as GameObject;
        Displayer_2.name = "Model Displayer_2";
        Displayer_2.transform.position = new Vector3(-(Screen.width / 2), -(Screen.height / 2) - 200, 0);
        var Displayer_3 = GameObject.Instantiate(Resources.Load("UI/Misc/Model Displayer"), UIManager.Instance.DicUI[this.Info].transform) as GameObject;
        Displayer_3.name = "Model Displayer_3";
        Displayer_3.transform.position = new Vector3(-(Screen.width / 2), -(Screen.height / 2) - 300, 0);
        #endregion
        #region -设置展示图像-
        RenderTexture RT_Role_1 = new RenderTexture(600, 950, 24);
        RenderTexture RT_Role_2 = new RenderTexture(600, 950, 24);
        RenderTexture RT_Role_3 = new RenderTexture(600, 950, 24);
        UIManager.Instance.UI_GetGameObject("Model Displayer_1").transform.Find("Display Camera").GetComponent<Camera>().targetTexture = RT_Role_1;
        UIManager.Instance.UI_GetGameObject("Model Displayer_2").transform.Find("Display Camera").GetComponent<Camera>().targetTexture = RT_Role_2;
        UIManager.Instance.UI_GetGameObject("Model Displayer_3").transform.Find("Display Camera").GetComponent<Camera>().targetTexture = RT_Role_3;
        UIManager.Instance.UI_GetGameObject("T_Role_1").transform.Find("Background").Find("RawImage").GetComponent<RawImage>().texture = RT_Role_1;
        UIManager.Instance.UI_GetGameObject("T_Role_2").transform.Find("Background").Find("RawImage").GetComponent<RawImage>().texture = RT_Role_2;
        UIManager.Instance.UI_GetGameObject("T_Role_3").transform.Find("Background").Find("RawImage").GetComponent<RawImage>().texture = RT_Role_3;
        #endregion
        #region -设置角色预览模型-

        var rc = Resources.Load<RuntimeAnimatorController>("Anim/New_Player_AC");
        for (int i = 0; i < 3; i++)
        {
            var displayModelConfig = GameManager.Instance.roleCaches[i];
            var Model = GameObject.Instantiate(displayModelConfig.GetModelPrefab(), UIManager.Instance.UI_GetGameObject($"Model Displayer_{i + 1}").transform.Find("Container"));
            Model.transform.localPosition = Vector3.down;
            var Animator = Model.GetComponent<Animator>();
            Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Anim/New_Player_AC");

        }
        #endregion
    }

    /// <summary>
    /// 返回按钮事件
    /// </summary>
    private void Btn_Back()
    {
        Debug.Log("返回按钮事件:弹出登录界面");

        TcpClientComponent.Instance.Send2ServerAsync(new C2S_账号登出()
        {
            accountID = GameManager.Instance.Account.id
        });

        //UserInterface.Instance.Fade(this, new LoginPanel(), false);
    }

    /// <summary>
    /// 退出按钮事件
    /// </summary>
    private void Btn_Quit()
    {
        Debug.Log("退出按钮事件:弹出提示面板");
        var _TipsPanel = UIManager.Instance.Push<TipsPanel>(new TipsPanel());
        _TipsPanel.isQuit = true;
        UIManager.Instance.UI_GetGameObject("Tips").GetComponent<TMP_Text>().text = "确定要退出吗?";
    }

    /// <summary>
    /// 开始按钮事件
    /// </summary>
    private void Btn_Start()
    {
        switch (localSelect)
        {
            case -1: //未选择造型时
                Debug.Log("开始按钮事件:弹出消息界面");
                UserInterface.Instance.ShowMessage(new MessagePanel(), "尚未选择造型");
                return;
            case 0://选择造型1时
            case 1://选择造型2时
            case 2://选择造型3时
                //Quaternion temp = Quaternion.Euler(0, -90, 0);
                //PlayerInfo playerInfo = new PlayerInfo()
                //{
                //    modelConfig = GameManager.Instance.Account.modelConfigs[localSelect],
                //    phoneNum = GameManager.Instance.Account.phoneNum,
                //    sceneName = MapNames.主场景,
                //    x = 0, y = 0, z = 2.5f,
                //    a= temp.x,b = temp.y,c = temp.z,d = temp.x
                //};

                TcpClientComponent.Instance.Send2ServerAsync(new C2S_角色开始登入()
                {
                    roleID = GameManager.Instance.Account.roles[localSelect]
                });

                //var _LoadingPanel = new LoadingPanel();
                //_LoadingPanel.MapName = "MainSquare";//Enter->SceneName
                //_LoadingPanel.Advertisement = null;
                //_LoadingPanel.PlayerRotation = new Vector3(0, -90, 0);
                //Debug.Log("开始按钮事件:弹出加载界面");
                //UserInterface.Instance.Fade(this, _LoadingPanel, false);
                //Log.Info("进入主场景---->");
                break;
        }
    }

    /// <summary>
    /// 角色选择功能
    /// </summary>
    /// <param name="number">角色序号</param>
    private void RoleSelect(int number)//输入-> 1 2 3
    {
        if (localSelect == number - 1)
        {
            Debug.Log("角色选择按钮事件:弹出角色编辑面板");
            var editorConfig = GameManager.Instance.roleCaches[localSelect].Clone();

            UIManager.Instance.Pop();
            UIManager.Instance.Push(new StylePanel(editorConfig));
        }
        else
        {
            UIManager.Instance.UI_GetGameObject("T_Role_1").GetComponent<Toggle>().isOn = number == 1;
            UIManager.Instance.UI_GetGameObject("T_Role_2").GetComponent<Toggle>().isOn = number == 2;
            UIManager.Instance.UI_GetGameObject("T_Role_3").GetComponent<Toggle>().isOn = number == 3;

            localSelect = number - 1;//localIndex取012  输入是1 2 3
            Debug.Log("角色选择按钮事件:弹出消息面板");
            UserInterface.Instance.ShowMessage(new MessagePanel(), "再次点击以进入角色编辑界面");
        }
    }
}
