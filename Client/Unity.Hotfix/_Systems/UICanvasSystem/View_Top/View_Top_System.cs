
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ZFramework
{
    //生命周期
    public class View_TopAwake : AwakeSystem<View_Top_Component>
    {
        public override void Awake(View_Top_Component component)
        {
            component.btn0Spr = new Sprite[2] 
            { 
                component.Refs.Get<Sprite>("账号按钮暗"),
                component.Refs.Get<Sprite>("账号按钮亮"),
            };
            component.btn1Spr = new Sprite[2]
            {
                component.Refs.Get<Sprite>("设置按钮暗"),
                component.Refs.Get<Sprite>("设置按钮亮"),
            };
            component.btn2Spr = new Sprite[2]
            {
                component.Refs.Get<Sprite>("关闭按钮暗"),
                component.Refs.Get<Sprite>("关闭按钮亮"),
            };

            ZEvent.UIEvent.AddListener(component.Refs.Get<Image>("Btn0"), component.Btn, 0);
            ZEvent.UIEvent.AddListener(component.Refs.Get<Image>("Btn1"), component.Btn, 1);
            ZEvent.UIEvent.AddListener(component.Refs.Get<Image>("Btn2"), component.Btn, 2);
        }
    }
    public class View_TopUpdate : UpdateSystem<View_Top_Component>
    {
        public override void Update(View_Top_Component component)
        {
        }
    }

    public class View_Top_Show : UIShowSystem<View_Top_Component>
    {
        public override void OnShow(View_Top_Component canvas)
        {
            canvas.gameObject.SetActive(true);
            canvas.SetRightPlane(true);
        }
    }

    public class View_Top_Hide : UIHideSystem<View_Top_Component>
    {
        public override void OnHide(View_Top_Component canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_Top_System
    {

        public static void Btn(this View_Top_Component component, UIEventData<int> eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    SoundHelper.MouseEnter();
                    switch (eventData.Data0)
                    {
                        case 0:
                            component.Refs.Get<Image>("Btn0").sprite = component.btn0Spr[1];
                            break;
                        case 1:
                            component.Refs.Get<Image>("Btn1").sprite = component.btn1Spr[1];
                            break;
                        case 2:
                            component.Refs.Get<Image>("Btn2").sprite = component.btn2Spr[1];
                            break;
                    }
                    break;
                case UIEventType.Exit:
                    switch (eventData.Data0)
                    {
                        case 0:
                            component.Refs.Get<Image>("Btn0").sprite = component.btn0Spr[0];
                            break;
                        case 1:
                            component.Refs.Get<Image>("Btn1").sprite = component.btn1Spr[0];
                            break;
                        case 2:
                            component.Refs.Get<Image>("Btn2").sprite = component.btn2Spr[0];
                            break;
                    }
                    break;
                case UIEventType.Click:
                    switch (eventData.Data0)
                    {
                        case 0:
                            Game.UI.Show(UIType.View_PersonalCenter);
                            break;
                        case 1:
                            Game.UI.Show(UIType.View_Option);
                            break;
                        case 2:
                            (Game.UI.Show(UIType.View_Message) as View_Message_Component).ShowInfo("退出", "确认退出系统?", (b) =>
                            {
                                if (b)
                                {
                                    Application.Quit();
                                }
                            });
                            break;
                    }
                    break;
            }
        }

        public static void SetRightPlane(this View_Top_Component component, bool isOn)
        { 
            component.Refs.Get<GameObject>("RightPlane").SetActive(isOn);

        }
    }
}

