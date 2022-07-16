using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
//using TMPro; 

public class PortControl : MonoBehaviour
{
    public string portName = "COM3";//串口名
    public int baudRate = 9600;//波特率
    public Parity parity = Parity.None;//效验位
    public int dataBits = 8;//数据位
    public StopBits stopBits = StopBits.One;//停止位
    SerialPort sp = null;
    Thread dataReceiveThread;

    void Start() {

        //var c = Convert.ToString(200000, 16);
        //var d = Convert.ToInt32("FFFCF2C0", 16);
        //Debug.Log(c);
        //Debug.Log(d);
    }

    void OnApplicationQuit()
    {
        ClosePort();
    }
    public void ClosePort()
    {
        try
        {
            sp.Close();
            dataReceiveThread.Abort();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    #region 接收数据
    void DataReceiveFunction()
    {
        #region 按字节数组发送处理信息，信息缺失
        byte[] buffer = new byte[1024];
        int bytes = 0;
        while (true)
        {
            if (sp != null && sp.IsOpen)
            {
                try
                {
                    bytes = sp.Read(buffer, 0, buffer.Length);//接收字节
                    if (bytes == 0)
                    {
                        continue;
                    }
                    else
                    {
                        for (int i = 0; i < bytes; i++)
                        {
                            var s = Convert.ToString(buffer[i], 16);
                            cc += s + " ";
                            Debug.Log(s);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(ThreadAbortException))
                    {
                        Debug.Log(ex.Message);
                    }
                }
            }
            Thread.Sleep(100);
        }
        #endregion
    }
    #endregion

    #region 发送数据
    public void WriteData(string dataStr,bool useCRC)
    {
        dataStr = dataStr.Replace(" ", "");
        var len = dataStr.Length;

        var a = new string[len/2];

        for (int i = 0; i < a.Length; i++)
        {
            a[i] = dataStr.Substring(i * 2, 2);
        }

        if (sp.IsOpen)
        {
            byte[] aa = new byte[a.Length + (useCRC ? 2 : 0)];

            for (int i = 0; i < a.Length; i++)
            {
                aa[i] = Convert.ToByte(a[i], 16);
            }
            if (useCRC)
            {
                var crc = Crc(aa, 0,a.Length);
                aa[a.Length + 0] = crc[0];
                aa[a.Length + 1] = crc[1];
            }
            sp.Write(aa, 0, aa.Length);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < aa.Length; i++)
            {
                sb.Append(Convert.ToString(aa[i], 16));
                sb.Append(" ");
            }
            Debug.Log("Write-->" + sb.ToString());
        }
    }

    #endregion


    string aa = "9600";
    string bb = "发送内容";
    string cc;
    void OnGUI()
    {
        if (sp == null || !sp.IsOpen)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("端口");
            portName = GUILayout.TextField(portName);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("波特率");
            aa = GUILayout.TextField(aa);
            if (int.TryParse(aa, out int i))
            {
                baudRate = i;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("波特率");
            foreach (Parity item in System.Enum.GetValues(typeof(Parity)))
            {
                if (GUILayout.Toggle(parity == item, item.ToString()))
                {
                    parity = item;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("停止位");
            foreach (StopBits item in System.Enum.GetValues(typeof(StopBits)))
            {
                if (GUILayout.Toggle(stopBits == item, item.ToString()))
                {
                    stopBits = item;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("打开"))
            {
                try
                {
                    if (sp == null)
                    {
                        sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
                        //sp.ReadTimeout = 100;
                        //sp.ReadBufferSize = 1;
                        //sp.DataReceived += (sender, e) =>
                        //{
                        //    SerialPort sp = (SerialPort)sender;

                        //    string indata = sp.ReadExisting();
                        //    Debug.Log(indata);
                        //};
                        sp.Open();

                        dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));
                        dataReceiveThread.Start();
                    }
                    else
                    {
                        Debug.Log("sp != null");
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex.Message);
                }
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("发送");
            bb = GUILayout.TextField(bb,GUILayout.Width(500));
            if (GUILayout.Button("Send Input"))
            {
                WriteData(bb, usecrc);
            }
            usecrc = GUILayout.Toggle(usecrc,"CRC");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("收到");
            cc = GUILayout.TextField(cc,GUILayout.Width(500));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("关闭"))
            {
                ClosePort();
            }
        }
    }
    public int aaaaa;
    bool usecrc;
    public static byte[] Crc(byte[] arr, int seat, int len)
    {
        UInt16 j, uwCrcReg = 0xFFFF;

        for (int i = seat; i < len; i++)
        {
            uwCrcReg ^= arr[i];
            for (j = 0; j < 8; j++)
            {
                if ((uwCrcReg & 0x0001) != 0)
                {
                    uwCrcReg = (UInt16)((UInt16)(uwCrcReg >> 1) ^ (UInt16)0xA001);
                }
                else
                {
                    uwCrcReg = (UInt16)(uwCrcReg >> 1);
                }
            }
        }
        byte[] CRC = new byte[2];
        CRC[0] = (byte)(uwCrcReg);
        CRC[1] = (byte)(uwCrcReg >> 8);
        return CRC;
    }
}