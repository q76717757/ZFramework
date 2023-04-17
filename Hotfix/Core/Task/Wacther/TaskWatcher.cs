using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public struct TaskWatcher
    {
        public delegate void ProcessCallback(float time, float progress);

        private TaskWatcherSource source;
        private ushort ver;
        private float time;
        private ProcessCallback callback;

        /// <summary>
        /// 当前状态
        /// </summary>
        public WatcherStatus Status
        {
            get
            {
                if (source == null)
                {
                    return WatcherStatus.CanUse;
                }
                if (((ITaskCompletionSource<WatcherResult>)source).Ver == ver)
                {
                    return WatcherStatus.Using;
                }
                return WatcherStatus.Exp;
            }
        }
        /// <summary>
        /// 倒计时流逝事件 每帧调用一次 返回剩余时间和流逝进度
        /// </summary>
        public event ProcessCallback OnCountDown
        { 
            add
            {
                switch (Status)
                {
                    case WatcherStatus.CanUse:
                        callback += value;
                        break;
                    case WatcherStatus.Using:
                        source.OnCountDown += value;
                        break;
                    case WatcherStatus.Exp:
                        Log.Error("Wacther已过期");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            remove
            {
                switch (Status)
                {
                    case WatcherStatus.CanUse:
                        callback -= value;
                        break;
                    case WatcherStatus.Using:
                        source.OnCountDown -= value;
                        break;
                    case WatcherStatus.Exp:
                        Log.Error("Wacther已过期");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        /// <summary>
        /// 超时时间(秒) 默认0(永不超时),如果设置了大于0,则ATask在时间内没完成 就会自动取消 从任务启动开始计时
        /// </summary>
        public float TimeOut
        {
            get
            {
                switch (Status)
                {
                    case WatcherStatus.CanUse:
                        return time;
                    case WatcherStatus.Using:
                        return source.TimeOut;
                    case WatcherStatus.Exp:
                        Log.Error("Watcher已过期");
                        return -1f;
                    default:
                        throw new NotImplementedException();
                }
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("TaskWatcher的超时时间不能小于0");
                }
                switch (Status)
                {
                    case WatcherStatus.CanUse:
                        time = value;
                        break;
                    case WatcherStatus.Using:
                        source.TimeOut = value;
                        break;
                    case WatcherStatus.Exp:
                        Log.Error("Watcher已过期,本次调用将被忽略");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// 手动取消   绑定的此Wacther的ATask会马上终结
        /// </summary>
        public void Cancel()
        {
            switch (Status)
            {
                case WatcherStatus.CanUse:
                    Log.Error("Watcher未使用 此次调用被忽略");
                    break;
                case WatcherStatus.Using:
                    source.Cancel();
                    break;
                case WatcherStatus.Exp:
                    Log.Error("Watcher过期 此次调用被忽略");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        internal void SetTarget(TaskWatcherSource source)
        {
            this.source = source;
            ver = (source as ITaskCompletionSource).Ver;
        }
    }
    public struct TaskWatcher<TResult>
    {
        private TaskWatcherSource<TResult> source;
        private ushort ver;
        private float time;
        private TaskWatcher.ProcessCallback callback;

        public WatcherStatus Status
        {
            get
            {
                if (source == null)
                {
                    return WatcherStatus.CanUse;
                }
                if (((ITaskCompletionSource<WatcherResult>)source).Ver == ver)
                {
                    return WatcherStatus.Using;
                }
                return WatcherStatus.Exp;
            }
        }
        public event TaskWatcher.ProcessCallback OnCountDown
        {
            add
            {
                switch (Status)
                {
                    case WatcherStatus.CanUse:
                        callback += value;
                        break;
                    case WatcherStatus.Using:
                        source.OnCountDown += value;
                        break;
                    case WatcherStatus.Exp:
                        Log.Error("Wacther已过期");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            remove
            {
                switch (Status)
                {
                    case WatcherStatus.CanUse:
                        callback -= value;
                        break;
                    case WatcherStatus.Using:
                        source.OnCountDown -= value;
                        break;
                    case WatcherStatus.Exp:
                        Log.Error("Wacther已过期");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        public float TimeOut
        {
            get
            {
                switch (Status)
                {
                    case WatcherStatus.CanUse:
                        return time;
                    case WatcherStatus.Using:
                        return source.TimeOut;
                    case WatcherStatus.Exp:
                        Log.Error("Watcher已过期");
                        return -1f;
                    default:
                        throw new NotImplementedException();
                }
            }
            set
            {
                if (value < 0)
                {
                    throw new Exception("TaskWatcher的超时时间不能小于0");
                }
                switch (Status)
                {
                    case WatcherStatus.CanUse:
                        time = value;
                        break;
                    case WatcherStatus.Using:
                        source.TimeOut = value;
                        break;
                    case WatcherStatus.Exp:
                        Log.Error("Watcher已过期,本次调用将被忽略");
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public void Cancel()
        {
            switch (Status)
            {
                case WatcherStatus.CanUse:
                    Log.Error("Watcher未使用 此次调用被忽略");
                    break;
                case WatcherStatus.Using:
                    source.Cancel();
                    break;
                case WatcherStatus.Exp:
                    Log.Error("Watcher过期 此次调用被忽略");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        internal void SetTarget(TaskWatcherSource<TResult> source)
        {
            this.source = source;
            ver = (source as ITaskCompletionSource).Ver;
        }
    }


    public partial struct ATask
    {
        public ATask<WatcherResult> SetWatcher(out TaskWatcher watcher)
        {
            watcher = new TaskWatcher();

            if (IsCompleted) //空任务或者已终结的任务  (不一定是完成终结 可能是异常终结或者取消终结 但肯定终结了)
            {
                Log.Error("已完成任务,忽略监听直接返回结果");//源过期了 this是只读结构体又没有缓存结果  这个result一直是default...
                var ex = source.GetException();
                if (ex != null)
                {
                    if (ex is TimeoutException)
                    {
                        return ATask.FromResult(new WatcherResult(TaskCompletionType.Timeout, null));
                    }
                    if (ex is CanceledException)
                    {
                        return ATask.FromResult(new WatcherResult(TaskCompletionType.Cancel, null));
                    }
                    return ATask.FromResult(new WatcherResult(TaskCompletionType.Exception, ex));
                }
                else
                {
                    return ATask.FromResult(new WatcherResult(TaskCompletionType.Success, null));
                }
            }
            else //新任务 或者 执行中任务  支持多watcher对一task?? 还是一watcher对多task??
            {
                var target = new TaskWatcherSource(source);//池化
                watcher.SetTarget(target);
                return ATask.FromSource(target); //正式建立连接
            }
        }
    }
    public partial struct ATask<TResult>
    {
        public ATask<WatcherResult<TResult>> SetWatcher(out TaskWatcher<TResult> watcher)
        {
            //TestCancel().SetWatcher(watcher).SetWatcher(watcher).Invoke(); //错误的套娃用法  如何处理???

            watcher = new TaskWatcher<TResult>();
            if (IsCompleted)
            {
                var ex = source.GetException();

                if (ex != null)
                {
                    if (ex is TimeoutException)
                    {
                        return ATask.FromResult(new WatcherResult<TResult>(TaskCompletionType.Timeout, null, default));
                    }
                    if (ex is CanceledException)
                    {
                        return ATask.FromResult(new WatcherResult<TResult>(TaskCompletionType.Cancel, null, default));
                    }
                    return ATask.FromResult(new WatcherResult<TResult>(TaskCompletionType.Exception, ex, default));
                }
                else
                {
                    //源过期了 又没有缓存  这个result一直是default... 除非task不是只读并且源完成要把结果返回来缓存
                    return ATask.FromResult(new WatcherResult<TResult>(TaskCompletionType.Success, null, result));
                }
            }
            else
            {
                TaskWatcherSource<TResult> target = new TaskWatcherSource<TResult>(source);
                watcher.SetTarget(target);
                return ATask.FromSource(target);
            }
        }
    }
}
