using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZFramework
{
    public class ZRawImage : RawImage, IGraphicElement
    {
        UIWindowLinker link;
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
