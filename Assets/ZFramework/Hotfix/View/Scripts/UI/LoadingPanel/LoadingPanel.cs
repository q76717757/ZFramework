using UnityEngine;
using UnityEngine.UI;
using ZFramework;

/// <summary>
/// 加载界面
/// </summary>
public class LoadingPanel : BasePanel
{
    public static readonly string path = "UI/LoadingPanel/LoadingPanel";
    public LoadingPanel(RoleLocationInfo location,bool isLogin) : base(new UI_Info(path)) 
    {
        this.isLogin = isLogin;
        this.Location = location;
    }

    public Sprite Advertisement = null;

    //public Role Role = null;
    //public string MapName = null;
    //public Vector3 PlayerPosition = Vector3.up;
    //public Quaternion PlayerRotation = Quaternion.identity;

    public bool isLogin;
    public RoleLocationInfo Location;

    public override void OnEnter()
    {
        base.OnEnter();
        EventManager.Instance.AddEventListener<float>("场景加载进度条更新", ProgressUpdate);
        MapManager.Instance.IAsynLoadScene(Location.mapName, 5, () => CompleteLoading());
        UIManager.Instance.UI_GetGameObject("AdvertisementMask").transform.Find("Advertisement").GetComponent<Image>().sprite = Advertisement;
    }

    /// <summary>
    /// 更新进度条事件
    /// </summary>
    /// <param name="progress">进度</param>
    private void ProgressUpdate(float progress)
    {
        UIManager.Instance.UI_GetGameObject("ProgressBar").GetComponent<Slider>().value = progress;
    }

    /// <summary>
    /// 加载完成事件
    /// </summary>
    private void CompleteLoading()
    {
        if (Location.role == null)//是被顶号的
        {
            UIManager.Instance.Pop();
            UIManager.Instance.Push(new LoginPanel());
        }
        else
        {
            ScenePlayerHandleComponent.Instance.CreateSelf(Location);

            UserInterface.Instance.Fade(this, new GamePanel(), false);
            //加载完场景 就把自己的角色创建出来

            EventSystem.Call(new ZFramework.EventType.OnSceneChangeFinish()
            {
                mapName = Location.mapName
            });

            
            if (isLogin)//login
            {
                //Sync Other Player     //请求这个场景内的所有角色
                TcpClientComponent.Instance.Send2ServerAsync(new C2S_角色完成登入()
                {
                    roleID = Location.role.id,
                    sceneName = Location.mapName
                });
            }
            else//move
            {
                //Sync Other Player     //请求这个场景内的所有角色
                TcpClientComponent.Instance.Send2ServerAsync(new C2S_角色完成传送()
                {
                     location = Location
                });

                GameManager.Instance.Location = Location;
            }

        }
        Location = null;
    }
}
