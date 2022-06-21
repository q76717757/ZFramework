
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ZFramework
{
    [UIType(UIType.View_Data)]
    public class View_Data_Component:UICanvasComponent
    {
        public Image[] btnImgs;
        public Sprite[] btnSprs;

        //rt0  转速图
        public ComputeShader computerShader;
        public RenderTexture _renderTexture;
        public ComputeBuffer bufferLINE;

        //rt1  波形图
        public ComputeShader computeShader2;
        public RenderTexture renderTexture2;
        public ComputeBuffer bufferHZ;
        public ComputeBuffer bufferSCALE;


        public static int RT0Width = 576;//需要是8的倍数
        public static int RT0Height = 288;

        public static int RT1Width = 704;
        public static int RT1Heidht = 448;
    }
}
