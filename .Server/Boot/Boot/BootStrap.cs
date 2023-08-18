using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ZFramework
{
    internal static class BootStrap
    {
        static AssemblyLoader assemblyLoader;
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
                assemblyLoader= new AssemblyLoader();
                Type[] types = assemblyLoader.LoadAssembly(Defines.DefaultAssemblyNames);
                Type entryType = types.First(type => type.FullName == "ZFramework.Game");
                game = Activator.CreateInstance(entryType, true) as IGameInstance;
                game.Start(types);

                Log.Info("=================Success=================");

                ReadLine();//给守护组件 其他组件不用提供命令行控制
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
                game.Close();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
		}

        static async Task ReadLine()
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
