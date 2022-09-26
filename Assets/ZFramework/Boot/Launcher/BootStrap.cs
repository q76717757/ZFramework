using System;
using UnityEngine;

/*-*
 * ���򼯵Ķ��弰���ԭ��
 * PC�˿����и��ַ���ʵ���ȸ��� �ڴ˼ܹ���������ȫ�����ȸ�һ���ļ���������ȥ Ȼ���ø���������������ֱ���ļ����滻���� ����û���κ�Լ��
 * �����ȸ��������Ҫ����ֻ�ƽ̨
 * 
 * Boot       ������       �湤�̷���һ�𷢲�  �����������ܹ������  **���������޸� ����֧���ȸ���
 * Core       ���Ĳ�       �����ܹ�����ĵ�ģ���װ�ڴ�  ����������MONO�����������������ϵ�Լ��ṩ������ģ���໥ͨ�ŵ��¼�ϵͳ
 * Editor     �༭����չ�� ��Ҫ��һЩ�༭������ �Լ�һЩ���ӻ����  ���༭��������
 * 
 * ---Data      �������ݲ�   ʵ��˫�˴��빲��Ĳ��� ��ƽ̨�޹� ��ʹ��unityAPI���������������� �����ֱ�Ӹ����ⲿ�ִ���
 * ---Func      �����߼���   ʵ��˫�˴��빲��Ĳ��� ��ƽ̨�޹� ��ʹ��unityAPI���������������� �����ֱ�Ӹ����ⲿ�ִ���
 * ---View      ��ͼ��   ����ض�ƽ̨�Ŀͻ��˴��� ʹ����unityAPI�������������  ͨ���޸���ͼ��Ĵ������ʵ���߼�����  ��ͨ����дview�ĳɱ���ն�
 * 
 * Mono       ���ز�       �̳���MonoBehaviour�Ľű���������  ������һ����.����  ʹ��wolong���Զ���������ȸ�  ���Խ����ȸ��� ����֧��reload (��Ҫ����)
 * 
 * Package    ��չ����     ��Ҫ���ű����õĲ��������    �ⲿ�����ⲿ������ɿ�  ��֧���ȸ�
 * Plugins    �����       ��һЩƽ̨�ض���Ͷ�̬��  ��Ĳ��ּ�����unity �����  ��֧���ȸ���
 */

namespace ZFramework
{ 
    [AddComponentMenu("ZFramework/BootStrap")]
    [DisallowMultipleComponent]
    public sealed class BootStrap : MonoBehaviour
    {
        static BootStrap instance;
        [SerializeField]
        private BootFile boot;
        private DriveStrap drive;

        void Awake()
        {
            if (instance == null)
            { 
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }
        void Start() => StartGame(boot);
        void OnApplicationQuit() => CloseGame();

        public static void StartGame(BootFile boot)
        {
            if (instance == null)
            {
                var go = new GameObject("[BootStrap]");
                go.AddComponent<BootStrap>().boot = boot;
            }
            else
            {
                instance.drive = instance.gameObject.AddComponent<DriveStrap>();
                instance.drive.hideFlags = HideFlags.HideInInspector;
                instance.drive.StartGame(boot);
            }
        }
        public static void CloseGame()
        {
            if (instance != null)
            {
                instance.drive.CloseGame();
                Destroy(instance.gameObject);
                instance = null;
            }
        }
    }
}