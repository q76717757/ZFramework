using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    internal struct UIWindowInitParams
    {
        public GameObject gameObject;
        public UIWindow parent;
        public RenderSortProcessor processor;
        public bool isModal;
    }

    public abstract class UIWindow : Component, IAwake<UIWindowInitParams>
    {
        [Flags]
        private enum ModalState : byte
        {
            //非模态
            NotModal = 1 << 0,
            //模态
            IsModal = 1 << 1,
            //被模态
            InModel = 1 << 2,
        }

        private ModalState state;
        private Canvas canvas;
        private List<UIWindow> children;

        public UISortLayer Layer { get; private set; }
        public GameObject gameObject { get; private set; }
        public RectTransform rectTransform { get; private set; }
        public  UIWindow Parent { get;private set; }
        public bool InModal { get => StateIs(ModalState.InModel); }
        public int Order { get => canvas.sortingOrder; set => canvas.sortingOrder = value; }

        void IAwake<UIWindowInitParams>.Awake(UIWindowInitParams args)
        {
            Layer = args.processor.layer;
            state = args.isModal ? ModalState.IsModal : ModalState.NotModal;

            //绑定window和unity资源
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
            args.processor.Registry(this);
        }

        bool StateIs(ModalState state)
        {
            return (this.state & state) == state;
        }
        void LinkParentAndChild(UIWindow parent, UIWindow child)
        {
            if (child != null)
            {
                child.Parent = parent;
                if (parent != null)
                {
                    if (parent.children == null)
                        parent.children = new List<UIWindow>();
                    parent.children.Add(child);
                }
                UpdateModalState(child, true);
            }
        }
        void UnlinkParentAndChild(UIWindow parent, UIWindow child)
        {
            if (child != null)
            {
                child.Parent = null;
                if (parent != null)
                {
                    parent.children.Remove(child);
                }
                UpdateModalState(child, false);
            }
        }
        void UpdateModalState(UIWindow window,bool additivity)
        {
            if (!window.StateIs(ModalState.IsModal))
            {
                return;
            }
            UIWindow parent = window.Parent;
            while (parent != null)
            {
                if (additivity)
                {
                    parent.state |= ModalState.InModel;
                }
                else
                {
                    parent.state &= ~ModalState.InModel;
                }
                if (parent.StateIs(ModalState.IsModal))
                {
                    return;
                }
                parent = parent.Parent;
            }
        }

        //生命周期
        internal void CallOpen()
        {
        }
        internal void CallClose()
        {
        }
        public virtual void OnAcviceChange(bool isActive)
        {
        }
        public virtual void OnShow()
        {
        }
        public virtual void OnHide()
        {
        }
        public virtual void OnDestroy()
        {
        }


        //API  创建子窗口
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
    }
}
