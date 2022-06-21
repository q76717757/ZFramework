/** Header
 *  UIEventDriver.cs
 *  UI事件驱动单元
 **/

using UnityEngine;
using UnityEngine.EventSystems;

namespace ZFramework
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]//从inspector面板隐藏掉
    public sealed class UIEventDriver : ZEventDriverBase<UIEventHandler>, 
        IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IScrollHandler
    {
        public void OnDrag(PointerEventData eventData)
            => SendData(UIEventType.Drag, eventData);
        public void OnPointerClick(PointerEventData eventData)
            => SendData(UIEventType.Click, eventData);
        public void OnPointerDown(PointerEventData eventData)
            => SendData(UIEventType.Down, eventData);
        public void OnPointerEnter(PointerEventData eventData)
            => SendData(UIEventType.Enter, eventData);
        public void OnPointerExit(PointerEventData eventData)
            => SendData(UIEventType.Exit, eventData);
        public void OnPointerUp(PointerEventData eventData)
            => SendData(UIEventType.Up, eventData);
        public void OnScroll(PointerEventData eventData)
            => SendData(UIEventType.Scroll, eventData);

        private void SendData(UIEventType eventType, PointerEventData unityEventData)
            => Handler?.CallGroup(InstanceID, gameObject, eventType, unityEventData);

    }
}

//接口列表 按需增加
//IPointerEnterHandler - OnPointerEnter - 当指针进入对象时调用

//IPointerExitHandler - OnPointerExit - 当指针退出对象时调用

//IPointerDownHandler - OnPointerDown - 当指针按在对象上时调用

//IPointerUpHandler - OnPointerUp - 当一个指针被释放时调用(在原始的被压对象上调用)

//IPointerClickHandler - OnPointerClick - 在同一对象上按下和释放指针时调用

//IInitializePotentialDragHandler - OnInitializePotentialDrag - 在找到拖动目标时调用，可用于初始化值

//IBeginDragHandler - OnBeginDrag - 在拖动即将开始时对拖动对象调用

//IDragHandler - OnDrag - 当拖动发生时，在拖动对象上调用

//IEndDragHandler - OnEndDrag - 在拖动结束时对拖动对象调用

//IDropHandler - OnDrop - 在拖动结束的对象上调用

//IScrollHandler - OnScroll - 当鼠标滚轮滚动时调用 ----------->>>这个可以添加 有点用处

//IUpdateSelectedHandler - OnUpdateSelected - 在选中的对象上每帧调用

//ISelectHandler - OnSelect - 当对象成为选定对象时调用

//IDeselectHandler - OnDeselect - 对选定对象的调用变为反选

//IMoveHandler - OnMove - 当移动事件发生时调用(左，右，上，下，等等)

//ISubmitHandler - OnSubmit - 当提交按钮被按下时调用

//ICancelHandler - OnCancel - 当按下取消按钮时调用
