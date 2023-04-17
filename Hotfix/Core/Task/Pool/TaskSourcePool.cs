using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    internal class TaskSourcePool<TSource>  where TSource : ITaskCompletionSource , new()
    {
        static Stack<TSource> pool = new Stack<TSource>();
        public TSource GetSource()
        {
            if (pool.Count > 0)
            {
                return pool.Pop();
            }
            else
            {
                return new TSource();
            }
        }
        public void Recycle(TSource source)
        {
        }
    }
}
