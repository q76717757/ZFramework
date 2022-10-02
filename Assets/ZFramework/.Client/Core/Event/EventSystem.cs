using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace ZFramework
{
    public sealed class EventSystem
    {
        internal static EventSystem Instance => Game.instance.EventSystem;

        private Dictionary<Type, List<IEvent>> allEvents = new Dictionary<Type, List<IEvent>>();//订阅发布模型的事件 //再分同步和异步事件 异步事件做取消
        private Dictionary<Type, List<IEvent>> allEvents2 = new Dictionary<Type, List<IEvent>>();//请求响应模型的事件 //再分同步和异步事件 异步事件做取消

        internal void Load(Type[] allTypes)
        {
            foreach (Type type in allTypes)
            {
                if (Activator.CreateInstance(type) is IEvent obj)
                {
                    Type eventType = obj.EventType;
                    if (!allEvents.ContainsKey(eventType))
                    {
                        allEvents.Add(eventType, new List<IEvent>());
                    }
                    allEvents[eventType].Add(obj);
                }
            }
        }
        internal void Reload()
        {

        }
        internal void Update()
        { 
        }
        internal void LateUpdate()
        { 
        }
        internal void Close()
        { 

        }



        //公开方法  事件系统写两套逻辑  基本能满足任何需求了

        //订阅-发布模型
        public static void Call<T>(T eventArg) where T : struct
        {
            if (!Instance.allEvents.TryGetValue(typeof(T), out List<IEvent> iEvents))
            {
                return;
            }
            foreach (EventCallback<T> ev in iEvents)
            {
                if (ev == null)
                {
                    continue;
                }
                try
                {
                    ev.Run(eventArg);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    throw;
                }
            }
        }
        public static void CallNext<T>(T eventArg) where T : struct
        {

        }
        public static async Task CallAsync<T>(T eventArg) where T : struct
        {

        }
        public static async Task CallNextAsync<T>(T eventArg) where T : struct
        { 

        }

        //请求-响应模型
        public static T GiveMe<T>()
        {
            if (Instance.allEvents2.TryGetValue(typeof(T), out List<IEvent> list))
            {
                return (list as IEvent2<T>).GetValue();
            }
            return default;
        }
        public static async Task<T> GiveMeAsync<T>()
        {
            if (Instance.allEvents2.TryGetValue(typeof(T), out List<IEvent> list))
            {
                return await (list as IEvent2<T>).GetValueAsync();
            }
            return default;
        }
    }
}
