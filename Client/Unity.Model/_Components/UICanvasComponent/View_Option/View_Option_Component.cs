
using UnityEngine;
using System;
using UnityEngine.UI;

namespace ZFramework
{
    [UIType(UIType.View_Option)]
    public class View_Option_Component:UICanvasComponent
    {
        public Sprite[] sprs;
        public Image[] imgs;

        public float slidervalue0 = 1;
        public float slidervalue1 = 1;
    }
}
