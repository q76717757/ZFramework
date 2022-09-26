using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    [CreateAssetMenu(fileName = "BootFile", menuName = "ZFramework/�����ļ�", order = 0)]
    public class BootFile : ScriptableObject
    {
        /// <summary> ��Ŀ���� </summary>
        public string ProjectCode;

        public string ����·��;
        public string ����·��;
        public string ����·��;//��Դ��·��  ���������ʲô·�� ��ͨ��globalconfig��������  


        public Platform platform;//��Щ�Ƶ�globalconfig��
        public Network network;

        public bool allowOffline;

        //Ϊ��ʵ��һ������  �������������̳��İ�  ����һ����ǰ����
        //boot��������Դ�ľ���·�� һ�㰴����Ŀ����������,   ����������ʹ��DATA(��������)_PROJECTNAME(��Ŀ����)_TICK(����ʱ���) �����ķ�ʽ������
        //����һ�������л�����һ������  ���л������ɵĳ���   ��ʱ�������ʵ���Բ���Ҫ�ظ�����,   �ڳ��򼯼����������ֵ���ʽ�������(��������д�� ֱ��ռ�����ڴ���)
        //�ڼ��س���֮ǰ���ж�һ���ڲ��ڻ�����  �ڵĻ�ֱ������,��������ִ�и�������,����ɹ�ִ�и�������,�����¼����°汾����,�������ָ��ǵ�
    }
}
