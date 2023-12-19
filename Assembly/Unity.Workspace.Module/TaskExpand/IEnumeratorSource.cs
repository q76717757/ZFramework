#if !SERVER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    internal class IEnumeratorSource : ITaskCompletionSource
    {
        IEnumerator enumerator;

        ushort ITaskCompletionSource.Ver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        internal IEnumeratorSource(IEnumerator enumerator)
        { 
            this.enumerator = enumerator;
        }
        void ITaskCompletionSource.TryStart()
        {
            enumerator.MoveNext();
        }
        TaskProcessStatus ITaskCompletionSource.GetStatus()
        {
            return enumerator.Current != null ? TaskProcessStatus.Running : TaskProcessStatus.Completion;
        }
        void ITaskCompletionSource.OnCompleted(Action continuation)
        {
            enumerator.MoveNext();
        }

        void ITaskCompletionSource.Break(Exception exception)
        {
            throw new NotImplementedException();
        }

        void ITaskCompletionSource.GetResultWithNotReturn()
        {
            throw new NotImplementedException();
        }

        Exception ITaskCompletionSource.GetException()
        {
            throw new NotImplementedException();
        }
    }

    public static partial class GetAwaiterExpansions
    {
        public static ATask GetAwaiter(this IEnumerator enumerator)
        {
            if (enumerator == null)
            {
                throw new ArgumentNullException(nameof(enumerator));
            }
            return ATask.FromSource(new IEnumeratorSource(enumerator));
        }
    }
}
#endif