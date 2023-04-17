using System;

namespace ZFramework
{
    /// <summary>
    /// �޷��ص�����Դ
    /// </summary>
    public interface ITaskCompletionSource //���з��ص�Դ�������  Ϊ�����з��ص�Դ������ʽת�����޷���Դ, ������������
    {
        ushort Ver { get; set; }
        //void Recycle();
        Exception GetException();

        /// <summary>
        /// ���ܻᱻ��ε���  ʵ��ʱҪȷ������ִ��һ��
        /// </summary>
        void TryStart();
        /// <summary>
        /// ����ע�᱾������ɺ�Ļص� continuation == �������MoveNext  ���ڷֲ��ȴ������   һ����������ж���� ��+=?
        /// </summary>
        void OnCompleted(Action continuation);
        TaskProcessStatus GetStatus();
        /// <summary>
        /// ���÷��Ǹ�����  ��������ɻ�Call������MoveNext   �������MoveNext������Call this.GetResult Ҳ���ܱ��ֶ��ظ�����
        /// </summary>
        void GetResultWithNotReturn();
        /// <summary>
        /// ͨ���쳣ǿ����ɱ�����
        /// </summary>
        void Break(Exception exception);
    }

    /// <summary>
    /// �з��ص�����Դ
    /// </summary>
    public interface ITaskCompletionSource<TResult> : ITaskCompletionSource
    {
        /// <summary>
        /// ���÷��Ǹ�����  ��������ɻ�Call������MoveNext   �������MoveNext������Call this.GetResult
        /// </summary>
        TResult GetResult();
    }
}
