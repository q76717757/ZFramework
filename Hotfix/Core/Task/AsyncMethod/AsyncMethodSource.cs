using System;
using System.Runtime.CompilerServices;

namespace ZFramework
{
    //���ǱȽ������һ������  �ܹ��������������״̬��  �����νӶ������Ԫ  ͬʱ���Լ�Ҳ������Ϊ������Դ
    internal class AsyncMethodSource<TResult> : IAsyncMethodRunner<TResult>  //�޷��ص�״̬���ͷ�<VoidResult>
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
        void IAsyncMethodRunner<TResult>.SetStateMachine<TStateMachine>(in TStateMachine stateMachine)//ͬ������  Async���������õ�ʱ�� ֻ����ATask ʵ�������ķ�����TCS�ӿڵ�Start
        {
            this.stateMachine = stateMachine;
            this.task = ATask.FromSource(this);
        }
        void IAsyncMethodRunner<TResult>.MoveNext()
        {
            //����һЩ���� �жϵ���״̬��Ҫ��Ҫ������ ȡ����ʱ���ǲ������ߵ�
            if (ex != null)
            {
                if (ex is OperationCanceledException)
                {
                    Log.Error("������ȡ��,��������ֵ��ֹͣ״̬��"); //ֱ�ӻ���Դ  ���ɿص�Դ(ʵ���ϲ���ȡ����Դ)  ���صĽ��ֱ�ӷ��� ��ֹ״̬��
                    return;
                }
                if (ex is TimeoutException)
                {
                    Log.Error("������ʱ,��������ֵ��ֹͣ״̬��");
                    return;
                }
                Log.Info("�ж�״̬��" + ex);
            }
            else
            {
                stateMachine.MoveNext();
            }
        }
        void IAsyncMethodRunner<TResult>.SetException(Exception exception)
        {
            //���Ǳ������񵽵��쳣 ������� save ex �ϼ�Get�ͻ��õ�  ���������¼�״̬��

            ex = exception;
            if (ex is OperationCanceledException)//ȡ���쳣
            {
                Log.Error("ȡ���쳣");
            }
            if (ex is TimeoutException)//��ʱ�쳣
            {
                Log.Error("��ʱ�쳣");
            }
            //�����쳣....
            Log.Error("�����쳣" + exception);
        } 
        void IAsyncMethodRunner<TResult>.SetResult(TResult result)// ״̬�����һ��ִ�еķ��� try catchû�в����쳣  �Ż������ 
        {
            this.result = result;
            state = TaskProcessStatus.Completion;

            var continuation = this.continuation; //�������MoveNext
            this.continuation = null;
            continuation?.Invoke();
        }


        //��Ϊ����Դ
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
                //�������쳣 �ϼ�����ͻᲶ��
                throw ex;
            }
            return result;
        }
        void ITaskCompletionSource.GetResultWithNotReturn() => (this as IAsyncMethodRunner<TResult>).GetResult();
        void ITaskCompletionSource.Break(Exception exception)
        {
            ex = exception;
            state = TaskProcessStatus.Completion;

            //��ǰ�������..... 

            var temp =  this.continuation;
            continuation = null;
            temp?.Invoke();

            //�����Ǳ���������ǰ���  �������

            //��ô���ݸ������� �����������쳣??

        }

       
    }

}
