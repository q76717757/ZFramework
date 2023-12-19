using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouchSocket.Core.ByteManager;
using TouchSocket.Core.Config;
using TouchSocket.Core;
using TouchSocket.Sockets;
using System.Net;

namespace ZFramework
{

    public class TcpServerComponent : Component
    {
        private TcpService m_service;

        public void StartServer()
        {
            m_service = new TcpService();
            m_service.Connecting += this.OnClientConnecting;//有客户端正在连接
            m_service.Connected += this.OnClientConnected;//有客户端连接
            m_service.Disconnected += this.OnClientDisconnected; ;//有客户端断开连接
            m_service.Received += this.Service_Received;

            m_service.Setup(new TouchSocketConfig()//载入配置
               .SetListenIPHosts(new IPHost[] {new IPHost(IPAddress.Any,7789)})//同时监听两个地址
               .SetMaxCount(10000).SetBufferLength(1024 * 64)
               .SetThreadCount(10)
               .UsePlugin()
               .SetDataHandlingAdapter(()=>new FixedHeaderPackageAdapter())
               .ConfigureContainer(a =>
               {
                   Log.Info("ConfigureContainer");
               })
               .SetKeepAliveValue(new KeepAliveValue()))
               .Start();
            Log.Info("服务器成功启动");
        }

        private void OnClientConnecting(SocketClient client, ClientOperationEventArgs e)
        {
            e.ID = $"{client.IP}:{client.Port}";
        }
        private void OnClientConnected(SocketClient client, TouchSocketEventArgs e)
        {
            Log.Info("客户端建立连接 ->" + client.ID);//只是建立了连接 还没登录上
        }
        private void OnClientDisconnected(SocketClient client, ClientDisconnectedEventArgs e)
        {
            Log.Info("客户端断开连接 ->" + client.ID);
        }

        private void Service_Received(SocketClient client, ByteBlock byteBlock, IRequestInfo requestInfo)
        {
            Log.Info("收到" + client.IP + ":" +  Encoding.UTF8.GetString(byteBlock.ToArray()));
        }

        ////发送给客户端
        public void Send2ClientAsync(string clientID, byte[] msg)
        {
            m_service.SendAsync(clientID, msg);
        }
        //广播
        public void SendAllClientAsync(byte[] msg)
        {
            foreach (var client in m_service.GetClients())
            {
                m_service.SendAsync(client.ID, msg);
            }
        }
        //发送并等待
        public void SendAndAwaitCallback(string clientID, string msg)
        {
            //先找到与远程TcpClient对应的SocketClient
            if (m_service.TryGetSocketClient(clientID, out SocketClient client))
            {
                try
                {
                    //然后调用GetWaitingClient获取到IWaitingClient的对象。该对象会复用。
                    byte[] returnData = client.GetWaitingClient(WaitingOptions.AllAdapter).SendThenReturn(Encoding.UTF8.GetBytes(msg));
                    Log.Info($"收到回应消息：{Encoding.UTF8.GetString(returnData)}");
                }
                catch (TimeoutException)
                {
                    Log.Error("超时");
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
            }
        }
        //断开客户端
        public void DispostClient(string clientID)
        {
            //先找到与远程TcpClient对应的SocketClient
            if (m_service.TryGetSocketClient(clientID, out SocketClient client))
            {
                client.SafeDispose();
            }
        }

    }
}
