using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    /// <summary>
    /// UI窗体  以Canvas为单位
    /// </summary>
    [FoucsCall]
    public abstract class UIWindowBase : IUIGameLoop
    {
        internal UIWindowBase Parent { get; private set; }
        internal UIWindowBase Root
        {
            get
            {
                UIWindowBase result = this;
                UIWindowBase recursionParent = Parent;
                while (recursionParent != null)
                {
                    if (recursionParent != null)
                        result = recursionParent;
                    recursionParent = recursionParent.Parent;
                }
                return result;
            }
        }
        public int ID { get; private set; }
        public bool IsModal { get; private set; }

        internal UIGroupType GroupType { get; private set; }
        internal bool Active { get; private set; }
        internal GameObject GameObject { get; private set; }
        internal Canvas Canvas
        {
            get;
            private set;
        }
        internal int Order
        {
            get
            {
                return Canvas.sortingOrder;
            }
            set
            {
                if (Canvas.overrideSorting == false)
                    Canvas.overrideSorting = true;
                Canvas.sortingOrder = value;
            }
        }
        protected References References
        {
            get;
            private set;
        }
        private CanvasGroup canvasGroup;
        protected CanvasGroup CanvasGroup
        {
            get
            {
                if (canvasGroup == null)
                {
                    canvasGroup = GameObject.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        canvasGroup = GameObject.AddComponent<CanvasGroup>();
                        canvasGroup.alpha = Active ? 1 : 0;
                    }
                }
                return canvasGroup;
            }
        }
        internal Dictionary<int, UIWindowBase> Children { get; private set; } = new Dictionary<int, UIWindowBase>();
        internal List<UIControl> uiContorls = new List<UIControl>();
        internal UIWindowBase Register(int id, UIGroupType groupType, bool isModal, GameObject gameObject)
        {
            this.ID = id;
            this.GroupType = groupType;
            this.IsModal = isModal;
            this.GameObject = gameObject;
            this.Canvas = gameObject.GetComponent<Canvas>();
            this.References = gameObject.GetComponent<References>();
            return this;
        }
        void IUIGameLoop.OnAwake()
        {
            OnAwake();
#if UNITY_EDITOR
            GameObject.AddComponent<UIWindowVisible>().Register(this);
#endif
        }
        void IUIGameLoop.OnDestroy()
        {
            if (Parent != null && Parent.Children.ContainsKey(ID))
                Parent.Children.Remove(ID);
            OnDestroy();
        }
        void IUIGameLoop.OnOpen()
        {
            SetVisible(true);
            Active = true;
            OnOpen();
        }
        void IUIGameLoop.OnClose()
        {
            SetVisible(false);
            Active = false;
            OnClose();
        }
        void IUIGameLoop.OnUpdate()
        {
            OnUpdate();
        }
        void IUIGameLoop.OnFocus()
        {
            OnFocus();
        }
        void IUIGameLoop.OnLoseFocus()
        {
            OnLoseFocus();
        }
        void IUIGameLoop.SetInteractable(bool state)
        {
            CanvasGroup.interactable = state;
            CanvasGroup.blocksRaycasts = state;
            SetInteractable(state);
        }

        protected virtual void OnAwake() { }
        protected virtual void OnDestroy() { }
        protected abstract void OnOpen();
        protected abstract void OnClose();
        protected virtual void OnUpdate() { }
        protected virtual void OnFocus() { }
        protected virtual void OnLoseFocus() { }
        protected virtual void SetInteractable(bool state) { }
        protected virtual void SetVisible(bool state) => CanvasGroup.alpha = state ? 1 : 0;

        public T AddControl<T>(T control) where T : UIControl
        {
            uiContorls.Add(control);
            return control;
        }

        internal T OpenChild<T>(UIWindowBase child) where T : UIWindowBase
        {
            if (child == null)
                return default;
            return OpenChild<T>(child.ID);
        }

        internal T OpenChild<T>(int childID) where T : UIWindowBase
        {
            if (Children == null)
                return default;
            foreach (var item in Children.Values)
            {
                if (item.ID == childID)
                {
                    UIManager.Instance.Open(item);
                    return (T)item;
                }
            }
            return default;
        }

        public async ATask<T> OpenNewChildAsync<T>(bool isModal) where T : UIWindowBase
        {
            T instance = await AddChildAsync<T>(isModal);
            UIManager.Instance.Open(instance);
            return instance;
        }

        public async ATask<T> AddChildAsync<T>(bool isModal ) where T : UIWindowBase
        {
            T instance = await UIManager.Instance.Creat<T>(isModal);
            Children.Add(instance.ID, instance);
            instance.Parent = this;
            return instance;
        }

        //深度优先,递归添加子UI
        internal void GetAllChildrenWindow(ref List<UIWindowBase> list, Func<UIWindowBase, bool> func)
        {
            if (Children != null)
            {
                foreach (var item in Children.Values)
                {
                    if (func == null || func(item))
                    {
                        list.Add(item);
                        item.GetAllChildrenWindow(ref list, func);
                    }
                }
            }
        }
    }

}
