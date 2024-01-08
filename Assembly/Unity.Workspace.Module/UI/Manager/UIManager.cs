using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace ZFramework
{
    public enum UISpaceType
    {
        TwoD,
        ThreeD
    }
    public sealed partial class UIManager : GlobalComponent<UIManager>, IAwake<int, int>
    {
        //UI设置映射表
        private readonly UISettingsMapper settings = new UISettingsMapper();
        //所有UI渲染层
        private Dictionary<UISortLayer, RenderSortProcessor> processor;
        //活跃窗口
        internal UIWindow ActiveWindow { get; set; }

        void IAwake<int, int>.Awake(int width, int height)
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
            processor = new Dictionary<UISortLayer, RenderSortProcessor>();
            UISortLayer[] layers = Enum.GetValues(typeof(UISortLayer)).Cast<UISortLayer>().ToArray();
            for (int i = 0, len = layers.Length; i < len; i++)
            {
                UISortLayer layer = layers[i];
                short minOrder = (short)layer;
                short maxOrder = i == (len - 1) ? short.MaxValue : (short)(layers[i + 1] - 1);

                RenderSortProcessor collections = Entity.AddComponentInChild<RenderSortProcessor>();
                collections.Init(layer, minOrder, maxOrder, root.transform);
                processor.Add(layer, collections);
            }
        }
        void Init3D(GameObject parent)
        {
            GameObject root = new GameObject("3D");
            root.transform.SetParent(parent.transform);
        }

        internal UISettingAttribute GetSettings(Type type)
        {
            return settings.GetSettings(type);
        }
        internal RenderSortProcessor GetProcessor(UISortLayer layer)
        {
            return processor[layer];
        }


        internal async ATask<UIWindow> CreateWindowInner(Type type, UIWindow parent = null, bool isModal = false)
        {
            //读配置
            UISettingAttribute settings = GetSettings(type);
            //加载资源
            GameObject prefab = await VirtualFileSystem.LoadAsync<GameObject>(settings.AssetPath);
            //实例化
            RenderSortProcessor processor;
            RectTransform parentRect;
            Entity entity;
            if (parent == null)//创建根窗体
            {
                processor = GetProcessor(settings.Layer);
                parentRect = processor.rectTransform;
                entity = processor.Entity;
            }
            else//创建子窗体
            {
                processor = GetProcessor(parent.Layer);
                parentRect = parent.rectTransform;
                entity = parent.Entity;
            }
            //实例化
            GameObject gameObject = GameObject.Instantiate(prefab, parentRect);
            UIWindowInitParams paramas = new UIWindowInitParams()
            {
                gameObject = gameObject,
                processor = processor,
                parent = parent,
                isModal = isModal,
            };
            UIWindow window = entity.AddComponentInChild(type, paramas) as UIWindow;
            return window;
        }
        internal void PopWindowInner(UIWindow window)
        {
            ThrowIfNull(window);
            GetProcessor(window.Layer).PopWindow(window);
        }
        internal void OpenWindowInner(UIWindow window)
        {
            ThrowIfNull(window);
            throw new NotImplementedException();
        }
        internal void CloseWindowInner(UIWindow window)
        {
            ThrowIfNull(window);
            throw new NotImplementedException();
        }
        internal void DestoryWindowInner(UIWindow window)
        {
            ThrowIfNull(window);
            throw new NotImplementedException();
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