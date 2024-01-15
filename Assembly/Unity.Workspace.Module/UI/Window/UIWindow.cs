using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    internal struct UIWindowInitParams
    {
        public GameObject gameObject;
        public UIWindow parent;
        public bool isModal;
        public UILayer layer;
    }

    public abstract class UIWindow : Component, IAwake<UIWindowInitParams> ,IDestory
    {
        [Flags]
        private enum WindowState : byte
        {
            None = 0,
            //固定的
            IsFixed = 1 << 0,  //TODO  固定窗口不参与排序也不能弹栈  未实现  后面看是否有必要??
            //模态的
            IsModal = 1 << 1,
            //可见的
            IsVisible = 1 << 2,
        }

        private WindowState state;
        private UILayer layer;
        private Canvas canvas;
        private UIWindow parent;
        private List<UIWindow> children;
        private int modalChild;

        public GameObject gameObject { get; private set; }
        public RectTransform rectTransform { get; private set; }
        public  UIWindow Parent { get => parent; }
        public UILayer Layer { get => layer; }
        public bool InModal { get => modalChild > 0; }
        public bool IsVisible { get => HasState(WindowState.IsVisible); }

        void IAwake<UIWindowInitParams>.Awake(UIWindowInitParams args)
        {
            layer= args.layer;
            state = args.isModal ? WindowState.IsModal : WindowState.None;

            //绑定window对象和unity资源
            gameObject = args.gameObject;
            canvas = gameObject.GetComponent<Canvas>();
            canvas.overrideSorting = true;
            rectTransform = gameObject.GetComponent<RectTransform>();

            if (!gameObject.TryGetComponent(out UIWindowLinker linker))
            {
                linker = gameObject.AddComponent<UIWindowLinker>();
            }
            linker.window = this;

            //绑定window间父子关系
            LinkParentAndChild(args.parent, this);

            //注册到UI渲染栈上
            UIManager.Instance.RegistryWindow(this);
        }

        //渲染排序
        internal void SetDepth(int order)
        {
            canvas.sortingOrder = order;
        }

        //窗口状态
        bool HasState(WindowState state)
        {
            return (this.state & state) == state;
        }
        void AddState(WindowState state)
        {
            this.state |= state;
        }
        void RemoveState(WindowState state)
        {
            this.state &= ~state;
        }

        //模态
        void AddModal(UIWindow window)
        {
            if (window.HasState(WindowState.IsModal))
            {
                UIWindow parent = window.parent;
                while (parent != null)
                {
                    parent.modalChild++;
                    parent = parent.parent;
                }
            }
        }
        void RemoveModal(UIWindow window)
        {
            if (window.HasState(WindowState.IsModal))
            {
                UIWindow parent = window.parent;
                while (parent != null)
                {
                    parent.modalChild--;
                    parent = parent.parent;
                }
            }
        }

        //父子关系
        void LinkParentAndChild(UIWindow parent, UIWindow child)
        {
            if (child != null)
            {
                child.parent = parent;
                if (parent != null)
                {
                    //添加父子关系
                    if (parent.children == null)
                        parent.children = new List<UIWindow>();
                    parent.children.Add(child);
                }
            }
        }
        void UnlinkParentAndChild(UIWindow parent, UIWindow child)
        {
            if (child != null)
            {
                child.parent = null;
                if (parent != null)
                {
                    parent.children.Remove(child);
                }
            }
        }


        //生命周期
        internal void CallOpen()
        {
            if (!IsVisible)
            {
                AddModal(this);
                AddState(WindowState.IsVisible);
                try
                {
                    OnShow();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        internal void CallClose()
        {
            if (IsVisible)
            {
                RemoveModal(this);
                RemoveState(WindowState.IsVisible);
                try
                {
                    OnHide();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        public virtual void OnShow()
        {
            canvas.enabled = true;
        }
        public virtual void OnHide()
        {
            canvas.enabled = false;
        }
        public virtual void OnAcviceChange(bool isActive)
        {
        }
        public virtual void OnDestory()
        {
            UIManager.Instance.UnregistryWindow(this);
            GameObject.Destroy(this.gameObject);
        }





        //API
        public void CreateChildWindow<T>(bool isModal) where T : UIWindow
        {
            UIManager.Instance.CreateWindowInner(typeof(T), this, isModal).Invoke();
        }
        public void CreateChildWindow(Type type, bool isModal)
        {
            UIManager.Instance.CreateWindowInner(type, this, isModal).Invoke();
        }
        public async ATask<T> CreateChildWindowAsync<T>(bool isModal) where T : UIWindow
        {
            return await UIManager.Instance.CreateWindowInner(typeof(T), this, isModal) as T;
        }
        public async ATask<UIWindow> CreateChildWindowAsync(Type type, bool isModal)
        {
            return await UIManager.Instance.CreateWindowInner(type, this, isModal);
        }



#if UNITY_EDITOR
        public override string ToString()
        {
            return InstanceID + ":" + canvas.sortingOrder + $"[{modalChild}]";
        }
#endif
    }
}
