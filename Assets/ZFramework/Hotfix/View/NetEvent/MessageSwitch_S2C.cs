using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Sockets;
using UnityEngine;

namespace ZFramework
{
    public class Message_S2C_账号登入返回 : EventCallback<S2C_账号登入返回>
    {
        public override void Callback(S2C_账号登入返回 arg)
        {
            GameManager.Instance.Account = arg.account;
            GameManager.Instance.roleCaches = arg.roles;

            UIManager.Instance.Pop();
            UIManager.Instance.Push(new RoleSelectPanel());
        }
    }
    public class Message_S2C_账号登出返回 : EventCallback<S2C_账号登出返回>
    {
        public override void Callback(S2C_账号登出返回 arg)
        {
            GameManager.Instance.Account = null;
            GameManager.Instance.roleCaches = null;

            UIManager.Instance.Pop();
            UIManager.Instance.Push(new LoginPanel());
        }
    }
    public class Message_S2C_顶号 : EventCallback<S2C_顶号>
    {
        public override void Callback(S2C_顶号 arg)
        {
            //两种逻辑  一般是二选一 支持顶号和不支持顶号  默认支持顶号
            if (GameManager.Instance.Account != null && GameManager.Instance.Account.id == arg.accountID)
            {
                Log.Info("被顶号了  退回登录界面");

                //要根据当前登录情况来处理顶号逻辑

                if (GameManager.Instance.Location == null) //1 在选择/编辑界面
                {
                    GameManager.Instance.Account = null;
                    GameManager.Instance.roleCaches = null;
                    GameManager.Instance.Location = null;

                    UIManager.Instance.Pop();
                    UIManager.Instance.Push(new LoginPanel());
                }
                else //2 在场景中
                {
                    GameManager.Instance.Account = null;
                    GameManager.Instance.roleCaches = null;
                    GameManager.Instance.Location = null;

                    ScenePlayerHandleComponent.Instance.ClearAll();

                    var location = new RoleLocationInfo()
                    {
                         mapName = MapNames.大厅
                    };
                    UIManager.Instance.Pop();
                    UIManager.Instance.Push(new LoadingPanel(location, true));
                }

                //3 在读条中??? 读条中被顶号没处理  会报错
                
            }
            else
            {
                Log.Info("账号已登录  弹个窗提示  重新点一下登录");
            }
        }
    }
    public class Message_S2C_保存模型返回 : EventCallback<S2C_保存模型返回>
    {
        public override void Callback(S2C_保存模型返回 arg)
        {
            //有返回就是OK  没写鉴定
            var config = arg.config;

            for (int i = 0; i < GameManager.Instance.roleCaches.Length; i++)
            {
                if (GameManager.Instance.roleCaches[i].id == config.id)
                {
                    GameManager.Instance.roleCaches[i] = config;
                    Debug.Log("角色已保存");
                    Debug.Log("保存按钮事件:弹出角色选择面板");
                    UIManager.Instance.Pop();
                    UIManager.Instance.Push(new RoleSelectPanel());
                    return;
                }
            }

            Log.Error("role保存成功 但跟本地不匹配");
        }
    }


    public class Message_S2C_角色登入返回 : EventCallback<S2C_角色开始登入返回>
    {
        public override void Callback(S2C_角色开始登入返回 arg)
        {
            var location = arg.location;
            GameManager.Instance.Location = location;

            //设置广告内容
            var _LoadingPanel = new LoadingPanel(location, true);
            _LoadingPanel.Advertisement = null;

            UIManager.Instance.Pop();
            UIManager.Instance.Push(_LoadingPanel);
            //_LoadingPanel加载完成 会创建self 并请求同步同地图的其他玩家 正式完成登入操作
        }
    }
    public class Message_S2C_角色完成登入返回 : EventCallback<S2C_角色完成登入返回>
    {
        public override void Callback(S2C_角色完成登入返回 arg)
        {
            //传一堆role 以及role的坐标过来
            var roles = arg.syncLocations;
            foreach (var item in roles)
            {
                ScenePlayerHandleComponent.Instance.CreateOther(item);
            }
        }
    }
    public class Message_S2C_角色登出返回 : EventCallback<S2C_角色登出返回>
    {
        public override void Callback(S2C_角色登出返回 arg)
        {
            if (arg.roleID == GameManager.Instance.Location.role.id)//是自己登出
            {
                GameManager.Instance.Location = null;
                ScenePlayerHandleComponent.Instance.ClearAll();

                UIManager.Instance.Pop();
                UIManager.Instance.Push(new RoleSelectPanel());
            }
            else//其他同地图玩家登出
            {
                ScenePlayerHandleComponent.Instance.RemoveOther(arg.roleID);
            }
        }
    }

    public class Message_S2C_角色开始传送 : EventCallback<S2C_角色开始传送返回>
    {
        public override void Callback(S2C_角色开始传送返回 arg)
        {
            ScenePlayerHandleComponent.Instance.ClearAll();
            UIManager.Instance.Pop();
            UIManager.Instance.Push(new LoadingPanel(arg.location,false));
        }
    }
    public class Message_S2C_角色完成传送 : EventCallback<S2C_角色完成传送返回>
    {
        public override void Callback(S2C_角色完成传送返回 arg)
        {
            foreach (var item in arg.syncLocations)
            {
                ScenePlayerHandleComponent.Instance.CreateOther(item);
            }
        }
    }



    public class Message_S2C_位置同步返回 : EventCallback<S2C_位置同步返回>
    {
        public override void Callback(S2C_位置同步返回 arg)
        {
            var play = ScenePlayerHandleComponent.Instance.GetSyncPlayFromRoleID(arg.roleID);
            if (play != null)
            {
                var p = new Vector3(arg.x, arg.y, arg.z);
                var q = new Quaternion(arg.a, arg.b, arg.c, arg.d);
                var k = arg.animKey;
                play.syncMovement.Input(p, q, k, arg.serverTime);
            }
        }
    }
    public class Message_S2C_角色离线 : EventCallback<S2C_角色离线>
    {
        public override void Callback(S2C_角色离线 arg)
        {
            ScenePlayerHandleComponent.Instance.RemoveOther(arg.roleID);
        }
    }
    public class Message_S2C_聊天室 : EventCallback<S2C_聊天室>
    {
        public override void Callback(S2C_聊天室 arg)
        {
            ZEvent.CustomEvent.Call("聊天室", arg);
        }
    }



















    public class Message_S2C_心跳 : EventCallback<S2C_心跳>
    {
        public override void Callback(S2C_心跳 arg)
        {
            var timeSend = arg.time;
            var timeServer = arg.serverTime;
            var timeNow = DateTime.UtcNow.Ticks / 10000 - Game.epochTick;

            var rtt = (timeNow - timeSend) / 2;//一半rtt  ->延迟
            Log.Info("RTT延迟->" + rtt);
            TcpClientComponent.Instance.rtt_2 = new TimeSpan(rtt * 10000);
            TcpClientComponent.Instance.serverTime = new DateTime((timeServer + rtt + Game.epochTick) * 10000);

            //客户端如果心跳超时  就提示掉线. 先不做
        }
    }
}
