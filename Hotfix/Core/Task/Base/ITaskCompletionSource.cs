using System;

namespace ZFramework
{
    /// <summary>
    /// �޷��ص�����Դ
    /// </summary>
    public interface ITaskCompletionSource  //���з��ص�Դ�������  Ϊ�����з��ص�Դ������ʽת�����޷���Դ, ������������
    {
        ushort Ver { get; set; }
        //void Recycle();
        Exception GetException();

        void TryStart();
        /// <summary>
        /// ����ע�᱾������ɺ�Ļص� continuation == �������MoveNext  ���ڷֲ��ȴ������   һ����������ж���� ��+=?
        /// </summary>
        void OnCompleted(Action continuation);
        TaskProcessStatus GetStatus();
        /// <summary>
        /// ���÷��Ǹ�����  ��������ɻ�Call������MoveNext   �������MoveNext������Call this.GetResult
        /// </summary>
        void GetResultWithNotReturn();
        /// <summary>
        /// ͨ���쳣ǿ���������
        /// </summary>
        void Break(Exception exception);//�ж����� ����ǳ�ʱ����ʹ���ʱ�쳣  ����Ƿ����쳣�ͰѾ�����쳣����ȥ?
    }

    /// <summary>
    /// �з��ص�����Դ
    /// </summary>
    public interface ITaskCompletionSource<out TResult> : ITaskCompletionSource
    {
        /// <summary>
        /// ���÷��Ǹ�����  ��������ɻ�Call������MoveNext   �������MoveNext������Call this.GetResult
        /// </summary>
        TResult GetResult();
    }
}
