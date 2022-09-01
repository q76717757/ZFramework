using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;
using TMPro;

namespace ZFramework
{
    public class Canvas_VideoAwake : AwakeSystem<Canvas_VideoComponent>
    {
        public override void OnAwake(Canvas_VideoComponent entity)
        {
            entity.gameObject.transform.position = RefHelper.References.Get<Transform>("会议室屏幕").position;
            entity.gameObject.transform.rotation = RefHelper.References.Get<Transform>("会议室屏幕").rotation;

            entity.mod0Img = entity.Refs.Get<Image>("Mod0");
            entity.mod1Img = entity.Refs.Get<Image>("Mod1");
            entity.mod0GO = entity.Refs.Get<GameObject>("Video");
            entity.mod1GO = entity.Refs.Get<GameObject>("PPT");

            entity.videoPlayer = entity.gameObject.GetComponent<UnityEngine.Video.VideoPlayer>();
            entity.videoPlayImg = entity.Refs.Get<Image>("VideoPlay");
            entity.videoPauseImg = entity.Refs.Get<Image>("VideoPause");

            entity.pptImg = entity.Refs.Get<Image>("PPTValue");
            entity.pptText = entity.Refs.Get<TextMeshProUGUI>("PPTText");
            entity.rawIMG = entity.Refs.Get<RawImage>("DrawRawimg");
            entity.t2d = new Texture2D((int)entity.rawIMG.rectTransform.rect.width, (int)entity.rawIMG.rectTransform.rect.height);
            Color32[] colors = new Color32[entity.t2d.width* entity.t2d.height];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.clear;
            }
            entity.t2d.SetPixels32(colors);
            entity.t2d.Apply(true);

            entity.rawIMG.texture = entity.t2d;

            entity.pptSprites = new Sprite[23];
            for (int i = 0; i < entity.pptSprites.Length; i++)
            {
                entity.pptSprites[i] = entity.Refs.Get<Sprite>("P" + i);
            }

            ZEvent.UIEvent.AddListener(entity.mod0Img, entity.Btn, 0);
            ZEvent.UIEvent.AddListener(entity.mod1Img, entity.Btn, 1);
            ZEvent.UIEvent.AddListener(entity.videoPlayImg, entity.Btn, 2);
            ZEvent.UIEvent.AddListener(entity.videoPauseImg, entity.Btn, 3);
            ZEvent.UIEvent.AddListener(entity.Refs.Get<GameObject>("Clear"), entity.Btn, 4);

            ZEvent.UIEvent.AddListener(entity.rawIMG, entity.DrawBtn);

            entity.ChangeMod(0);
        }
    }

    public static class Canvas_VideoSystem
    {
        public static void ChangeMod(this Canvas_VideoComponent component, int mod)
        {
            component.localMod = mod;
            component.mod0Img.color = mod == 0 ? Color.green : Color.white;
            component.mod1Img.color = mod == 1 ? Color.green : Color.white;
            component.mod0GO.SetActive(mod == 0);
            component.mod1GO.SetActive(mod == 1);

            if (mod == 0)
            {
                component.videoPlayer.Play();
                component.videoPlayImg.gameObject.SetActive(false);
                component.videoPauseImg.gameObject.SetActive(true);
            }
            else
            {
                component.videoPlayer.Stop();
                component.ShowPPT(0);
            }
        }
        public static void CallLeft(this Canvas_VideoComponent component)
        {
            if (component.localMod == 1)
            {
                Log.Info("Left");
                int next = (component.pptIndex + component.pptSprites.Length - 1) % component.pptSprites.Length;
                component.ShowPPT(next);
            }
        }
        public static void CallRight(this Canvas_VideoComponent component)
        {
            if (component.localMod == 1)
            {
                Log.Info("Right");
                int next = (component.pptIndex + component.pptSprites.Length + 1) % component.pptSprites.Length;
                component.ShowPPT(next);
            }
        }
        public static void ShowPPT(this Canvas_VideoComponent component, int index)
        {
            component.pptIndex = Mathf.Clamp(index, 0, component.pptSprites.Length);
            component.pptImg.sprite = component.pptSprites[index];
            component.pptText.text = $"{index + 1}/{component.pptSprites.Length}";
        }


        public static void Btn(this Canvas_VideoComponent component, UIEventData<int> eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    break;
                case UIEventType.Exit:
                    break;
                case UIEventType.Down:
                    break;
                case UIEventType.Up:
                    break;
                case UIEventType.Click:
                    switch (eventData.Data0)
                    {
                        case 0:
                            component.ChangeMod(0);
                            break;
                        case 1:
                            component.ChangeMod(1);
                            break;
                        case 2:
                            component.videoPlayer.Play();
                            component.videoPlayImg.gameObject.SetActive(false);
                            component.videoPauseImg.gameObject.SetActive(true);
                            break;
                        case 3:
                            component.videoPlayer.Pause();
                            component.videoPlayImg.gameObject.SetActive(true);
                            component.videoPauseImg.gameObject.SetActive(false);
                            break;
                        case 4://清图
                            Color32[] colors = new Color32[component.t2d.width * component.t2d.height];
                            for (int i = 0; i < colors.Length; i++)
                            {
                                colors[i] = Color.clear;
                            }
                            component.t2d.SetPixels32(colors);
                            component.t2d.Apply(true);
                            break;
                    }
                    break;
            }
        }
        public static void DrawBtn(this Canvas_VideoComponent component, UIEventData eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    break;
                case UIEventType.Exit:
                    break;
                case UIEventType.Down:
                    component.lastPPTPos = component.GetUIPos(eventData.UnityEventData.pointerCurrentRaycast.worldPosition);
                    break;
                case UIEventType.Up:
                    break;
                case UIEventType.Click:
                    break;
                case UIEventType.Drag:
                    Vector2 uipos = component.GetUIPos(eventData.UnityEventData.pointerCurrentRaycast.worldPosition);
                    if (uipos.x>0 && uipos.x < 1920f &&uipos.y>0 && uipos.y < 1080)
                    {
                        Color32[] colors = new Color32[100];
                        for (int i = 0; i < 25; i++)
                        {
                            colors[i] = Color.green;
                        }

                        float len = Vector2.Distance(uipos, component.lastPPTPos);
                        float offset = 1 / len;
                        for (int i = 0; i < (int)len; i++)
                        {
                            var p = Vector2.Lerp(component.lastPPTPos, uipos, offset * i);
                            component.t2d.SetPixels32(Mathf.Clamp((int)p.x, 3, 1920 - 3), Mathf.Clamp((int)p.y, 3, 1080 - 3), 5, 5, colors);
                        }

                        component.t2d.Apply();
                    }
                    component.lastPPTPos = uipos;
                    break;
                case UIEventType.Scroll:
                    break;
                default:
                    break;
            }
        }
        public static Vector2 GetUIPos(this Canvas_VideoComponent component,Vector3 worldPos)
        {
            var tran = component.gameObject.transform;
            var zeroPos = tran.position + tran.right * -1920 / 2 * 0.002f + tran.up * -1080 / 2 * 0.002f;

            var cos = Vector3.Dot(worldPos - zeroPos, tran.right);
            var sin = Vector3.Dot(worldPos - zeroPos, tran.up);
            Vector2 uipos = new Vector2(cos, sin) * 500f;
            return uipos;
        }

    }
}
