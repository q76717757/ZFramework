/** Header
 *  UnitEventDriver.cs
 *  单位事件驱动单元
 *  UI穿透方案综合考虑决定使用Camere+Raycaster
 **/

using UnityEngine;
using UnityEngine.EventSystems;

namespace ZFramework
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]//从inspector面板隐藏掉
    public sealed class UnitEventDriver : ZEventDriverBase<UnitEventHandler>,
        IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
            => SendData(UnitEventType.Drag, eventData);
        public void OnPointerClick(PointerEventData eventData)
            => SendData(UnitEventType.Click, eventData);
        public void OnPointerDown(PointerEventData eventData)
            => SendData(UnitEventType.Down, eventData);
        public void OnPointerEnter(PointerEventData eventData)
            => SendData(UnitEventType.Enter, eventData);
        public void OnPointerExit(PointerEventData eventData)
            => SendData(UnitEventType.Exit, eventData);
        public void OnPointerUp(PointerEventData eventData)
            => SendData(UnitEventType.Up, eventData);

        private void SendData(UnitEventType eventType, PointerEventData unityEventData)
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

//IScrollHandler - OnScroll - 当鼠标滚轮滚动时调用

//IUpdateSelectedHandler - OnUpdateSelected - 在选中的对象上每帧调用

//ISelectHandler - OnSelect - 当对象成为选定对象时调用

//IDeselectHandler - OnDeselect - 对选定对象的调用变为反选

//IMoveHandler - OnMove - 当移动事件发生时调用(左，右，上，下，等等)

//ISubmitHandler - OnSubmit - 当提交按钮被按下时调用

//ICancelHandler - OnCancel - 当按下取消按钮时调用
