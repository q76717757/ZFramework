using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;
using Excel;
using System.IO;
using System.Data;

public class ExcelHelper
{
    //备忘
    //read在发布win.exe的时候报空引用  把Unity\Editor\Data\Mono\lib\mono\unity下的所有I18N*.dll
    //复制到打包后的***_Data/Managed下 可以解决
    //或者全部引入Plugins也可以

    public static void ReadExcel() {
        string filePath = Path.Combine(Application.streamingAssetsPath, "123.xlsx");
        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);//读xlsx

        //string filePath2 = Path.Combine(Application.streamingAssetsPath, "123.xls");//读xls
        //FileStream stream2 = File.Open(filePath2, FileMode.Open, FileAccess.Read, FileShare.Read);
        //var excelReader2 = ExcelReaderFactory.CreateBinaryReader(stream2);

        DataSet result = excelReader.AsDataSet();
        int columns = result.Tables[0].Columns.Count;
        int rows = result.Tables[0].Rows.Count;

        for (int i = 0; i < columns; i++)//下标从0开始
        {
            for (int j = 0; j < rows; j++)
            {
                var aa = result.Tables[0].Rows[j][i];
            }
        }

        //tables可以按照sheet名获取，也可以按照sheet索引获取
        //return result.Tables[0].Rows;
        //return result.Tables["Sheet1"].Rows;
    }

    public static void WriteExcel() {
        string filePath = Path.Combine(Application.streamingAssetsPath, "123.xlsx");
        FileInfo newFile = new FileInfo(filePath);
        if (newFile.Exists)
        {
            newFile.Delete();
            newFile = new FileInfo(filePath);
        }

        using (ExcelPackage package = new ExcelPackage(newFile))
        {
            //新建工作表
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

            //两种方式都可以写入  下标从1开始
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Product";
            worksheet.Cells[1, 3].Value = "Quantity";
            worksheet.Cells[1, 4].Value = "Price";
            worksheet.Cells[1, 5].Value = "Value";

            worksheet.Cells["A3"].Value = 123.45f;
            worksheet.Cells["B3"].Value = "Hammer";
            worksheet.Cells["C3"].Value = 5;
            worksheet.Cells["D3"].Value = 12.10;

            //保存excel
            package.Save();
        }
    }


}
