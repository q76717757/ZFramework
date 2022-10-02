using System;
using System.Threading.Tasks;

namespace ZFramework
{
    public interface IEvent
    {
        Type EventType { get; }
    }

    public interface IEvent2<T> : IEvent
    {
        T GetValue();
        Task<T> GetValueAsync();
    }
}
