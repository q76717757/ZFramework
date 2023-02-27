using System;
using System.Collections.Generic;
using TouchSocket.Sockets;

namespace ZFramework
{
    public class ProtocolUpdate : OnUpdateImpl<ProtocolHandleComponent>
    {
        public override void OnUpdate(ProtocolHandleComponent self)
        {
            self.Update();
        }
    }

    //临时协议解析  收到消息在这里解析 然后派发事件出去
    public class ProtocolHandleComponent :SingleComponent<ProtocolHandleComponent>
    {
        Queue<INetSerialize> msgQueue = new Queue<INetSerialize>();
        public void Client2Server(SocketClient client , byte[] bytes)
        {
            lock (msgQueue)
            {
                PType type = (PType)bytes[0];
                C2S_Message obj;
                switch (type)
                {
                    case PType.C2S_心跳:
                        obj = new C2S_心跳();
                        break;
                    case PType.C2S_账号登入:
                        obj = new C2S_账号登入();
                        break;
                    case PType.C2S_账号登出:
                        obj = new C2S_账号登出();
                        break;
                    case PType.C2S_保存模型:
                        obj = new C2S_保存模型();
                        break;
                    case PType.C2S_角色开始登入:
                        obj = new C2S_角色开始登入();
                        break;
                    case PType.C2S_角色完成登入:
                        obj = new C2S_角色完成登入();
                        break;
                    case PType.C2S_角色登出:
                        obj = new C2S_角色登出();
                        break;
                    case PType.C2S_角色开始传送:
                        obj = new C2S_角色开始传送();
                        break;
                    case PType.C2S_角色完成传送:
                        obj = new C2S_角色完成传送();
                        break;
                    case PType.C2S_位置同步:
                        obj = new C2S_位置同步();
                        break;
                    case PType.C2S_聊天室:
                        obj = new C2S_聊天室();
                        break;
                    default: return;
                }
                obj.Client = client;
                obj.SetMesssage(bytes.RemoveCode());

                msgQueue.Enqueue(obj);
            }
        }

        public void Server2Client(TcpClient client, byte[] bytes)
        {
            lock (msgQueue)
            {
                PType type = (PType)bytes[0];
                S2C_Message obj;
                switch (type)
                {
                    case PType.S2C_心跳:
                        obj = new S2C_心跳();
                        break;
                    case PType.S2C_账号登入返回:
                        obj = new S2C_账号登入返回();
                        break;
                    case PType.S2C_账号登出返回:
                        obj = new S2C_账号登出返回();
                        break;
                    case PType.S2C_顶号:
                        obj = new S2C_顶号();
                        break;
                    case PType.S2C_保存模型返回:
                        obj = new S2C_保存模型返回();
                        break;
                    case PType.S2C_角色开始登入返回:
                        obj = new S2C_角色开始登入返回();
                        break;
                    case PType.S2C_角色完成登入返回:
                        obj = new S2C_角色完成登入返回();
                        break;
                    case PType.S2C_角色登出返回:
                        obj = new S2C_角色登出返回();
                        break;
                    case PType.S2C_角色开始传送返回:
                        obj = new S2C_角色开始传送返回();
                        break;
                    case PType.S2C_角色完成传送返回:
                        obj = new S2C_角色完成传送返回();
                        break;
                    case PType.S2C_位置同步返回:
                        obj = new S2C_位置同步返回();
                        break;
                    case PType.S2C_角色离线:
                        obj = new S2C_角色离线();
                        break;
                    case PType.S2C_聊天室:
                        obj = new S2C_聊天室();
                        break;
                    default: return;
                }
                obj.Client = client;
                obj.SetMesssage(bytes.RemoveCode());
                msgQueue.Enqueue(obj);
            }
        }

        public void Update()
        {
            while (msgQueue.Count > 0)
            {
                INetSerialize obj;
                lock (msgQueue)
                {
                    obj = msgQueue.Dequeue();
                }
                if (obj == null)
                    continue;

                switch (obj.Code)
                {
                    case PType.C2S_心跳:
                        EventSystem.Call((C2S_心跳)obj);
                        break;
                    case PType.S2C_心跳:
                        EventSystem.Call((S2C_心跳)obj);
                        break;


                    case PType.C2S_账号登入:
                        EventSystem.Call((C2S_账号登入)obj);
                        break;
                    case PType.C2S_账号登出:
                        EventSystem.Call((C2S_账号登出)obj);
                        break;
                    case PType.C2S_保存模型:
                        EventSystem.Call((C2S_保存模型)obj);
                        break;
                    case PType.C2S_角色开始登入:
                        EventSystem.Call((C2S_角色开始登入)obj);
                        break;
                    case PType.C2S_角色完成登入:
                        EventSystem.Call((C2S_角色完成登入)obj);
                        break;
                    case PType.C2S_角色登出:
                        EventSystem.Call((C2S_角色登出)obj);
                        break;
                    case PType.C2S_位置同步:
                        EventSystem.Call((C2S_位置同步)obj);
                        break;
                    case PType.C2S_角色开始传送:
                        EventSystem.Call((C2S_角色开始传送)obj);
                        break;
                    case PType.C2S_角色完成传送:
                        EventSystem.Call((C2S_角色完成传送)obj);
                        break;
                    case PType.C2S_聊天室:
                        EventSystem.Call((C2S_聊天室)obj);
                        break;


                    case PType.S2C_账号登入返回:
                        EventSystem.Call((S2C_账号登入返回)obj);
                        break;
                    case PType.S2C_账号登出返回:
                        EventSystem.Call((S2C_账号登出返回)obj);
                        break;
                    case PType.S2C_顶号:
                        EventSystem.Call((S2C_顶号)obj);
                        break;
                    case PType.S2C_保存模型返回:
                        EventSystem.Call((S2C_保存模型返回)obj);
                        break;
                    case PType.S2C_角色开始登入返回:
                        EventSystem.Call((S2C_角色开始登入返回)obj);
                        break;
                    case PType.S2C_角色完成登入返回:
                        EventSystem.Call((S2C_角色完成登入返回)obj);
                        break;
                    case PType.S2C_角色登出返回:
                        EventSystem.Call((S2C_角色登出返回)obj);
                        break;
                    case PType.S2C_角色开始传送返回:
                        EventSystem.Call((S2C_角色开始传送返回)obj);
                        break;
                    case PType.S2C_角色完成传送返回:
                        EventSystem.Call((S2C_角色完成传送返回)obj);
                        break;

                    case PType.S2C_位置同步返回:
                        EventSystem.Call((S2C_位置同步返回)obj);
                        break;
                    case PType.S2C_角色离线:
                        EventSystem.Call((S2C_角色离线)obj);
                        break;
                    case PType.S2C_聊天室:
                        EventSystem.Call((S2C_聊天室)obj);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
