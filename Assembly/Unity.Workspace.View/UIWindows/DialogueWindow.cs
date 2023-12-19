using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ZFramework
{
    [UISetting("Prefabs/Canvas_Dialogue", UIGroupType.Middle, UIPopType.Pop)]
    internal class DialogueWindow : UIWindowBase, IUIInput
    {
        protected override void OnAwake()
        {
            Image btn = References.Get<Image>("Image");
            ZEvent.UIEvent.AddListener(btn, (UI) =>
            {
                if (UI.EventType == UIEventType.Drag && UI.PointerType == PointerType.Touch2)
                {
                    Debug.Log(UI.PointerType);
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



        Cnavas_冻结 child;
        async ATask AA()
        {
            Cnavas_冻结 a = await AddChildAsync<Cnavas_冻结>();
            child = a;
        }
        protected override void OnOpen()
        {
            UIManager.Instance.Open(child);

        }
        protected override void OnClose()
        {
            Log.Info("Close" + ID);
        }
        protected override void OnUpdate()
        {

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
                if (data.callbackContext.action.name == "Submit")
                {
                    OpenNewChildAsync<Cnavas_冻结>().Invoke();
                }

                if (data.callbackContext.action.name == "RightClick")
                {
                    OpenNewChildAsync<Canvas_Pop>().Invoke();
                }
            }
        }
    }
}
