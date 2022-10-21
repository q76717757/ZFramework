using System;
using System.Collections.Generic;

namespace ZFramework
{
    public class TaskGroup
    {
        private int count;

        private List<AsyncTask> tcss = new List<AsyncTask>();
        public TaskGroup(int count)
        {
            this.count = count;
        }
        public async AsyncTask FinishOne()
        {
            --this.count;
            if (this.count < 0)
            {
                return;
            }
            if (this.count == 0)
            {
                List<AsyncTask> t = this.tcss;
                this.tcss = null;
                foreach (AsyncTask ttcs in t)
                {
                    ttcs.SetResult();
                }

                return;
            }
            AsyncTask tcs = AsyncTask.CreateInstance();
            tcss.Add(tcs);
            await tcs;
        }


        public static async AsyncTask<bool> WaitAny(AsyncTask[] tasks, TaskCancelToken cancellationToken = null)
        {
            if (tasks == null || tasks.Length == 0)
            {
                return false;
            }

            TaskGroup group = new TaskGroup(2);

            foreach (AsyncTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            async AsyncVoid RunOneTask(AsyncTask task)
            {
                await task;
                await group.FinishOne();
            }

            await group.FinishOne();

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }
        public static async AsyncTask<bool> WaitAll(AsyncTask[] tasks, TaskCancelToken cancellationToken = null)
        {
            if (tasks == null || tasks.Length == 0)
            {
                return false;
            }

            TaskGroup group = new TaskGroup(tasks.Length + 1);

            foreach (AsyncTask task in tasks)
            {
                RunOneTask(task).Coroutine();
            }

            await group.FinishOne();

            async AsyncVoid RunOneTask(AsyncTask task)
            {
                await task;
                await group.FinishOne();
            }

            if (cancellationToken == null)
            {
                return true;
            }

            return !cancellationToken.IsCancel();
        }
    }
}
