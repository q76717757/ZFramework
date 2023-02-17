using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 获取验证码按钮
/// </summary>
public class Btn_GetVCode : MonoBehaviour
{
    /// <summary>
    /// 组件
    /// </summary>
    public Button button;
    public TMP_Text text;

    /// <summary>
    /// 点击方法
    /// </summary>
    public void OnClick()
    {
        if (UIManager.Instance.UI_GetGameObject("IF_PhoneNumber").GetComponent<TMP_InputField>().text.Length < 11)
        {
            Debug.Log("获取验证码按钮事件:弹出消息面板");
            UserInterface.Instance.ShowMessage(new MessagePanel(), "请输入正确的手机号码");
            return;
        }
        Debug.Log("已发送验证码");
        button.interactable = false;
        text.color = new Vector4(0.007843138f, 0.2196079f, 0.4588236f, 0.5f);
        StartCoroutine(CountDown());
    }

    /// <summary>
    /// 验证码冷却
    /// </summary>
    private IEnumerator CountDown()
    {
        var time = 60.0f;
        while (time > 0)
        {
            if (time < 10)
            {
                text.SetText("0" + time + "秒后重试");
            }
            else
            {
                text.SetText(time + "秒后重试");
            }
            time--;
            yield return new WaitForSeconds(1);
        }
        text.color = new Vector4(0.007843138f, 0.2196079f, 0.4588236f, 1.0f);
        text.SetText("│短信获取");
        button.interactable = true;
    }
}
