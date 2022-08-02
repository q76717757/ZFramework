using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using ZFramework;

public class PortControl : MonoBehaviour
{
    public const string EV = "端口信息";

    public static PortControl instance;

    SerialPort sp = null;
    Thread dataReceiveThread;

    void Awake()
    {
        instance = this;
        //var c = Convert.ToString(200000, 16);
        //var d = Convert.ToInt32("FFFCF2C0", 16);
        //Debug.Log(c);
        //Debug.Log(d);
        //OpenPort();
    }
    void OnApplicationQuit()=> ClosePort();

    Queue<byte> callbackQueue = new Queue<byte>();//收到的数据  待处理队列
    byte[] sendBuff = new byte[1024];//即将发送的数据  进行CRC校验 至少有1位
    int sendBuffIndex = 0;//缓冲区游标  指向第一个空位(如  02 01  index = 2 )

    public bool OpenPort(string portName)
    {
        try
        {
            if (sp == null)
            {
                int baudRate = 9600;//波特率
                int dataBits = 8;//数据位
                Parity parity = Parity.None;//效验位
                StopBits stopBits = StopBits.One;//停止位
                sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
                sp.Open();

                dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));
                dataReceiveThread.Start();

                return true;
            }
            else
            {
                Debug.Log($"端口{portName}已打开");
                return false;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return false;
        }
    }
    void ClosePort()
    {
        try
        {
            if (sp != null)
            {
                sp.Close();
                dataReceiveThread.Abort();
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    #region 接收数据
    void DataReceiveFunction()
    {
        byte[] buffer = new byte[1024];
        int bytes = 0;
        while (true)
        {
            if (sp != null && sp.IsOpen)
            {
                try
                {
                    bytes = sp.Read(buffer, 0, buffer.Length);//接收字节   用事件没反应
                    if (bytes == 0)
                    {
                        continue;
                    }
                    else
                    {

                        //全量接受数据 进待处理消息队列
                        for (int i = 0; i < bytes; i++)
                        {
                            callbackQueue.Enqueue(buffer[i]);
                        }

                        //收完就进行处理 对数据进行逐位CRC验证
                        while (callbackQueue.Count > 0)
                        {
                            var b = callbackQueue.Dequeue();
                            sendBuff[sendBuffIndex] = b;
                            sendBuffIndex++;

                            //电机协议最少的回调是8bit 游标超过把再进行CRC验证
                            if (sendBuffIndex >= 8)
                            {
                                var crc = GetCRC(sendBuff, 0, sendBuffIndex - 2);
                                if (sendBuff[sendBuffIndex - 2] == crc[0] && sendBuff[sendBuffIndex - 1] == crc[1])//crc验证通过
                                {
                                    var sendByte = new byte[sendBuffIndex];
                                    for (int i = 0; i < sendBuffIndex; i++)
                                    {
                                        sendByte[i] = sendBuff[i];
                                    }

                                    //-------
                                    StringBuilder sb = new StringBuilder();
                                    for (int i = 0; i < sendByte.Length; i++)
                                    {
                                        var s = Convert.ToString(sendByte[i], 16);
                                        if (s.Length == 1)
                                        {
                                            sb.Append("0");
                                        }
                                        sb.Append(s);
                                    }
                                    SendMSG($"收到来自串口的消息<-{sb.ToString()}");
                                    //-----------------

                                    sendBuffIndex = 0;
                                    CallbackAction?.Invoke(sendByte);
                                    break;
                                }
                            }
                            
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
            Thread.Sleep(50);
        }
    }
    #endregion

    #region 发送数据
    public static void Write(string dataStr, bool useCRC = false) => instance.WriteData(dataStr, useCRC);
    void WriteData(string dataStr,bool useCRC = false)
    {
        if (sp == null || !sp.IsOpen) return;

        dataStr = dataStr.Replace(" ", "");
        var code = new string[dataStr.Length / 2];// 57 DE 01 06 60 40 00 ...

        for (int i = 0; i < code.Length; i++)
        {
            code[i] = dataStr.Substring(i * 2, 2);
        }

        byte[] bytes = new byte[code.Length + (useCRC ? 2 : 0)];
        for (int i = 0; i < code.Length; i++)
        {
            bytes[i] = Convert.ToByte(code[i], 16);
        }

        if (useCRC)
        {
            var crc = GetCRC(bytes, 0, bytes.Length);
            bytes[code.Length + 0] = crc[0];
            bytes[code.Length + 1] = crc[1];
        }

        sp.Write(bytes, 0, bytes.Length);

#if UNITY_EDITOR
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            sb.Append(Convert.ToString(bytes[i], 16));
            sb.Append(" ");
        }
        Debug.Log("Write-->" + sb.ToString());
#endif
    }
    public static string GetCRC(string input)
    {
        input = input.Replace(" ", "");
        var byteCount = input.Length / 2;

        byte[] bytes = new byte[byteCount];
        for (int i = 0; i < byteCount; i++)
        {
            bytes[i] = Convert.ToByte(input.Substring(i * 2, 2), 16);
        }
        return GetCRC(bytes);
    }
    public static string GetCRC(byte[] bytes)
    {
        var crc = GetCRC(bytes, 0, bytes.Length);
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
    public static byte[] GetCRC(byte[] arr, int seat, int len)
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

    #endregion      

    public static void MoveLeft()
    {
        SendMSG("<color=green>左移</color>");
        Write("01 10 60 40 00 06 0C 00 0F 00 03 00 00 00 28 03 32 03 E8 F6 EE");
    }
    public static void MoveRight()
    {
        SendMSG("<color=green>右移</color>");
        Write("01 10 60 40 00 06 0C 00 0F 00 03 FF FF FF D8 03 32 03 E8 E2 FF");
    }
    public static void StopMove()
    {
        SendMSG("<color=green>刹车</color>");
        Write("01 06 60 40 00 03 D6 1F");
    }
    public static void GetValue()
    {
        SendMSG("<color=green>查询</color>");
        Write("01 03 60 64 00 02 9B D4");
    }

    public static async void 设置回零()
    {
        SendMSG("设置回零");

        Write("01 06 60 40 00 01 57 DE");//上电初始化
        SendMSG($"------------------驱动器上电初始化");
        await new WaitPortCallback("01066040000157de");

        Write("01 06 60 40 00 03 D6 1F");//刹车 // 等价 ->StopMove();
        SendMSG($"------------------驱动器正常运行，但电机未使能，松开刹车");
        await new WaitPortCallback("010660400003d61f");

        Write("01 06 60 40 00 0F D6 1A");//给电
        SendMSG($"------------------电机给电，处于可运行状态");
        await new WaitPortCallback("01066040000fd61a");

        //左对齐
        PortControl.MoveLeft();
        await new WaitPortCallback("0110604000065fdf");

        PortControl.GetValue();
        int last = await new WaitGetValueCallback();

        while (true)
        {
            await Task.Delay(500);

            var next = await new WaitGetValueCallback();

            if (Mathf.Abs(last - next) < 100)
            {
                Debug.Log("到达左限位器");
                SendMSG($"------------------到达左限位器");

                PortControl.StopMove();
                await new WaitPortCallback("010660400003d61f");
                break;
            }
            last = next;
        }


        Write("01 06 60 60 00 06 17 D6");//设置成回零模式
        SendMSG($"------------------设置成回零模式");
        await new WaitPortCallback("01066060000617d6");

        Write("01 06 60 98 00 00 16 25");//设置回零方式
        SendMSG($"------------------设置回零方式");
        await new WaitPortCallback("0106609800001625");

        //Write($"01 10 60 99 00 02 04 00 00 00 {BootStrap.optionValues[2]} 13 7E");//设置回零速度
        SendMSG($"------------------设置回零速度");
        await new WaitPortCallback("0110609900028fe7");

        Write("01 06 60 9A 03 E8 B7 5B");//设置回零加减速
        SendMSG($"------------------设置回零加减速");
        await new WaitPortCallback("0106609a03e8b75b");

        Write("01 06 60 40 00 1F D7 D6");//启动回零
        SendMSG($"------------------启动回零");
        await new WaitPortCallback("01066040001fd7d6");

        //位置模式--->

        Write("01 06 60 60 00 01 56 14");//设置位置模式
        SendMSG($"------------------设置位置模式");
        await  new WaitPortCallback("0106606000015614");

        //Write($"01 10 60 81 00 02 04 {BootStrap.optionValues[3]} 92 1F");//设置目标速度
        SendMSG($"------------------设置目标速度");
        await new WaitPortCallback("0110608100020fe0");

        //Write($"01 06 60 83 {BootStrap.optionValues[4]} 66 9C");//设置加速度
        SendMSG($"------------------设置加速度");
        await new WaitPortCallback("0106608303e8669c");

        //Write($"01 06 60 84 {BootStrap.optionValues[5]} D7 5D");//设置减速度
        SendMSG($"------------------设置减速度");
        await new WaitPortCallback("0106608403e8d75d");
    }

    static bool isMoving = false;
    public static async Task 移动到(int value)
    {
        if (isMoving) return;
        isMoving = true;
        SendMSG("移动到->" + value);

        Write("01 06 60 40 00 0F D6 1A");//上电使能
        SendMSG($"------------------上电使能");
        await new WaitPortCallback("01066040000fd61a");

        var valueStr = Convert.ToString(value, 16);
        var zeroCount = 8 - valueStr.Length;
        for (int i = 0; i < zeroCount; i++)
        {
            valueStr = valueStr.Insert(0, "0");//补0到8位
        }

        Write($"01 10 60 7A 00 02 04 {valueStr}", true);//设置目标位置（绝对定位）
        SendMSG($"------------------设置目标位置");
        await new WaitPortCallback("0110607a00027e11");

        Write("01 06 60 40 00 1F D7 D6");//启动运行到目标位置 和启动回零同一个指令
        SendMSG($"------------------启动运行到目标位置");
        await new WaitPortCallback("01066040001fd7d6");

        //等待回零 然后刹车
        while (true)
        {
            await Task.Delay(1000);
            PortControl.GetValue();
            int last = await new WaitGetValueCallback();

            if (Mathf.Abs(value - last) < 100)
            {
                Debug.Log("到达目的地");
                SendMSG($"------------------到达目的地");
                StopMove();
                await new WaitPortCallback("010660400003d61f");
                isMoving = false;
                return;
            }
        }
    }

    public static Action<byte[]> CallbackAction;//缓冲区的内容超过8位并通过CRC验证  就发出广播 游标归零


    public static void SendMSG(string msg) => messageAction?.Invoke(msg);
    public static Action<string> messageAction;

    #region 旧的

    static int LastPosition;//上一次查询的位置
    public static void Callback(string value)//回调
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
                return;
            }
        }
        if (value.Length == 18)//查询位置反馈
        {
            if (value.Substring(0, 6) == "010304")
            {
                var remoteCRC = value.Substring(value.Length - 4, 4);
                Debug.Log("RetemoCRC->" + remoteCRC);
                var notCrc = value.Substring(0, value.Length - 4);
                var LocalCRC = GetCRC(notCrc);
                Debug.Log("LocalCRC->" + LocalCRC);

                if (remoteCRC == LocalCRC)//校验通过
                {
                    var valueString = notCrc.Substring(6, notCrc.Length - 6);
                    LastPosition = Convert.ToInt32(valueString, 16);

                    Debug.Log($"<color=green>位置->{LastPosition}</color>");
                }
            }
        }
    } 

    #endregion

}

public class WaitPortCallback
{
    byte[] keys;
    TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();
    public WaitPortCallback(string input)
    {
        input = input.Replace(" ", "");
        keys = new byte[input.Length / 2];
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i] = Convert.ToByte(input.Substring(i * 2, 2), 16);
        }
    }

    public TaskAwaiter GetAwaiter()
    {
        PortControl.CallbackAction += Callback;
        return ((Task)taskCompletionSource.Task).GetAwaiter();
    }

    void Callback(byte[] value)
    {
        if (value.Length == keys.Length)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] != keys[i])
                {
                    taskCompletionSource.SetResult(false);
                    return;
                }
            }
        }
        PortControl.CallbackAction -= Callback;
        taskCompletionSource.SetResult(null);
    }
}

