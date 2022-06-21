
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using Paroxe.PdfRenderer;
using UnityEngine.Video;

namespace ZFramework
{
    //生命周期
    public class View_Pdf2Awake : AwakeSystem<View_Pdf2_Component>
    {
        public override void Awake(View_Pdf2_Component component)
        {
            component.pdfFilePath = Path.Combine(Application.streamingAssetsPath, "PDF", "原理学习文档");
            component.View = component.Refs.Get<PDFViewer>("PDFViewer");

            DirectoryInfo directoryInfo = new DirectoryInfo(component.pdfFilePath);
            var files = directoryInfo.GetFiles("*.pdf", SearchOption.TopDirectoryOnly);

            var itemPre = component.Refs.Get<GameObject>("PDFItem");
            var content = component.Refs.Get<RectTransform>("content");
            bool first = true;
            int index = 0;
            component.items = new Dictionary<FileInfo, GameObject>();

            var jhzj = component.Refs.Get<JiaoHuZuJian>("交互组件");
            ZEvent.UIEvent.AddListener(jhzj.arrowA, component.DragArrow, 0);
            ZEvent.UIEvent.AddListener(jhzj.arrowB, component.DragArrow, 1);

            component.t2dL = new Texture2D(200, 200);
            component.t2dR = new Texture2D(200, 200);
            component.Refs.Get<JiaoHuZuJian>("交互组件").rawimgL.texture = component.t2dL;
            component.Refs.Get<JiaoHuZuJian>("交互组件").rawimgR.texture = component.t2dR;

            foreach (var file in files)
            {
                var item = UnityEngine.GameObject.Instantiate<GameObject>(itemPre, content);
                item.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = file.Name.Replace(".pdf", "").Replace("_", "\r\n");
                ZEvent.UIEvent.AddListener(item, component.Btn, file, index++);
                component.items.Add(file, item);

                if (first)
                {
                    component.View.LoadDocumentFromStreamingAssets("PDF/原理学习文档", file.Name, "");
                    component.Show(0);

                    component.localSelect = file;
                    item.GetComponent<UnityEngine.UI.Image>().color = Color.white;

                    first = false;
                }
            }

            ZEvent.UIEvent.AddListener(component.Refs.Get<UnityEngine.UI.Image>("Image"), component.Close);
        }
    }

    public class View_Pdf2_Show : UIShowSystem<View_Pdf2_Component>
    {
        public override void OnShow(View_Pdf2_Component canvas)
        {
            canvas.gameObject.SetActive(true);
        }
    }

