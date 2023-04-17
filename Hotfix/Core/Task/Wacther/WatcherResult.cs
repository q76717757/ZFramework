using System;

namespace ZFramework
{
    public readonly struct WatcherResult
    {
        public readonly TaskCompletionType CompletionType { get; }
        public readonly Exception Exception { get; }
        internal WatcherResult(TaskCompletionType completionType,Exception exception )
        { 
            CompletionType = completionType;
            Exception = exception;
        }
    }

    public readonly struct WatcherResult<TResult>
    {
        public readonly TaskCompletionType CompletionType { get; }
        public readonly Exception Exception { get; }
        public readonly TResult Result { get; }
        internal WatcherResult(TaskCompletionType completionType, Exception exception, TResult result)
        { 
            CompletionType = completionType;
            Exception = exception;
            Result = result;
        }
    }
}
