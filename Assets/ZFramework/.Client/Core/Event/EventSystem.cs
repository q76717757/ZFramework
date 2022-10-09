using System.Collections.Generic;
using System;

namespace ZFramework
{
    public sealed class EventSystem
    {
        internal static EventSystem Instance => Game.instance.EventSystem;

        private readonly Dictionary<Type, List<IEvent>> allEvents = new Dictionary<Type, List<IEvent>>();//订阅发布模型的事件 //再分同步和异步事件 异步事件做取消

        internal void Load(Type[] allTypes)
        {
            allEvents.Clear();
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
        internal void Update()
        {
        }
        internal void LateUpdate()
        {
        }
        internal void Close()
        {

        }

        //订阅-发布模型
        public static void Call<T>(T eventArg)
        {
            if (!Instance.allEvents.TryGetValue(typeof(IEventCallback<T>), out List<IEvent> iEvents))
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
        public static async AsyncTask WaitAnyAsync<T>(T eventArg)
        {
        }
        public static async AsyncTask WaitAllAsync<T>(T eventArg)
        {
            if (!Instance.allEvents.TryGetValue(typeof(IEventCallbackAsync<T>), out List<IEvent> iEvents))
            {
                return;
            }
            foreach (IEventCallbackAsync<T> ev in iEvents)
            {
                if (ev == null)
                {
                    continue;
                }
                try
                {
                    await ev.Callback(eventArg);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }
        }



        //请求-响应模型
        public static T Get<T>()
        {
            return default;
        }
        public static T Get<T, A>(A a)
        {
            return default;
        }
        public static T Get<T, A, B>(A a, B b)
        {
            return default;
        }
        public static T Get<T, A, B, C>(A a, B b, C c)
        {
            return default;
        }
        public static async AsyncTask<T> GetAsync<T>()
        {
            return default;
        }
        public static async AsyncTask<T> GetAsync<T, A>(A a)
        {
            return default;
        }
        public static async AsyncTask<T> GetAsync<T, A, B>(A a, B b)
        {
            return default;
        }
        public static async AsyncTask<T> GetAsync<T, A, B, C>(A a, B b, C c)
        {
            return default;
        }

    }
}
