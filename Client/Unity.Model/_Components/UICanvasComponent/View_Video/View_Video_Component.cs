using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Video;

namespace ZFramework
{
    [UIType(UIType.View_Video)]
    public class View_Video_Component:UICanvasComponent
    {
        public RenderTexture renderTexture;
        public VideoPlayer player;
        public bool finish;

    }
}
