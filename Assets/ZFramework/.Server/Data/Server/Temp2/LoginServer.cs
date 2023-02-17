using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Sockets;

namespace ZFramework
{
    public class LoginServer:SingleComponent<LoginServer>
    {
        //key = accountID
        public Dictionary<long, SocketClient> onlineAccounts = new Dictionary<long, SocketClient>();//所有在线账号的连接 未必已登录进在场景中

        public void Login(long accountID,SocketClient newSocket)
        {
            if (onlineAccounts.TryGetValue(accountID, out SocketClient socket))//已登录账号
            {
                Log.Info("已登录账号-> 顶号" + accountID);

                //处理顶号  发消息给旧登陆 让其登出
                Log.Info("旧设备登出" + socket.ID);
                TcpServerComponent.Instance.Send2ClientAsync(socket.ID, new S2C_顶号() { accountID = accountID });

                MapServer.Instance.OnClientDisconnected(socket.ID);
                onlineAccounts.Remove(accountID);

                if (socket.ID == newSocket.ID)
                {
                    Log.Error("同客户端自己顶自己  前端有重复登录,这个是不应该存在的?");//就算强退秒登  会从新端口进来 不会认为是同设备 如果正常流程退出不应该还存在顶号
                    return;
                }
                //发消息给新登录  提示已登录
                Log.Info("新设备登入" + newSocket.ID);
                TcpServerComponent.Instance.Send2ClientAsync(newSocket.ID, new S2C_顶号() { accountID = accountID });
            }
            else//正常登录
            {
                var account = TempDB.Instance.GetAccount(accountID);

                Log.Info("账号登录->" + account.ToJson());

                var send = new S2C_账号登入返回()
                {
                    account = account,
                    roles = new Role[account.roles.Count],
                };

                for (int i = 0; i < send.roles.Length; i++)
                {
                    var role = TempDB.Instance.GetRole(account.roles[i]);
                    if (role == null)
                    {
                        return;
                    }
                    send.roles[i] = role;
                }
                TcpServerComponent.Instance.Send2ClientAsync(newSocket.ID, send);
                onlineAccounts.Add(accountID, newSocket);
            }
        }

        public void Logout(long accountID)
        {
            if (onlineAccounts.TryGetValue(accountID,out SocketClient socket))
            {
                Log.Info("账号登出->" + accountID);
                onlineAccounts.Remove(accountID);

                MapServer.Instance.OnClientDisconnected(socket.ID);
            }
            TcpServerComponent.Instance.Send2ClientAsync(socket.ID, new S2C_账号登出返回());
        }

        public void OnClientDisconnected(string clientID)//强退/断网
        {
           
            foreach (var item in onlineAccounts)
            {
                if (item.Value.ID == clientID)
                {
                    Log.Info($"账号{item.Key}:{clientID} --> 掉线");

                    onlineAccounts.Remove(item.Key);
                    MapServer.Instance.OnClientDisconnected(clientID);
                    return;
                }
            }
        }
    }
}
