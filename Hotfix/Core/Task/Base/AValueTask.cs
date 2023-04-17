using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    [StructLayout(LayoutKind.Auto)]
    public struct AValueTask : ICriticalNotifyCompletion
    {

        internal AValueTask(ITaskCompletionSource source) { }
        internal AValueTask(Exception exception) { }





        public AValueTask GetAwaiter() => this;


        public bool IsCompleted => true;
        public void GetResult() { }
        void INotifyCompletion.OnCompleted(Action continuation)
        {
        }
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
        }
    }


    [StructLayout(LayoutKind.Auto)]
    public struct AValueTask<TResult> : ICriticalNotifyCompletion
    {
        TResult result;


        internal AValueTask(ITaskCompletionSource source) { this = default; }
        internal AValueTask(Exception exception) { this = default; }
        internal AValueTask(TResult result) { this = default; }




        public AValueTask<TResult> GetAwaiter() => this;


        public bool IsCompleted => true;
        TResult GetResult() => default;
        void INotifyCompletion.OnCompleted(Action continuation)
        {
        }
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation)
        {
        }
    }
}
