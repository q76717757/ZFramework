
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ZFramework
{
    //生命周期
    public class View_VideoAwake : AwakeSystem<View_Video_Component>
    {
        public override void Awake(View_Video_Component component)
        {
            component.renderTexture = new RenderTexture(1920, 1080, 24);
            component.player = component.Refs.Get<UnityEngine.Video.VideoPlayer>("VP");

            component.player.targetTexture = component.renderTexture;
            component.player.clip = component.Refs.Get<UnityEngine.Video.VideoClip>("开场动画");

            var rawimg = component.Refs.Get<RawImage>("RawImage");
            rawimg.texture = component.renderTexture;
            ZEvent.UIEvent.AddListener(rawimg, component.Click);
        }
    }

    public class View_Video_Show : UIShowSystem<View_Video_Component>
    {
        public override void OnShow(View_Video_Component canvas)
        {
            canvas.gameObject.SetActive(true);
            canvas.player.Play();
            canvas.Refs.Get<Text>("Text").color = new Color(0, 0, 0, 0);
            canvas.finish = true;
            Wait(canvas);
        }

        async void Wait(View_Video_Component canvas) {
            await Task.Delay(25000);
            if (canvas.finish)
            {
                //canvas.player.Stop();
                //canvas.Refs.Get<Text>("Text").DOColor(Color.black, 1f);
                //await Task.Delay(4000);
                //if (canvas.finish)
                //{
                    canvas.Finish();
                //}
            }
        }
    }

    public class View_Video_Hide : UIHideSystem<View_Video_Component>
    {
        public override void OnHide(View_Video_Component canvas)
        {
            canvas.finish = false;
            canvas.player.Stop();
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_Video_System
    {
        public static void Click(this View_Video_Component component,UIEventData eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Click:
                    component.Finish();
                    break;
            }
        }

        public static void Finish(this View_Video_Component component) 
        {
            Game.UI.Hide(UIType.View_Video);
            Game.UI.Show(UIType.View_Login);

            SoundHelper.instance.BGMStart();
        }
    }
}

