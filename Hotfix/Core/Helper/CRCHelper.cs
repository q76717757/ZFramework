using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public static class CRCHelper
    {
        public static ushort GetCRC(byte[] data, int offset, int count)
        {
            ushort crc = 0xFFFF; // CRC初始值
            ushort polynomial = 0xA001;  // CRC校验多项式

            for (int i = offset; i < count; i++)
            {
                crc ^= data[i];   // 低8位与CRC异或
                for (ushort j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0) //crc最后一位不为零
                    {
                        crc = (ushort)((ushort)(crc >> 1) ^ polynomial);   //右移一位  异或多项式
                    }
                    else   //CRC的最后一位为0
                    {
                        crc = (ushort)(crc >> 1);   // 右移一位
                    }
                }
            }
            return crc;
        }
        public static ushort GetCRC(string input)
        {
            input = input.Replace(" ", "");
            var byteCount = input.Length / 2;

            byte[] bytes = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
            {
                bytes[i] = Convert.ToByte(input.Substring(i * 2, 2), 16);
            }
            return GetCRC(bytes, 0, byteCount);
        }


        //大小端转换
        public static ushort Swap(ushort input)
        {
            return (ushort)((ushort)((input & 0x00ff) << 8) | ((ushort)(input & 0xff00) >> 8));
        }

    }
}
