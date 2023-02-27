using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZFramework;

/// <summary>
/// 角色编辑面板
/// </summary>
public class StylePanel : BasePanel
{
    public static readonly string path = "UI/StylePanel/StylePanel";
    public StylePanel(Role editorConfig) : base(new UI_Info(path))
    {
        this.editConfigCache = editorConfig;
    }
    /// <summary>
    /// 性别
    /// </summary>
    //private string Gender = "Male";
    /// <summary>
    /// 细节面板标题
    /// </summary>
    private string Type = "Hair Style";
    /// <summary>
    /// 当前选取道具的名字
    /// </summary>
    private string ItemName = null;
    /// <summary>
    /// 当前选取道具的颜色
    /// </summary>
    private Color ItemColor = default;
    /// <summary>
    /// 装饰列表
    /// </summary>
    private List<Decoration> Decorations = new List<Decoration>();

    Role editConfigCache;
    GameObject displayModelCache;

    public override void OnEnter()
    {
        base.OnEnter();
        #region -Initialize-
        SetModelDisplayer();
        GenderBox();
        TypeBox();
        SetDecirationList();
        SetDetailsBox();
        UIManager.Instance.UI_GetGameObject("Btn_Back").GetComponent<Button>().onClick.AddListener(() => { Btn_Back(); });
        UIManager.Instance.UI_GetGameObject("Btn_Save").GetComponent<Button>().onClick.AddListener(() => { Btn_Save(); });
        UIManager.Instance.UI_GetGameObject("Title").GetComponent<TMP_Text>().text = Type;
        #endregion
    }

    /// <summary>
    /// 初始化模型展示容器
    /// </summary>
    private void SetModelDisplayer()
    {
        //初始化模型展示容器
        var Displayer = GameObject.Instantiate(Resources.Load("UI/Misc/Model Displayer"), UIManager.Instance.DicUI[this.Info].transform) as GameObject;
        Displayer.name = "Model Displayer";
        Displayer.transform.position = new Vector3(-(Screen.width / 2), -(Screen.height / 2) - 100, 0);
        RenderTexture RT_Role = new RenderTexture(600, 950, 24);
        UIManager.Instance.UI_GetGameObject("Display Camera").GetComponent<Camera>().targetTexture = RT_Role;
        UIManager.Instance.UI_GetGameObject("Display").GetComponent<RawImage>().texture = RT_Role;

        //拖动事件
        var targetDisplayGO = Displayer.transform.Find("Container");//传的是容器 转的也是容器
        UIManager.Instance.UI_GetGameObject("Display").transform.Find("DragZone").gameObject.AddComponent<ModelDisplay>().Init(targetDisplayGO);

        CreateModel();
    }

    void CreateModel()
    {
        if (displayModelCache != null)
        {
            GameObject.Destroy(displayModelCache);
        }
        var Container = UIManager.Instance.UI_GetGameObject("Model Displayer").transform.Find("Container");
        displayModelCache = GameObject.Instantiate(editConfigCache.GetModelPrefab(), Container);
        displayModelCache.transform.localPosition = Vector3.down;

        var Animator = displayModelCache.GetComponent<Animator>();
        Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Anim/New_Player_AC");
    }

    /// <summary>
    /// 返回按钮事件
    /// </summary>
    private void Btn_Back()
    {
        Debug.Log("返回按钮事件:弹出角色选择面板");
        UIManager.Instance.Pop();
        UIManager.Instance.Push(new RoleSelectPanel());
    }

    /// <summary>
    /// 保存按钮事件
    /// </summary>
    private void Btn_Save()
    {
        var sendConfig = new C2S_保存模型()
        {
            roleConfig = editConfigCache
        };
        TcpClientComponent.Instance.Send2ServerAsync(sendConfig);

        //选完角色
        //GameManager.Instance.local[editConfigCache.index] = editConfigCache;
        //Debug.Log("角色已保存");
        //Debug.Log("保存按钮事件:弹出角色选择面板");
        //UIManager.Instance.Pop(); 
        //UIManager.Instance.Push(new RoleSelectPanel());
    }

    /// <summary>
    /// 性别选择功能
    /// </summary>
    private void GenderBox()
    {
        var _GenderBox = UIManager.Instance.UI_GetGameObject("GenderBox");
        List<Toggle> list = new List<Toggle>(_GenderBox.transform.GetComponentsInChildren<Toggle>());

        for (int i = 0; i < list.Count; i++)
        {
            list[i].SetIsOnWithoutNotify(i == editConfigCache.gender);

            int type = i;//0男 1女
            list[i].onValueChanged.AddListener((isOn)=>
            {
                if (isOn)
                {
                    if (editConfigCache.gender != type)
                    {
                        editConfigCache.gender = type;
                        CreateModel();

                        SetDetailsBox();
                    }
                }
            });
        }
    }

