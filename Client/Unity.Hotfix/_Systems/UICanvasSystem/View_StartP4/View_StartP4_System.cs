
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
    public class View_StartP4Awake : AwakeSystem<View_StartP4_Component>
    {
        public override void Awake(View_StartP4_Component component)
        {
            var btn = component.Refs.Get<Image>("Btn");
            ZEvent.UIEvent.AddListener(btn, component.Btn);

            ZEvent.UIEvent.AddListener(component.Refs.Get<Image>("Image"), component.Close);
        }
    }
    public class View_StartP4_Show : UIShowSystem<View_StartP4_Component>
    {
        public override void OnShow(View_StartP4_Component canvas)
        {
            canvas.gameObject.SetActive(true);
        }
    }

    public class View_StartP4_Hide : UIHideSystem<View_StartP4_Component>
    {
        public override void OnHide(View_StartP4_Component canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_StartP4_System
    {
        public static void Btn(this View_StartP4_Component component, UIEventData eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    SoundHelper.MouseEnter();
                    break;
                case UIEventType.Exit:
                    break;
                case UIEventType.Down:
                    break;
                case UIEventType.Up:
                    break;
                case UIEventType.Click:
                    Game.UI.Hide(UIType.View_StartP4);
                    Game.UI.Show(UIType.View_Line);
                    break;
            }
        }

        public static void Close(this View_StartP4_Component component, UIEventData eventData)
        {
            if (eventData.EventType == UIEventType.Click)
            {
                (Game.UI.Get(UIType.View_Left) as View_Left_Component).Clear();
            }
        }

    }
}
