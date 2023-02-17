using System;
using System.Collections.Generic;

namespace ZFramework
{
    internal interface IEvent
    {
        Type EventType { get; }
    }

    //订阅发布模型
    internal interface IEventCallback<T> : IEvent
    {
        void Callback(T arg);
    }
    //internal interface IEventCallbackAsync<T> : IEvent
    //{
    //    AsyncTask Callback(T arg);
    //}

    //请求响应模型
    internal interface IEventReq<T> : IEvent
    {
        T GetValue();
    }
    internal interface IEventReq<T, A> : IEvent
    {
        T GetValue(A a);
    }
    //internal interface IEventReqAsync<T> : IEvent
    //{
    //    AsyncTask<T> GetValueAsync();
    //}
    //internal interface IEventReqAsync<T, A> : IEvent
    //{
    //    AsyncTask<T> GetValueAsync(A a);
    //}
    internal interface IEventReqAll<T> : IEvent
    {
        List<T> GetValues();
    }
    //internal interface IEventReqAllAsync<T> : IEvent
    //{
    //    AsyncTask<List<T>> GetValues();
    //}
}
