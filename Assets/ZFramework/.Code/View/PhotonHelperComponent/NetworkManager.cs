///**   ZFramework
//* FileName:         NetworkManager.cs
//* Author:           Rucie
//* Email:            76717757@qq.com
//* CreateTime:       2021-09-17 10:11
//* Description:      
//**/

//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using ExitGames.Client.Photon;
//using Photon.Pun;
//using Photon.Realtime;
//using UnityEngine;
//using ZFramework;

//public class NetEvent {
//    public const string 建立连接 = "建立连接";
//    public const string 断开连接 = "断开连接";

//    public const string 加入大厅 = "加入大厅";
//    public const string 离开大厅 = "离开大厅";

//    public const string 创建房间 = "创建房间";
//    public const string 房间列表更新 = "房间列表更新";//大厅里面的人响应
//    public const string 房间属性更新 = "房间属性更新";//同房间内的人响应

//    public const string 加入房间 = "加入房间";
//    public const string 离开房间 = "离开房间";

//    public const string 玩家进入 = "玩家进入";
//    public const string 玩家退出 = "玩家退出";
//    public const string 玩家属性修改 = "玩家属性修改";

//}

//public class NetworkManager : MonoBehaviourPunCallbacks
//{
//    private static NetworkManager instance;
//    public static NetworkManager Instance
//    {
//        get
//        {
//            if (instance == null)
//            {
//                instance = new GameObject($"[{typeof(NetworkManager).Name}]").AddComponent<NetworkManager>();
//                UnityEngine.GameObject.DontDestroyOnLoad(instance.gameObject);
//            }
//            return instance;
//        }
//    }

//    GameObject pn;//主机有值 成员为空
//    public List<RoomInfo> RoomList { get; } = new List<RoomInfo>();//所在大厅的房间列表

//    #region 建立连接相关
//    public void StartConnect(string nickName)
//    {
//        if (!PhotonNetwork.IsConnected)
//        {
//            PhotonNetwork.AutomaticallySyncScene = false;//同时切换场景
//            PhotonNetwork.NetworkingClient.SerializationProtocol = SerializationProtocol.GpBinaryV16;

//            PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
//            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = null;
//            PhotonNetwork.PhotonServerSettings.AppSettings.Server = "127.0.0.1";// BootStrap.Options.PhotonIP;
//            PhotonNetwork.PhotonServerSettings.AppSettings.Port = "5055";// BootStrap.Options.PhotonPort;
//            PhotonNetwork.GameVersion = "1"; //BootStrap.Options.PhotonVersion;//这是一个重要的字段 以保证不同的发布版本之间不相互连接 (协议版本号)  好像不管用??
//            PhotonNetwork.NickName = nickName;
//            PhotonNetwork.ConnectUsingSettings();           //初始化设置，连接服务器
//        }
//    }
//    public void DisConnect()
//    {
//        if (PhotonNetwork.IsConnected)
//            PhotonNetwork.Disconnect();
//    }

//    public bool JoinLobby()=> PhotonNetwork.JoinLobby(TypedLobby.Default);

//    public bool LevelLobby() => PhotonNetwork.LeaveLobby();

//    public bool CreateRoom(TaskInfo property,SpaceStationProperty spaceStation, string password)
//    {
//        RoomOptions roomOptions = new RoomOptions { PublishUserId = true, IsOpen = true, IsVisible = true, MaxPlayers = (byte)property.maxPlayers };
//        roomOptions.CustomRoomPropertiesForLobby = new string[] {
//            RoomProperty.PnVersion,
//            RoomProperty.创建者ID,
//            RoomProperty.创建者姓名,
//            RoomProperty.任务名称,
//            RoomProperty.房间密码,
//            RoomProperty.最大人数,

//            RoomProperty.房间创建时间,
//            RoomProperty.Env开始时间,
//            RoomProperty.Env结束时间,
//            RoomProperty.EnvT0,

