using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace ZFramework
{
    [UISetting("Prefabs/Cnavas_Pop", UISortLayer.Normal)]
    public class Canvas_Pop : UIWindow, IAwake, IUpdate
    {
        TextMeshProUGUI text;
        References refs;
        public void Awake()
        {
            var img =  gameObject.GetComponentInChildren<ZImage>();
            img.color = new Color(UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f), UnityEngine.Random.Range(0, 1f));

            refs = gameObject.GetComponent<References>();

            refs.Get<ZButton>("title").AddListener(TitleBtn);
            refs.Get<ZButton>("btn").AddListener(CreateChildBtn);
            refs.Get<ZButton>("btn2").AddListener(CreateModalBtn);
            refs.Get<ZButton>("closeBtn").AddListener(CloseBtn);

            text = refs.Get<TextMeshProUGUI>("text");
        }

        Vector2 offset;
        void TitleBtn(UIEventData eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Down:
                    var start = rectTransform.anchoredPosition;
                    var pos = eventData.Position;
                    offset = start - pos;
                    break;
                case UIEventType.Drag:
                    rectTransform.anchoredPosition = eventData.Position + offset;
                    break;
            }
        }
        void CreateChildBtn(UIEventData eventData)
        {
            if (eventData.EventType == UIEventType.Click)
            {
                this.CreateChildWindow<Canvas_Pop>(false);
            }
        }
        void CreateModalBtn(UIEventData eventData)
        {
            if (eventData.EventType == UIEventType.Click)
            {
                this.CreateChildWindow<Canvas_Pop>(true);
            }
        }
        void CloseBtn(UIEventData eventData)
        {
            if (eventData.EventType == UIEventType.Click)
            {
                UIManager.CloseWindow(this);
            }
        }

        public void Update()
        {
            refs.Get<ZImage>("titleIMG").color = UIManager.GetActiveWindow() == this ? Color.green : Color.white;

            if (InModal)
            {
                refs.Get<ZImage>("titleIMG").color *= Color.gray;
                refs.Get<ZImage>("btn1IMG").color = Color.gray;
                refs.Get<ZImage>("btn2IMG").color = Color.gray;
            }
            else
            {
                refs.Get<ZImage>("titleIMG").color *= Color.white;
                refs.Get<ZImage>("btn1IMG").color = Color.white;
                refs.Get<ZImage>("btn2IMG").color = Color.white;
            }

            text.text = InstanceID + ":" + Order;
        }


    }
}