    public class View_Pdf2_Hide : UIHideSystem<View_Pdf2_Component>
    {
        public override void OnHide(View_Pdf2_Component canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_Pdf2_System
    {
        public static void Btn(this View_Pdf2_Component component, UIEventData<FileInfo,int> eventData)
        {
            switch (eventData.EventType)
            {
                case UIEventType.Enter:
                    SoundHelper.MouseEnter();
                    if (component.localSelect != eventData.Data0)
                    {
                        eventData.Target.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                    }
                    break;
                case UIEventType.Exit:
                    if (component.localSelect != eventData.Data0)
                    {
                        eventData.Target.GetComponent<UnityEngine.UI.Image>().color = Color.clear;
                    }
                    break;
                case UIEventType.Click:
                    if (eventData.Data0.Exists)
                    {
                        if (eventData.Data0 != component.localSelect)
                        {
                            if (component.localSelect != null)
                            {
                                component.items[component.localSelect].GetComponent<UnityEngine.UI.Image>().color = Color.clear;
                            }

                            component.View.LoadDocumentFromStreamingAssets("PDF/原理学习文档", eventData.Data0.Name, "");
                            eventData.Target.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                            component.Show(eventData.Data1);
                            component.localSelect = eventData.Data0;
                        }

                    }
                    break;
            }
        }

        public static void Show(this View_Pdf2_Component component, int index)
        {
            switch (index)
            {
                case 0://拖动
                    component.Refs.Get<GameObject>("Ctrl").SetActive(true);
                    component.View.GetComponent<RectTransform>().sizeDelta = new Vector2(660.5f, 818.15f);
                    component.View.ShowThumbnailsViewer = false;

                    component.Refs.Get<VideoPlayer>("VideoPlayer").gameObject.SetActive(false);
                    component.Refs.Get<JiaoHuZuJian>("交互组件").gameObject.SetActive(true);


                    var jhzj = component.Refs.Get<JiaoHuZuJian>("交互组件");
                    var localPos = new Vector2(200, 100);
                    var qua = Quaternion.FromToRotation(Vector3.right, new Vector3(localPos.x, localPos.y, 0));
                    var qua2 = Quaternion.FromToRotation(new Vector3(localPos.x, localPos.y, 0), Vector3.right);

                    //jhzj.arrowA.localRotation = qua;
                    //jhzj.arrowB.localRotation = qua2;

                    jhzj.lineA.localRotation = qua;
                    jhzj.lineB.localRotation = qua2;

                    jhzj.bg.rectTransform.localRotation = qua2;
                    jhzj.bg.fillAmount = qua.eulerAngles.z * 2 / 360f;

                    float dis = Vector2.Distance(Vector2.zero, localPos);
                    dis = Mathf.Clamp(dis, 10, 350);

                    jhzj.lineA.sizeDelta = new Vector2(dis, 5);
                    jhzj.lineB.sizeDelta = new Vector2(dis, 5);

                    jhzj.arrowA.anchoredPosition = localPos.normalized * dis;
                    jhzj.arrowB.anchoredPosition = (localPos * new Vector2(1, -1)).normalized * dis;

                    jhzj.bg.rectTransform.sizeDelta = Vector2.one * dis;

                    component.Drag(dis, qua.eulerAngles.z);

                    component.Refs.Get<VideoPlayer>("VideoPlayer").gameObject.SetActive(false);
                    component.Refs.Get<VideoPlayer>("VideoPlayer2").gameObject.SetActive(false);
                    break;
                case 1://视频
                    component.Refs.Get<GameObject>("Ctrl").SetActive(true);
                    component.View.GetComponent<RectTransform>().sizeDelta = new Vector2(660.5f, 818.15f);
                    component.View.ShowThumbnailsViewer = false;

                    component.Refs.Get<VideoPlayer>("VideoPlayer").gameObject.SetActive(true);
                    component.Refs.Get<VideoPlayer>("VideoPlayer2").gameObject.SetActive(false);
                    component.Refs.Get<VideoPlayer>("VideoPlayer").clip = component.Refs.Get<VideoClip>("三相绕组");
                    component.Refs.Get<JiaoHuZuJian>("交互组件").gameObject.SetActive(false);

                    break;
                case 2://视频
                    component.Refs.Get<GameObject>("Ctrl").SetActive(true);
                    component.View.GetComponent<RectTransform>().sizeDelta = new Vector2(660.5f, 818.15f);
                    component.View.ShowThumbnailsViewer = false;

                    component.Refs.Get<VideoPlayer>("VideoPlayer").gameObject.SetActive(true);
                    component.Refs.Get<VideoPlayer>("VideoPlayer2").gameObject.SetActive(false);
                    component.Refs.Get<VideoPlayer>("VideoPlayer").clip = component.Refs.Get<VideoClip>("两相绕组");
                    component.Refs.Get<JiaoHuZuJian>("交互组件").gameObject.SetActive(false);
                    break;
                case 3://视频
                    component.Refs.Get<GameObject>("Ctrl").SetActive(true);
                    component.View.GetComponent<RectTransform>().sizeDelta = new Vector2(660.5f, 818.15f);
                    component.View.ShowThumbnailsViewer = false;

                    component.Refs.Get<VideoPlayer>("VideoPlayer").gameObject.SetActive(false);
                    component.Refs.Get<VideoPlayer>("VideoPlayer2").gameObject.SetActive(true);
                    component.Refs.Get<VideoPlayer>("VideoPlayer2").clip = component.Refs.Get<VideoClip>("SVPWM");

                    component.Refs.Get<JiaoHuZuJian>("交互组件").gameObject.SetActive(false);
                    break;
                case 4://none
                    component.Refs.Get<VideoPlayer>("VideoPlayer").gameObject.SetActive(false);
                    component.Refs.Get<VideoPlayer>("VideoPlayer2").gameObject.SetActive(false);

                    component.Refs.Get<GameObject>("Ctrl").SetActive(false);
                    component.View.GetComponent<RectTransform>().sizeDelta = new Vector2(1333.3f, 818.15f);
                    component.View.ShowThumbnailsViewer = true;
                    break;
            }

        }

        public static void DragArrow(this View_Pdf2_Component component,UIEventData<int> eventData)
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
                    break;
                case UIEventType.Drag:
                    var jhzj = component.Refs.Get<JiaoHuZuJian>("交互组件");
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(jhzj.圆心, eventData.Position, null, out Vector2 localPos))
                    {
                        //eventData.Target.GetComponent<RectTransform>().anchoredPosition = localPos;
                        //eventData.Target.GetComponent<RectTransform>().localRotation = qua;

                        var qua = Quaternion.FromToRotation(Vector3.right, new Vector3(localPos.x, localPos.y, 0));
                        var qua2 = Quaternion.FromToRotation(new Vector3(localPos.x, localPos.y, 0), Vector3.right);


                        float dis = Vector2.Distance(Vector2.zero, localPos);
                        dis = Mathf.Clamp(dis, 10, 350);

                        jhzj.lineA.sizeDelta = new Vector2(dis, 5);
                        jhzj.lineB.sizeDelta = new Vector2(dis, 5);
                        jhzj.bg.rectTransform.sizeDelta = Vector2.one * dis;

                        switch (eventData.Data0)
                        {
                            case 0://拖的A
                                jhzj.arrowA.anchoredPosition = localPos.normalized * dis;
                                jhzj.arrowB.anchoredPosition = (localPos * new Vector2(1, -1)).normalized * dis;
                                //jhzj.arrowB.localRotation = qua2;

                                jhzj.lineA.localRotation = qua;
                                jhzj.lineB.localRotation = qua2;

                                if (localPos.y > 0)
                                {
                                    jhzj.bg.rectTransform.localRotation = qua2;
                                    jhzj.bg.fillAmount = qua.eulerAngles.z * 2 / 360f;
                                }
                                else
                                { 
                                    jhzj.bg.rectTransform.localRotation = qua;
                                    jhzj.bg.fillAmount = qua2.eulerAngles.z * 2 / 360f;
                                }

                                component.Drag(dis, qua.eulerAngles.z);
                                break;
                            case 1://拖的B
                                jhzj.arrowA.anchoredPosition = (localPos * new Vector2(1, -1)).normalized * dis;
                                jhzj.arrowB.anchoredPosition = localPos.normalized * dis;
                                //jhzj.arrowA.localRotation = qua2;

                                jhzj.lineA.localRotation = qua2;
                                jhzj.lineB.localRotation = qua;

                                if (localPos.y > 0)
                                {
                                    jhzj.bg.rectTransform.localRotation = qua2;
                                    jhzj.bg.fillAmount = qua.eulerAngles.z * 2 / 360f;
                                }
                                else
                                {
                                    jhzj.bg.rectTransform.localRotation = qua;
                                    jhzj.bg.fillAmount = qua2.eulerAngles.z * 2 / 360f;
                                }

                                component.Drag(dis, 360 - qua.eulerAngles.z);
                                break;
                        }
                    }
                    break;
                case UIEventType.Scroll:
                    break;
                default:
                    break;
            }
        }

