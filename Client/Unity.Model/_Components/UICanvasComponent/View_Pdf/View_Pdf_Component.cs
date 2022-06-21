
using UnityEngine;
using System;
using Paroxe.PdfRenderer;
using System.IO;
using System.Collections.Generic;

namespace ZFramework
{
    [UIType(UIType.View_Pdf)]
    public class View_Pdf_Component : UICanvasComponent
    {
        public string pdfFilePath;
        public PDFViewer View;

        public FileInfo localSelect;
        public Dictionary<FileInfo, GameObject> items;
    }
}
