using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace ZFramework
{
    [AddComponentMenu("ZFramework/UI/ZButton")]
    public class ZButton : InteractiveElement, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IScrollHandler
    {
        protected ZButton()
        {
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
            if (CanInteractable())
                Call(UIEventType.Down, eventData);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            if (CanInteractable())
                Call(UIEventType.Up, eventData);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (CanInteractable())
                Call(UIEventType.Enter, eventData);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            if (CanInteractable())
                Call(UIEventType.Exit, eventData);
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (CanInteractable())
                Call(UIEventType.Click, eventData);
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (CanInteractable())
                Call(UIEventType.Drag, eventData);
        }
        public void OnScroll(PointerEventData eventData)
        {
            if (CanInteractable())
                Call(UIEventType.Scroll, eventData);
        }




        //临时接入ZEvent.UI
        void Call(UIEventType type, PointerEventData eventData)
        {
            ZEvent.UIEvent.GetHandler().CallGroup(gameObject.GetInstanceID(), gameObject, type, eventData);
        }
        public void AddListener(Action<UIEventData> listener)
        {
            var newData = ZEvent.GetNewData<UIEventData>().SetData();
            var newListener = ZEvent.GetNewListener<UIEventListener<UIEventData>>().SetData(gameObject, listener, newData);
            ZEvent.UIEvent.GetHandler().AddListenerForUIFramework(newListener);
        }
        public void AddListener<A>(Action<UIEventData<A>> listener, A a)
        {
            var newData = ZEvent.GetNewData<UIEventData<A>>().SetData(a);
            var newListener = ZEvent.GetNewListener<UIEventListener<UIEventData<A>>>().SetData(gameObject, listener, newData);
            ZEvent.UIEvent.GetHandler().AddListenerForUIFramework(newListener);
        }

    }
}
