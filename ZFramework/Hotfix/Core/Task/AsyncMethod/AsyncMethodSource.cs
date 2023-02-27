using System;

namespace ZFramework
{
    //这是比较特殊的一类任务   能够被编译器编译成状态机  负责衔接多个任务单元 (没写泛型 这是个容器 装有状态机的MoveNext)
    internal class AsyncMethodSource : ITaskCompletionSource
    {
        ATask task;
        //起点任务或者子任务共用的字段 Invoke时是Self.Movenext  Await时是Parent.MoveNext 也就是Invoke和OnCompleted不会同时被调用
        Action moveNext;
        ATaskStatus state;

        public ATask Task => task;//builder.Task
        public void Start(Action moveNext)//builder.Start
        {
            this.moveNext = moveNext;
            this.task = new ATask(this);
        }
        public void SetResult()//builder.SetResult
        {
            state = ATaskStatus.Success;
            var movenext = moveNext;//Parent.MoveNext  从OnCompled赋值
            moveNext = null;
            movenext?.Invoke();
        }
        public void SetException(Exception exception) //builder.SetException  Task里面的错误会从这里出来 并且中断状态机的执行
        {
            Log.Error(exception);
        }

        void ITaskCompletionSource.Invoke()
        {
            if (moveNext == null)
            {

            }
            else
            {
                if (state == ATaskStatus.Created)
                {
                    state = ATaskStatus.Running;
                    var movenext = moveNext;//Self.MoveNext 从Start赋值
                    moveNext = null;
                    movenext?.Invoke();
                }
            }
        }
        void ITaskCompletionSource.OnCompleted(Action continuation)
        {
            moveNext = continuation;
        }
        ATaskStatus ITaskCompletionSource.GetStatus()
        {
            return state;
        }
    }

    internal class AsyncMethodSource<TResult> : ITaskCompletionSource<TResult>
    {
        ATaskStatus state;
        TResult result;
        ATask<TResult> task;
        Action moveNext;

        public ATask<TResult> Task => task;
        public void Init(Action moveNext)
        {
            this.moveNext = moveNext;
            this.task = new ATask<TResult>(this);
        }
        public void SetResult(TResult result)
        {
            state = ATaskStatus.Success;
            this.result = result;
            var movenext = moveNext;
            moveNext = null;
            movenext?.Invoke();
        }
        public void SetException(Exception exception)
        {
            Log.Error(exception);
        }

        void ITaskCompletionSource.Invoke()
        {
            if (moveNext == null)
            {

            }
            else
            {
                if (state == ATaskStatus.Created)
                {
                    state = ATaskStatus.Running;
                    var movenext = moveNext;//Self.MoveNext 从Start赋值
                    moveNext = null;
                    movenext?.Invoke();
                }
            }
        }
        void ITaskCompletionSource.OnCompleted(Action continuation)
        {
            moveNext = continuation;
        }
        ATaskStatus ITaskCompletionSource.GetStatus()
        {
            return state;
        }
        TResult ITaskCompletionSource<TResult>.GetResult()
        {
            return result;
        }
    }

}
