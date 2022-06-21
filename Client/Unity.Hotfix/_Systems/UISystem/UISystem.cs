﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ZFramework
{
    public class UISystemAwake : AwakeSystem<UIComponent>
    {
        public override void Awake(UIComponent component)
        {
            var ui = new UnityEngine.GameObject("[UI]");
            var canvas = ui.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = ui.AddComponent<CanvasScaler>();
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            component.root = ui.transform;
            UnityEngine.GameObject.DontDestroyOnLoad(component.root.gameObject);

            component.UICanvas = new Dictionary<string, UICanvasComponent>();

            BulidUITypeMaps(component);
            BulidUILiveMaps(component);
        }

        void BulidUITypeMaps(UIComponent component)
        {
            //UI类型映射表
            component.types = new Dictionary<string, Type>();
            var uitypes = AssemblyLoader.GetTypesByAttribute(typeof(UITypeAttribute));
            foreach (var uitype in uitypes)
            {
                if (uitype.IsAbstract)
                {
                    continue;
                }
                var attributes = uitype.GetCustomAttributes(false);
                if (attributes.Length > 0)
                {
                    var uiType = ((UITypeAttribute)attributes[0]).Name;
                    component.types.Add(uiType, uitype);
                }
            }
        }
        void BulidUILiveMaps(UIComponent component)
        {
            //UI生命周期映射表
            component.maps = new Dictionary<Type, Dictionary<Type, IUILiveSystem>>();
            foreach (Type uiLiveTypes in AssemblyLoader.GetTypesByAttribute(typeof(UILiveAttribute)))
            {
                object uiLiveSystemObj = Activator.CreateInstance(uiLiveTypes);
                if (uiLiveSystemObj is IUILiveSystem iSystem)
                {
                    if (!component.maps.ContainsKey(iSystem.ComponentType))
                    {
                        component.maps.Add(iSystem.ComponentType, new Dictionary<Type, IUILiveSystem>());
                    }
                    if (!component.maps[iSystem.ComponentType].ContainsKey(iSystem.UILiveType))
                    {
                        component.maps[iSystem.ComponentType].Add(iSystem.UILiveType, iSystem);
                    }
                    else
                    {
                        Log.Error($"重复的UI生命周期!-->{iSystem.ComponentType.Name}:{iSystem.UILiveType.Name}");
                    }
                }
            }
        }

    }

    public static class UISystem
    {
        public static UICanvasComponent Create(this UIComponent component, string uiType)
        {
            var uiPre = Game.Root.GetComponent<BundleComponent>().LoadAsset(uiType + ".unity3d", uiType) as GameObject;
            if (uiPre != null && component.types.TryGetValue(uiType,out Type type))
            {
                var uicanvas = (UICanvasComponent)Component.CreateComponent(type);
                var uiInstance = UnityEngine.GameObject.Instantiate<GameObject>(uiPre, component.root);
                uicanvas.gameObject = uiInstance;
                uicanvas.rect = uiInstance.GetComponent<RectTransform>();

                component.UICanvas.Add(uiType, uicanvas);
                PlayLoop.Instance.Awake(uicanvas);
                return uicanvas;
            }
            else
            {
                Log.Info("Create" + uiType + " Fail");
                Log.Info(uiPre != null);
                return null;
            }
        }

        public static UICanvasComponent Show(this UIComponent component, string uiType)
        {
            if (component.UICanvas.TryGetValue(uiType,out UICanvasComponent uICanvas))//拿实体
            {
                var uiCanvasType = uICanvas.GetType();
                if (component.maps.TryGetValue(uiCanvasType, out Dictionary<Type,IUILiveSystem> value))//通过实体类型拿辅助对象
                {
                    if (value.TryGetValue(typeof(IUIShow),out IUILiveSystem system))
                    {
                        ((IUIShow)system).OnShow(uICanvas);//通过辅助对象执行生命周期
                    }
                }
                return uICanvas;
            }
            else
            {
                if (component.Create(uiType) != null)
                {
                    return component.Show(uiType);
                }
                else
                {
                    Log.Info("Show" + uiType + " Fail");
                    return null;
                }
            }
        }

        public static UICanvasComponent Get(this UIComponent component, string uiType)
        {
            if (component.UICanvas.TryGetValue(uiType, out UICanvasComponent uICanvas))//拿实体
            {
                return uICanvas;
            }
            return null;
        }

        public static void Hide(this UIComponent component, string uiType)
        {
            if (component.UICanvas.TryGetValue(uiType, out UICanvasComponent uICanvas))//拿实体
            {
                var uiCanvasType = uICanvas.GetType();
                if (component.maps.TryGetValue(uiCanvasType, out Dictionary<Type, IUILiveSystem> value))//通过实体类型拿辅助对象
                {
                    if (value.TryGetValue(typeof(IUIHide), out IUILiveSystem system))
                    {
                        ((IUIHide)system).OnHide(uICanvas);//通过辅助对象执行生命周期
                    }
                }
            }
        }
    }
}
