using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ZFramework
{
    public class TestOpenXR : MonoBehaviour
    {
        public Image img;
        public Text text;

        public InputActionReference inputActionReference;
        public InputAction inputAction;


        // Start is called before the first frame update
        void Start()
        {
            ZEvent.UIEvent.AddListener(img, UI);
        }


        void UI(UIEventData eventData)
        {
            Debug.Log($"{eventData.EventType}:{eventData.Position}:{eventData.PointerType}");
            text.text = eventData.Position.ToString();
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    img.color = Color.red;
                    break;
                case UIEventType.Exit:
                    img.color = Color.white;
                    break;
                case UIEventType.Down:
                    img.color = Color.blue;
                    break;
                case UIEventType.Up:
                    img.color = Color.green;
                    break;
                case UIEventType.Click:
                    break;
                case UIEventType.Drag:
                    break;
                case UIEventType.Scroll:
                    img.color = Color.yellow;
                    break;
                default:
                    break;
            }
        }

        //// Update is called once per frame
        //void Update()
        //{
        //    if (null == inputAction || inputAction.controls.Count == 0)
        //        return;
        //    var device = inputAction.controls[0].device;
        //    Debug.Log($"{device.name}\n{device.deviceId}");
        //}
    }
}
