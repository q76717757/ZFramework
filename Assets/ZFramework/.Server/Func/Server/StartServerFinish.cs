using ZFramework.EventType;

namespace ZFramework
{
    public class Launcher : EventCallback<EventType.OnGameStart>
    {
        public override async void Callback(OnGameStart arg)
        {
            Log.Info("----------");
            await TestZT();
            Log.Info("++++++++++");
        }
        public async ZTask TestZT()
        {
            Log.Info("A");
            await TestZT2();
            Log.Info("B");
            await TestZT2();
            Log.Info("C");
        }
        public async ZTask TestZT2()
        {
            Log.Info("TestZT2  11111111111");
            //await Task.Delay(1000);//使用task就会把线程跳到子线程  就回不来主线程了
            Log.Info("TestZT2  2222222222");
            //await Task.Delay(1000);
            Log.Info("TestZT2  3333333333");
            Log.Info(await TestZTSSS());
        }
        public async ZTask<string> TestZTSSS()
        {
            Log.Info("TestZTSSS  1");
            //await Task.Delay(1000);
            Log.Info("TestZTSSS  2");
            //await Task.Delay(1000);
            Log.Info("TestZTSSS  3");
            return "123";
        }
    }
}
