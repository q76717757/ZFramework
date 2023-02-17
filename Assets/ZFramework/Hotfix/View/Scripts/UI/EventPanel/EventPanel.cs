using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 活动面板
/// </summary>
public class EventPanel : BasePanel
{
    public static readonly string path = "UI/EventPanel/EventPanel";
    public EventPanel() : base(new UI_Info(path)) { }

    private string TagName = "T_Tag_1";

    public List<YCGS_EVENT> ycgs_events = new List<YCGS_EVENT>();
    public List<YCGS_TASK> ycgs_tasks = new List<YCGS_TASK>();
    public List<YCGS_ZONE> ycgs_zones = new List<YCGS_ZONE>();
    public List<YCGS_ZONETASK> ycgs_zonetasks = new List<YCGS_ZONETASK>();

    public override void OnEnter()
    {
        base.OnEnter();
        #region -Initialize-
        Test();
        SetContentBox();
        TagsSelect();
        UIManager.Instance.UI_GetGameObject("EventBox").transform.Find("Btn_Close").GetComponent<Button>().onClick.AddListener(() => { Btn_Close(); });
        #endregion
    }

    /// <summary>
    /// 便签选择功能
    /// </summary>
    private void TagsSelect()
    {
        var _Tags = UIManager.Instance.UI_GetGameObject("EventBox").transform.Find("Tags").gameObject;
        List<Toggle> list = new List<Toggle>(_Tags.transform.GetComponentsInChildren<Toggle>());
        foreach (Toggle toggle in list)
        {
            toggle.onValueChanged.AddListener(delegate (bool isOn)
            {
                if (isOn)
                {
                    //获取当前标签页信息
                    TagName = toggle.name;
                    SetContentBox();
                }
            });
        }
    }

    /// <summary>
    /// 设置内容盒
    /// </summary>
    private void SetContentBox()
    {
        var _ContentBox_1 = UIManager.Instance.UI_GetGameObject("EventBox").transform.Find("ContentBox_1").gameObject;
        var _ContentBox_2 = UIManager.Instance.UI_GetGameObject("EventBox").transform.Find("ContentBox_2").gameObject;
        var _ContentBox_3 = UIManager.Instance.UI_GetGameObject("EventBox").transform.Find("ContentBox_3").gameObject;
        switch (TagName)
        {
            default:
                return;
            case "T_Tag_1":
                _ContentBox_1.SetActive(true);
                _ContentBox_2.SetActive(false);
                _ContentBox_3.SetActive(false);
                SetContent_1();
                break;
            case "T_Tag_2":
                _ContentBox_1.SetActive(false);
                _ContentBox_2.SetActive(true);
                _ContentBox_3.SetActive(false);
                SetContent_2();
                break;
            case "T_Tag_3":
                _ContentBox_1.SetActive(false);
                _ContentBox_2.SetActive(false);
                _ContentBox_3.SetActive(true);
                SetContent_3();
                break;
        }
    }

