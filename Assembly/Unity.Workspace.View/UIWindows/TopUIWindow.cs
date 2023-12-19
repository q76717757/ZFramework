
using System;
using TMPro;
using UnityEngine.UI;

namespace ZFramework
{
    [UISetting("Prefabs/Canvas_Top", UIGroupType.Top)]
    public class TopUIWindow : UIWindowBase
    {
        Image 新建对话Btn;
        TMP_InputField inputField_指定ID;

        Image 打开指定ID的窗体;
        Image 关闭指定ID的窗体;
        Image 销毁指定ID的窗体;
        Image 打开所有对话类型的窗体;
        Image 关闭所有对话类型的窗体;
        Image 打开所有窗体;
        Image 关闭所有窗体;
        Image 销毁所有对话类型的窗体;
        Image 销毁所有窗体;

        protected override void OnAwake()
        {
            新建对话Btn = References.Get<Image>("新建对话窗体");
            ZEvent.UIEvent.AddListener(新建对话Btn, OnClick对话Btn);
            inputField_指定ID = References.Get<TMP_InputField>("指定IDInputField");

            ZEvent.UIEvent.AddListener(References.Get<Image>("打开指定ID的窗体"), OnClickOtherBtn, "打开指定ID的窗体");
            ZEvent.UIEvent.AddListener(References.Get<Image>("关闭指定ID的窗体"), OnClickOtherBtn, "关闭指定ID的窗体");
            ZEvent.UIEvent.AddListener(References.Get<Image>("销毁指定ID的窗体"), OnClickOtherBtn, "销毁指定ID的窗体");
            ZEvent.UIEvent.AddListener(References.Get<Image>("打开所有对话类型的窗体"), OnClickOtherBtn, "打开所有对话类型的窗体");
            ZEvent.UIEvent.AddListener(References.Get<Image>("关闭所有对话类型的窗体"), OnClickOtherBtn, "关闭所有对话类型的窗体");
            ZEvent.UIEvent.AddListener(References.Get<Image>("打开所有窗体"), OnClickOtherBtn, "打开所有窗体");
            ZEvent.UIEvent.AddListener(References.Get<Image>("关闭所有窗体"), OnClickOtherBtn, "关闭所有窗体");
            ZEvent.UIEvent.AddListener(References.Get<Image>("销毁所有对话类型的窗体"), OnClickOtherBtn, "销毁所有对话类型的窗体");
            ZEvent.UIEvent.AddListener(References.Get<Image>("销毁所有窗体"), OnClickOtherBtn, "销毁所有窗体");
        }

        private void OnClick对话Btn(UIEventData data)
        {
            if (data.EventType == UIEventType.Click)
            {
                UIManager.Instance.OpenNew<DialogueWindow>().Invoke();
            }
        }

        private void OnClickOtherBtn(UIEventData<string> data)
        {
            int uiID = 0;
            if (int.TryParse(inputField_指定ID.text, out int value))
            {
                uiID = value;
            }

            if (data.EventType == UIEventType.Click)
            {
                switch (data.Data0)
                {
                    case "打开指定ID的窗体":
                        UIManager.Instance.Open(uiID);
                        break;
                    case "关闭指定ID的窗体":
                        UIManager.Instance.Close(uiID);
                        break;
                    case "销毁指定ID的窗体":
                        UIManager.Instance.Destroy(uiID);
                        break;
                    case "打开所有对话类型的窗体":
                        UIManager.Instance.OpenByType<DialogueWindow>();
                        break;
                    case "关闭所有对话类型的窗体":
                        UIManager.Instance.CloseByType<DialogueWindow>();
                        break;
                    case "打开所有窗体":
                        UIManager.Instance.OpenAll();
                        break;
                    case "关闭所有窗体":
                        UIManager.Instance.CloseAll();
                        break;
                    case "销毁所有对话类型的窗体":
                        UIManager.Instance.DestroyByType<DialogueWindow>();
                        break;
                    case "销毁所有窗体":
                        UIManager.Instance.DestroyAll();
                        break;
                }

            }
        }

        protected override void OnOpen()
        {

        }
        protected override void OnClose()
        {

        }
        protected override void OnUpdate()
        {

        }
        protected override void OnFocus()
        {

        }
        protected override void OnLoseFocus()
        {

        }
        protected override void OnDestroy()
        {

        }
    }
}
