using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ZFramework
{
    internal static class BootStrap
    {
        static IGameInstance game;
        static void Main(string[] args)
        {
            Log.ILog = new ConsoleLog();
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Error(e.ExceptionObject.ToString());
            };

            try
            {
                //// 异步方法全部会回掉到主线程
                //SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
                //// 命令行参数
                //Parser.Default.ParseArguments<Options>(args)
                //    .WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
                //    .WithParsed(Game.AddSingleton);

                List<Type> types = new List<Type>();  //server.boot依赖core+data
                types.AddRange(typeof(Game).Assembly.GetTypes());//core
                types.AddRange(typeof(EntityRoot).Assembly.GetTypes());//data  data依赖core
                types.AddRange(AssemblyLoader.LoadFunction().GetTypes());//func   func依赖data+core

                game = typeof(Game)
                    .GetMethod("CreateInstance", BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(null, Array.Empty<object>()) as IGameInstance;
                game.Start(types.ToArray());

                //ReadLine().Coroutine();//移到守护进程上 让守护进程接受命令输入
                Log.Info("=================Success=================");
                while (true)
                {
                    try
                    {
                        Thread.Sleep(10);
                        game.Update();
                        game.LateUpdate();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
		}

        static async AsyncVoid ReadLine()
        {
            while (true)
            {
                string line = await Task.Factory.StartNew(() =>
                {
                    return Console.In.ReadLine();
                });
                Log.Info("输入->" + line);
            }
        }
    }
}