        public static void Drag(this View_Pdf2_Component component,float dis,  float ang)
        {
            Color32[] clear = new Color32[component.t2dL.width * component.t2dL.height];
            component.t2dL.SetPixels32(clear);

            for (int i = 0; i < component.t2dL.width; i++)
            {
                var scale = 0.1f;
                var x = Mathf.Lerp(-Mathf.PI, Mathf.PI, (float)i / component.t2dL.width);
                var y = component.t2dL.height / 2 + 2 * dis * scale * Mathf.Cos(Mathf.Deg2Rad * ang) * Mathf.Cos(x);

                if (y< 0 || y > component.t2dL.height)
                {
                    continue;
                }
                component.t2dL.SetPixel(i, (int)y, Color.green);
            }
            component.t2dL.Apply();


            //---------------------------------

            Color32[] clearR = new Color32[component.t2dR.width * component.t2dR.height];
            component.t2dR.SetPixels32(clearR);

            for (int i = 0; i < component.t2dR.width; i++)
            {
                var scale = 0.1f;
                var x = Mathf.Lerp(-Mathf.PI, Mathf.PI, (float)i / component.t2dR.width);


                var y1 = dis * scale * Mathf.Cos(Mathf.Deg2Rad * ang - x) + component.t2dR.height / 2;
                if (y1 >= 0 && y1 < component.t2dR.height)
                {
                    component.t2dR.SetPixel(i, (int)y1, Color.cyan);
                }

                var y2 = dis * scale * Mathf.Cos(Mathf.Deg2Rad * ang + x) + component.t2dR.height / 2;
                if (y2 >= 0 && y2 < component.t2dR.height)
                {
                    component.t2dR.SetPixel(i, (int)y2, Color.red);
                }
            }
            component.t2dR.Apply();
        }

        public static void Close(this View_Pdf2_Component component, UIEventData eventData)
        {
            if (eventData.EventType == UIEventType.Click)
            {
                (Game.UI.Get(UIType.View_Left) as View_Left_Component).Clear();
            }
        }
    }
}
