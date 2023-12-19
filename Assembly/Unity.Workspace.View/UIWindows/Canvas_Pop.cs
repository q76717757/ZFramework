using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace ZFramework
{
    [UISetting("Prefabs/Cnavas_Pop", UIGroupType.Middle)]
    public class Canvas_Pop : UIWindowBase, IUIInput
    {

        protected override void OnFocus()
        {
            base.OnFocus();
            Log.Info("OnFocus" + ID);
        }
        protected override void OnLoseFocus()
        {
            Log.Info("OnLoseFocus" + ID);
        }
        protected override void OnClose()
        {
            Log.Info("Close:" + ID);
        }
        protected override void OnAwake()
        {
            base.OnAwake();
            Image btn = References.Get<Image>("Image");
            ZEvent.UIEvent.AddListener(btn, (UI) =>
            {
                if (UI.EventType == UIEventType.Drag && UI.PointerType == PointerType.Touch2)
                {
                    RectTransform rt = UI.Target.GetComponent<RectTransform>();
                    rt.anchoredPosition += UI.UnityEventData.delta;
                }
                if (UIEventType.Down == UI.EventType && UI.PointerType == PointerType.Touch2)
                {
                    UIManager.Instance.Pop(this);
                }
            });

        }


        protected override void OnDestroy()
        {
            Log.Info("Destroy" + ID);
        }

        public void OnInput(UIFoucsEventData data)
        {
            if (data.keyType == UIFoucsEventData.KeyType.down)
            {
                Log.Info(data.callbackContext.action.name);
                if (data.callbackContext.action.name == "Cancel")
                {
                    UIManager.Instance.Close(this);
                }
            }
        }

        protected override void OnOpen()
        {
         
        }
    }
}