    /// <summary>
    /// 服饰选择功能
    /// </summary>
    private void TypeBox()
    {
        var _TypeBox = UIManager.Instance.UI_GetGameObject("TypeBox");
        List<Toggle> list = new List<Toggle>(_TypeBox.transform.GetComponentsInChildren<Toggle>());
        foreach (Toggle toggle in list)
        {
            toggle.onValueChanged.AddListener(delegate (bool isOn)
            {
                if (isOn)
                {
                    //获取当前细节面板信息
                    Type = toggle.name.Substring(toggle.name.LastIndexOf('_') + 1);
                    UIManager.Instance.UI_GetGameObject("Title").GetComponent<TMP_Text>().text = Type;
                    SetDetailsBox();
                }
            });
        }
    }

    /// <summary>
    /// 更新层级面板内容
    /// </summary>
    private void SetDetailsBox()
    {
        var CurrentItems = new List<GameObject>();
        var Content = UIManager.Instance.DicUI[this.Info].transform.Find("DetailsBox/Scroll View/Viewport/Content");
        var C_Content = UIManager.Instance.DicUI[this.Info].transform.Find("ColorPanel/Scroll View/Viewport/Content");
        for (int i = 0; i < Content.childCount; i++)
        {
            GameObject.Destroy(Content.GetChild(i).gameObject);
        }
        for (int i = 0; i < C_Content.childCount; i++)
        {
            GameObject.Destroy(C_Content.GetChild(i).gameObject);
        }
        foreach (Decoration dec in Decorations)
        {
            if (dec._Gender == editConfigCache.gender && dec._Type == Type)
            {
                var item = GameObject.Instantiate(Resources.Load("UI/StylePanel/T_Item"), Content) as GameObject;
                item.name = dec._Name;
                item.GetComponent<Toggle>().group = Content.GetComponent<ToggleGroup>();
                CurrentItems.Add(item);
            }
        }
        foreach(GameObject item in CurrentItems)
        {
            item.GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn)
            {
                for (int i = 0; i < C_Content.childCount; i++)
                {
                    GameObject.Destroy(C_Content.GetChild(i).gameObject);
                }
                if (isOn)
                {
                    ItemName = item.name;
                    Debug.Log("当前选取的道具是:" + ItemName);
                    foreach(Decoration dec in Decorations)
                    {
                        if(dec._Name == ItemName)
                        {
                            foreach(Color color in dec._Colors)
                            {
                                var _color = GameObject.Instantiate(Resources.Load("UI/StylePanel/T_Color"), C_Content) as GameObject;
                                _color.transform.Find("Background").GetComponent<Image>().color = color;
                                _color.GetComponent<Toggle>().group = C_Content.GetComponent<ToggleGroup>();
                                _color.GetComponent<Toggle>().onValueChanged.AddListener(delegate (bool isOn)
                                {
                                    if (isOn)
                                    {
                                        ItemColor = color;
                                        Debug.Log("当前选取的颜色是:" + ItemColor);
                                    }
                                });
                            }
                            C_Content.GetChild(0).GetComponent<Toggle>().isOn = true;
                        }
                    }
                }
            });
        }
    }

    /// <summary>
    /// 测试功能
    /// </summary>
    public void SetDecirationList()
    {
        List<Color> Colors = new List<Color>();
        Colors.Add(new Color(0, 0, 0, 1));
        Colors.Add(new Color(0, 0, 1, 1));
        List<Color> Colors2 = new List<Color>();
        Colors2.Add(new Color(0, 1, 0, 1));
        Colors2.Add(new Color(1, 0, 0, 1));
        Decorations.Add(new Decoration(0, "Hair Style", "", "", "AAA", Colors));
        Decorations.Add(new Decoration(0, "Hair Style", "", "", "ABC", Colors));
        Decorations.Add(new Decoration(0, "Hair Style", "", "", "CCC", Colors));
        Decorations.Add(new Decoration(0, "Head Dress", "", "", "BBB", Colors2));
        Decorations.Add(new Decoration(0, "Head Dress", "", "", "DDD", Colors2));
    }
}

/// <summary>
/// 装饰品类
/// </summary>
public class Decoration
{
    public int _Gender { get; private set; }
    public string _Type { get; private set; }
    public Sprite _Sprite { get; private set; }
    public GameObject _Model { get; private set; }
    public string _Name { get; private set; }
    public List<Color> _Colors { get; private set; }

    public Decoration(int gender, string type, string spritePath, string modelPath, string name, List<Color> colors)
    {
        _Gender = gender;
        _Type = type;
        _Sprite = Resources.Load(spritePath) as Sprite;
        _Model = Resources.Load(modelPath) as GameObject;
        _Name = name;
        _Colors = colors;
    }
}