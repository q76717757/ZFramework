using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public interface IEvent
    {
        Type EventType { get; }
    }

    [Event]
    public abstract class EventCalblack<T> : IEvent where T : struct
    {
        public Type EventType => typeof(T);
        public abstract void Run(T arg = default);
    }

    public class EventComponent : Component
    {
        public Dictionary<Type, List<IEvent>> allEvents = new Dictionary<Type, List<IEvent>>();
    }
}
