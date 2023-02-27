using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ��ȡ��֤�밴ť
/// </summary>
public class Btn_GetVCode : MonoBehaviour
{
    /// <summary>
    /// ���
    /// </summary>
    public Button button;
    public TMP_Text text;

    /// <summary>
    /// �������
    /// </summary>
    public void OnClick()
    {
        if (UIManager.Instance.UI_GetGameObject("IF_PhoneNumber").GetComponent<TMP_InputField>().text.Length < 11)
        {
            Debug.Log("��ȡ��֤�밴ť�¼�:������Ϣ���");
            UserInterface.Instance.ShowMessage(new MessagePanel(), "��������ȷ���ֻ�����");
            return;
        }
        Debug.Log("�ѷ�����֤��");
        button.interactable = false;
        text.color = new Vector4(0.007843138f, 0.2196079f, 0.4588236f, 0.5f);
        StartCoroutine(CountDown());
    }

    /// <summary>
    /// ��֤����ȴ
    /// </summary>
    private IEnumerator CountDown()
    {
        var time = 60.0f;
        while (time > 0)
        {
            if (time < 10)
            {
                text.SetText("0" + time + "�������");
            }
            else
            {
                text.SetText(time + "�������");
            }
            time--;
            yield return new WaitForSeconds(1);
        }
        text.color = new Vector4(0.007843138f, 0.2196079f, 0.4588236f, 1.0f);
        text.SetText("�����Ż�ȡ");
        button.interactable = true;
    }
}
