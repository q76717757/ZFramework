using System.Collections.Generic;
using ZFramework.EventType;

namespace ZFramework
{
    public class Launcher : EventCallback<EventType.OnGameStart>
    {
        public override async void Callback(OnGameStart arg)
        {
            //Log.Info("----------");
            //await TestZT();
            //Log.Info("++++++++++");
            //try
            //{
            //    AA();//.A();//用asyncvoid可以正确捕获到方法内的异常  但是编译器会包绿   在里面写个空方法调用一下编译器就不会包绿 实际没有什么区别
            //}
            //catch (System.Exception e)
            //{
            //    Log.Info("121212" + e.Message);
            //    //throw;
            //}

            var a = await TaskGroup.WaitAny(new AsyncTask[] {
                TestZT(),
                TestZT2()
            });

            Log.Info("CCCCCCCCCCCCCC");
        }
        public async AsyncTask TestZT()
        {
            Log.Info("A");
            await TestZT2();
            Log.Info("B");
            await TestZT2();
            Log.Info("C");
        }
        public async AsyncTask TestZT2()
        {
            Log.Info("TestZT2  11111111111");
            //await Task.Delay(1000);//使用task就会把线程跳到子线程  就回不来主线程了
            Log.Info("TestZT2  2222222222");
            //await Task.Delay(1000);
            Log.Info("TestZT2  3333333333");

            //Log.Info(await TestZTSSS());
        }
        public async AsyncTask<string> TestZTSSS()
        {
            //throw new System.Exception("TestEXC");
            Log.Info("TestZTSSS  1");
            //await Task.Delay(1000);
            Log.Info("TestZTSSS  2");
            //await Task.Delay(1000);
            Log.Info("TestZTSSS  3");
            return "123";
        }

        public async void AA()//如果是async void 方法内抛出了一场  将会捕获不到引起崩溃 用asyncvoid包装就可以正常拿到异常了 用全局的未处理事件捕获也可以,但是捕获到上下文已经跑偏了 没法处理异常
        {
          
            var s = TestZTSSS();
            var ss = await s;
            Log.Info(ss + "1111111");
           
        }
    }
}