public class WaitGetValueCallback
{
    TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>();

    public TaskAwaiter<int> GetAwaiter()
    {
        PortControl.CallbackAction += Callback;
        return taskCompletionSource.Task.GetAwaiter();
    }

    void Callback(byte[] value)
    {
        if (value.Length == 9)//查询位置反馈 固定是9位
        {
            //if (value.Substring(0, 6) == "010304")
            if (value[0] == 1 && value[1] == 3 && value[2] == 4)
            {
                byte[] remoteCRC = new byte[] { value[7], value[8] };
                byte[] LocalCRC = PortControl.GetCRC(value, 0, 9 - 2);

                if (remoteCRC[0] == LocalCRC[0] && remoteCRC[1] == LocalCRC[1])//校验通过
                {
                    var a = Convert.ToString(value[3], 16);
                    var b = Convert.ToString(value[4], 16);
                    var c = Convert.ToString(value[5], 16);
                    var d = Convert.ToString(value[6], 16);

                    var LastPosition = Convert.ToInt32($"{a}{b}{c}{d}", 16);

                    Debug.Log($"<color=green>位置->{LastPosition}</color>");
                    PortControl.SendMSG($"<color=green>位置------------->{LastPosition}</color>");

                    PortControl.CallbackAction -= Callback;
                    taskCompletionSource.SetResult(LastPosition);
                }
            }
        }
    }
}

