using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZFramework
{
    public interface IUIGameLoop
    {
        void OnAwake();
        void OnDestroy();
        void OnOpen();
        void OnClose();
        void OnFocus();
        void OnLoseFocus();
        void OnUpdate();//考虑增加一个生命周期 一个是Active才Update 一个是全程Update(允许UI后台持续做些事情)
        void SetInteractable(bool state);
    }

    [FoucsCall]
    public abstract partial class UIWindowBase : IUIGameLoop
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
        internal UIPopType PopType { get; private set; }
        internal UIGroupType GroupType { get; private set; }
        internal bool Active { get; private set; }
        internal Transform Transform { get; private set; }
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
                    canvasGroup = Transform.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        canvasGroup = Transform.gameObject.AddComponent<CanvasGroup>();
                        canvasGroup.alpha = Active ? 1 : 0;
                    }
                }
                return canvasGroup;
            }
        }
        internal Dictionary<int, UIWindowBase> Children { get; private set; } = new Dictionary<int, UIWindowBase>();
        internal List<UIControl> uiContorls = new List<UIControl>();
        internal UIWindowBase Register(int id, UIGroupType groupType, UIPopType popType, Transform transform)
        {
            this.ID = id;
            this.GroupType = groupType;
            this.PopType = popType;
            this.Transform = transform;
            this.Canvas = transform.GetComponent<Canvas>();
            this.References = transform.GetComponent<References>();
            return this;
        }
        void IUIGameLoop.OnAwake()
        {
            OnAwake();
#if UNITY_EDITOR
            Transform.gameObject.AddComponent<UIWindowVisible>().Register(this);
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

        public async ATask<T> OpenNewChildAsync<T>() where T : UIWindowBase
        {
            T instance = await AddChildAsync<T>();
            UIManager.Instance.Open(instance);
            return instance;
        }

        public async ATask<T> AddChildAsync<T>() where T : UIWindowBase
        {
            T instance = await UIManager.Instance.Creat<T>();
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

    public abstract class UIControl
    {
        public UIWindowBase uiWindow;
        public UIControl(UIWindowBase ui)
        {
            this.uiWindow = ui;
        }
    }

    public class Button : UIControl
    {
        protected Graphic graphic;
        protected Action<UIEventDataBase> btnCallBack;
        public Button(UIWindowBase ui, Graphic graphic) : base(ui)
        {
            this.graphic = graphic;
        }

        public Button 注册事件(Action<UIEventData> data)
        {
            ZEvent.UIEvent.AddListener(graphic.gameObject, data);
            return this;
        }

        public Button 注册事件<T>(Action<UIEventData<T>> data)
        {
            ZEvent.UIEvent.AddListener(graphic.gameObject, data);
            return this;
        }

        public Button 注册事件<T, T1>(Action<UIEventData<T, T1>> data)
        {
            ZEvent.UIEvent.AddListener(graphic.gameObject, data);
            return this;
        }

        public Button 移除事件(Action<UIEventData> data)
        {
            ZEvent.UIEvent.RemoveListener(graphic.gameObject, data);
            return this;
        }

        public Button 移除事件<T>(Action<UIEventData<T>> data)
        {
            ZEvent.UIEvent.RemoveListener(graphic.gameObject, data);
            return this;
        }

        public Button 移除事件<T, T1>(Action<UIEventData<T, T1>> data)
        {
            ZEvent.UIEvent.RemoveListener(graphic.gameObject, data);
            return this;
        }

    }


}
