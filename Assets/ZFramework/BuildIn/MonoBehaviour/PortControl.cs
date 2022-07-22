using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
//using TMPro; 

public class PortControl : MonoBehaviour
{
    public static PortControl instance;

    public string portName = "COM3";//串口名

    [NonSerialized]
    public int baudRate = 9600;//波特率
    [NonSerialized]
    public int dataBits = 8;//数据位
    [NonSerialized]
    public Parity parity = Parity.None;//效验位
    [NonSerialized]
    public StopBits stopBits = StopBits.One;//停止位

    SerialPort sp = null;
    Thread dataReceiveThread;

    static object lockObj = new object();

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        //var c = Convert.ToString(200000, 16);
        //var d = Convert.ToInt32("FFFCF2C0", 16);
        //Debug.Log(c);
        //Debug.Log(d);

        //OpenPort();
    }
    void OnApplicationQuit()=> ClosePort();

    public void OpenPort()
    {
        try
        {
            if (sp == null)
            {
                sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
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
    void ClosePort()
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
                            if (s.Length == 1)
                            {
                                s = $"0{s}";
                            }

                            lock (lockObj)
                            {
                                cc += s;
                            }
                            Call(cc);
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
    public static void Write(string dataStr, bool useCRC) => instance.WriteData(dataStr, useCRC);
    void WriteData(string dataStr,bool useCRC)
    {
        if (!string.IsNullOrEmpty(cc))
        {
            Debug.LogError("CC has value");//有接受到的消息还没有处理
            return;
        }
        
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
    static string GetCRC(string input)
    {
        input = input.Replace(" ", "");
        var len = input.Length;

        var a = new string[len / 2];

        for (int i = 0; i < a.Length; i++)
        {
            a[i] = input.Substring(i * 2, 2);
        }

        byte[] aa = new byte[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            aa[i] = Convert.ToByte(a[i], 16);
        }

        var crc = Crc(aa, 0, a.Length);

        var c0 = Convert.ToString(crc[0], 16);
        var c1 = Convert.ToString(crc[1], 16);
        if (c0.Length == 1)
        {
            c0 = "0" + c0;
        }
        if (c1.Length == 1)
        {
            c1 = "0" + c1;
        }
        return $"{c0}{c1}";
    }

    #endregion


    string aa = "9600";
    string bb = "";
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
                OpenPort();
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
            //cc = GUILayout.TextField(cc,GUILayout.Width(500));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("关闭"))
            {
                ClosePort();
            }

            if (GUILayout.Button("左移"))
            {
                MoveLeft();
            }
            if (GUILayout.Button("右移"))
            {
                MoveRight();
            }
            if (GUILayout.Button("刹车"))
            {
                StopMove();
            }
            if (GUILayout.Button("查询"))
            {
                GetValue();
            }

            moveTargetStr = GUILayout.TextField(moveTargetStr);
            if (!string.IsNullOrEmpty(moveTargetStr) && int.TryParse(moveTargetStr,out int value))
            {
                if (GUILayout.Button("移动"))
                {
                    移动到(value);
                }
            }

            if (GUILayout.Button("归零"))
            {
                PortControl.设置回零();
            }
            
        }
    }
    string moveTargetStr;


    bool usecrc;
    
    
    
    static byte[] Crc(byte[] arr, int seat, int len)
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

    public static void MoveLeft()//左移
    {
        instance.WriteData("01 10 60 40 00 06 0C 00 0F 00 03 00 00 00 28 03 32 03 E8", true);
    }
    public static void MoveRight()//右移
    {
        instance.WriteData("01 10 60 40 00 06 0C 00 0F 00 03 FF FF FF D8 03 32 03 E8", true);
    }
    public static void StopMove()//刹车
    {
        instance.WriteData("01 06 60 40 00 03", true);
    }
    public static void GetValue()//查询
    {
        instance.WriteData("01 03 60 64 00 02", true);
    }


    public static async void 设置回零()
    {
        int t = 200;

        PortControl.Write("01 06 60 40 00 01 57 DE", false);//上电初始化
        await Task.Delay(t);

        PortControl.Write("01 06 60 40 00 03 D6 1F", false);//刹车
        await Task.Delay(t);

        PortControl.Write("01 06 60 40 00 0F D6 1A", false);//给电
        await Task.Delay(t);

        //左对齐
        PortControl.MoveLeft();
        await Task.Delay(t);
        while (true)
        {
            int last = PortControl.LastPosition;

            PortControl.GetValue();
            await Task.Delay(t);
            int next = PortControl.LastPosition;

            if (Mathf.Abs(last - next) < 100)
            {
                Debug.Log("到达左限位器");
                PortControl.StopMove();
                break;
            }
        }

        await Task.Delay(t);
        PortControl.Write("01 06 60 60 00 06 17 D6", false);//设置成回零模式
        await Task.Delay(t);
        PortControl.Write("01 06 60 98 00 00 16 25", false);//设置回零方式
        await Task.Delay(t);
        PortControl.Write("01 10 60 99 00 02 04 00 00 00 32 13 7E", false);//设置回零速度
        await Task.Delay(t);
        PortControl.Write("01 06 60 9A 03 E8 B7 5B", false);//设置回零加减速
        await Task.Delay(t);
        PortControl.Write("01 06 60 40 00 1F D7 D6", false);//启动回零
        await Task.Delay(t);

        PortControl.Write("01 06 60 60 00 01 56 14", false);//设置位置模式
        await Task.Delay(t);
        PortControl.Write("01 10 60 81 00 02 04 00 00 00 28 92 1F", false);//设置目标速度
        await Task.Delay(t);
        PortControl.Write("01 06 60 83 03 E8 66 9C", false);//设置加速度
        await Task.Delay(t);
        PortControl.Write("01 06 60 84 03 E8 D7 5D", false);//设置减速度

    }
    public static async void 移动到(int value)
    {
        PortControl.Write("01 06 60 40 00 0F D6 1A", false);//上电使能
        await Task.Delay(200);

        var valueStr = Convert.ToString(value, 16);

        var zeroCount = 8 - valueStr.Length;
        for (int i = 0; i < zeroCount; i++)
        {
            valueStr = valueStr.Insert(0, "0");
        }

        PortControl.Write($"01 10 60 7A 00 02 04  {valueStr}", true);//设置目标位置（绝对定位）
        await Task.Delay(200);

        PortControl.Write("01 06 60 40 00 1F D7 D6", false);//启动运行到目标位置 和启动回零同一个指令

    }


    public static void Call(string value)//回调
    {
        //Debug.Log("回执->" + value);
        value = value.Replace(" ", "");

        if (value.Length == 16)//回执
        {
            bool clear = false;
            switch (value)
            {
                case "01066040000157de"://上电初始化
                //case "010660400003d61f"://松开刹车
                //case "01066040000fd61a"://给电

                case "010660400003d61f"://刹车
                case "0110604000065fdf"://左右移动
                case "01066060000617d6"://设置回零模式
                case "0106609800001625"://设置回零方式
                case "0110609900028fe7"://设置回零速度
                case "0106609a03e8b75b"://设置回零加减速
                case "01066040001fd7d6"://启动回零

                case "0106606000015614"://设置位置模式
                case "0110608100020fe0"://设置目标速度
                case "0106608303e8669c"://设置加速度
                case "0106608403e8d75d"://设置减速度 -->  初始化完成 //01030400000000fa33

                case "01066040000fd61a"://上电使能
                case "0110607a00027e11"://设置目标位置
                    //case "01066040001fd7d6"://启动运行到目标位置（绝对定位） 和启动回零同一个指令


                    Debug.Log("回执->" + value);
                    clear = true;
                    break;
            }

            if (clear)
            {
                lock (lockObj)
                {
                    instance.cc = String.Empty;
                }
                return;
            }
        }

        if (value.Length == 18)//查询位置反馈
        {
            Debug.Log("查询回调->" + value);
            var head =  value.Substring(0, 6);
            if (head == "010304")
            {
                var notCrc = value.Substring(0, value.Length - 4);
                Debug.Log("Not CRC-->" + notCrc);
                var remoteCRC = value.Substring(value.Length - 4, 4);
                Debug.Log("RetemoCRC->" + remoteCRC);
                var LocalCRC = GetCRC(notCrc);
                Debug.Log("LocalCRC->" + LocalCRC);

                if (remoteCRC == LocalCRC)//校验通过
                {
                    var valueString = notCrc.Substring(6, notCrc.Length - 6);
                    LastPosition = Convert.ToInt32(valueString, 16);

                    lock (lockObj)
                    {
                        instance.cc = String.Empty;
                    }
                    Debug.Log($"<color=green>位置->{LastPosition}</color>");
                }
            }
        }
    }

    public static int LastPosition;

}