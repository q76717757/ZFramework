using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using ZFramework;

namespace ZFramework
{
    public class HTTPAwake : OnAwakeImpl<HttpServer>//用System.Net.HttpListener 简单实现一下http监听接口
    {
        public override void OnAwake(HttpServer self)
        {
            HttpServer httpServer = new HttpServer();

            httpServer.AddPrefixes("/test", (value) =>
            {
                Log.Info("test" + value);
            });

            httpServer.AddPrefixes("/vac", (value) =>
            {
                Log.Info("vac" + value);
            });
            httpServer.Start(8080);
        }
    }

    public class HttpServer : Component
    {
        private HttpListener listener = new HttpListener();
        private Dictionary<string, Action<string>> actionDict = new Dictionary<string, Action<string>>();
        public void AddPrefixes(string url, Action<string> action)
        {
            actionDict.Add(url, action);
        }

        public void Start(int port)
        {
            if (!HttpListener.IsSupported)
            {
                Log.Info("无法在当前系统上运行服务(Windows XP SP2 or Server 2003)。" + DateTime.Now.ToString());
                return;
            }

            if (actionDict.Count <= 0)
            {
                Log.Info("没有监听端口");
                return;
            }

            foreach (var item in actionDict)
            {
                var url = string.Format("http://+:{0}{1}", port, item.Key + "/");
                listener.Prefixes.Add(url);  //监听的是以item.Key + "/"+XXX接口
                Log.Info(url);
            }

            listener.Start();
            listener.BeginGetContext(Result, null);

            Log.Info("开始监听");
        }
        private void Result(IAsyncResult asy)
        {
            //listener.BeginGetContext(Result, null);

            var context = listener.EndGetContext(asy);
            var req = context.Request;
            var rsp = context.Response;

            // 
            rsp.StatusCode = 200;
            rsp.ContentType = "text/plain;charset=UTF-8";//告诉客户端返回的ContentType类型为纯文本格式，编码为UTF-8
            rsp.AddHeader("Content-type", "text/plain");//添加响应头信息
            rsp.ContentEncoding = Encoding.UTF8;

            //对接口所传数据处理
            HandleHttpMethod(context);
            //对接口处理
            HandlerReq(req.RawUrl);
            try
            {
                using (var stream = rsp.OutputStream)
                {
                    ///获取数据，要返回给接口的
                    string data = System.DateTime.Now.ToString();
                    byte[] dataByte = Encoding.UTF8.GetBytes(data);
                    stream.Write(dataByte, 0, data.Length);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            rsp.Close();

        }

        /// <summary>
        /// 对客户端来的url处理
        /// </summary>
        /// <param name="url"></param>
        private void HandlerReq(string url)
        {
            try
            {
                Log.Info("url : " + url);

                string subUrl = url.Substring(1);
                int urlIndex = subUrl.IndexOf("/");
                subUrl = url.Substring(0, urlIndex + 1);
                Log.Info(subUrl);
                var action = actionDict[subUrl];
                action(url);
            }
            catch (Exception e)
            {
                Log.Info(e);
            }
        }
        //处理接口所传数据 Post和Get
        private void HandleHttpMethod(HttpListenerContext context)
        {
            if (context.Request.ContentType != "text/plain")
            {
                Log.Info("参数格式错误");
                return;
            }
            switch (context.Request.HttpMethod)
            {
                case "POST":

                    string contextLength = context.Request.Headers.GetValues("Content-Length")[0];

                    using (Stream stream = context.Request.InputStream)
                    {
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string content = reader.ReadToEnd();
                        Log.Info(content);
                        int length = content.Length;
                        ///解析  接口所传数据
                    }
                    break;
                case "GET":
                    var data = context.Request.QueryString;
                    break;
            }
        }

        //Call---------->

        private static string data;
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        public string GetMyData()
        {
            new Task(() =>
            {
                Invoke();
            }).Start();
            Log.Info("我是主线程");

            if (data == null) return null;
            return data;
        }

        public static async Task<string> Invoke()
        {
            var result = GetPostLoad();
            data = await result;  //等待返回
            Log.Info(data);  //输出返回
            return data;
        }
        public static async Task<string> GetPostLoad()
        {
            HttpWebRequest request = WebRequest.Create("http:/*****************") as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            string data = "userId=123&agentId=456&companyId=789&versionTime=165665430918";
            byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);
            request.ContentLength = buf.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(buf, 0, buf.Length);
            newStream.Close();
            HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
            string result = reader.ReadToEnd();
            return result;
        }
    }
}




namespace A
{
    internal class HttpServer
    {
        private IPEndPoint _IP;
        private TcpListener _Listeners;
        private volatile bool IsInit = false;
        HashSet<string> Names;

