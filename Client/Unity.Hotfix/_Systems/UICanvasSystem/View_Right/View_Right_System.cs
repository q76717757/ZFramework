
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ZFramework
{
    //生命周期
    public class View_RightAwake : AwakeSystem<View_Right_Component>
    {
        public override void Awake(View_Right_Component component)
        {
            component.btnImgs = new Image[]
            { 
                component.Refs.Get<Image>("reset"),
                component.Refs.Get<Image>("last"),
                component.Refs.Get<Image>("next"),
            };
            component.btnSprs = new Sprite[,]
            {
                {component.Refs.Get<Sprite>("重置暗"), component.Refs.Get<Sprite>("重置亮")},
                {component.Refs.Get<Sprite>("上一步暗"), component.Refs.Get<Sprite>("上一步亮")},
                {component.Refs.Get<Sprite>("下一步暗"), component.Refs.Get<Sprite>("下一步亮")},
            };

            ZEvent.UIEvent.AddListener(component.btnImgs[0], component.Btn, 0);
            ZEvent.UIEvent.AddListener(component.btnImgs[1], component.Btn, 1);
            ZEvent.UIEvent.AddListener(component.btnImgs[2], component.Btn, 2);
        }
    }

    public class View_Right_Show : UIShowSystem<View_Right_Component>
    {
        public override void OnShow(View_Right_Component canvas)
        {
            canvas.gameObject.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                canvas.btnImgs[i].sprite = canvas.btnSprs[i, 0];
            }
            canvas.ShowTip("");
        }
    }

    public class View_Right_Hide : UIHideSystem<View_Right_Component>
    {
        public override void OnHide(View_Right_Component canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_Right_System
    {
        public static void Btn(this View_Right_Component component, UIEventData<int> eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    SoundHelper.MouseEnter();
                    component.btnImgs[eventData.Data0].sprite = component.btnSprs[eventData.Data0, 1];
                    break;
                case UIEventType.Exit:
                    component.btnImgs[eventData.Data0].sprite = component.btnSprs[eventData.Data0, 0];
                    break;
                case UIEventType.Click:
                    switch (eventData.Data0)
                    {
                        case 0:
                            Game.Task.End();
                            //Game.Task.Run(0);
                            Game.UI.Show(UIType.View_StartP4);
                            break;
                        case 1:
                            Game.Task.BackStep();
                            break;
                        case 2:
                            var data = Game.UI.Get(UIType.View_Data);
                            if (data == null || !data.gameObject.activeSelf)
                            {
                                Game.UI.Show(UIType.View_Data);
                            }
                            else
                            {
                                Game.UI.Hide(UIType.View_Data);
                            }
                            break;
                    }
                    break;
            }
        }

        public static void ShowTip(this View_Right_Component component, string tip)
        {
            component.Refs.Get<GameObject>("tipPlane").SetActive(!string.IsNullOrEmpty(tip));
            component.Refs.Get<TextMeshProUGUI>("tip").text = tip;

        }
    }
}
