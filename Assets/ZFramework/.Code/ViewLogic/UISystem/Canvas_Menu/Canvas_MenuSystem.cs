using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ZFramework
{
    public class CanvasMenuAwake : OnAwakeImpl<Canvas_MenuComponent>
    {
        public override void OnAwake(Canvas_MenuComponent entity)
        {
            ZEvent.UIEvent.AddListener(entity.Refs.Get<GameObject>("Btn0"), entity.BtnCall, 0);
            ZEvent.UIEvent.AddListener(entity.Refs.Get<GameObject>("Btn1"), entity.BtnCall, 1);
            ZEvent.UIEvent.AddListener(entity.Refs.Get<GameObject>("Btn2"), entity.BtnCall, 2);
            ZEvent.UIEvent.AddListener(entity.Refs.Get<GameObject>("Btn3"), entity.BtnCall, 3);
            ZEvent.UIEvent.AddListener(entity.Refs.Get<GameObject>("close"), entity.BtnCall, 4);

            entity.Refs.Get<GameObject>("page0").SetActive(false);
            entity.Refs.Get<GameObject>("page1").SetActive(false);
            entity.Refs.Get<GameObject>("page2").SetActive(false);
            entity.Refs.Get<GameObject>("page3").SetActive(false);
            entity.Refs.Get<GameObject>("close").SetActive(false);
        }
    }

    public class CanvasMenuShow : UIShowSystem<Canvas_MenuComponent>
    {
        public override void OnShow(Canvas_MenuComponent canvas)
        {
            if (canvas.gameObject.activeSelf)
            {
                canvas.Hide();
            }
            else
            {
                canvas.Show();
            }
        }
    }
    public class CanvasMenuHide : UIHideSystem<Canvas_MenuComponent>
    {
        public override void OnHide(Canvas_MenuComponent canvas)
        {
            canvas.Hide();
        }
    }

    public static class Canvas_MenuSystem
    {
        public static void Show(this Canvas_MenuComponent component)
        {
            component.gameObject.SetActive(true);

            component.Refs.Get<GameObject>("page0").SetActive(false);
            component.Refs.Get<GameObject>("page1").SetActive(false);
            component.Refs.Get<GameObject>("page2").SetActive(false);
            component.Refs.Get<GameObject>("page3").SetActive(false);

            component.Refs.Get<GameObject>("close").SetActive(false);

            component.Refs.Get<GameObject>("Btn0").SetActive(true);
            component.Refs.Get<GameObject>("Btn1").SetActive(true);
            component.Refs.Get<GameObject>("Btn2").SetActive(true);
            component.Refs.Get<GameObject>("Btn3").SetActive(true);

#if VR
            var followHead = Valve.VR.InteractionSystem.Player.instance.hmdTransform;
            var forw = followHead.forward;

            var canvas = component.gameObject.GetComponent<Transform>();
            canvas.position = followHead.transform.position + new Vector3(forw.x, 0, forw.z);
            canvas.LookAt(followHead, Vector3.up);
            canvas.rotation *= Quaternion.Euler(0, 180, 0);
#endif
            //component.Process.GetComponent<VRHelperComponent>().SetUILine(true);
        }
        public static void Hide(this Canvas_MenuComponent component)
        {
            //component.Process.GetComponent<VRHelperComponent>().SetUILine(false);
            component.gameObject.SetActive(false);
        }



        public static void BtnCall(this Canvas_MenuComponent component, UIEventData<int> eventData)
        {
            int btnIndex = eventData.Data0;
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    break;
                case UIEventType.Exit:
                    break;
                case UIEventType.Down:
                    break;
                case UIEventType.Up:
                    break;
                case UIEventType.Click:
                    component.Refs.Get<GameObject>("page0").SetActive(btnIndex == 0);
                    component.Refs.Get<GameObject>("page1").SetActive(btnIndex == 1);
                    component.Refs.Get<GameObject>("page2").SetActive(btnIndex == 2);
                    component.Refs.Get<GameObject>("page3").SetActive(btnIndex == 3);

                    component.Refs.Get<GameObject>("close").SetActive(btnIndex != 4);

                    component.Refs.Get<GameObject>("Btn0").SetActive(btnIndex == 4);
                    component.Refs.Get<GameObject>("Btn1").SetActive(btnIndex == 4);
                    component.Refs.Get<GameObject>("Btn2").SetActive(btnIndex == 4);
                    component.Refs.Get<GameObject>("Btn3").SetActive(btnIndex == 4);
                    break;
                case UIEventType.Drag:
                    break;
                case UIEventType.Scroll:
                    break;
                default:
                    break;
            }
        }


    }
}
