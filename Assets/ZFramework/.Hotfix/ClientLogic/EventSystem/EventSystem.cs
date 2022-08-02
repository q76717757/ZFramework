using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    public class EventReload : ReloadSystem<EventComponent>
    {
        public override void OnReload(EventComponent component)
        {
            component.Reset();
        }
    }

    public class EventAwake : AwakeSystem<EventComponent>
    {
        public override void OnAwake(EventComponent component)
        {
            component.Reset();
        }
    }

    public static class EventSystem
    {
        public static void Call<T>(this EventComponent component, T eventArg) where T : struct
        {
            if (!component.allEvents.TryGetValue(typeof(T), out List<IEvent> iEvents))
            {
                return;
            }
            foreach (EventCalblack<T> ev in iEvents)
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

        public static void Reset(this EventComponent component) {
            var allEvents = component.allEvents;
            allEvents.Clear();
            foreach (Type useLifeTypes in Game.PlayLoop.GetTypesByAttribute(typeof(EventAttribute)))
            {
                IEvent obj = Activator.CreateInstance(useLifeTypes) as IEvent;
                if (obj != null)
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

    }

}
