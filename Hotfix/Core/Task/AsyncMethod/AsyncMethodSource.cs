using System;
using System.Runtime.CompilerServices;

namespace ZFramework
{
    //这是比较特殊的一类任务  能够被编译器编译成状态机  负责衔接多个任务单元  同时他自己也可以作为子任务源
    internal class AsyncMethodSource<TResult> : IAsyncMethodRunner<TResult>  //无返回的状态机就泛<VoidResult>
    {
        ATask<TResult> task;
        Exception ex;
        TaskProcessStatus state;
        TResult result;
        IAsyncStateMachine stateMachine;
        Action continuation;
        ushort ver;

        ushort ITaskCompletionSource.Ver { get => ver; set =>ver = value; }

        Exception ITaskCompletionSource.GetException() => ex;
        ATask<TResult> IAsyncMethodRunner<TResult>.GetTask()
        {
            return task;
        }
        void IAsyncMethodRunner<TResult>.SetStateMachine<TStateMachine>(in TStateMachine stateMachine)//同步调用  Async方法被调用的时候 只构造ATask 实际启动的方法在TCS接口的Start
        {
            this.stateMachine = stateMachine;
            this.task = ATask.FromSource(this);
        }
        void IAsyncMethodRunner<TResult>.MoveNext()
        {
            //增加一些步骤 判断到底状态机要不要往下走 取消的时候是不往下走的
            if (ex != null)
            {
                if (ex is OperationCanceledException)
                {
                    Log.Error("本任务被取消,放弃返回值并停止状态机"); //直接回收源  不可控的源(实际上不能取消的源)  返回的结果直接放弃 终止状态机
                    return;
                }
                if (ex is TimeoutException)
                {
                    Log.Error("本任务超时,放弃返回值并停止状态机");
                    return;
                }
                Log.Info("中断状态机" + ex);
            }
            else
            {
                stateMachine.MoveNext();
            }
        }
        void IAsyncMethodRunner<TResult>.SetException(Exception exception)
        {
            //这是本级捕获到的异常 如果这里 save ex 上级Get就会拿到  并不会走下级状态机

            ex = exception;
            if (ex is OperationCanceledException)//取消异常
            {
                Log.Error("取消异常");
            }
            if (ex is TimeoutException)//超时异常
            {
                Log.Error("超时异常");
            }
            //其他异常....
            Log.Error("其他异常" + exception);
        } 
        void IAsyncMethodRunner<TResult>.SetResult(TResult result)// 状态机最后一条执行的方法 try catch没有捕获到异常  才会走这个 
        {
            this.result = result;
            state = TaskProcessStatus.Completion;

            var continuation = this.continuation; //父任务的MoveNext
            this.continuation = null;
            continuation?.Invoke();
        }


        //作为任务源
        void ITaskCompletionSource.TryStart()
        {
            if (state == TaskProcessStatus.Created)
            {
                state = TaskProcessStatus.Running;
                stateMachine.MoveNext();
            }
        }
        void ITaskCompletionSource.OnCompleted(Action continuation)
        {
            this.continuation += continuation;
        }
        TaskProcessStatus ITaskCompletionSource.GetStatus()
        {
            return state;
        }
        TResult ITaskCompletionSource<TResult>.GetResult()
        {
            if (ex != null)
            {
                //这里抛异常 上级任务就会捕获到
                throw ex;
            }
            return result;
        }
        void ITaskCompletionSource.GetResultWithNotReturn() => (this as IAsyncMethodRunner<TResult>).GetResult();
        void ITaskCompletionSource.Break(Exception exception)
        {
            ex = exception;
            state = TaskProcessStatus.Completion;

            //提前完成任务..... 

            var temp =  this.continuation;
            continuation = null;
            temp?.Invoke();

            //上面是本机任务提前完成  抛弃结果

            //怎么传递给子任务 让子任务抛异常??

        }

       
    }

}
