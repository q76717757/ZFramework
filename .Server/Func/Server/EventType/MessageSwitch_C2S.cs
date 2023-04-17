//using System;
//using System.Collections.Generic;
//using System.ComponentModel.Design;
//using System.Linq;
//using System.Text;
//using TouchSocket.Sockets;
////using System.Threading.Tasks;

//namespace ZFramework
//{
//    public class Message_C2S_账号登入 : EventCallback<C2S_账号登入>
//    {
//        public override void Callback(C2S_账号登入 arg)
//        {
//            Log.Info("账号登入->" + arg.PhoneNum);
//            //输入手机号查询
//            var phoneNum = arg.PhoneNum;
//            var account = TempDB.Instance.RegisterAccount(phoneNum);

//            //登录服
//            LoginServer.Instance.Login(account.id, arg.Client);
//        }
//    }
//    public class Message_C2S_账号登出 : EventCallback<C2S_账号登出>
//    {
//        public override void Callback(C2S_账号登出 arg)
//        {
//            LoginServer.Instance.Logout(arg.accountID);
//        }
//    }
//    public class Message_C2S_保存模型 : EventCallback<C2S_保存模型>
//    {
//        public override void Callback(C2S_保存模型 arg)
//        {
//            var saveConfig = arg.roleConfig;
//            //保存
//            var success = TempDB.Instance.SetRole(saveConfig);
//            if (success)
//            {
//                var send = new S2C_保存模型返回()
//                {
//                    config = saveConfig
//                };
//                TcpServerComponent.Instance.Send2ClientAsync(arg.Client.ID, send);
//            }
//            else
//            {
//                //保存失败 无效角色
//            }
//        }
//    }


//    public class Message_C2S_角色准备登入 : EventCallback<C2S_角色开始登入>
//    {
//        public override void Callback(C2S_角色开始登入 arg)
//        {
//            var lastLocation = TempDB.Instance.GetLocation(arg.roleID);
//            TcpServerComponent.Instance.Send2ClientAsync(arg.Client.ID, new S2C_角色开始登入返回()
//            {
//                location = lastLocation,
//            });
//        }
//    }
//    public class Message_C2S_角色完成登入 : EventCallback<C2S_角色完成登入>
//    {
//        public override void Callback(C2S_角色完成登入 arg)
//        {
//            MapServer.Instance.RoleLogin(arg.sceneName, arg.roleID, arg.Client);
//        }
//    }
//    public class Message_C2S_角色登出 : EventCallback<C2S_角色登出>
//    {
//        public override void Callback(C2S_角色登出 arg)
//        {
//            MapServer.Instance.RoleLogout(arg.levelMap, arg.roleID);
//        }
//    }
//    public class Message_C2S_角色开始传送 : EventCallback<C2S_角色开始传送>
//    {
//        public override void Callback(C2S_角色开始传送 arg)
//        {
//            var roleID = arg.roleID;
//            var exit = arg.exitMap;
//            var enter = arg.enterMap;
//            MapServer.Instance.RoleExit(roleID, exit, enter);
//        }
//    }
//    public class Message_C2S_角色完成传送 : EventCallback<C2S_角色完成传送>
//    {
//        public override void Callback(C2S_角色完成传送 arg)
//        {
//            MapServer.Instance.RoleEnter(arg.location);
//        }
//    }


//    public class Message_C2S_位置同步 : EventCallback<C2S_位置同步>
//    {
//        public override void Callback(C2S_位置同步 arg)
//        {
//            var roles = MapServer.Instance.GetRolesFromMap(arg.sceneName);
            
//            if (roles != null)
//            {
//                var send = new S2C_位置同步返回()
//                {
//                    roleID = arg.roleID,
//                    //sceneName = arg.sceneName,
//                    x = arg.x,
//                    y = arg.y,
//                    z = arg.z,
//                    a = arg.a,
//                    b = arg.b,
//                    c = arg.c,
//                    d = arg.d,
//                    animKey = arg.animKey,
//                    serverTime = DateTime.UtcNow.Ticks / 10000 - Game.epochTick - arg.time
//                };

//                foreach (var item in roles)
//                {
//                    var socket = MapServer.Instance.GetSocketFromRoleID(item);
//                    if (socket != null)
//                    {
//                        TcpServerComponent.Instance.Send2ClientAsync(socket.ID, send);
//                    }

//                }
//            }
//        }
//    }
//    public class Message_C2S_聊天室 : EventCallback<C2S_聊天室>
//    {
//        public override void Callback(C2S_聊天室 arg)
//        {
//            var otherRole = MapServer.Instance.GetThisMapOtherRolesFromRoleID(arg.roleID);
//            var send = new S2C_聊天室()
//            {
//                roleID = arg.roleID,
//                roleName = TempDB.Instance.GetRole(arg.roleID).roleName,
//                msg = arg.msg,
//            };
//            foreach (var item in otherRole)
//            {
//                var socket = MapServer.Instance.GetSocketFromRoleID(item);
//                if (socket != null)
//                {
//                    TcpServerComponent.Instance.Send2ClientAsync(socket.ID, send);
//                }
//            }
//            TcpServerComponent.Instance.Send2ClientAsync(arg.Client.ID, send);
//        }
//    }





















//    public class Message_C2S_心跳 : EventCallback<C2S_心跳>
//    {
//        public override void Callback(C2S_心跳 arg)
//        {
//            var send = new S2C_心跳()
//            {
//                time = arg.time,
//                serverTime = DateTime.UtcNow.Ticks / 10000 - Game.epochTick
//            };
//            TcpServerComponent.Instance.Send2ClientAsync(arg.Client.ID, send);

//            //服务端自己要不要存起来看情况
//        }
//    }
//}

