using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// �������ع�����
/// </summary>
public class MapManager : Singleton<MapManager>
{
    /// <summary>
    /// ͬ�����س���
    /// ������Ϸ����
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="action">���ܺ���</param>
    public void LoadScene(string name,UnityAction action)
    {
        //��ɳ������غ�Ŵ������ܺ���
        SceneManager.LoadScene(name);
        {
            action();
        }
    }

    /// <summary>
    /// �첽���س����Ľӿڷ���
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="time">ʱ��</param>
    /// <param name="action">���ܺ���</param>
    public void IAsynLoadScene(string name, float time = 0, UnityAction action = null)
    {
        if (time < 0) return;           
        RealTimeManager.Instance.StartCoroutine(AsynLoadScene(name, time, action));
    }

    /// <summary>
    /// �첽���س���
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="time">ʱ��</param>
    /// <param name="action">���ܺ���</param>
    private IEnumerator AsynLoadScene(string name, float time, UnityAction action)
    {
        //չʾ�ý���(���ڽ���������)
        float DisplayProgress = 0f;
        //ʵ�ʽ���(�������ؽ���)
        float RealProgress = 0f;
        //�����첽�¼�
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        //�����Զ���ת
        operation.allowSceneActivation = false;
        /*
         * ��ʵ�ʽ���С��90%,��չʾ�ý���С��ʵ�ʽ���ʱ
         * ��Ԥ���ٶȸ���չʾ�ý���,������������"�������ؽ���������"�¼���ί�к���
         */
        while (RealProgress < 0.9f)
        {
            RealProgress = operation.progress;
            while (DisplayProgress < RealProgress)
            {
                DisplayProgress += 0.01f;
                EventManager.Instance.EventTrigger("�������ؽ���������", DisplayProgress);
                yield return null;
            }
        }
        /*
         * ����ʵ���Ȳ�С��90%ʱ,��չʾ�ý���δ��ʱ
         * ��Ԥ���ٶȸ���չʾ�ý���,������������"�������ؽ���������"�¼���ί�к���
         */
        while (DisplayProgress < 1)
        {
            DisplayProgress += 0.01f;
            EventManager.Instance.EventTrigger("�������ؽ���������", DisplayProgress);
            yield return null;
        }
        /*
         * ����������δ���,��չʾ�ý�������ʱ
         * ��չʾ�ý�������Ϊ��ֵ,������������"�������ؽ���������"�¼���ί�к���
         */
        while (!operation.isDone)
        {
            EventManager.Instance.EventTrigger("�������ؽ���������", 1f);
            //���ҽ���չʾ�ý��Ƚӽ���ʱ,�������Զ���ת,���������δ��ʱ��ת
            if (DisplayProgress >= 0.95f)
            {
                operation.allowSceneActivation = true;
            }
            if (time != 0)
                yield return new WaitForSeconds(time);
            else yield return null;
        }
        //����������ɺ�ִ�й��ܺ���
        action?.Invoke();
    }
}
