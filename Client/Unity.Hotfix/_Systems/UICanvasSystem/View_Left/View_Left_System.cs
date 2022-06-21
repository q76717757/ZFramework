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
    public class View_LeftAwake : AwakeSystem<View_Left_Component>
    {
        public override void Awake(View_Left_Component component)
        {
            component.icons = new Sprite[4, 2]
            {
                {component.Refs.Get<Sprite>("实验介绍暗"),component.Refs.Get<Sprite>("实验介绍亮") },
                {component.Refs.Get<Sprite>("结构学习暗"),component.Refs.Get<Sprite>("结构学习亮") },
                {component.Refs.Get<Sprite>("原理学习暗"),component.Refs.Get<Sprite>("原理学习亮") },
                {component.Refs.Get<Sprite>("负载实验暗"),component.Refs.Get<Sprite>("负载实验亮") },
            };

            ZEvent.UIEvent.AddListener(component.Refs.Get<Image>("Btn0"), component.Btn, 0, component.Refs.Get<Image>("Btn0"));
            ZEvent.UIEvent.AddListener(component.Refs.Get<Image>("Btn1"), component.Btn, 1, component.Refs.Get<Image>("Btn1"));
            ZEvent.UIEvent.AddListener(component.Refs.Get<Image>("Btn2"), component.Btn, 2, component.Refs.Get<Image>("Btn2"));
            ZEvent.UIEvent.AddListener(component.Refs.Get<Image>("Btn3"), component.Btn, 3, component.Refs.Get<Image>("Btn3"));
            component.localSelect = -1;

        }
    }
    public class View_Left_Show : UIShowSystem<View_Left_Component>
    {
        public override void OnShow(View_Left_Component canvas)
        {
            canvas.gameObject.SetActive(true);
            for (int i = 0; i < 4; i++)
            {
                canvas.Refs.Get<Image>($"Btn{i}").sprite = canvas.icons[i, canvas.localSelect == i ? 1 : 0];
            }
        }
    }

    public class View_Left_Hide : UIHideSystem<View_Left_Component>
    {
        public override void OnHide(View_Left_Component canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_Left_System
    {
        public static void Clear(this View_Left_Component component)
        {
            for (int i = 0; i < 4; i++)
            {
                component.Refs.Get<Image>($"Btn{i}").sprite = component.icons[i, 0];
            }
            switch (component.localSelect)
            {
                case 0:
                    Game.UI.Hide(UIType.View_Pdf);
                    break;
                case 1:
                    Game.UI.Hide(UIType.View_Boom);
                    break;
                case 2:
                    Game.UI.Hide(UIType.View_Pdf2);
                    break;
                case 3:
                    Game.UI.Hide(UIType.View_Data);
                    Game.UI.Hide(UIType.View_Right);
                    Game.UI.Hide(UIType.View_StartP4);
                    Game.UI.Hide(UIType.View_Line);
                    Game.Task.End();
                    (Game.UI.Get(UIType.View_Top) as View_Top_Component).SetRightPlane(true);
                    break;
            }
            Scene01Helper.instance.一个柱子.SetActive(true);
            Scene01Helper.instance.六个柱子.SetActive(false);
            (Game.UI.Get(UIType.View_Top) as View_Top_Component).SetRightPlane(true);
            Game.Task.ResetCamera(false);
            component.localSelect = -1;
        }

        public static void Btn(this View_Left_Component component, UIEventData<int,Image> eventData)
        {
            if (component.localSelect == eventData.Data0)
                return;

            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    SoundHelper.MouseEnter();
                    eventData.Data1.sprite = component.icons[eventData.Data0, 1];
                    break;
                case UIEventType.Exit:
                    eventData.Data1.sprite = component.icons[eventData.Data0, 0];
                    break;
                case UIEventType.Click:
                    if (component.localSelect != -1)
                    { 
                        component.Refs.Get<Image>($"Btn{component.localSelect}").sprite = component.icons[component.localSelect, 0];
                        switch (component.localSelect)
                        {
                            case 0:
                                Game.UI.Hide(UIType.View_Pdf);
                                break;
                            case 1:
                                Game.UI.Hide(UIType.View_Boom);
                                break;
                            case 2:
                                Game.UI.Hide(UIType.View_Pdf2);
                                break;
                            case 3:
                                Game.UI.Hide(UIType.View_Data);
                                Game.UI.Hide(UIType.View_Right);
                                Game.UI.Hide(UIType.View_StartP4);
                                Game.UI.Hide(UIType.View_Line);
                                Game.Task.End();
                                break;
                        }
                        (Game.UI.Get(UIType.View_Top) as View_Top_Component).SetRightPlane(true);
                    }

                    component.localSelect = eventData.Data0;
                    eventData.Data1.sprite = component.icons[eventData.Data0, 1];

                    switch (eventData.Data0)
                    {
                        case 0:
                            Log.Info("理论学习");
                            Game.UI.Show(UIType.View_Pdf);
                            Scene01Helper.instance.一个柱子.SetActive(true);
                            Scene01Helper.instance.六个柱子.SetActive(false);
                            (Game.UI.Get(UIType.View_Top) as View_Top_Component).SetRightPlane(false);
                            break;
                        case 1:
                            Log.Info("结构学习");
                            Game.UI.Show(UIType.View_Boom);
                            Scene01Helper.instance.一个柱子.SetActive(true);
                            Scene01Helper.instance.六个柱子.SetActive(false);
                            (Game.UI.Get(UIType.View_Top) as View_Top_Component).SetRightPlane(false);
                            break;
                        case 2:
                            Log.Info("原理学习");
                            Game.UI.Show(UIType.View_Pdf2);
                            Scene01Helper.instance.一个柱子.SetActive(true);
                            Scene01Helper.instance.六个柱子.SetActive(false);
                            (Game.UI.Get(UIType.View_Top) as View_Top_Component).SetRightPlane(false);
                            break;
                        case 3:
                            Log.Info("负载实验");
                            Scene01Helper.instance.一个柱子.SetActive(false);
                            Scene01Helper.instance.六个柱子.SetActive(true);

                            (Game.UI.Get(UIType.View_Top) as View_Top_Component).SetRightPlane(false);
                            Game.UI.Show(UIType.View_StartP4);
                            break;
                    }
                    break;
            }
        }
    }
}