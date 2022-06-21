
using UnityEngine;
using System;
using System.Collections.Generic;

namespace ZFramework
{

    [UIType(UIType.View_Line)]
    public class View_Line_Component:UICanvasComponent
    {
        public RectTransform LineRoot;
        public Dictionary<DragPortType, DragPort> Ports;//所有的交互点

        public int 电流夹数量 = 3;
        public Dictionary<DeviceType, RectTransform> btns;//左侧所有设备按钮 不包括电流夹

        public Dictionary<DeviceType, RectTransform> box;//右边所有设备框
        public Dictionary<DeviceType, GameObject> devices;//右边所有的纸片设备

        public DeviceType HoldingType;
        public GameObject HoldingBtn;

        public bool CanPutDown;

        public DragPort HoldPort;
        public DragPort EnterPort;
        public DrawLine DrawLine;
    }
}
