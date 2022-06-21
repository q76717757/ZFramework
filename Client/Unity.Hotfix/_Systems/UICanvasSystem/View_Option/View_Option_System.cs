
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
    public class View_OptionAwake : AwakeSystem<View_Option_Component>
    {
        public override void Awake(View_Option_Component component)
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

            component.Refs.Get<Slider>("Slider0").onValueChanged.AddListener(component.OnSliderValueChange0);
            component.Refs.Get<Slider>("Slider1").onValueChanged.AddListener(component.OnSliderValueChange1);
        }
    }
    public class View_Option_Show : UIShowSystem<View_Option_Component>
    {
        public override void OnShow(View_Option_Component canvas)
        {
            canvas.gameObject.SetActive(true);
            canvas.imgs[0].sprite = canvas.sprs[0];
            canvas.imgs[1].sprite = canvas.sprs[0];
        }
    }

    public class View_Option_Hide : UIHideSystem<View_Option_Component>
    {
        public override void OnHide(View_Option_Component canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_Option_System
    {
        public static void Btn(this View_Option_Component component, UIEventData<int> eventData)
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
                case UIEventType.Click:
                    Game.Root.GetComponent<UIComponent>().Hide(UIType.View_Option);

                    if (eventData.Data0 == 0)//保存
                    {
                        component.slidervalue0 = component.Refs.Get<Slider>("Slider0").value;
                        component.slidervalue1 = component.Refs.Get<Slider>("Slider1").value;
                    }
                    else//取消
                    {
                        component.Refs.Get<Slider>("Slider0").value = component.slidervalue0;
                        component.Refs.Get<Slider>("Slider1").value = component.slidervalue1;
                        SoundHelper.instance.bgm.volume = component.slidervalue0;
                        SoundHelper.instance.sound.volume = component.slidervalue1;
                    }
                    break;
            }
        }

        public static void OnSliderValueChange0(this View_Option_Component component, float value)
        {
            SoundHelper.instance.bgm.volume = value;
        }
        public static void OnSliderValueChange1(this View_Option_Component component, float value)
        {
            SoundHelper.instance.sound.volume = value;
        }

    }
}
