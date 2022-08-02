using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{

    public class TimerAwake : AwakeSystem<TimerComponent>
    {
        public override void OnAwake(TimerComponent component)
        {
            component.Init();
        }
    }



    public static class TimerSystem
    {
        public static void Init(this TimerComponent component)
        {
            Log.Info("TimerAwake");
        }
    }
}
