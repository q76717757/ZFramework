using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace ZFramework
{
    //生命周期
    public class View_DataAwake : AwakeSystem<View_Data_Component>
    {
        public override void Awake(View_Data_Component component)
        {
            component.btnImgs = new Image[]
            {
                component.Refs.Get<Image>("Btn0"),
                component.Refs.Get<Image>("Btn1"),
            };
            component.btnSprs = new Sprite[]
            { 
                component.Refs.Get<Sprite>("按钮暗"),
                component.Refs.Get<Sprite>("按钮亮"),
            };

            ZEvent.UIEvent.AddListener(component.btnImgs[0], component.Btn, 0);
            ZEvent.UIEvent.AddListener(component.btnImgs[1], component.Btn, 1);

            //转速图
            var raw0 = component.Refs.Get<RawImage>("raw0");
            component._renderTexture = CreateRenderRexture(View_Data_Component.RT0Width, View_Data_Component.RT0Height, true);
            raw0.texture = component._renderTexture;
            component.computerShader = UnityEngine.Object.Instantiate<ComputeShader>(component.Refs.Get<ComputeShader>("computeShader"));
            component.computerShader.SetTexture(1, "Result", component._renderTexture);

            component.bufferLINE = new ComputeBuffer(component._renderTexture.width, sizeof(float));
            component.computerShader.SetBuffer(1, "LineValue", component.bufferLINE);//频率系数

            //波形图
            var raw1 = component.Refs.Get<RawImage>("raw1");
            component.renderTexture2 = CreateRenderRexture(View_Data_Component.RT1Width, View_Data_Component.RT1Heidht, true);
            raw1.texture = component.renderTexture2;
            component.computeShader2 = UnityEngine.Object.Instantiate<ComputeShader>(component.Refs.Get<ComputeShader>("computeShader"));
            component.computeShader2.SetTexture(0, "Result", component.renderTexture2);

            component.bufferHZ = new ComputeBuffer(component.renderTexture2.width, sizeof(float));
            component.bufferSCALE = new ComputeBuffer(component.renderTexture2.width, sizeof(float));
            component.computeShader2.SetBuffer(0, "HZ", component.bufferHZ);//频率系数
            component.computeShader2.SetBuffer(0, "SCALE", component.bufferSCALE);//振幅系数
        }

        private RenderTexture CreateRenderRexture(int width, int height, bool linear)
        {
            RenderTextureReadWrite readWrite = linear ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB;
            RenderTexture rt = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32, readWrite);
            rt.hideFlags = HideFlags.DontSave;
            rt.useMipMap = false;
            rt.filterMode = FilterMode.Point;
            rt.wrapMode = TextureWrapMode.Clamp;
            rt.enableRandomWrite = true;
            rt.Create();
            return rt;
        }
    }
    public class View_DataUpdate : UpdateSystem<View_Data_Component>
    {
        public override void Update(View_Data_Component component)
        {
            if (!component.gameObject.activeSelf) return;

            //数字
            float value0 = Game.Task.扭矩;
            float value1 = 电机整体.instance.转速;
            component.Refs.Get<TextMeshProUGUI>("text0").text = value0.ToString("N2");
            component.Refs.Get<TextMeshProUGUI>("text1").text = value1.ToString("N0");
            component.Refs.Get<TextMeshProUGUI>("text2").text = (value0 * value1 * 1000 / 9550).ToString("N2");

            //转速图
            int rt0Width = component._renderTexture.width;
            List<float> line = Game.Task.lineValue.GetRange(Game.Task.lineValue.Count - rt0Width, rt0Width);
            component.DrawComputerShader转速(line);


            //波形图
            int rt1Width = component.renderTexture2.width;
            List<float> hz = Game.Task.dataHZ.GetRange(Game.Task.dataHZ.Count - rt1Width, rt1Width);
            List<float> scale = Game.Task.dataSCALE.GetRange(Game.Task.dataSCALE.Count - rt1Width, rt1Width);
            component.DrawComputerShader波形(hz, scale);
        }
    }

    public class View_Data_Show : UIShowSystem<View_Data_Component>
    {
        public override void OnShow(View_Data_Component canvas)
        {
            canvas.gameObject.SetActive(true);
            canvas.btnImgs[0].sprite = canvas.btnSprs[0];
            canvas.btnImgs[1].sprite = canvas.btnSprs[0];

            if (Game.Task.stepIndex == 38)//一阶段结束 有重置转速按钮
            {
                canvas.btnImgs[0].gameObject.SetActive(true);
                canvas.btnImgs[1].gameObject.SetActive(true);
            }
            else
            {
                canvas.btnImgs[0].gameObject.SetActive(false);
                canvas.btnImgs[1].gameObject.SetActive(false);
            }
            if (Game.Task.stepIndex == 49)//过载结束 没有重置转速按钮
            {
                canvas.btnImgs[0].gameObject.SetActive(false);
                canvas.btnImgs[1].gameObject.SetActive(true);
            }
        }
    }

    public class View_Data_Hide : UIHideSystem<View_Data_Component>
    {
        public override void OnHide(View_Data_Component canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_Data_System
    {
        public static void Btn(this View_Data_Component component, UIEventData<int> eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    SoundHelper.MouseEnter();
                    component.btnImgs[eventData.Data0].sprite = component.btnSprs[1];
                    break;
                case UIEventType.Exit:
                    component.btnImgs[eventData.Data0].sprite = component.btnSprs[0];
                    break;

                case UIEventType.Click:
                    Game.UI.Hide(UIType.View_Data);
                    switch (eventData.Data0)
                    {
                        case 0:
                            Log.Info("重设转速");
                            Game.Task.Run(100);
                            break;
                        case 1:
                            Log.Info("实验结束");
                            (Game.UI.Get(UIType.View_Left) as View_Left_Component).Clear();//不能写进task.end里面 会无限递归 
                            //Game.Task.End(); //已经包含了task.end
                            break;
                    }
                    break;
            }
        }

        public static void DrawComputerShader转速(this View_Data_Component component,List<float> line)
        {
            component.bufferLINE.SetData<float>(line);

            int threadGroupsX = Mathf.CeilToInt(component._renderTexture.width / 8f);
            int threadGroupsY = Mathf.CeilToInt(component._renderTexture.height / 8f);
            component.computerShader.Dispatch(1, threadGroupsX, threadGroupsY, 1);
        }

        public static void DrawComputerShader波形(this View_Data_Component component,List<float> hz,List<float> scale)
        {
            component.bufferHZ.SetData<float>(hz);//频率
            component.bufferSCALE.SetData<float>(scale);//振幅

            //15.36
            //float hzP = Mathf.PI * 2 / 768f;
            //component.computeShader2.SetFloats("add", Game.Task.dataHZ.Count * 0.02f * 50);// /*(Mathf.PI * 2) / (768 * 0.02f) / hzP * */Time.time*50);//相位  20毫秒1像素  = 15.36秒全程
            //推算出来最终的结果是时间x频率   数组长度成0.02就是真实时间  乘以时间的倒数就是频率  相乘抵消掉  结果就等于数组长度
            component.computeShader2.SetFloats("add", Game.Task.dataHZ.Count);

            //component.computerShader.SetFloats("hz", Mathf.Lerp(0.1f, 1, sliderValue0));//频率
            //component.computerShader.SetFloats("scale", Mathf.Lerp(0, 1, sliderValue1));//振幅

            int threadGroupsX = Mathf.CeilToInt(component.renderTexture2.width / 8f);
            int threadGroupsY = Mathf.CeilToInt(component.renderTexture2.height / 8f);
            component.computeShader2.Dispatch(0, threadGroupsX, threadGroupsY, 1);

            //sampleBuffer.Release();
            //sampleBuffer.Dispose();
        }

    }
}
