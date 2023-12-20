using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ZFramework
{
    [UISetting("Prefabs/Cnavas_冻结", UIGroupType.Middle)]
    public class Cnavas_冻结 : UIWindowBase, IUIInput
    {
        protected override void OnClose()
        {
            Log.Info("Close" + ID);
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
            AA().Invoke();
        }
        protected override void OnDestroy()
        {
            Log.Info("Destroy" + ID);
        }
        protected override void OnOpen()
        {
            UIManager.Instance.Open(child);
        }

        protected override void OnFocus()
        {
            base.OnFocus();
            Log.Info("OnFocus" + ID);
        }
        protected override void OnLoseFocus()
        {
            Log.Info("OnLoseFocus" + ID);
        }


        Canvas_Pop child;
        async ATask AA()
        {
            Canvas_Pop a = await AddChildAsync<Canvas_Pop>(false);
            child = a;
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
    }
}
