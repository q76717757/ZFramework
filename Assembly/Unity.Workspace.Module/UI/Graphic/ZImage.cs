using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MPUIKIT;

namespace ZFramework
{
    [AddComponentMenu("ZFramework/UI/ZImage")]
    public class ZImage : MPImageBasic, IGraphicElement
    {
        UIWindowLinker link;//uiwindow
        UIWindowLinker Linker
        {
            get
            {
                if (link == null)
                {
                    link = GetComponentInParent<UIWindowLinker>();
                }
                return link;
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (Linker != null)
            {
                UIManager.PopWindow(Linker.window);
            }
        }
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            link = null;
        }
    }
}
