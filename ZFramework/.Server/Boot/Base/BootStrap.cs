using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ZFramework
{
    internal static class BootStrap
    {
        static IGameInstance game;
        static bool Running = true;

        static void Main(string[] args)
        {
            Log.ILog = new ConsoleLog();
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error(e.ExceptionObject.ToString());
            };

            try
            {
                List<Type> types = new List<Type>();  //server.boot依赖core+data
                types.AddRange(typeof(Game).Assembly.GetTypes());//core
                types.AddRange(typeof(EntityRoot).Assembly.GetTypes());//data  data依赖core
                types.AddRange(AssemblyLoader.LoadFunction().GetTypes());//func   func依赖data+core

                game = typeof(Game)
                    .GetMethod("CreateInstance", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, Array.Empty<object>()) as IGameInstance;
                game.Init(types.ToArray());

                Log.Info("=================Success=================");

                ReadLine().Invoke();//给守护组件 其他组件不用提供命令行控制
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    timeBeginPeriod(1);//时钟默认精度 win/15ms linux/1ms
                }


                while (Running)
                {
                    try
                    {
                        Thread.Sleep(1);
                        game.Update();
                        game.LateUpdate();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.StackTrace);
                    }
                }                                                                                                                                                                                                   
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            Console.ReadKey();
		}

        static async ATask ReadLine()
        {
            while (true)
            {
                string line = await Task.Factory.StartNew(() =>
                {
                    return Console.In.ReadLine();
                });

                if (line == "exit")//临时用一下 退出    **重载应该在一帧完整结束之后执行 
                {
                    Running = false;
                }
            }
        }

        [DllImport("winmm.dll")]
        private static extern void timeBeginPeriod(int t);//调整windows平台时钟精度
        //[DllImport("winmm.dll")]
        //static extern void timeEndPeriod(int t);
    }

}
