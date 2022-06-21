
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace ZFramework
{
    //生命周期
    public class View_BoomAwake : AwakeSystem<View_Boom_Component>
    {
        public override void Awake(View_Boom_Component component)
        {
            component.电机 = Scene01Helper.instance.电机;
            component.btnImgs = new Image[]
            { 
                component.Refs.Get<Image>("Btn0"),
                component.Refs.Get<Image>("Btn1"),
            };
            component.btnSprs = new Sprite[2,2]
            {
                { component.Refs.Get<Sprite>("分解") ,component.Refs.Get<Sprite>("组合")},
                { component.Refs.Get<Sprite>("返回 暗") ,component.Refs.Get<Sprite>("返回 亮")},
            };

            ZEvent.UIEvent.AddListener(component.btnImgs[0], component.Btn, 0);
            ZEvent.UIEvent.AddListener(component.btnImgs[1], component.Btn, 1);
        }
    }

    public class View_BoomUpdate : UpdateSystem<View_Boom_Component>
    {
        public override void Update(View_Boom_Component component)
        {
            if (component.tags == null) return;

            foreach (var item in component.tags)
            {
                var rect = item.Value;
                var part = item.Key.transform;

                var screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, part.position);
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(component.gameObject.transform as RectTransform,screenPos,null,out Vector2 localPos))
                {
                    rect.anchoredPosition = localPos;
                }
            }
        }
    }

    public class View_Boom_Show : UIShowSystem<View_Boom_Component>
    {
        public override void OnShow(View_Boom_Component canvas)
        {
            canvas.gameObject.SetActive(true);
            canvas.btnImgs[0].sprite = canvas.btnSprs[0, 0];
            canvas.btnImgs[1].sprite = canvas.btnSprs[1, 0];

            canvas.电机.rota = true;
            canvas.电机.transform.localRotation = Quaternion.identity;
            canvas.电机.open = false;
            canvas.电机.Value = 0;

            var cam = Camera.main.transform;
            cam.position = canvas.电机.transform.parent.Find("Camera").position;
            cam.rotation = canvas.电机.transform.parent.Find("Camera").rotation;

            var itemPre = canvas.Refs.Get<GameObject>("boomTagItemPre");

            canvas.tags = new Dictionary<DianJiPart, RectTransform>();
            var parts = canvas.电机.GetComponentsInChildren<DianJiPart>();
            foreach (var item in parts)
            {
                var itemRect = GameObject.Instantiate<GameObject>(itemPre, canvas.gameObject.transform).GetComponent<RectTransform>();
                itemRect.Find("Text").GetComponent<TextMeshProUGUI>().text = item.title;
                itemRect.GetComponent<CanvasGroup>().alpha = 0;
                canvas.tags.Add(item, itemRect);

                ZEvent.UIEvent.AddListener(itemRect, canvas.ClickTag, item);
            }

            canvas.Refs.Get<GameObject>("Plane").SetActive(false);
        }
    }

    public class View_Boom_Hide : UIHideSystem<View_Boom_Component>
    {
        public override void OnHide(View_Boom_Component canvas)
        {
            canvas.gameObject.SetActive(false);

            canvas.电机.rota = true;
            canvas.电机.transform.localRotation = Quaternion.identity;
            canvas.电机.open = false;
            canvas.电机.Value = 0;

            Game.Task.ResetCamera(false);
            //var cam = Camera.main.transform;
            //cam.position = Game.Task.camIdelPos;
            //cam.rotation = Game.Task.camIdleRot;

            foreach (var item in canvas.tags)
            {
                GameObject.Destroy(item.Value.gameObject);
            }
            canvas.tags = null;
        }
    }

    //逻辑
    public static class View_Boom_System
    {
        public static void Btn(this View_Boom_Component component, UIEventData<int> eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    SoundHelper.MouseEnter();
                    if (eventData.Data0 == 0)
                    {
                        //component.btnImgs[eventData.Data0].sprite = component.btnSprs[eventData.Data0, 1];
                    }
                    else
                    {
                        component.btnImgs[eventData.Data0].sprite = component.btnSprs[eventData.Data0, 1];
                    }
                    break;
                case UIEventType.Exit:
                    if (eventData.Data0 == 0)
                    {
                        //component.btnImgs[eventData.Data0].sprite = component.btnSprs[eventData.Data0, 0];
                    }
                    else
                    {
                        component.btnImgs[eventData.Data0].sprite = component.btnSprs[eventData.Data0, 0];
                    }
                    break;
                case UIEventType.Click:
                    if (eventData.Data0 == 0)
                    {
                        //分解
                        component.电机.open = !component.电机.open;
                        foreach (var item in component.tags.Values)
                        {
                            item.GetComponent<CanvasGroup>().DOFade(component.电机.open ? 1 : 0, 0.5f);
                        }
                        component.电机.transform.DOKill();
                        if (component.电机.open)
                        {
                            component.电机.rota = false;
                            component.电机.transform.DORotate(Vector3.zero, 0.5f);
                        }
                        else
                        {
                            component.电机.rota = true;
                        }
                        component.btnImgs[0].sprite = component.btnSprs[0, component.电机.open ? 1 : 0];
                    }
                    else
                    {
                        (Game.UI.Get(UIType.View_Left) as View_Left_Component).Clear();
                        //退出
                    }
                    break;
            }
        }

        public static void ClickTag(this View_Boom_Component component, UIEventData<DianJiPart> eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    if (component.电机.open)
                    {
                        var outlie = eventData.Data0.GetComponent<EPOOutline.Outlinable>();
                        if (outlie != null)
                        {
                            outlie.enabled = true;
                        }
                        component.ShowInfo(eventData.Data0.title, eventData.Data0.info);
                    }
                    break;
                case UIEventType.Exit:
                    var outline = eventData.Data0.GetComponent<EPOOutline.Outlinable>();
                    if (outline != null)
                    {
                        outline.enabled = false;
                    }
                    component.HideInfo();
                    break;
                case UIEventType.Click:
                    //if (!string.IsNullOrEmpty(eventData.Data0.info))
                    //{
                    //    if (component.电机.open)
                    //    {
                    //        (Game.UI.Show(UIType.View_Message) as View_Message_Component).ShowInfo(eventData.Data0.title, eventData.Data0.info, (Action)null);
                    //    }
                    //}
                    break;
            }

        }
        public static void ShowInfo(this View_Boom_Component component,string title,string info)
        {
            if (string.IsNullOrEmpty(info))
            {
                component.HideInfo();
            }
            else
            {
                component.Refs.Get<GameObject>("Plane").SetActive(true);
                component.Refs.Get<TextMeshProUGUI>("title").text = title;
                component.Refs.Get<TextMeshProUGUI>("info").text = "\u3000\u3000\u3000\u3000\u3000\u3000\u3000\u3000" + info;
            }
        }
        public static void HideInfo(this View_Boom_Component component)
        {
            component.Refs.Get<GameObject>("Plane").SetActive(false);
        }
    }
}
