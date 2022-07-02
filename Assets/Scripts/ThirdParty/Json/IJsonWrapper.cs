/** Header
 * Json数据容器，定义能够处理各种JSON的数据类型
 * 具体是由JsonData实现的。
 **/


using System.Collections;
using System.Collections.Specialized;


namespace ZFramework
{
    /// <summary> 指示JsonData内部的类型 </summary>
    public enum JsonType
    {
        None,

        Object,
        Array,
        String,
        Int,
        Long,
        Double,
        Boolean
    }

    /// <summary> Json容器接口 用于定义一个json容器对象 </summary>
    internal interface IJsonWrapper : IList, IOrderedDictionary
    {
        bool IsArray { get; }
        bool IsBoolean { get; }
        bool IsDouble { get; }
        bool IsInt { get; }
        bool IsLong { get; }
        bool IsObject { get; }
        bool IsString { get; }

        bool GetBoolean();
        double GetDouble();
        int GetInt();
        JsonType GetJsonType();
        long GetLong();
        string GetString();

        void SetBoolean(bool val);
        void SetDouble(double val);
        void SetInt(int val);
        void SetJsonType(JsonType type);
        void SetLong(long val);
        void SetString(string val);

        string ToJson();
        void ToJson(JsonWriter writer);

        T ToObject<T>();
    }
}
