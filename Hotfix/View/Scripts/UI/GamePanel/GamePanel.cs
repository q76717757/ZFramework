using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZFramework;

/// <summary>
/// 游戏面板
/// </summary>
public class GamePanel : BasePanel
{
    public static readonly string path = "UI/GamePanel/GamePanel";
    public GamePanel() : base(new UI_Info(path)) { }

    References refs;

    public override void OnEnter()
    {
        base.OnEnter();

        refs = UIManager.Instance.GetCurrentUI().GetComponent<References>();
        JoyStickComponent.Instance.Build(refs);

        #region -Initialize-
        UIManager.Instance.UI_GetGameObject("PlayerBox").transform.Find("Btn_Personal Information").GetComponent<Button>().onClick.AddListener(() => { Btn_Personal(); });
        UIManager.Instance.UI_GetGameObject("PlayerBox").transform.Find("Btn_Camera").GetComponent<Button>().onClick.AddListener(() => { Btn_Camera(); });
        UIManager.Instance.UI_GetGameObject("FunctionBox").transform.Find("Btn_Event").GetComponent<Button>().onClick.AddListener(() => { Btn_Event(); });
        UIManager.Instance.UI_GetGameObject("FunctionBox").transform.Find("Btn_Friend").GetComponent<Button>().onClick.AddListener(() => { Btn_Friend(); });
        UIManager.Instance.UI_GetGameObject("FunctionBox").transform.Find("Btn_Team").GetComponent<Button>().onClick.AddListener(() => { Btn_Team(); });
        UIManager.Instance.UI_GetGameObject("FunctionBox").transform.Find("Btn_Bag").GetComponent<Button>().onClick.AddListener(() => { Btn_Bag(); });
        UIManager.Instance.UI_GetGameObject("FunctionBox").transform.Find("Btn_Option").GetComponent<Button>().onClick.AddListener(() => { Btn_Option(); });
        UIManager.Instance.UI_GetGameObject("MiniMapBox").GetComponent<Button>().onClick.AddListener(() => { MiniMapBox(); });
        UIManager.Instance.UI_GetGameObject("Btn_Interact").GetComponent<Button>().onClick.AddListener(() => { });

        UIManager.Instance.UI_GetGameObject("ChatBox").transform.Find("Btn_Emoji").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(); });

        ZEvent.UIEvent.AddListener(refs.Get<GameObject>("Btn_OpenChatBox"), OpenChatBox);
        ZEvent.UIEvent.AddListener(refs.Get<GameObject>("SendBtn"), SendBox);
        ZEvent.UIEvent.AddListener(refs.Get<GameObject>("Btn_Jump"), BtnJump);
        ZEvent.UIEvent.AddListener(refs.Get<GameObject>("ViewController"), ViewCall);
        ZEvent.UIEvent.AddListener(refs.Get<GameObject>("Btn_Hide"), HideChatBoxBtn);

        ///* JUMP */
        //var Trigger = UIManager.Instance.UI_GetGameObject("Btn_Jump").GetComponent<EventTrigger>();
        //EventTrigger.Entry entry_down = new EventTrigger.Entry();
        //entry_down.eventID = EventTriggerType.PointerDown;
        //entry_down.callback.AddListener((data) => { Btn_Jump_Down((PointerEventData)data); });
        //EventTrigger.Entry entry_up = new EventTrigger.Entry();
        //entry_up.eventID = EventTriggerType.PointerUp;
        //entry_up.callback.AddListener((data) => { Btn_Jump_Up((PointerEventData)data); });
        //Trigger.triggers.Add(entry_down);
        //Trigger.triggers.Add(entry_up);
        /* END */

        UIManager.Instance.UI_GetGameObject("Btn_Reset").GetComponent<Button>().onClick.AddListener(() => { Log.Info("重置视角?"); });
        #endregion


        refs.Get<Text>("NickName").text = GameManager.Instance.Account.phoneNum;

        var mapCamera = ScenePlayerHandleComponent.Instance.self.mapCamera;
        mapCamera.targetTexture = refs.Get<RenderTexture>("RT_MiniMap");

        ZEvent.CustomEvent.AddListener<S2C_聊天室>("聊天室", GetMsg);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        ZEvent.CustomEvent.RemoveListener<S2C_聊天室>("聊天室", GetMsg);
        Log.Info("Destory GamePlane");
    }

  
    void SendBox(UIEventData eventData)
    {
        if (eventData.EventType == UIEventType.Click)
        {
            var input = refs.Get<TMP_InputField>("InputField");
            var msg = input.text;
            if (msg.Length > 0)//空格也能发
            {
                SendMsg(msg);
                input.text = "";
            }
        }
    }
    void SendMsg(string msg)
    {
        Debug.Log("chat->" + msg);
        var send = new C2S_聊天室()
        {
            roleID = GameManager.Instance.Location.role.id,
            msg = msg,
        };
        TcpClientComponent.Instance.Send2ServerAsync(send);
    }
    void GetMsg(CustomEventData<S2C_聊天室> eventData)
    {
        var rolename = eventData.Data0.roleName;
        var msg = eventData.Data0.msg;
        Debug.Log(rolename + ":" + msg);
        CreateChatItem(rolename, msg);
    }
    void CreateChatItem(string roleName,string msg)
    {
        var msgBoxPre = Refs.Get<GameObject>("MessageBox");
        var msgBox = GameObject.Instantiate<GameObject>(msgBoxPre, Refs.Get<RectTransform>("Content"));
        var msgBoxRefs = msgBox.GetComponent<References>();
        msgBoxRefs.Get<Text>("Role").text = roleName;
        msgBoxRefs.Get<Text>("Msg").text = msg;
    }


    void OpenChatBox(UIEventData eventData)
    {
        if (eventData.EventType == UIEventType.Click)
        {
            var cl = refs.Get<GameObject>("ChatList");
            cl.SetActive(!cl.activeSelf);
        }
    }
    void HideChatBoxBtn(UIEventData eventData)
    {
        if (eventData.EventType == UIEventType.Click)
        {
            var cl = refs.Get<GameObject>("ChatList");
            cl.SetActive(false);
        }
    }
    void ViewCall(UIEventData eventData)
    {
        switch (eventData.EventType)
        {
            case UIEventType.Enter:
                break;
            case UIEventType.Exit:
                break;
            case UIEventType.Down:
                break;
            case UIEventType.Up:
                break;
            case UIEventType.Click:
                var r = Camera.main.ScreenPointToRay(eventData.Position);
                var all = Physics.RaycastAll(r, 100);
                foreach (var item in all)
                {
                    if (item.collider.name == "Cube")
                    {
                        item.collider.transform.rotation *= Quaternion.Euler(60, 60, 60);
                        return;
                    }
                }

                break;
            case UIEventType.Drag:
                ZEvent.CustomEvent.Call(CameraController.镜头, eventData.UnityEventData.delta);
                break;
            case UIEventType.Scroll:
                break;
            default:
                break;
        }
    }
    void BtnJump(UIEventData eventData)
    {
        switch (eventData.EventType)
        {
            case UIEventType.Enter:
                break;
            case UIEventType.Exit:
                break;
            case UIEventType.Down:
                ZEvent.CustomEvent.Call(Movement.跳跃);
                break;
            case UIEventType.Up:
                ZEvent.CustomEvent.Call(Movement.停止跳跃);
                break;
            case UIEventType.Click:
                break;
            case UIEventType.Drag:
                break;
            case UIEventType.Scroll:
                break;
            default:
                break;
        }
    }



    /// <summary>
    /// 个人信息按钮事件
    /// </summary>
    private void Btn_Personal()
    {
        Debug.Log("个人信息按钮事件:弹出个人信息面板");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new PersonalInformationPanel());
    }

    /// <summary>
    /// 摄像机按钮事件
    /// </summary>
    private void Btn_Camera()
    {
        Debug.Log("摄像机按钮事件:弹出摄像机面板");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new CameraPanel());
    }

    /// <summary>
    /// 表情按钮事件
    /// </summary>
    private void Btn_Emoji()
    {
        Debug.Log("表情按钮事件:弹出表情面板");
        UIManager.Instance.Push(new EmojiPanel());
    }

    /// <summary>
    /// 活动按钮事件
    /// </summary>
    private void Btn_Event()
    {
        Debug.Log("活动按钮事件:弹出活动面板");
        UIManager.Instance.Push(new EventPanel());
    }

    /// <summary>
    /// 好友按钮事件
    /// </summary>
    private void Btn_Friend()
    {
        Debug.Log("好友按钮事件:弹出好友面板");
        UIManager.Instance.Push(new FriendPanel());
    }

    /// <summary>
    /// 队伍按钮事件
    /// </summary>
    private void Btn_Team()
    {
        Debug.Log("队伍按钮事件:弹出队伍面板");
        UIManager.Instance.Push(new TeamPanel());
    }

    /// <summary>
    /// 背包按钮事件
    /// </summary>
    private void Btn_Bag()
    {
        Debug.Log("背包按钮事件:弹出背包面板");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new BagPanel());
    }

    /// <summary>
    /// 设置按钮事件
    /// </summary>
    private void Btn_Option()
    {
        Debug.Log("设置按钮事件:弹出设置面板");
        UIManager.Instance.Push(new OptionPanel());
    }

    /// <summary>
    /// 地图按钮事件
    /// </summary>
    private void MiniMapBox()
    {
        Debug.Log("地图按钮事件:弹出地图面板");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new MapPanel());


    }

    //private void Btn_Jump_Down(PointerEventData data)
    //{
    //    EventManager.Instance.EventTrigger("JumpState", true);
    //    EventManager.Instance.EventTrigger("AllowJump", false);
    //}
    //private void Btn_Jump_Up(PointerEventData data)
    //{
    //    EventManager.Instance.EventTrigger("JumpState", false);
    //    EventManager.Instance.EventTrigger("AllowJump", true);
    //}
}
