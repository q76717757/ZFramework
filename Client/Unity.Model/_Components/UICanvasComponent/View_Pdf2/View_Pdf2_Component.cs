
using UnityEngine;
using System;
using Paroxe.PdfRenderer;
using System.IO;
using System.Collections.Generic;

namespace ZFramework
{
    [UIType(UIType.View_Pdf2)]
    public class View_Pdf2_Component:UICanvasComponent
    {
        public string pdfFilePath;
        public PDFViewer View;
        //public List<FileInfo> files;


        public Texture2D t2dL;
        public Texture2D t2dR;

        public FileInfo localSelect;
        public Dictionary<FileInfo, GameObject> items;
    }
}
