﻿using System;
using System.Threading;

namespace ZFramework
{
    internal static class BootStrap
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
			//AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
			//{
			//	Log.Error(e.ExceptionObject.ToString());
			//};

			//// 异步方法全部会回掉到主线程
			//SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);


			//// 命令行参数
			//Parser.Default.ParseArguments<Options>(args)
			//	.WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
			//	.WithParsed(Game.AddSingleton);

			//Game.AddSingleton<RandomGenerator>();
			//Game.AddSingleton<TimeInfo>();
			//Game.AddSingleton<Logger>().ILog = new NLogger(Options.Instance.AppType.ToString(), Options.Instance.Process, "../Config/NLog/NLog.config");
			//Game.AddSingleton<ObjectPool>();
			//Game.AddSingleton<IdGenerater>();
			//Game.AddSingleton<EventSystem>();
			//Game.AddSingleton<NetServices>();
			//Game.AddSingleton<Root>();

			//ETTask.ExceptionHandler += Log.Error;

			//Game.AddSingleton<CodeLoader>().Start();

			//Log.Console($"app start: {Game.Scene.Id} options: {JsonHelper.ToJson(Options.Instance)} ");

			//new AssemblyLoader().Start();

			while (true)
			{
				try
				{
					Thread.Sleep(1);
					//Game.Update();
					//Game.LateUpdate();
				}
				catch (Exception e)
				{
					//Log.Error(e);
				}
			}

		}
    }
}