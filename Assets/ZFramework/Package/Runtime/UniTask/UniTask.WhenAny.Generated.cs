#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using Cysharp.Threading.Tasks.Internal;

namespace Cysharp.Threading.Tasks
{
    public partial struct UniTask
    {
        public static UniTask<(int winArgumentIndex, T1 result1, T2 result2)> WhenAny<T1, T2>(UniTask<T1> task1, UniTask<T2> task2)
        {
            return new UniTask<(int winArgumentIndex, T1 result1, T2 result2)>(new WhenAnyPromise<T1, T2>(task1, task2), 0);
        }

        sealed class WhenAnyPromise<T1, T2> : IUniTaskSource<(int, T1 result1, T2 result2)>
        {
            int completedCount;
            UniTaskCompletionSourceCore<(int, T1 result1, T2 result2)> core;

            public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();

                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAnyPromise<T1, T2>, UniTask<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();

                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAnyPromise<T1, T2>, UniTask<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAnyPromise<T1, T2> self, in UniTask<T1>.Awaiter awaiter)
            {
                T1 result;
                try
                {
                    result = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }

                if (Interlocked.Increment(ref self.completedCount) == 1)
                {
                    self.core.TrySetResult((0, result, default));
                }
            }

            static void TryInvokeContinuationT2(WhenAnyPromise<T1, T2> self, in UniTask<T2>.Awaiter awaiter)
            {
                T2 result;
                try
                {
                    result = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }

                if (Interlocked.Increment(ref self.completedCount) == 1)
                {
                    self.core.TrySetResult((1, default, result));
                }
            }
            
            public (int, T1 result1, T2 result2) GetResult(short token)
            {
                TaskTracker.RemoveTracking(this);
                GC.SuppressFinalize(this);
                return core.GetResult(token);
            }
            
            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }
        }

        public static UniTask<(int winArgumentIndex, T1 result1, T2 result2, T3 result3)> WhenAny<T1, T2, T3>(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3)
        {
            return new UniTask<(int winArgumentIndex, T1 result1, T2 result2, T3 result3)>(new WhenAnyPromise<T1, T2, T3>(task1, task2, task3), 0);
        }

        sealed class WhenAnyPromise<T1, T2, T3> : IUniTaskSource<(int, T1 result1, T2 result2, T3 result3)>
        {
            int completedCount;
            UniTaskCompletionSourceCore<(int, T1 result1, T2 result2, T3 result3)> core;

            public WhenAnyPromise(UniTask<T1> task1, UniTask<T2> task2, UniTask<T3> task3)
            {
                TaskTracker.TrackActiveTask(this, 3);

                this.completedCount = 0;
                {
                    var awaiter = task1.GetAwaiter();

                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT1(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAnyPromise<T1, T2, T3>, UniTask<T1>.Awaiter>)state)
                            {
                                TryInvokeContinuationT1(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task2.GetAwaiter();

                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT2(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAnyPromise<T1, T2, T3>, UniTask<T2>.Awaiter>)state)
                            {
                                TryInvokeContinuationT2(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
                {
                    var awaiter = task3.GetAwaiter();

                    if (awaiter.IsCompleted)
                    {
                        TryInvokeContinuationT3(this, awaiter);
                    }
                    else
                    {
                        awaiter.SourceOnCompleted(state =>
                        {
                            using (var t = (StateTuple<WhenAnyPromise<T1, T2, T3>, UniTask<T3>.Awaiter>)state)
                            {
                                TryInvokeContinuationT3(t.Item1, t.Item2);
                            }
                        }, StateTuple.Create(this, awaiter));
                    }
                }
            }

            static void TryInvokeContinuationT1(WhenAnyPromise<T1, T2, T3> self, in UniTask<T1>.Awaiter awaiter)
            {
                T1 result;
                try
                {
                    result = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }

                if (Interlocked.Increment(ref self.completedCount) == 1)
                {
                    self.core.TrySetResult((0, result, default, default));
                }
            }

            static void TryInvokeContinuationT2(WhenAnyPromise<T1, T2, T3> self, in UniTask<T2>.Awaiter awaiter)
            {
                T2 result;
                try
                {
                    result = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }

                if (Interlocked.Increment(ref self.completedCount) == 1)
                {
                    self.core.TrySetResult((1, default, result, default));
                }
            }

            static void TryInvokeContinuationT3(WhenAnyPromise<T1, T2, T3> self, in UniTask<T3>.Awaiter awaiter)
            {
                T3 result;
                try
                {
                    result = awaiter.GetResult();
                }
                catch (Exception ex)
                {
                    self.core.TrySetException(ex);
                    return;
                }

                if (Interlocked.Increment(ref self.completedCount) == 1)
                {
                    self.core.TrySetResult((2, default, default, result));
                }
            }


            public (int, T1 result1, T2 result2, T3 result3) GetResult(short token)
            {
                TaskTracker.RemoveTracking(this);
                GC.SuppressFinalize(this);
                return core.GetResult(token);
            }

            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            void IUniTaskSource.GetResult(short token)
            {
                GetResult(token);
            }
        }

    }
}