        /// <summary>
        /// 初始化服务器
        /// </summary>
        public HttpServer(string ip, int port, HashSet<string> names)
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Parse(ip), port);
            this._IP = localEP;
            Names = names;
            if (Names == null)
            {
                Names = new HashSet<string>();
            }
            try
            {
                foreach (var item in names)
                {
                    Log.Info(string.Format(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff：") + "Start Listen Http Socket -> {0}:{1}{2} ", ip, port, item));
                }
                this._Listeners = new TcpListener(IPAddress.Parse(ip), port);
                this._Listeners.Start(5000);
                IsInit = true;
                this.AcceptAsync();
            }
            catch (Exception ex)
            {
                Log.Info(ex);
                this.Dispose();
            }
        }

        private void AcceptAsync()
        {
            try
            {
                this._Listeners.BeginAcceptTcpClient(new AsyncCallback(AcceptAsync_Async), null);
            }
            catch (Exception) { }
        }

        private void AcceptAsync_Async(IAsyncResult iar)
        {
            this.AcceptAsync();
            try
            {
                TcpClient client = this._Listeners.EndAcceptTcpClient(iar);
                var socket = new HttpClient(client);
                Log.Info(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff：") + "Create Http Socket Remote Socket LocalEndPoint：" + client.Client.LocalEndPoint + " RemoteEndPoint：" + client.Client.RemoteEndPoint.ToString());
                foreach (var item in Names)
                {
                    if (socket.http_url.StartsWith(item))
                    {
                        try
                        {
                            socket.process();
                            return;
                        }
                        catch { break; }
                    }
                }
                socket.WriteFailure();
                socket.Close();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (IsInit)
            {
                IsInit = false;
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 释放所占用的资源
        /// </summary>
        /// <param name="flag1"></param>
        protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool flag1)
        {
            if (flag1)
            {
                if (_Listeners != null)
                {
                    try
                    {
                        Log.Info(string.Format("Stop Http Listener -> {0}:{1} ", this.IP.Address.ToString(), this.IP.Port));
                        _Listeners.Stop();
                        _Listeners = null;
                    }
                    catch { }
                }
            }
        }

        /// <summary>
        /// 获取绑定终结点
        /// </summary>
        public IPEndPoint IP { get { return this._IP; } }
    }
    public class HttpClient
    {
        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB
        private const int BUF_SIZE = 4096;
        private Stream inputStream;
        public StreamWriter OutputStream;
        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();
        internal TcpClient _Socket;

        /// <summary>
        /// 这个是服务器收到有效链接初始化
        /// </summary>
        internal HttpClient(TcpClient client)
        {
            this._Socket = client;
            inputStream = new BufferedStream(_Socket.GetStream());
            OutputStream = new StreamWriter(new BufferedStream(_Socket.GetStream()), UTF8Encoding.Default);
            ParseRequest();
        }

        internal void process()
        {
            try
            {
                if (http_method.Equals("GET"))
                {
                    HTTPAPP.Pool.ActiveHttp(this, GetRequestExec());
                }
                else if (http_method.Equals("POST"))
                {
                    HTTPAPP.Pool.ActiveHttp(this, PostRequestExec());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                WriteFailure();
            }
        }

        public void Close()
        {
            OutputStream.Flush();
            inputStream.Dispose();
            inputStream = null;
            OutputStream.Dispose();
            OutputStream = null; // bs = null;            
            this._Socket.Close();
        }


        #region 读取流的一行 private string ReadLine()
        /// <summary>
        /// 读取流的一行
        /// </summary>
        /// <returns></returns>
        private string ReadLine()
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = this.inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }

        #endregion


        #region 转化出 Request private void ParseRequest()
        /// <summary>
        /// 转化出 Request
        /// </summary>
        private void ParseRequest()
        {
            string request = ReadLine();
            if (request != null)
            {
                string[] tokens = request.Split(' ');
                if (tokens.Length != 3)
                {
                    throw new Exception("invalid http request line");
                }
                http_method = tokens[0].ToUpper();
                http_url = tokens[1];
                http_protocol_versionstring = tokens[2];
            }
            String line;
            while ((line = ReadLine()) != null)
            {
                if (line.Equals(""))
                {
                    break;
                }
                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++;//过滤键值对的空格
                }
                string value = line.Substring(pos, line.Length - pos);
                httpHeaders[name] = value;
            }
        }

        #endregion


        #region 读取Get数据 private Dictionary<string, string> GetRequestExec()
        /// <summary>
        /// 读取Get数据
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetRequestExec()
        {
            Dictionary<string, string> datas = new Dictionary<string, string>();
            int index = http_url.IndexOf("?", 0);
            if (index >= 0)
            {
                string data = http_url.Substring(index + 1);
                datas = getData(data);
            }
            WriteSuccess();
            return datas;
        }

        #endregion


        #region 读取提交的数据 private void handlePOSTRequest()
        /// <summary>
        /// 读取提交的数据
        /// </summary>
        private Dictionary<string, string> PostRequestExec()
        {
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                //内容的长度
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE) { throw new Exception(String.Format("POST Content-Length({0}) 对于这个简单的服务器太大", content_len)); }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {
                    int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    if (numread == 0)
                    {
                        if (to_read == 0) { break; }
                        else { throw new Exception("client disconnected during post"); }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            WriteSuccess();
            StreamReader inputData = new StreamReader(ms);
            string data = inputData.ReadToEnd();
            return getData(data);
        }

        #endregion


        #region 输出状态
        /// <summary>
        /// 输出200状态
        /// </summary>
        public void WriteSuccess()
        {
            OutputStream.WriteLine("HTTP/1.0 200 OK");
            OutputStream.WriteLine("Content-Type: text/html");
            OutputStream.WriteLine("Connection: close");
            OutputStream.WriteLine("");
        }

        /// <summary>
        /// 输出状态404
        /// </summary>
        public void WriteFailure()
        {
            OutputStream.WriteLine("HTTP/1.0 404 File not found");
            OutputStream.WriteLine("Content-Type: text/html");
            OutputStream.WriteLine("Connection: close");
            OutputStream.WriteLine("");
        }

        #endregion

        /// <summary>
        /// 分析http提交数据分割
        /// </summary>
        /// <param name="rawData"></param>
        /// <returns></returns>
        private static Dictionary<string, string> getData(string rawData)
        {
            var rets = new Dictionary<string, string>();
            string[] rawParams = rawData.Split('&');
            foreach (string param in rawParams)
            {
                string[] kvPair = param.Split('=');
                string key = kvPair[0];
                string value = HttpUtility.UrlDecode(kvPair[1]);
                rets[key] = value;
            }
            return rets;
        }
    }
    public class HTTPAPP
    {
        public static MessagePool Pool = new MessagePool();
        void Start()
        {
            HttpServer https = new HttpServer("127.0.0.1", 80, new HashSet<string>() { "/test/", "/flie/" });
        }
    }
    public class MessagePool : ISocketPool
    {
        public void ActiveHttp(HttpClient client, Dictionary<string, string> parms)
        {
            Thread.Sleep(new Random().Next(0, 3000));
            foreach (var item in parms)
            {
                Console.WriteLine(DateTime.Now.ToString() + "item.Key：" + item.Key + "； item.Value：" + item.Value);
            }
            string strHtml = @"
 <html><head></head>
 <body>
 <div>&nbsp;</div>
 <div>&nbsp;</div>
 <div>&nbsp;</div>
 <div>&nbsp;</div>
 <div>&nbsp;</div>
 {0}
 </body>
 </html>
 ";
            client.OutputStream.WriteLine(string.Format(strHtml, DateTime.Now.ToString() + "xxxxxxxxxxx"));
            client.Close();
        }
    }
    public interface ISocketPool
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        void ActiveHttp(HttpClient client, Dictionary<string, string> parms);
    }
}