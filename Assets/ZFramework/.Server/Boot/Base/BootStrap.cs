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
                game.Start(types.ToArray());

                Log.Info("=================Success=================");

                ReadLine().Coroutine();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    timeBeginPeriod(1);
                }
                while (true)
                {
                    try
                    {
                        Thread.Sleep(1);
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
                Log.Info("->" + line);
            }
        }


        [DllImport("winmm.dll")]
        private static extern void timeBeginPeriod(int t);//调整windows平台时钟精度
        //[DllImport("winmm.dll")]
        //static extern void timeEndPeriod(int t);
    }

}
