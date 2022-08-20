///**
//* FileName:         RpcHelper.cs
//* Author:           Rucie
//* Email:            76717757@qq.com
//* CreateTime:       2021-09-13 17:53
//* Description:      作为一个中转站  由房主创建  房主收集大家提交的数据  整合打包后群发回去  以及进行一些通信交换
//**/

//using Photon.Pun;
//using Photon.Realtime;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine; 
//using ZFramework;

//public class RpcHelper : MonoBehaviour//, IPunObservable
//{
//    static PhotonView spv;
//    [SerializeField]
//    private PhotonView pv;

//    static List<(RPC协议, RpcTarget, object[])> cache = new List<(RPC协议, RpcTarget, object[])>();//延迟列表  spv有时候会报空的bug 用这个解决
//    static List<(RPC协议, Player, object[])> cache2 = new List<(RPC协议, Player, object[])>();//这个应该用不上

//    public void Awake()
//    {
//        if (pv != null)
//        {
//            spv = pv;
//        }
//        DontDestroyOnLoad(gameObject);//重要  带入太空场景  其他成员才会自动实例
//    }
//    void Start() {
//        foreach (var item in cache)
//        {
//            SendRPC(item.Item1, item.Item2, item.Item3);
//        }
//        cache.Clear();
//        foreach (var item in cache2)
//        {
//            SendRPC(item.Item1, item.Item2, item.Item3);
//        }
//        cache2.Clear();
//    }

//    /*
//    Unreliable  更新如实发送，但可能会丢失。这个想法是，下一次更新很快到来，并提供所需的正确的/绝对的值。这对于位置和其他绝对数据来说是有利的，但对于像切换武器这样触发器来说是不好的。当用于同步的游戏对象的位置，它会总是发送更新，即使该游戏对象停止运动（这是不好的）。
//    Unreliable on Change(默认) 将检查每一个更新的更改。如果所有值与之前发送的一样，该更新将作为可靠的被发送，然后所有者停止发送更新直到事情再次发生变化。这对于那些可能会停止运动的以及暂时不会创建进一步更新的游戏对象来说是有利的。例如那些在找到自己的位置后就不再移动的箱子。
//    Reliable Delta Compressed 将更新的每个值与它之前的值进行比较。未更改的值将跳过以保持低流量。接收端只需填入先前更新的值。任何你通过OnPhotonSerializeView写入的都会自动进行检查并以这种方式被压缩。如果没有改变， OnPhotonSerializeView不会在接收客户端调用。该“可靠的”部分需要一些开销，所以对于小的更新，应该考虑这些开销。
//     */
//    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//    //{
//    //    //if (stream.IsWriting)//主机写
//    //    //{
//    //    //    //stream.SendNext(123);
//    //    //    //ZLog.Info("Writing");
//    //    //}
//    //    //if (stream.IsReading)//其他人读
//    //    //{
//    //    //    //ZLog.Info("Read:" + stream.ReceiveNext());
//    //    //}
//    //}

//    //因为photon.rpc方法可变参数的缘故 当一个参数的时候 rpc回调并不是obj数组回调? 会报错 这样做为了强制变成2个以上参数 让他可以正确指向执行标记方法
//    static object PackingParms(RPC协议 rpc, params object[] parms)
//    {
//        object[] package;
//        if (parms == null || parms.Length == 0)
//        {
//            package = new object[2];
//            package[1] = null;
//        }
//        else
//        {
//            package = new object[parms.Length + 1];
//            for (int i = 0; i < parms.Length; i++)
//                package[i + 1] = parms[i];
//        }
//        package[0] = (byte)rpc;
//        return package;
//    }

//    public static void SendRPC(RPC协议 rpc, RpcTarget target, params object[] parms)
//    {
//        if (spv != null)
//        {
//            var sendParms = PackingParms(rpc, parms);
//            spv.RPC("RpcMessage", target, sendParms);
//        }
//        else
//        {
//            //Log.Error("spv is null");
//            cache.Add((rpc, target, parms));
//        }
//    }
//    public static void SendRPC(RPC协议 rpc, Player target, params object[] parms)
//    {
//        if (spv != null)
//        {
//            var sendParms = PackingParms(rpc, parms);
//            spv.RPC("RpcMessage", target, sendParms);
//        }
//        else
//        {
//            //Log.Error("spv is null");
//            cache2.Add((rpc, target, parms));
//        }
//    }

//    public static void ClientSendRequest(DTRequestType type)//所有人 包括主机和成员
//    {
//        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom) return;
//        Log.Info($"Player[{PhotonNetwork.LocalPlayer.ActorNumber}]->向主机请求数据:{type}");
//        RpcHelper.SendRPC(RPC协议.成员请求数据, RpcTarget.MasterClient, (int)type, PhotonNetwork.LocalPlayer.ActorNumber);
//    }
//    public static void MasterSendJson(DTRequestType type,int playerID, string json)//房主
//    {
//        if (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom || !PhotonNetwork.LocalPlayer.IsMasterClient) return;
//        if (PhotonNetwork.CurrentRoom.Players.TryGetValue(playerID,out Player player))
//        {
//            Log.Info($"Player[{playerID}]->向成员发送数据:{type}:{json}");
//            RpcHelper.SendRPC(RPC协议.房主发送数据, player, (int)type, (int)playerID, json);
//        }
//    }

//    [PunRPC]
//    public void RpcMessage(object[] parms)
//    {
//        if (parms == null || parms.Length < 2) return;
//        if (parms[0] is byte)
//        {
//            var code = (RPC协议)parms[0];
//            //Log.Info($"RPC.Code->{code}", Color.green);
//            RpcSwitch.Switch(code, parms);
//        }
//    }

//    void TestRpc2() {
//        //换场景用这个    
//        PhotonNetwork.IsMessageQueueRunning = false;    //防止PRC丢失  临时暂停消息队列  换完场景需要再打开队列
//        UnityEngine.SceneManagement.SceneManager.LoadScene("123");
//        PhotonNetwork.IsMessageQueueRunning = true;     //换场景换好之后  

//        //或者这个
//        PhotonNetwork.AutomaticallySyncScene = true;
//        PhotonNetwork.LoadLevel("123"); //这里面就处理了延迟消息队列
//    }

//}

