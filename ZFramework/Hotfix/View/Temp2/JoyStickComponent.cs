using System;
using UnityEngine;

namespace ZFramework
{
    //public class JoyStickAwake : OnAwakeImpl<JoyStickComponent,References>
    //{
    //    public override void OnAwake(JoyStickComponent self,References a)
    //    {
    //        self.Init(a);
    //    }
    //}

    public class JoyStickComponent :  SingleComponent<JoyStickComponent>
    {
        RectTransform plane;
        RectTransform joy;
        RectTransform range;

        public void Build(References refs)
        {
            plane = refs.Get<RectTransform>("JoyStickBase");
            range = refs.Get<RectTransform>("Range");
            joy = refs.Get<RectTransform>("JoyStick");
            ZEvent.UIEvent.AddListener(plane, UIEvent);
        }

        void UIEvent(UIEventData eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Down:
                    range.gameObject.SetActive(true);
                    MoveJoy(eventData.Position);
                    break;
                case UIEventType.Up:
                    range.gameObject.SetActive(false);
                    joy.anchoredPosition = Vector2.zero;

                    ZEvent.CustomEvent.Call(Movement.摇杆, Vector2.zero);
                    break;
                case UIEventType.Drag:
                    MoveJoy(eventData.Position);
                    break;
            }
        }

        void MoveJoy(Vector2 pos)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(plane, pos, null, out Vector2 localPos))
            {
                var dis = Vector2.Distance(localPos, Vector2.zero);
                var dir = (localPos - Vector2.zero).normalized;

                var qua = Quaternion.FromToRotation(Vector2.up, dir);
                range.localRotation = qua;

                joy.anchoredPosition = dir * Mathf.Clamp(dis, 0, 130);

                ZEvent.CustomEvent.Call(Movement.摇杆, dir);
            }
        }
    }
}
