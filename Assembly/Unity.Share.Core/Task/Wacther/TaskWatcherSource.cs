using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    internal class TaskWatcherSource : ITaskCompletionSource<WatcherResult>
    {
        ITaskCompletionSource targetSource;
        Action moveNext;//父任务的MoveNext

        private float time;
        public event TaskWatcher.ProcessCallback OnCountDown;//这个和倒计时配合用  没有开启倒计时 这个不调用
        public float TimeOut//把Cancel注册到中心计时器 通过中心计时器来发起取消
        {
            get
            {
                return time;
            }
            set
            { 
                time = value;
            }
        }

        ushort ITaskCompletionSource.Ver { get; set; }

        internal TaskWatcherSource(ITaskCompletionSource target)
        {
            this.targetSource = target;
        }
        void ITaskCompletionSource.TryStart()
        {
            //因为观察源Status等于目标源Status  所以当目标源已启动  就不会调观察源Start了  否则观察源将通过这个间接Start目标源
            //如果多个监听源看同一个任务  就存在多次启动的问题?
            targetSource.TryStart();
        }
        TaskProcessStatus ITaskCompletionSource.GetStatus()
        {
            return targetSource.GetStatus();
        }
        void ITaskCompletionSource.OnCompleted(Action continuation)
        {
            moveNext = continuation; //缓存状态机MoveNext
            //当目标源完成  会先回调到观察源  经过观察源中间商赚差价 然后间接Call MoveNext
            targetSource.OnCompleted(TargetCompleted);//目标源
        }
        WatcherResult ITaskCompletionSource<WatcherResult>.GetResult()
        {
            //当目标源完成会调TargetCompleted   TargetCompleted再调MoveNext  间接完成观察源
            //所以目标源必然是先完成了的  只要区分完成的情景是什么
            var ex = targetSource.GetException();
            if (ex == null)
            {
                //正常完成
                return new WatcherResult(TaskCompletionType.Success, null);
            }
            else
            {
                if (ex is OperationCanceledException)//手动取消
                {
                    return new WatcherResult(TaskCompletionType.Cancel, null);
                }
                if (ex is TimeoutException)//超时取消
                {
                    return new WatcherResult(TaskCompletionType.Timeout, null);
                }
                //异常取消
                return new WatcherResult(TaskCompletionType.Exception, ex);
            }
        }
        void TargetCompleted()
        {
            //目标源完成
            if (targetSource.GetException() != null)
            {
                Log.Error("目标完成,但有异常");
            }

            var moveNext = this.moveNext;
            this.moveNext = null;
            moveNext?.Invoke();
        }
        void ITaskCompletionSource.GetResultWithNotReturn() => (this as ITaskCompletionSource<WatcherResult>).GetResult();

        void ITaskCompletionSource.Break(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Cancel()//手动取消目标源
        {
            //取消命令直接将目标源强制完成 并且缓存一个超时异常  目标源完成的时候状态机下一状态会调GerResult 这个方法里面检查目标源异常
            //如果是取消异常 则抛出  这样外部就知道是被取消了
            //必须借用GetResult方法抛异常 这样才会被状态机内部的try捕获到

            targetSource.Break(new OperationCanceledException());
            //如何给子传递这个异常?  子的类型不确定 引用也没用
        }

        Exception ITaskCompletionSource.GetException()
        {
            throw new NotImplementedException();
        }
    }

    internal class TaskWatcherSource<TResult> : ITaskCompletionSource<WatcherResult<TResult>>
    {
        ITaskCompletionSource<TResult> targetSource;
        private TResult result;

        private float time;

        public event TaskWatcher.ProcessCallback OnCountDown;//这个和倒计时配合用  没有开启倒计时 这个不调用
        public float TimeOut//把Cancel注册到中心计时器 通过中心计时器来发起取消
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
            }
        }

        ushort ITaskCompletionSource.Ver { get; set; }

        internal TaskWatcherSource(ITaskCompletionSource<TResult> target)
        {
            this.targetSource = target;
        }

        void ITaskCompletionSource.TryStart()
        {
            throw new NotImplementedException();
        }
        TaskProcessStatus ITaskCompletionSource.GetStatus()
        {
            throw new NotImplementedException();
        }
        void ITaskCompletionSource.OnCompleted(Action continuation)
        {
            throw new NotImplementedException();
        }
        WatcherResult<TResult> ITaskCompletionSource<WatcherResult<TResult>>.GetResult()
        {
            throw new NotImplementedException();
        }
        void ITaskCompletionSource.GetResultWithNotReturn()
        {
            throw new NotImplementedException();
        }
        void ITaskCompletionSource.Break(Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {

        }

        Exception ITaskCompletionSource.GetException()
        {
            throw new NotImplementedException();
        }
    }
}
