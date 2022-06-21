
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ZFramework
{
    //生命周期
    public class View_MessageAwake : AwakeSystem<View_Message_Component>
    {
        public override void Awake(View_Message_Component component)
        {
            var canvas = component.gameObject.GetComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 10;

            component.sprs = new Sprite[2]
            {
                component.Refs.Get<Sprite>("按钮暗"),
                component.Refs.Get<Sprite>("按钮亮"),
            };
            component.imgs = new Image[2]
            {
                component.Refs.Get<Image>("Btn0"),
                component.Refs.Get<Image>("Btn1"),
            };
            ZEvent.UIEvent.AddListener(component.imgs[0], component.Btn, 0);
            ZEvent.UIEvent.AddListener(component.imgs[1], component.Btn, 1);
        }
    }

    public class View_Message_Show : UIShowSystem<View_Message_Component>
    {
        public override void OnShow(View_Message_Component canvas)
        {
            canvas.gameObject.SetActive(true);
            canvas.imgs[0].sprite = canvas.sprs[0];
            canvas.imgs[1].sprite = canvas.sprs[0];
        }
    }

    public class View_Message_Hide : UIHideSystem<View_Message_Component>
    {
        public override void OnHide(View_Message_Component canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_Message_System
    {

        public static void ShowInfo(this View_Message_Component component, string title, string info,Action<bool> callback)
        { 
            component.Refs.Get<TextMeshProUGUI>("title").text = title;
            component.Refs.Get<TextMeshProUGUI>("info").text = info;
            component.callbackBool = callback;
            component.callbackVoid = null;

            component.imgs[1].gameObject.SetActive(true);
            component.imgs[0].rectTransform.anchoredPosition = new Vector2(-100,15);
            component.imgs[1].rectTransform.anchoredPosition = new Vector2(100, 15);
        }

        public static void ShowInfo(this View_Message_Component component, string title, string info, Action callback)
        {
            component.Refs.Get<TextMeshProUGUI>("title").text = title;
            component.Refs.Get<TextMeshProUGUI>("info").text = info;
            component.callbackBool = null;
            component.callbackVoid = callback;

            component.imgs[1].gameObject.SetActive(false);
            component.imgs[0].rectTransform.anchoredPosition = new Vector2(0, 15);
            component.imgs[1].rectTransform.anchoredPosition = new Vector2(100, 15);
        }

        public static void Btn(this View_Message_Component component, UIEventData<int> eventData)
        {
            int btnIndex = eventData.Data0;
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    SoundHelper.MouseEnter();
                    component.imgs[btnIndex].sprite = component.sprs[1];
                    break;
                case UIEventType.Exit:
                    component.imgs[btnIndex].sprite = component.sprs[0];
                    break;
                case UIEventType.Down:
                    break;
                case UIEventType.Up:
                    break;
                case UIEventType.Click:
                    Game.Root.GetComponent<UIComponent>().Hide(UIType.View_Message);
                    if (component.callbackBool != null)
                    {
                        component.callbackBool.Invoke(eventData.Data0 == 0);
                        component.callbackBool = null;
                        return;
                    }
                    if (component.callbackVoid != null)
                    {
                        component.callbackVoid.Invoke();
                        component.callbackVoid = null;
                        return;
                    }
                    break;
            }
        }
    }
}
