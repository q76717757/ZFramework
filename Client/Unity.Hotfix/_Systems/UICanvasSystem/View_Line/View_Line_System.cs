
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
    public class View_LineAwake : AwakeSystem<View_Line_Component>
    {
        public override void Awake(View_Line_Component component)
        {
            component.LineRoot = component.Refs.Get<RectTransform>("LineRoot");
            //左边的设备
            component.btns = new Dictionary<DeviceType, RectTransform>()
            {
                [DeviceType.电机控制器] = component.Refs.Get<RectTransform>("1"),
                [DeviceType.示波器] = component.Refs.Get<RectTransform>("2"),
                [DeviceType.三相交流电源] = component.Refs.Get<RectTransform>("3"),
                [DeviceType.扭矩传感器] = component.Refs.Get<RectTransform>("4"),
                [DeviceType.直流电源] = component.Refs.Get<RectTransform>("5"),
                //[DeviceType.电流夹] = component.Refs.Get<RectTransform>("6"),
            };
            //右边的设备位置
            component.box = new Dictionary<DeviceType, RectTransform>()
            {
                [DeviceType.电机控制器] = component.Refs.Get<RectTransform>("Box0"),
                [DeviceType.示波器] = component.Refs.Get<RectTransform>("Box1"),
                [DeviceType.三相交流电源] = component.Refs.Get<RectTransform>("Box2"),
                [DeviceType.扭矩传感器] = component.Refs.Get<RectTransform>("Box3"),
                [DeviceType.直流电源] = component.Refs.Get<RectTransform>("Box4"),
            };
            //放上去的设备
            component.devices = new Dictionary<DeviceType, GameObject>()
            {
                [DeviceType.电机控制器] = component.Refs.Get<GameObject>("D0"),
                [DeviceType.示波器] = component.Refs.Get<GameObject>("D1"),
                [DeviceType.三相交流电源] = component.Refs.Get<GameObject>("D2"),
                [DeviceType.扭矩传感器] = component.Refs.Get<GameObject>("D3"),
                [DeviceType.直流电源] = component.Refs.Get<GameObject>("D4"),
            };
            foreach (var item in component.btns)
            {
                ZEvent.UIEvent.AddListener(item.Value.gameObject, component.OnLeftBtn, item.Key);
            }

            foreach (var item in component.box)
            {
                ZEvent.UIEvent.AddListener(item.Value.gameObject, component.OnRightBox, item.Key);
            }

            //连线的端口
            component.Ports = new Dictionary<DragPortType, DragPort>();
            var dps = component.gameObject.transform.GetComponentsInChildren<DragPort>();
            foreach (var p in dps)
            {
                component.Ports.Add(p.type, p);
                ZEvent.UIEvent.AddListener(p, component.OnDragPort, p);
            }

            ZEvent.UIEvent.AddListener(component.Refs.Get<Image>("Image"), component.Close);
        }
    }

    public class View_Line_Show : UIShowSystem<View_Line_Component>
    {
        public override void OnShow(View_Line_Component canvas)
        {
            canvas.gameObject.SetActive(true);
            foreach (var item in canvas.btns)
            {
                var btn = item.Value.GetComponent<Image>();
                btn.color = Color.white;
                btn.raycastTarget = true;
            }
            foreach (var item in canvas.box)
            {
                item.Value.gameObject.SetActive(true);
                item.Value.gameObject.GetComponent<Image>().color = Color.gray;
            }
            foreach (var item in canvas.devices)
            {
                item.Value.SetActive(false);
            }
            foreach (var item in canvas.Ports)
            {
                if (item.Value.UsingLine != null)
                    UnityEngine.GameObject.Destroy(item.Value.UsingLine.gameObject);
                item.Value.UsingLine = null;

                //item.Value.GetComponent<Image>().color = Color.white;
            }
            canvas.Ports.Remove(DragPortType.电流夹左);
            canvas.Ports.Remove(DragPortType.电流夹中);
            canvas.Ports.Remove(DragPortType.电流夹右);
        }
    }
    public class View_Line_Hide : UIHideSystem<View_Line_Component>
    {
        public override void OnHide(View_Line_Component canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_Line_System
    {
        public static void OnLeftBtn(this View_Line_Component component, UIEventData<DeviceType> eventData)
        {
            if (eventData.PointerType != PointerType.Left) return;
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    eventData.Target.GetComponent<Image>().color = Color.green;
                    break;
                case UIEventType.Exit:
                    eventData.Target.GetComponent<Image>().color = Color.white;
                    break;
                case UIEventType.Down:
                    if (component.HoldingBtn == null && RectTransformUtility.ScreenPointToLocalPointInRectangle(component.LineRoot, eventData.Position, null, out Vector2 pos))
                    {
                        component.HoldingType = eventData.Data0;
                        component.HoldingBtn = GameObject.Instantiate<GameObject>(component.btns[eventData.Data0].gameObject,component.LineRoot);

                        component.HoldingBtn.GetComponent<RectTransform>().anchoredPosition = pos;
                        component.HoldingBtn.GetComponent<Image>().raycastTarget = false;

                        UnityEngine.GameObject.Destroy(component.HoldingBtn.GetComponent<UIEventDriver>());
                    }
                    break;
                case UIEventType.Up:
                    if (component.HoldingBtn != null)
                    {
                        UnityEngine.GameObject.Destroy(component.HoldingBtn);
                        component.HoldingBtn = null;

                        if (component.CanPutDown)
                        {
                            component.box[component.HoldingType].gameObject.SetActive(false);
                            component.devices[component.HoldingType].gameObject.SetActive(true);
                            var btn = component.btns[component.HoldingType].GetComponent<Image>();
                            btn.color = btn.color * new Color(1, 1, 1, 0.2f);
                            btn.raycastTarget = false;
                            component.CanPutDown = false;
                        }
                    }
                    break;
                case UIEventType.Drag:
                    if (component.HoldingBtn != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(component.LineRoot, eventData.Position, null, out pos))
                    {
                        component.HoldingBtn.GetComponent<RectTransform>().anchoredPosition = pos;
                    }
                    break;
            }
        }
        public static void OnRightBox(this View_Line_Component component,UIEventData<DeviceType> eventData)
        {
            if (eventData.PointerType != PointerType.Left) return;
            if (component.HoldingBtn == null) return;
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    if (eventData.Data0 == component.HoldingType)
                    {
                        eventData.Target.GetComponent<Image>().color = Color.white;
                        component.CanPutDown = true;
                    }
                    break;
                case UIEventType.Exit:
                    if (eventData.Data0 == component.HoldingType)
                    {
                        eventData.Target.GetComponent<Image>().color = Color.gray;
                        component.CanPutDown = false;
                    }
                    break;
            }
        }

        public static void OnDragPort(this View_Line_Component component,UIEventData<DragPort> eventData)
        {
            if (eventData.PointerType != PointerType.Left) return;
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    eventData.Data0.OnPointEnter();
                    component.EnterPort = eventData.Data0;
                    if (component.Ports.TryGetValue(eventData.Data0.targetType, out DragPort dp))
                    { 
                        dp.OnPointEnter();
                    }
                    break;
                case UIEventType.Exit:
                    eventData.Data0.OnPointExit();
                    component.EnterPort = null;
                    if (component.Ports.TryGetValue(eventData.Data0.targetType, out dp))
                    {
                        dp.OnPointExit();
                    }
                    break;
                case UIEventType.Down:
                    if (eventData.Data0.UsingLine != null || component.HoldPort != null) return;

                    component.HoldPort = eventData.Data0;
                    component.DrawLine = GameObject.Instantiate<GameObject>(component.Refs.Get<GameObject>("LinePre"), component.LineRoot).GetComponent<DrawLine>();
                    component.DrawLine.SetStartPos(eventData.Data0.transform.position, eventData.Data0.LineColor);

                    break;
                case UIEventType.Up:
                    if (component.HoldPort == null) return;

                    if (component.EnterPort == null)
                    {
                        UnityEngine.GameObject.Destroy(component.DrawLine.gameObject);
                    }
                    else
                    {
                        if (component.EnterPort.UsingLine == null && component.EnterPort.targetType == component.HoldPort.type && component.HoldPort.targetType == component.EnterPort.type)
                        {
                            component.HoldPort.UsingLine = component.DrawLine;
                            component.EnterPort.UsingLine = component.DrawLine;

                            if (component.EnterPort.targetType == DragPortType.W || component.HoldPort.targetType == DragPortType.W)
                            {
                                Vector3 ppos;
                                if (component.HoldPort.targetType == DragPortType.W)
                                {
                                    ppos = component.EnterPort.transform.position + (component.HoldPort.transform.position - component.EnterPort.transform.position) * 0.3f;
                                }
                                else
                                {
                                    ppos = component.HoldPort.transform.position + (component.EnterPort.transform.position - component.HoldPort.transform.position) * 0.3f;
                                }

                                var port = component.DrawLine.Open电流夹(DragPortType.电流夹左, ppos, component.Ports[DragPortType.示波器左].LineColor);
                                ZEvent.UIEvent.AddListener(port, component.OnDragPort, port);
                                component.Ports.Add(DragPortType.电流夹左, port);
                            }
                            if (component.EnterPort.targetType == DragPortType.V || component.HoldPort.targetType == DragPortType.V)
                            {
                                Vector3 ppos;
                                if (component.HoldPort.targetType == DragPortType.V)
                                {
                                    ppos = component.EnterPort.transform.position + (component.HoldPort.transform.position - component.EnterPort.transform.position) * 0.5f;
                                }
                                else
                                {
                                    ppos = component.HoldPort.transform.position + (component.EnterPort.transform.position - component.HoldPort.transform.position) * 0.5f;
                                }
                                var port = component.DrawLine.Open电流夹(DragPortType.电流夹中, ppos, component.Ports[DragPortType.示波器中].LineColor);
                                ZEvent.UIEvent.AddListener(port, component.OnDragPort, port);
                                component.Ports.Add(DragPortType.电流夹中, port);
                            }
                            if (component.EnterPort.targetType == DragPortType.U || component.HoldPort.targetType == DragPortType.U)
                            {
                                Vector3 ppos;
                                if (component.HoldPort.targetType == DragPortType.U)
                                {
                                    ppos = component.EnterPort.transform.position + (component.HoldPort.transform.position - component.EnterPort.transform.position) * 0.8f;
                                }
                                else
                                {
                                    ppos = component.HoldPort.transform.position + (component.EnterPort.transform.position - component.HoldPort.transform.position) * 0.8f;
                                }
                                var port = component.DrawLine.Open电流夹(DragPortType.电流夹右, ppos, component.Ports[DragPortType.示波器右].LineColor);
                                ZEvent.UIEvent.AddListener(port, component.OnDragPort, port);
                                component.Ports.Add(DragPortType.电流夹右, port);
                                
                            }

                            if (component.EnterPort.targetType == DragPortType.电流夹左 ||
                                component.EnterPort.targetType == DragPortType.电流夹中 ||
                                component.EnterPort.targetType == DragPortType.电流夹右
                                )
                            {
                                component.DrawLine.localLine.Find("Image2").gameObject.SetActive(true);
                            }

                            if (component.HoldPort.targetType == DragPortType.电流夹左 ||
                                component.HoldPort.targetType == DragPortType.电流夹中 ||
                                component.HoldPort.targetType == DragPortType.电流夹右)
                            {
                                component.DrawLine.localLine.Find("Image").gameObject.SetActive(true);
                            }

                            component.EnterPort.Stop();
                            component.HoldPort.Stop();
                            //component.HoldPort.GetComponent<Image>().color = Color.white;
                            //component.EnterPort.GetComponent<Image>().color = Color.white;

                            component.DragLine(component.EnterPort.transform.position);
                            component.CheckFinish();
                        }
                        else
                        {
                            UnityEngine.GameObject.Destroy(component.DrawLine.gameObject);
                        }
                    }
                    component.DrawLine = null;
                    component.HoldPort = null;
                    break;
                case UIEventType.Drag:
                    component.DragLine(eventData.Position);
                    break;
            }
        }
        public static void DragLine(this View_Line_Component component,Vector2 mousePos)
        {
            if (component.HoldPort == null) return;
            component.DrawLine.DragLine(mousePos);
        }

        public static void CheckFinish(this View_Line_Component component)
        {
            bool isFinish = true;
            foreach (var item in component.Ports.Values)
            {
                if (item.UsingLine == null)
                {
                    Log.Info(item.type + "为连接");
                    isFinish = false;
                }
            }

            if (Define.RunMode == RunMode.HotReload_EditorRuntime || isFinish)
            {
                (Game.UI.Show(UIType.View_Message) as View_Message_Component).ShowInfo("提示", "恭喜您,已经完成接线步骤,请让我们开始下一步骤吧!", component.Finish);
            }
        }

        public static void Finish(this View_Line_Component component)
        {
            Game.UI.Hide(UIType.View_Line);
            Game.Task.Run(0);
        }

        public static void Close(this View_Line_Component component, UIEventData eventData)
        {
            if (eventData.EventType == UIEventType.Click)
            {
                //(Game.UI.Get(UIType.View_Left) as View_Left_Component).Clear();
                component.Finish();
            }
        }
    }
}
