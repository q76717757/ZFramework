//------------------------------------------------------------------------------
//  此代码版权（除特别声明或在XREF结尾的命名空间的代码）归作者本人若汝棋茗所有
//  源代码使用协议遵循本仓库的开源协议及附加协议，若本仓库没有设置，则按MIT开源协议授权
//  CSDN博客：https://blog.csdn.net/qq_40374647
//  哔哩哔哩视频：https://space.bilibili.com/94253567
//  Gitee源代码仓库：https://gitee.com/RRQM_Home
//  Github源代码仓库：https://github.com/RRQM
//  API首页：https://www.yuque.com/rrqm/touchsocket/index
//  交流QQ群：234762506
//  感谢您的下载和使用
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using TouchSocket.Core;
using TouchSocket.Core.ByteManager;
using ZFramework;

namespace TouchSocket.Sockets
{
    /// <summary>
    /// 固定包头数据包处理适配器，支持Byte、UShort、Int三种类型作为包头。使用<see cref="TouchSocketBitConverter.DefaultEndianType"/>大小端设置。
    /// </summary>
    public class FixedHeaderPackageAdapter : DealDataHandlingAdapter
    {
        private byte[] m_agreementTempBytes;
        private int m_surPlusLength = 0;

        //包剩余长度
        private ByteBlock m_tempByteBlock;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanSendRequestInfo => false;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanSplicingSend => true;

        /// <summary>
        /// 设置包头类型，默认为int
        /// </summary>
        public FixedHeaderType FixedHeaderType { get; set; } = FixedHeaderType.Int;

        /// <summary>
        /// 获取或设置包数据的最小值（默认为0）
        /// </summary>
        public int MinPackageSize { get; set; } = 0;

        /// <summary>
        /// 当接收到数据时处理数据
        /// </summary>
        /// <param name="byteBlock">数据流</param>
        protected override void PreviewReceived(ByteBlock byteBlock)
        {
            byte[] buffer = byteBlock.Buffer;
            int r = byteBlock.Len;

            if (this.CacheTimeoutEnable && DateTime.Now - this.LastCacheTime > this.CacheTimeout)
            {
                this.Reset();
            }

            if (this.m_agreementTempBytes != null)
            {
                this.SeamPackage(buffer, r);
            }
            else if (this.m_tempByteBlock == null)
            {
                this.SplitPackage(buffer, 0, r);
            }
            else
            {
                if (this.m_surPlusLength == r)
                {
                    this.m_tempByteBlock.Write(buffer, 0, this.m_surPlusLength);
                    this.PreviewHandle(this.m_tempByteBlock);
                    this.m_tempByteBlock = null;
                    this.m_surPlusLength = 0;
                }
                else if (this.m_surPlusLength < r)
                {
                    this.m_tempByteBlock.Write(buffer, 0, this.m_surPlusLength);
                    this.PreviewHandle(this.m_tempByteBlock);
                    this.m_tempByteBlock = null;
                    this.SplitPackage(buffer, this.m_surPlusLength, r);
                }
                else
                {
                    this.m_tempByteBlock.Write(buffer, 0, r);
                    this.m_surPlusLength -= r;
                    if (this.UpdateCacheTimeWhenRev)
                    {
                        this.LastCacheTime = DateTime.Now;
                    }
                }
            }
        }

