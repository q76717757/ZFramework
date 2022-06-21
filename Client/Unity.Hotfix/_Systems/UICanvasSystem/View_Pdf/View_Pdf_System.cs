
using Paroxe.PdfRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using UnityEngine.UI;


namespace ZFramework
{
    //生命周期
    public class View_PdfAwake : AwakeSystem<View_Pdf_Component>
    {
        public override void Awake(View_Pdf_Component component)
        {
            component.pdfFilePath = Path.Combine(Application.streamingAssetsPath, "PDF");
            component.View = component.Refs.Get<PDFViewer>("PDFViewer");

            DirectoryInfo directoryInfo = new DirectoryInfo(component.pdfFilePath);
            var files = directoryInfo.GetFiles("*.pdf", SearchOption.TopDirectoryOnly);

            component.items = new Dictionary<FileInfo, GameObject>();
            var itemPre = component.Refs.Get<GameObject>("PDFItem");
            var content = component.Refs.Get<RectTransform>("content");
            bool first = true;
            foreach (var file in files)
            {
                var item = UnityEngine.GameObject.Instantiate<GameObject>(itemPre, content);
                item.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = file.Name.Replace(".pdf", "").Replace("_", "\r\n");
                ZEvent.UIEvent.AddListener(item, component.Btn, file);
                component.items.Add(file, item);

                if (first)
                {
                    component.View.LoadDocumentFromStreamingAssets("PDF", file.Name, "");
                    component.localSelect = file;
                    item.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                    first = false;
                }
            }
            ZEvent.UIEvent.AddListener(component.Refs.Get<Image>("Image"), component.Close);
        }
    }

    public class View_Pdf_Show : UIShowSystem<View_Pdf_Component>
    {
        public override void OnShow(View_Pdf_Component canvas)
        {
            canvas.gameObject.SetActive(true);
        }
    }

    public class View_Pdf_Hide : UIHideSystem<View_Pdf_Component>
    {
        public override void OnHide(View_Pdf_Component canvas)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    //逻辑
    public static class View_Pdf_System
    {
        public static void Btn(this View_Pdf_Component component, UIEventData<FileInfo> eventData)
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

                            component.View.LoadDocumentFromStreamingAssets("PDF", eventData.Data0.Name, "");
                            eventData.Target.GetComponent<UnityEngine.UI.Image>().color = Color.white;
                            component.localSelect = eventData.Data0;
                        }
                    }
                    break;
            }
          
        }

        public static void Close(this View_Pdf_Component component, UIEventData eventData)
        {
            if (eventData.EventType == UIEventType.Click)
            {
                (Game.UI.Get(UIType.View_Left) as View_Left_Component).Clear();
            }
        }
    }
}
