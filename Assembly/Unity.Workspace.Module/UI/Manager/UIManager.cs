using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace ZFramework
{
    public sealed partial class UIManager : GlobalComponent<UIManager>
    {
        //UI设置
        private readonly UISettingsMapper settings = new UISettingsMapper();
        //所有UI渲染层
        private Dictionary<UILayer, RenderSortProcessor> processor;
        //活跃窗口
        private UIWindow activeWindow;

        public void Init(int width, int height)
        {
            //加载UI配置
            settings.Load();

            //创建根节点
            GameObject go = new GameObject("[UI]");
            GameObject.DontDestroyOnLoad(go);

            //创建2D节点
            Init2D(go, width, height);

            //创建3D节点
            Init3D(go);
        }
        void Init2D(GameObject parent,int width,int height)
        {
            GameObject root = new GameObject("2D");
            root.transform.SetParent(parent.transform);

            //创建canvas
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = root.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            scaler.referenceResolution = new Vector2(width, height);

            //创建层节点
            processor = new Dictionary<UILayer, RenderSortProcessor>();
            UILayer[] layers = Enum.GetValues(typeof(UILayer)).Cast<UILayer>().ToArray();
            for (int i = 0, len = layers.Length; i < len; i++)
            {
                UILayer layer = layers[i];
                short minOrder = (short)layer;
                short maxOrder = i == (len - 1) ? short.MaxValue : (short)(layers[i + 1] - 1);

                RectTransform rectTransform = new GameObject($"{layer} [{minOrder}-{maxOrder}]").AddComponent<RectTransform>();
                rectTransform.SetParent(root.transform);
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;

                RenderSortProcessor collections = Entity.AddComponentInChild<RenderSortProcessor>();
                collections.Init(minOrder, maxOrder, rectTransform);
                processor.Add(layer, collections);
            }
        }
        void Init3D(GameObject parent)
        {
            GameObject root = new GameObject("3D");
            root.transform.SetParent(parent.transform);
        }

        UISettingAttribute GetSettings(Type type)
        {
            return settings.GetSettings(type);
        }
        RenderSortProcessor GetProcessor(UILayer layer)
        {
            return processor[layer];
        }
        void SetActiveWindow(UIWindow window)
        {
            if (activeWindow == window)
            {
                return;
            }
            if (activeWindow != null)
            {
                activeWindow.OnAcviceChange(false);
                activeWindow = null;
            }
            if (window == null)
            {
                return;
            }
            if (window.InModal)//如果是被模态中窗口   则找这个模态窗口的最深子   活跃他的最深子
            {
                activeWindow = GetProcessor(window.Layer).GetDeepestChildNode(window.InstanceID).Window;
            }
            else
            {
                activeWindow = window;
            }
            if (activeWindow != null)
            {
                activeWindow.OnAcviceChange(true);
            }
        }

        internal void RegistryWindow(UIWindow window)
        {
            GetProcessor(window.Layer).Registry(window);
        }
        internal void UnregistryWindow(UIWindow window)
        {
            RenderDepthNode left = GetProcessor(window.Layer).Unregistry(window);
            if (left != null)
            {
                SetActiveWindow(left.Window);
            }
        }
        internal async ATask<UIWindow> CreateWindowInner(Type type, UIWindow parent = null, bool isModal = false)
        {
            //读配置
            UISettingAttribute settings = GetSettings(type);
            //构造初始化参数
            RectTransform parentRect;
            Entity entity;
            UILayer layer;
            RenderSortProcessor processor;
            if (parent == null)//创建根窗体
            {
                layer = settings.Layer;
                processor = GetProcessor(layer);
                parentRect = processor.rectTransform;
                entity = processor.Entity;
            }
            else//创建子窗体
            {
                layer = parent.Layer;
                processor = GetProcessor(layer);
                parentRect = parent.rectTransform;
                entity = parent.Entity;
            }
            //加载资源
            GameObject prefab = await VirtualFileSystem.LoadAsync<GameObject>(settings.AssetPath);
            //实例化
            GameObject gameObject = GameObject.Instantiate(prefab, parentRect);
            UIWindowInitParams paramas = new UIWindowInitParams()
            {
                gameObject = gameObject,
                layer = layer,
                parent = parent,
                isModal = isModal,
            };
            UIWindow window = entity.AddComponentInChild(type, paramas) as UIWindow;
            OpenWindowInner(window);
            return window;
        }
        internal void PopWindowInner(UIWindow window)
        {
            ThrowIfNull(window);
            GetProcessor(window.Layer).PopNode(window.InstanceID);
            SetActiveWindow(window);
        }
        internal void OpenWindowInner(UIWindow window)
        {
            ThrowIfNull(window);
            window.CallOpen();
            SetActiveWindow(window);
        }
        internal void CloseWindowInner(UIWindow window)
        {
            ThrowIfNull(window);
            RenderSortProcessor processor = GetProcessor(window.Layer);
            RenderDepthNode start = processor.GetNode(window.InstanceID);
            RenderDepthNode end = processor.GetDeepestChildNode(window.InstanceID);
            while (start != end)//倒序关闭
            {
                end.Window.CallClose();
                end = end.Left;
            }
            start.Window.CallClose();
            SetActiveWindow(start.Left.Window);
        }
        internal void DestoryWindowInner(UIWindow window)
        {
            ThrowIfNull(window);
            Entity.Destory(window.Entity);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ThrowIfNull(UIWindow window)
        {
            if (window == null)
            {
                throw new ArgumentNullException();
            }
        }
    }
}