        /// <summary>
        /// 当发送数据前处理数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="isAsync"></param>
        protected override void PreviewSend(byte[] buffer, int offset, int length, bool isAsync)
        {
            if (length < this.MinPackageSize)
            {
                throw new Exception("发送数据小于设定值，相同解析器可能无法收到有效数据，已终止发送");
            }

            if (length > this.MaxPackageSize)
            {
                throw new Exception("发送数据大于设定值，相同解析器可能无法收到有效数据，已终止发送");
            }

            ByteBlock byteBlock = null;
            byte[] lenBytes = null;

            switch (this.FixedHeaderType)
            {
                case FixedHeaderType.Byte:
                    {
                        byte dataLen = (byte)(length - offset);
                        byteBlock = BytePool.GetByteBlock(dataLen + 1);
                        lenBytes = new byte[] { dataLen };
                        break;
                    }
                case FixedHeaderType.Ushort:
                    {
                        ushort dataLen = (ushort)(length - offset);
                        byteBlock = BytePool.GetByteBlock(dataLen + 2);
                        lenBytes = TouchSocketBitConverter.Default.GetBytes(dataLen);
                        break;
                    }
                case FixedHeaderType.Int:
                    {
                        int dataLen = length - offset;
                        byteBlock = BytePool.GetByteBlock(dataLen + 4);
                        lenBytes = TouchSocketBitConverter.Default.GetBytes(dataLen);
                        break;
                    }
            }

            try
            {
                byteBlock.Write(lenBytes);
                byteBlock.Write(buffer, offset, length);
                if (isAsync)
                {
                    byte[] data = byteBlock.ToArray();
                    this.GoSend(data, 0, data.Length, isAsync);//使用ByteBlock时不能异步发送
                }
                else
                {
                    this.GoSend(byteBlock.Buffer, 0, byteBlock.Len, isAsync);
                }
            }
            finally
            {
                byteBlock.Dispose();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="transferBytes"></param>
        /// <param name="isAsync"></param>
        protected override void PreviewSend(IList<ArraySegment<byte>> transferBytes, bool isAsync)
        {
            int length = 0;
            foreach (var item in transferBytes)
            {
                length += item.Count;
            }

            if (length < this.MinPackageSize)
            {
                throw new Exception("发送数据小于设定值，相同解析器可能无法收到有效数据，已终止发送");
            }

            if (length > this.MaxPackageSize)
            {
                throw new Exception("发送数据大于设定值，相同解析器可能无法收到有效数据，已终止发送");
            }

            ByteBlock byteBlock = null;
            byte[] lenBytes = null;

            switch (this.FixedHeaderType)
            {
                case FixedHeaderType.Byte:
                    {
                        byte dataLen = (byte)length;
                        byteBlock = BytePool.GetByteBlock(dataLen + 1);
                        lenBytes = new byte[] { dataLen };
                        break;
                    }
                case FixedHeaderType.Ushort:
                    {
                        ushort dataLen = (ushort)length;
                        byteBlock = BytePool.GetByteBlock(dataLen + 2);
                        lenBytes = TouchSocketBitConverter.Default.GetBytes(dataLen);
                        break;
                    }
                case FixedHeaderType.Int:
                    {
                        byteBlock = BytePool.GetByteBlock(length + 4);
                        lenBytes = TouchSocketBitConverter.Default.GetBytes(length);
                        break;
                    }
            }

            try
            {
                byteBlock.Write(lenBytes);

                foreach (var item in transferBytes)
                {
                    byteBlock.Write(item.Array, item.Offset, item.Count);
                }

                if (isAsync)
                {
                    byte[] data = byteBlock.ToArray();
                    this.GoSend(data, 0, data.Length, isAsync);//使用ByteBlock时不能异步发送
                }
                else
                {
                    this.GoSend(byteBlock.Buffer, 0, byteBlock.Len, isAsync);
                }
            }
            finally
            {
                byteBlock.Dispose();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="isAsync"></param>
        protected override void PreviewSend(IRequestInfo requestInfo, bool isAsync)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        protected override void Reset()
        {
            this.m_agreementTempBytes = null;
            this.m_surPlusLength = default;
            this.m_tempByteBlock?.Dispose();
            this.m_tempByteBlock = null;
        }

        private void PreviewHandle(ByteBlock byteBlock)
        {
            try
            {
                this.GoReceived(byteBlock, null);
            }
            finally
            {
                byteBlock.Dispose();
            }
        }

        /// <summary>
        /// 缝合包
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="r"></param>
        private void SeamPackage(byte[] buffer, int r)
        {
            ByteBlock byteBlock = BytePool.GetByteBlock(r + this.m_agreementTempBytes.Length);
            byteBlock.Write(this.m_agreementTempBytes);
            byteBlock.Write(buffer, 0, r);
            r += this.m_agreementTempBytes.Length;
            this.m_agreementTempBytes = null;
            this.SplitPackage(byteBlock.Buffer, 0, r);
            byteBlock.Dispose();
        }

        /// <summary>
        /// 分解包
        /// </summary>
        /// <param name="dataBuffer"></param>
        /// <param name="index"></param>
        /// <param name="r"></param>
        private void SplitPackage(byte[] dataBuffer, int index, int r)
        {
            while (index < r)
            {
                if (r - index <= (byte)this.FixedHeaderType)
                {
                    this.m_agreementTempBytes = new byte[r - index];
                    Array.Copy(dataBuffer, index, this.m_agreementTempBytes, 0, this.m_agreementTempBytes.Length);
                    if (this.UpdateCacheTimeWhenRev)
                    {
                        this.LastCacheTime = DateTime.Now;
                    }
                    return;
                }
                int length = 0;

                switch (this.FixedHeaderType)
                {
                    case FixedHeaderType.Byte:
                        length = dataBuffer[index];
                        break;

                    case FixedHeaderType.Ushort:
                        length = TouchSocketBitConverter.Default.ToUInt16(dataBuffer, index);
                        break;

                    case FixedHeaderType.Int:
                        length = TouchSocketBitConverter.Default.ToInt32(dataBuffer, index);
                        break;
                }

                if (length < 0)
                {
                    throw new Exception("接收数据长度错误，已放弃接收");
                }
                else if (length < this.MinPackageSize)
                {
                    throw new Exception("接收数据长度小于设定值，已放弃接收");
                }
                else if (length > this.MaxPackageSize)
                {
                    //Log.Info(this.FixedHeaderType + "-->" +length);
                    throw new Exception("接收数据长度大于设定值，已放弃接收");
                    Log.Error("接收数据长度大于设定值，已放弃接收");
                }

                int recedSurPlusLength = r - index - (byte)this.FixedHeaderType;
                if (recedSurPlusLength >= length)
                {
                    ByteBlock byteBlock = BytePool.GetByteBlock(length);
                    byteBlock.Write(dataBuffer, index + (byte)this.FixedHeaderType, length);
                    this.PreviewHandle(byteBlock);
                    this.m_surPlusLength = 0;
                }
                else//半包
                {
                    this.m_tempByteBlock = BytePool.GetByteBlock(length);
                    this.m_surPlusLength = length - recedSurPlusLength;
                    this.m_tempByteBlock.Write(dataBuffer, index + (byte)this.FixedHeaderType, recedSurPlusLength);
                    if (this.UpdateCacheTimeWhenRev)
                    {
                        this.LastCacheTime = DateTime.Now;
                    }
                }
                index += (length + (byte)this.FixedHeaderType);
            }
        }
    }
}