//            RoomProperty.用户列表,
//            RoomProperty.空间站配置,
//        };
//        roomOptions.CustomRoomProperties = new Hashtable {
//            { RoomProperty.PnVersion,BootStrap.Options.PhotonVersion},//因为共用后台,确保不同发布版本不互通,避免BUG
//            { RoomProperty.创建者ID,property.userid },
//            { RoomProperty.创建者姓名,property.realname },
//            { RoomProperty.任务名称, property.missionname },
//            { RoomProperty.房间密码, password },
//            { RoomProperty.最大人数, property.maxPlayers },

//            { RoomProperty.房间创建时间, property.createtime },
//            { RoomProperty.Env开始时间, property.starttime },
//            { RoomProperty.Env结束时间, property.endtime },
//            { RoomProperty.EnvT0, property.selecttime },

//            { RoomProperty.用户列表, new List<int>{ property.userid }.ToJson() },
//            { RoomProperty.空间站配置, spaceStation.ToJson()},
//        };
//        var ronnName = ((DateTime.Now.Ticks - T.E) / 10000).ToString("X2");
//        return PhotonNetwork.CreateRoom(ronnName, roomOptions, TypedLobby.Default);
//    }

//    public bool JoinRoom(string roomName) {
//        CursorCtrl.Busy = true;
//        return PhotonNetwork.JoinRoom(roomName);
//    }
//    public bool LevelRoom(bool becomeInactive = true)
//    {
//        CursorCtrl.Busy = true;//离开房间会返回大厅  OnConnectedToMaster 有set true
//        return PhotonNetwork.LeaveRoom(becomeInactive);
//    }
//    #endregion

//    #region Pun Callbacks
//    public override void OnConnectedToMaster()
//    {
//        Log.Info("OnConnectedToMaster");
//        CursorCtrl.Busy = false;
//        ZEvent.CustomEvent.Call(NetEvent.建立连接);
//    }
//    public override void OnDisconnected(DisconnectCause cause) {
//        Log.Info($"OnDisconnected:{cause}");
//        ZEvent.CustomEvent.Call(NetEvent.断开连接, cause);
//        RoomList.Clear();
//    }

//    public override void OnJoinedLobby()
//    {
//        Log.Info("OnJoinedLobby");
//        RoomList.Clear();
//        ZEvent.CustomEvent.Call(NetEvent.加入大厅);
//    }
//    public override void OnLeftLobby()
//    {
//        Log.Info("OnLeftLobby");
//        RoomList.Clear();
//        ZEvent.CustomEvent.Call(NetEvent.离开大厅);
//    }

//    public override void OnCreatedRoom()
//    {
//        Log.Info("OnCreatedRoom");
//        pn = PhotonNetwork.Instantiate("RpcHelper", Vector3.zero, Quaternion.identity);//这是PRC消息的代理 其他人进房间会自动创建 出房间会自动删除 由房主持有
//        //CursorCtrl.Busy = false;//创建房间还会调onJoinRoom  这个不需要
//        ZEvent.CustomEvent.Call(NetEvent.创建房间, true, "Success");
//    }
//    public override void OnCreateRoomFailed(short returnCode, string message)
//    {
//        Log.Info($"OnCreateRoomFailed:{returnCode}:{message}");
//        //code 32766 ==>  已存在同名的房间
//        CursorCtrl.Busy = false;
//        ZEvent.CustomEvent.Call(NetEvent.创建房间, false, message);
//    }

//    public override void OnJoinedRoom()
//    {
//        Log.Info("OnJoinedRoom");//(大厅和房间不共存) 加入房间会先离开大厅 然后再尝试加入房间 并不执行离开大厅的回调  需要把缓存的房间列表清掉 当重新回到大厅的时候再生成新的房间列表  ph可能有缓存  也许不需要重建列表?
//        if (PhotonNetwork.LocalPlayer.IsMasterClient)
//        {
//            var table = new Hashtable()
//            {
//                { PlayerProperty.账号ID, BootStrap.Account.id },
//                { PlayerProperty.姓名, BootStrap.Account.realname },
//                { PlayerProperty.终端, (int)ClientType.管理员},
//                { PlayerProperty.准备情况, true},
//            };
//            PhotonNetwork.LocalPlayer.SetCustomProperties(table);
//        }
//        else
//        {
//            var table = new Hashtable()
//            {
//                { PlayerProperty.账号ID, BootStrap.Account.id },
//                { PlayerProperty.姓名, BootStrap.Account.realname },
//                { PlayerProperty.终端, (int)ClientType.未选择},
//                { PlayerProperty.准备情况, false},
//            };
//            PhotonNetwork.LocalPlayer.SetCustomProperties(table);
//        }
//        CursorCtrl.Busy = false;
//        ZEvent.CustomEvent.Call(NetEvent.加入房间, true, "Success");//spv初始化偶尔会比onjoin回调慢  用延迟列表解决
//    }

