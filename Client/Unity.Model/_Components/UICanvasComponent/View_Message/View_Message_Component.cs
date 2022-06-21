
using UnityEngine;
using System;
using UnityEngine.UI;

namespace ZFramework
{
    [UIType(UIType.View_Message)]
    public class View_Message_Component:UICanvasComponent
    {
        public Sprite[] sprs;
        public Image[] imgs;

        public Action<bool> callbackBool;
        public Action callbackVoid;
    }
}
