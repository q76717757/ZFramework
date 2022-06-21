
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ZFramework
{
    [UIType(UIType.View_Boom)]
    public class View_Boom_Component:UICanvasComponent
    {
        public Image[] btnImgs;
        public Sprite[,] btnSprs;

        public Dianji 电机;

        public Dictionary<DianJiPart, RectTransform> tags;
    }
}
