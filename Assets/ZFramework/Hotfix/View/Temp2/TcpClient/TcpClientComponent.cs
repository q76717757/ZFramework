using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using TouchSocket.Core.ByteManager;
using TouchSocket.Core.Config;
using TouchSocket.Sockets;
using UnityEngine;
using UnityEngine.UI;
using TcpClient = TouchSocket.Sockets.TcpClient;

namespace ZFramework
{
    public class TcpClientAwake : OnAwakeImpl<TcpClientComponent>
    {
        public override void OnAwake(TcpClientComponent self)
        {
            self.StartClient();
        }
    }

    public class TcpClientUpdate : OnUpdateImpl<TcpClientComponent>
    {
        float t = 5;
        public override void OnUpdate(TcpClientComponent self)
        {
            if (self.ClientInstance != null && self.ClientInstance.Online)
            {
                t += UnityEngine.Time.deltaTime;
                if (t > 5)
                {
                    t = 0;
                    TcpClientComponent.Instance.Send2ServerAsync(new C2S_心跳()
                    {
                        time = DateTime.UtcNow.Ticks / 10000 - Game.epochTick
                    });
                }
            }
        }
    }


    public class TcpClientComponent :SingleComponent<TcpClientComponent>
    {
       
        /// <summary>
        /// RTT消息往返时间的一半 (延迟)
        /// </summary>
        public TimeSpan rtt_2;
        /// <summary>
        /// 估算的服务器时间
        /// </summary>
        public DateTime serverTime;


        private TcpClient m_tcpClient;//长链接
        public TcpClient ClientInstance => m_tcpClient;

        public void StartClient()
        {
            m_tcpClient = new TcpClient();
            //声明配置
            TouchSocketConfig config = new TouchSocketConfig();
            config.SetRemoteIPHost(new IPHost($"{Game.Host}:7789"))
                .UsePlugin()
                .SetBufferLength(1024 * 64)
                .ConfigureContainer(a =>
                {
                    Log.Info("ConfigureContainer");
                })
                .SetDataHandlingAdapter(() => new FixedHeaderPackageAdapter())
                .ConfigurePlugins(a =>
                {
                    Log.Info("ConfigurePlugins");
                })
                .SetKeepAliveValue(new KeepAliveValue());

            m_tcpClient.Connected += (client, e) =>
            {
                Log.Info("成功连接" + client.IP + ":" + client.Port);
            };//成功连接到服务器
            m_tcpClient.Disconnected += (client, e) =>
            {
                Log.Info($"断开连接，信息：{e.Message}");
            };
            m_tcpClient.Received += this.TcpClient_Received;

            //载入配置
            m_tcpClient.Setup(config);
            m_tcpClient.Connect();
        }
        void TcpClient_Received(TcpClient client, ByteBlock byteBlock, IRequestInfo requestInfo)//子线程
        {
            ProtocolHandleComponent.Instance.Server2Client(client, byteBlock.ToArray());
        }
        public void DisConnent()
        {
            m_tcpClient.SafeDispose();
        }

        public void Send2ServerAsync(INetSerialize p)
        {
            if (m_tcpClient != null && m_tcpClient.Online)
            {
                m_tcpClient.SendAsync(p.GetMessage().AddCode(p.Code));
            }
        }



    }
}
