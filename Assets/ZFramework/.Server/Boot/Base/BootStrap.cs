using System;
using System.Threading;

namespace ZFramework
{
    internal static class BootStrap
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

			//// 异步方法全部会回掉到主线程
			//SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);

			//// 命令行参数
			//Parser.Default.ParseArguments<Options>(args)
			//	.WithNotParsed(error => throw new Exception($"命令行格式错误! {error}"))
			//	.WithParsed(Game.AddSingleton);

			//Game.AddSingleton<CodeLoader>().Start();
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
