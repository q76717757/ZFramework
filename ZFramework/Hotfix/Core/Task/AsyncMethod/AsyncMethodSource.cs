using System;

namespace ZFramework
{
    //���ǱȽ������һ������   �ܹ��������������״̬��  �����νӶ������Ԫ (ûд���� ���Ǹ����� װ��״̬����MoveNext)
    internal class AsyncMethodSource : ITaskCompletionSource
    {
        ATask task;
        //�����������������õ��ֶ� Invokeʱ��Self.Movenext  Awaitʱ��Parent.MoveNext Ҳ����Invoke��OnCompleted����ͬʱ������
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
            var movenext = moveNext;//Parent.MoveNext  ��OnCompled��ֵ
            moveNext = null;
            movenext?.Invoke();
        }
        public void SetException(Exception exception) //builder.SetException  Task����Ĵ������������ �����ж�״̬����ִ��
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
                    var movenext = moveNext;//Self.MoveNext ��Start��ֵ
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
                    var movenext = moveNext;//Self.MoveNext ��Start��ֵ
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
