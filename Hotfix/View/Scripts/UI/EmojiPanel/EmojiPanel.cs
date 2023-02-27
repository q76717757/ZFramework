using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������
/// </summary>
public class EmojiPanel : BasePanel
{
    public static readonly string path = "UI/GamePanel/EmojiPanel";
    public EmojiPanel() : base(new UI_Info(path)) { }

    public override void OnEnter()
    {
        base.OnEnter();
        #region -Initialize-
        UIManager.Instance.UI_GetGameObject("Btn_Back").GetComponent<Button>().onClick.AddListener(() => { Btn_Back(); });
        UIManager.Instance.UI_GetGameObject("Btn_Emoji_1").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(1); });
        UIManager.Instance.UI_GetGameObject("Btn_Emoji_2").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(2); });
        UIManager.Instance.UI_GetGameObject("Btn_Emoji_3").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(3); });
        UIManager.Instance.UI_GetGameObject("Btn_Emoji_4").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(4); });
        UIManager.Instance.UI_GetGameObject("Btn_Emoji_5").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(5); });
        UIManager.Instance.UI_GetGameObject("Btn_Emoji_6").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(6); });
        UIManager.Instance.UI_GetGameObject("Btn_Emoji_7").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(7); });
        UIManager.Instance.UI_GetGameObject("Btn_Emoji_8").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(8); });
        UIManager.Instance.UI_GetGameObject("Btn_Emoji_9").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(9); });
        UIManager.Instance.UI_GetGameObject("Btn_Emoji_10").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(10); });
        UIManager.Instance.UI_GetGameObject("Btn_Emoji_11").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(11); });
        UIManager.Instance.UI_GetGameObject("Btn_Emoji_12").GetComponent<Button>().onClick.AddListener(() => { Btn_Emoji(12); });
        #endregion
    }

    /// <summary>
    /// ���ذ�ť�¼�
    /// </summary>
    private void Btn_Back()
    {
        Debug.Log("���ذ�ť�¼�:���ٴ����");
        UIManager.Instance.Pop();
    }

    /// <summary>
    /// ���鰴ť�¼�
    /// </summary>
    /// <param name="Number">������</param>
    private void Btn_Emoji(int Number)
    {
        Debug.Log($"���鰴ť�¼�:����{Number}�ű��鶯��");
        Debug.Log("���鰴ť�¼�:���ٴ����");
        UIManager.Instance.Pop();
    }
}