//    public override void OnJoinRoomFailed(short returnCode, string message)
//    {
//        Log.Info($"OnJoinRoomFailed:{returnCode}:{message}");
//        CursorCtrl.Busy = false; //创建房间失败 会不会执行加入失败回调??  不确定
//        ZEvent.CustomEvent.Call(NetEvent.加入房间, false, message);
//    }
//    public override void OnLeftRoom()
//    {
//        Log.Info("OnLeftRoom");
//        if (pn != null)
//            PhotonNetwork.Destroy(pn);

//        ZEvent.CustomEvent.Call(NetEvent.离开房间);
//    }

//    public override void OnRoomListUpdate(List<RoomInfo> roomList)
//    {
//        Log.Info($"OnRoomListUpdate:{roomList.Count}");//只包含发生变化的房间
//        foreach (var room in roomList)
//        {
//            Log.Info(room.ToStringFull());
//            bool has = false;
//            for (int i = 0; i < RoomList.Count; i++)
//            {
//                if (RoomList[i].Name == room.Name)//房间名是唯一的
//                {
//                    if (room.RemovedFromList)
//                    {
//                        RoomList.RemoveAt(i);
//                    }
//                    else
//                    {
//                        RoomList[i] = room;
//                    }
//                    has = true;
//                    break;
//                }
//            }
//            if (!has)//找不到一样的 就是新增的
//            {
//                if (!room.RemovedFromList)
//                {
//                    RoomList.Add(room);
//                }
//            }
//        }
//        ZEvent.CustomEvent.Call(NetEvent.房间列表更新);
//    }//在大厅时 房间增减或属性变更
//    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
//    {
//        Log.Info($"OnRoomPropertiesUpdate" );
//        foreach (var item in propertiesThatChanged)
//            Log.Info($"{item.Key}:{item.Value}");

//        ZEvent.CustomEvent.Call(NetEvent.房间属性更新, propertiesThatChanged);
//    }//在房间内时 本房间属性变更

//    public override void OnPlayerEnteredRoom(Player newPlayer)
//    {
//        Log.Info($"OnPlayerEnteredRoom:{newPlayer.NickName}");
//        ZEvent.CustomEvent.Call(NetEvent.玩家进入, newPlayer);
//    }
//    public override void OnPlayerLeftRoom(Player otherPlayer)
//    {
//        Log.Info($"OnPlayerLeftRoom:{otherPlayer.NickName}");
//        ZEvent.CustomEvent.Call(NetEvent.玩家退出, otherPlayer);
//    }
//    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
//    {
//        Log.Info($"OnPlayerPropertiesUpdate:{targetPlayer.NickName}:{changedProps.Count}");
//        foreach (var item in changedProps)
//        {
//            Log.Info(item.Key + ":" + item.Value);
//        }
//        ZEvent.CustomEvent.Call(NetEvent.玩家属性修改, targetPlayer, changedProps);
//    }

//    public override void OnMasterClientSwitched(Player newMasterClient)
//    { 
//        Log.Info($"OnMasterClientSwitched:{newMasterClient.NickName}");
//        //在需求中 房主不能移交  只有房主退出或者掉线才会执行这个   房主退出  所有人都跟着退出
//        LevelRoom();
//    }
//    public override void OnErrorInfo(ErrorInfo errorInfo) => Log.Error($"Photon.OnErrorInfo:{errorInfo.Info}");
//    #endregion



//}
