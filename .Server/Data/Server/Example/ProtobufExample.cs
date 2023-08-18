//------------------------------------------------------------
// File : ProtobufExample.cs
// Gitlab: http://192.168.0.128:7680/Mr_Tan
// Emial:
// Desc : Protobuf使用案例
//------------------------------------------------------------
namespace ZFramework.Example
{
    //引入ProtoBuf包,已经植入框架内,Server.Data下的Protobuf
    using ProtoBuf;
    using System.IO;
    using System;
    public class ProtobufExample
    {

        #region 消息结构基类
        public enum PType
        {
            C2S_账号登入
        }
        public interface INetSerialize
        {
            public PType Code { get; }
        }
        #endregion

        /// <summary>
        /// 如果想被Protobuf序列化和反序列化，需要在类型和字段上标记特性
        /// 特性: [ProtoContract] 用于标记类型
        /// 特性: [ProtoMember(int tag)] 用于标记字段,tag不可重复
        /// </summary>
        [ProtoContract]
        public class C2S_账号登入 : INetSerialize
        {
            public PType Code => PType.C2S_账号登入;
            [ProtoMember(1)]
            public string id;
        }


        // 将消息序列化为二进制的方法
        // < param name="model">要序列化的对象< /param>
        public static byte[] Serialize<T>(T model) where T : INetSerialize
        {
            try
            {
                //涉及格式转换，需要用到流，将二进制序列化到流中
                using (MemoryStream ms = new MemoryStream())
                {
                    PType type = model.Code;
                    //使用ProtoBuf工具的序列化方法
                    Serializer.Serialize(ms, model);
                    //定义二级制数组，保存序列化后的结果
                    byte[] result = new byte[ms.Length + 1];
                    result[0] = (byte)type;
                    //将流的位置设为0，起始点
                    ms.Position = 0;
                    //将流中的内容读取到二进制数组中
                    ms.Read(result, 1, result.Length - 1);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Log.Error("序列化失败: " + ex.ToString());
                return null;
            }
        }

        // 将收到的消息反序列化成对象
        // < returns>The serialize.< /returns>
        // < param name="msg">收到的消息.</param>
        public static T DeSerialize<T>(byte[] msg) where T : INetSerialize
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //将消息写入流中
                    ms.Write(msg, 1, msg.Length - 1);
                    //将流的位置归0
                    ms.Position = 0;
                    //使用工具反序列化对象
                    T result = Serializer.Deserialize<T>(ms);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Log.Error("反序列化失败: " + ex.ToString());
                return default;
            }
        }
    }
}

