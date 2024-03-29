using System.Collections.Generic;
using System;

namespace ZFramework
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class EventAttribute : BaseAttribute
    {

    }

    public sealed class EventSystem : Component
    {
        //事件映射表   订阅发布模型的事件 //再分同步和异步事件 异步事件做取消  
        private readonly static Dictionary<Type, List<IEvent>> allEvents = new Dictionary<Type, List<IEvent>>();
        private readonly static Dictionary<Type, List<IEvent>> allAsyncEvents = new Dictionary<Type, List<IEvent>>();//未实现..

        internal void Load()
        {
            allEvents.Clear();
            foreach (Type type in Game.GetTypesByAttribute<EventAttribute>())
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
        internal void Update()
        {
        }
        internal void LateUpdate()
        {
        }

        //订阅-发布模型
        public static void Call<T>(T eventArg)
        {
            if (!allEvents.TryGetValue(typeof(IEventCallback<T>), out List<IEvent> iEvents))
            {
                return;
            }
            foreach (IEventCallback<T> ev in iEvents)
            {
                if (ev == null)
                {
                    continue;
                }
                try
                {
                    ev.Callback(eventArg);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }
        }
        public static void CallNext() { }
        //public static async AsyncTask WaitAnyAsync<T>(T eventArg)
        //{
        //}
        //public static async AsyncTask WaitAllAsync<T>(T eventArg)
        //{
        //    if (!Game.instance.EventSystem.allEvents.TryGetValue(typeof(IEventCallbackAsync<T>), out List<IEvent> iEvents))
        //    {
        //        return;
        //    }
        //    foreach (IEventCallbackAsync<T> ev in iEvents)
        //    {
        //        if (ev == null)
        //        {
        //            continue;
        //        }
        //        try
        //        {
        //            await ev.Callback(eventArg);
        //        }
        //        catch (Exception e)
        //        {
        //            Log.Error(e.Message);
        //        }
        //    }
        //}



        //请求-响应模型
        public static T Get<T>()
        {
            return default;
        }
        public static T Get<T, A>(A a)
        {
            return default;
        }
        //public static async AsyncTask<T> GetAsyncAny<T>()
        //{
        //    return default;
        //}
        //public static async AsyncTask<T> GetAsyncAny<T, A>(A a)
        //{
        //    return default;
        //}
        //public static async AsyncTask<List<T>> GetAsyncAll<T>()
        //{
        //    return default;
        //}
        //public static async AsyncTask<List<T>> GetAsyncAll<T, A>(A a)
        //{
        //    return default;
        //}

    }
}
