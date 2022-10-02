using System;
using System.Threading.Tasks;

namespace ZFramework
{
    [Event]
    public abstract class EventCallback<T> : IEvent
    {
        public Type EventType => typeof(T);
        public abstract void Run(T arg);

    }

    public abstract class EventCallback_Give<T> : IEvent2<T>
    {
        public Type EventType => typeof(T);

        public T GetValue() => GetValueCall();
        public abstract T GetValueCall();

        public Task<T> GetValueAsync() => GetValueAsyncA();
        public abstract Task<T> GetValueAsyncA();
    }
}
