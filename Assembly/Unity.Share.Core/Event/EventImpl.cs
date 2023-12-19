using System;

namespace ZFramework
{
    [Event]
    public abstract class EventCallback<T> : IEventCallback<T>
    {
        public Type EventType => typeof(IEventCallback<T>);
        void IEventCallback<T>.Callback(T arg) => Callback(arg);
        public abstract void Callback(T arg);
    }
    //[Event]
    //public abstract class EventCallbackAsync<T> : IEventCallbackAsync<T>
    //{
    //    public Type EventType => typeof(IEventCallbackAsync<T>);
    //    AsyncTask IEventCallbackAsync<T>.Callback(T arg) => Callback(arg);
    //    public abstract AsyncTask Callback(T agg);
    //}







    public abstract class OnEventReq<T> : IEventReq<T>
    {
        public Type EventType => typeof(IEventReq<T>);
        T IEventReq<T>.GetValue() => GetValue();
        public abstract T GetValue();
    }

}
