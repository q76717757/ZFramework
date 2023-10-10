using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouchSocket.Http;
using TouchSocket.Core.Config;
using TouchSocket.Http.Plugins;
using TouchSocket.Sockets;
using TouchSocket.Core;
using System.IO;
using System.Threading;

namespace ZFramework
{
    public class HTAwake : OnAwakeImpl<HttpTouch>
    {
        public override void OnAwake(HttpTouch self)
        {
            self.Init();
        }
    }
     
    public class HttpTouch : Component
    {
        HttpService httpService;
        HttpPlugn plugn;

        public void Init()
        {
            httpService = new HttpService();

            httpService.Setup(new TouchSocketConfig()//载入配置
              //.SetListenIPHosts(new IPHost[] { new IPHost($"{Game.InnerHost}:7887") })
              .SetMaxCount(10000)
              .SetThreadCount(10)
              ).Start();//启动

            plugn = new HttpPlugn();
            httpService.PluginsManager.Add(plugn);
            Log.Info("Start http");
        }
    }

    public class HttpPlugn : HttpPluginBase
    {
        protected override Task OnPostAsync(ITcpClientBase client, HttpContextEventArgs e)//文件上传服务
        {
            Log.Info("ID-->" + Thread.CurrentThread.ManagedThreadId);

            var url = e.Context.Request.URL;
            Log.Info("OnPostAsync: " + url);

            var d = new DirectoryInfo("./Files");
            if (!d.Exists)
            {
                d.Create();
            }
            var Request = e.Context.Request;//请求
            var Response = e.Context.Response;//响应

            if (Request.UrlEquals("/upload"))
            {
                var filepath = Request.GetHeader("filepath").Replace("/", "\\");
                if (filepath.HasValue())
                {
                    var fullpath = d.FullName + filepath;
                    Log.Info("上传文件到-->" + fullpath);
                    Log.Info(Request.ContentLength);
                    byte[] buffer;// = new byte[1024];
                    var readLen = 0;

                    int index = fullpath.LastIndexOf("\\");
                    string dirPath = fullpath.Substring(0, index);
                    //string filename = fullpath.Substring(index + 1);  //截取文件名

                    Log.Info("保存目录->" + dirPath);
                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }
                    Log.Info(Request.ContentLen);

                    if (Request.TryGetContent(out buffer))
                    {
                        using (FileStream fs = new FileStream(fullpath, FileMode.Create))
                        {
                            //while (true)
                            {
                                //Log.Info("BUFFERStart");
                                //int count = Request.Read(buffer, 0, buffer.Length);//1k读一下
                                //Log.Info("BUFFER长度是" + count);
                                fs.Write(buffer, 0, buffer.Length);
                                //readLen += count;

                                //if (count == 0)
                                //{
                                //    Log.Info("io完成" + readLen);
                                //    break;
                                //}
                            }
                        }

                    }




                    Response.FromText(filepath).Answer();
                    Response.Dispose();
                }
            }
            return Task.CompletedTask;
        }
        protected override Task OnGetAsync(ITcpClientBase client, HttpContextEventArgs e)//文件下载服务
        {
            var url = e.Context.Request.URL;
            Log.Info("OnGetAsync: " + url);

            var d = new DirectoryInfo("./Files");
            if (!d.Exists)
            {
                d.Create();
            }
            var Request = e.Context.Request;//请求
            var Response = e.Context.Response;//响应

            if (Request.UrlEquals("/download"))
            {
                var ps = Request.Query["file"];
                if (ps.HasValue())
                {
                    Log.Info("请求文件->" + ps);
                    string path = Path.Combine("./Files/" + ps);
                    if (File.Exists(path))
                    {
                        Log.Info("返回文件");
                        Response.FromFile(path, null);
                    }
                    else
                    {
                        Log.Info("路径错误");
                        Response.StatusCode = "400";
                        Response.StatusMessage = "路径错误";
                        Response.FromText("路径错误").Answer();
                    }
                }
                else
                {
                    Log.Info("参数错误");
                    Response.StatusCode = "400";
                    Response.StatusMessage = "参数错误";
                    Response.FromText("参数错误").Answer();// .Answer();
                }
            }
            return Task.CompletedTask;
        }

    }
}
