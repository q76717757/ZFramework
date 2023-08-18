using System.Collections;
using System.Collections.Generic;
using System.Text;
using TouchSocket.Core.ByteManager;
using TouchSocket.Core.Config;
using TouchSocket.Sockets;

namespace ZFramework
{
    public class TcpClientComponent : Component
    {
        public static TcpClientComponent instance;

        TcpClient tcpClient;
        public void StartClient()
        {
            instance = this;

            tcpClient = new TcpClient();
            tcpClient.Connected += OnConnected;
            tcpClient.Disconnected += OnDisconnected;
            tcpClient.Received += OnReceived;

            //声明配置
            TouchSocketConfig config = new TouchSocketConfig();
            config.SetRemoteIPHost(new IPHost("127.0.0.1:7789"))
                .UsePlugin()
                .SetDataHandlingAdapter(() => new FixedHeaderPackageAdapter());

            //载入配置
            tcpClient.Setup(config);
            tcpClient.Connect();
        }

        void OnConnected(ITcpClient client, MsgEventArgs e)
        {
            Log.Info("成功连接到服务器");
        }
        void OnDisconnected(ITcpClientBase client, MsgEventArgs e)
        {
            Log.Info("从服务器断开连接，当连接不成功时不会触发。");
        }
        void OnReceived(TcpClient client,ByteBlock byteBlock,IRequestInfo requestInfo)
        {
            string mes = Encoding.UTF8.GetString(byteBlock.Buffer, 0, byteBlock.Len);
            Log.Info($"接收到信息：{mes}");
        }

        public void SendMSG(string msg)
        {
            tcpClient?.Send(msg);
        }
    }


}