    #region 设置内容盒内容
    private void SetContent_1()
    {
        var Content = UIManager.Instance.DicUI[this.Info].transform.Find("EventBox/ContentBox_1/Scroll View/Viewport/Content");
        var SubContent = UIManager.Instance.DicUI[this.Info].transform.Find("EventBox/ContentBox_1/SubContent");
        var items = new List<GameObject>();
        for (int i = 0; i < Content.childCount; i++)
        {
            GameObject.Destroy(Content.GetChild(i).gameObject);
        }
        foreach (YCGS_EVENT _EVENT in ycgs_events)
        {
            var item = GameObject.Instantiate(Resources.Load("UI/EventPanel/T_Event"), Content) as GameObject;
            item.name = _EVENT._Title;
            item.GetComponent<Toggle>().group = Content.GetComponent<ToggleGroup>();
            item.transform.Find("Title").GetComponent<Text>().text = _EVENT._Title; 
            item.transform.Find("SubTitle").GetComponent<Text>().text = _EVENT._SubTitle;
            items.Add(item);
        }
        foreach (GameObject item in items)
        {
            item.GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn)
            {
                if (isOn)
                {
                    item.transform.Find("Title").GetComponent<Text>().color = new Vector4(0.007843138f, 0.2196079f, 0.4588236f, 1.0f);
                    item.transform.Find("SubTitle").GetComponent<Text>().color = new Vector4(0.007843138f, 0.2196079f, 0.4588236f, 1.0f);
                    foreach (YCGS_EVENT _EVENT in ycgs_events)
                    {
                        if(_EVENT._Title == item.name)
                        {
                            SubContent.GetComponent<Image>().sprite = _EVENT._Sprite;
                        }
                    }
                }
                else
                {
                    item.transform.Find("Title").GetComponent<Text>().color = new Vector4(0.75f, 0.75f, 0.75f, 1.0f);
                    item.transform.Find("SubTitle").GetComponent<Text>().color = new Vector4(0.75f, 0.75f, 0.75f, 1.0f);
                }
            });
        }
    }

    private void SetContent_2()
    {
        var Content = UIManager.Instance.DicUI[this.Info].transform.Find("EventBox/ContentBox_2/Scroll View/Viewport/Content");
        int ID = 0;
        for (int i = 0; i < Content.childCount; i++)
        {
            GameObject.Destroy(Content.GetChild(i).gameObject);
        }
        foreach(YCGS_TASK _TASK in ycgs_tasks)
        {
            if (!_TASK._isDone)
            {
                var task = GameObject.Instantiate(Resources.Load("UI/EventPanel/TaskBar"), Content) as GameObject;
                task.name = _TASK._Title;
                ID++;
                task.transform.Find("ID/Text").GetComponent<Text>().text = ID.ToString();
                task.transform.Find("Content").GetComponent<Text>().text = _TASK._Title;
                task.transform.Find("Progress").GetComponent<Text>().text = _TASK._Count + "/" + _TASK._MaxCount;
                task.transform.Find("RewardBox/Mask/Reward").GetComponent<Image>().sprite = _TASK._Sprite;
                task.transform.Find("Btn_Receive").GetComponent<Button>().onClick.AddListener(() => 
                { 
                    //在此处增加Btn_Receive的事件
                });
            }
        }
    }

    private void SetContent_3()
    {
        var Content = UIManager.Instance.DicUI[this.Info].transform.Find("EventBox/ContentBox_3/Zones/Viewport/Content");
        var M_Content = UIManager.Instance.DicUI[this.Info].transform.Find("EventBox/ContentBox_3/Merchants/Viewport/Content");
        var SubContent = UIManager.Instance.DicUI[this.Info].transform.Find("EventBox/ContentBox_3/DetailsBox");
        SubContent.gameObject.SetActive(false);
        YCGS_ZONE currentZone = null;
        for (int i = 0; i < Content.childCount; i++)
        {
            GameObject.Destroy(Content.GetChild(i).gameObject);
        }
        foreach (YCGS_ZONE _ZONE in ycgs_zones) 
        {
            var zone = GameObject.Instantiate(Resources.Load("UI/EventPanel/T_Zone"), Content) as GameObject;
            zone.name = _ZONE._Title;
            zone.transform.Find("Label").GetComponent<Text>().text = _ZONE._Title;
            zone.GetComponent<Toggle>().group = Content.GetComponent<ToggleGroup>();
            zone.GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn)
            {
                if (isOn)
                {
                    for (int i = 0; i < M_Content.childCount; i++)
                    {
                        GameObject.Destroy(M_Content.GetChild(i).gameObject);
                    }
                    SubContent.gameObject.SetActive(false);
                    currentZone = _ZONE;
                    foreach (YCGS_ZONETASK zonetask in ycgs_zonetasks) 
                    {
                        if(zonetask._ZONE == _ZONE)
                        {
                            var MerchantBox = GameObject.Instantiate(Resources.Load("UI/EventPanel/MerchantBox"), M_Content) as GameObject;
                            MerchantBox.name = zonetask._Title;
                            MerchantBox.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                //在此处添加点击事件
                                SubContent.gameObject.SetActive(true);
                                SubContent.Find("Title").GetComponent<Text>().text = zonetask._Title;
                                SubContent.Find("ContentBox_1/Viewport/Content/Text").GetComponent<Text>().text = zonetask._Target;
                            });
                        }
                    }
                }

            });
        }
        Content.GetChild(0).GetComponent<Toggle>().isOn = true;
    }
    #endregion

    /// <summary>
    /// 关闭按钮事件
    /// </summary>
    private void Btn_Close()
    {
        Debug.Log("关闭按钮事件:销毁此面板");
        UIManager.Instance.Pop();
    }

    public void Test()
    {
        ycgs_events.Add(new YCGS_EVENT("MetartLand", "元创共生", ""));
        ycgs_events.Add(new YCGS_EVENT("MetaRay", "拾光科技", ""));
        ycgs_tasks.Add(new YCGS_TASK("元创共生任务测试", "", 5));
        ycgs_tasks.Add(new YCGS_TASK("元创共生任务测试_2", "", 3));
        ycgs_tasks.Add(new YCGS_TASK("元创共生任务测试_3", "", 1));
        var zone1 = new YCGS_ZONE("数字藏品区");
        var zone2 = new YCGS_ZONE("商业区");
        var zone3 = new YCGS_ZONE("主城区");
        ycgs_zones.Add(zone1);
        ycgs_zones.Add(zone2);
        ycgs_zones.Add(zone3);
        ycgs_zonetasks.Add(new YCGS_ZONETASK(zone1, "MetaRay", "111.完成XXX任务", null));
        ycgs_zonetasks.Add(new YCGS_ZONETASK(zone1, "MetaRay1", "222.完成XXX任务", null));
        ycgs_zonetasks.Add(new YCGS_ZONETASK(zone2, "MetaRay2", "333.完成XXX任务", null));
    }

}

/// <summary>
/// 热门活动类
/// </summary>
public class YCGS_EVENT
{
    public string _Title { get; private set; }
    public string _SubTitle { get; private set; }
    public Sprite _Sprite { get; private set; }

    public YCGS_EVENT(string title, string subTitle, string spritePath)
    {
        _Title = title;
        _SubTitle = subTitle;
        _Sprite = Resources.Load(spritePath) as Sprite;
    }
}
/// <summary>
/// 日常任务类
/// </summary>
public class YCGS_TASK
{
    public string _Title { get; private set; }
    public Sprite _Sprite { get; private set; }
    public int _MaxCount { get; private set; }
    public int _Count;
    public bool _isDone;
    public YCGS_TASK(string title, string spritePath, int maxCount)
    {
        _Title = title;
        _Sprite = Resources.Load(spritePath) as Sprite;
        _MaxCount = maxCount;
    }
}
/// <summary>
/// 任务区域类
/// </summary>
public class YCGS_ZONE
{
    public string _Title { get; private set; } 
    public YCGS_ZONE (string title)
    {
        _Title = title;
    }
}
/// <summary>
/// 展商任务类
/// </summary>
public class YCGS_ZONETASK
{
    public YCGS_ZONE _ZONE { get; private set; }
    public string _Title { get; private set; }
    public string _Target { get; private set; }
    public string _Reward { get; private set; }

    public YCGS_ZONETASK (YCGS_ZONE zone,string title,string target,string reward)
    {
        _ZONE = zone;
        _Title = title;
        _Target = target;
        _Reward = reward;
    }
}
