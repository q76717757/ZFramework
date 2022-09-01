using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

namespace ZFramework
{
    [UIType(UIType.Canvas_Video)]
    public class Canvas_VideoComponent :UICanvasComponent
    {
        public int localMod;

        public VideoPlayer videoPlayer;
        public Image videoPlayImg;
        public Image videoPauseImg;

        public int pptIndex;
        public Sprite[] pptSprites;
        public Image pptImg;
        public TextMeshProUGUI pptText;
        public RawImage rawIMG;
        public Texture2D t2d;

        public bool hold;
        public Vector2 lastPPTPos;


        public Image mod0Img;
        public Image mod1Img;
        public GameObject mod0GO;
        public GameObject mod1GO;

    }
}
