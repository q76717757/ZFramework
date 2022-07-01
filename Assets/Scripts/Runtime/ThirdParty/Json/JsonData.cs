/** Header
 * JsonData.cs
 * 用于保存JSON数据(对象、数组等)的容器。
 * 这是 .ToObject()返回的默认类型。
 * JsonData的类型一旦确定不会变更  有3种容器形态  值/对象/数组
 **/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;

namespace ZFramework
{
    public sealed class JsonData : IJsonWrapper, IEquatable<JsonData>
    {
        #region 内部字段  存放实体数据
        private bool inst_boolean;
        private double inst_double;
        private int inst_int;
        private long inst_long;
        private string inst_string;

        private IList<JsonData> inst_array;
        private IDictionary<string, JsonData> inst_object;
        private JsonType type;

        //每次发生修改时候 都会被置空  而读取时 如果是null则会进行重新写入
        private string json;//这个jsonData对应的json串

        // 用于实现IOrderedDictionary接口  有序字典
        private IList<KeyValuePair<string, JsonData>> object_list;
        #endregion


        #region 属性
        public int Count
        {
            get { return EnsureCollection().Count; }
        }

        public bool IsArray
        {
            get { return type == JsonType.Array; }
        }

        public bool IsBoolean
        {
            get { return type == JsonType.Boolean; }
        }

        public bool IsDouble
        {
            get { return type == JsonType.Double; }
        }

        public bool IsInt
        {
            get { return type == JsonType.Int; }
        }

        public bool IsLong
        {
            get { return type == JsonType.Long; }
        }

        public bool IsObject
        {
            get { return type == JsonType.Object; }
        }

        public bool IsString
        {
            get { return type == JsonType.String; }
        }

        public JsonType Type {
            get { return type; } 
        }

        public ICollection<string> Keys
        {
            get { EnsureDictionary(); return inst_object.Keys; }
        }

        public bool ContainsKey(string key)
        {
            EnsureDictionary();
            return this.inst_object.Keys.Contains(key);
        }
        #endregion


        #region ICollection 接口(定义操作泛型集合的方法)
        int ICollection.Count
        {
            get
            {
                return Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return EnsureCollection().IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return EnsureCollection().SyncRoot;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            EnsureCollection().CopyTo(array, index);
        }
        #endregion


        #region IDictionary 接口
        bool IDictionary.IsFixedSize
        {
            get
            {
                return EnsureDictionary().IsFixedSize;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return EnsureDictionary().IsReadOnly;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                EnsureDictionary();
                IList<string> keys = new List<string>();

                foreach (KeyValuePair<string, JsonData> entry in
                         object_list)
                {
                    keys.Add(entry.Key);
                }

                return (ICollection)keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                EnsureDictionary();
                IList<JsonData> values = new List<JsonData>();

                foreach (KeyValuePair<string, JsonData> entry in
                         object_list)
                {
                    values.Add(entry.Value);
                }

                return (ICollection)values;
            }
        }
        #endregion


        #region IJsonWrapper 接口
        bool IJsonWrapper.IsArray
        {
            get { return IsArray; }
        }

        bool IJsonWrapper.IsBoolean
        {
            get { return IsBoolean; }
        }

        bool IJsonWrapper.IsDouble
        {
            get { return IsDouble; }
        }

        bool IJsonWrapper.IsInt
        {
            get { return IsInt; }
        }

        bool IJsonWrapper.IsLong
        {
            get { return IsLong; }
        }

        bool IJsonWrapper.IsObject
        {
            get { return IsObject; }
        }

        bool IJsonWrapper.IsString
        {
            get { return IsString; }
        }

        //IEnumerable Methods
        IEnumerator IEnumerable.GetEnumerator()
        {
            return EnsureCollection().GetEnumerator();
        }
        #endregion


        #region IList 接口
        bool IList.IsFixedSize
        {
            get
            {
                return EnsureList().IsFixedSize;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return EnsureList().IsReadOnly;
            }
        }
        #endregion


        #region IDictionary 索引器
        object IDictionary.this[object key]
        {
            get
            {
                return EnsureDictionary()[key];
            }
            set
            {
                if (!(key is string))
                    throw new Exception(
                        "键必须是字符串");

                JsonData data = ToJsonData(value);
                this[(string)key] = data;
            }
        }
        void IDictionary.Add(object key, object value)
        {
            JsonData data = ToJsonData(value);

            EnsureDictionary().Add(key, data);

            KeyValuePair<string, JsonData> entry =
                new KeyValuePair<string, JsonData>((string)key, data);
            object_list.Add(entry);

            json = null;
        }

        void IDictionary.Clear()
        {
            EnsureDictionary().Clear();
            object_list.Clear();
            json = null;
        }

        bool IDictionary.Contains(object key)
        {
            return EnsureDictionary().Contains(key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return ((IOrderedDictionary)this).GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            EnsureDictionary().Remove(key);

            for (int i = 0; i < object_list.Count; i++)
            {
                if (object_list[i].Key == (string)key)
                {
                    object_list.RemoveAt(i);
                    break;
                }
            }

            json = null;
        }
        #endregion


        #region IOrderedDictionary 索引器
        object IOrderedDictionary.this[int idx]
        {
            get
            {
                EnsureDictionary();
                return object_list[idx].Value;
            }

            set
            {
                EnsureDictionary();
                JsonData data = ToJsonData(value);

                KeyValuePair<string, JsonData> old_entry = object_list[idx];

                inst_object[old_entry.Key] = data;

                KeyValuePair<string, JsonData> entry =
                    new KeyValuePair<string, JsonData>(old_entry.Key, data);

                object_list[idx] = entry;
            }
        }
        #endregion


        #region IList 索引器
        object IList.this[int index]
        {
            get
            {
                return EnsureList()[index];
            }

            set
            {
                EnsureList()[index] = ToJsonData(value);
            }
        }
        #endregion


        #region 暴露索引器
        public JsonData this[string prop_name]
        {
            get
            {
                EnsureDictionary();
                return inst_object[prop_name];
            }

            set
            {
                EnsureDictionary();

                KeyValuePair<string, JsonData> entry =
                    new KeyValuePair<string, JsonData>(prop_name, value);

                //加进列表
                if (inst_object.ContainsKey(prop_name))
                {
                    for (int i = 0; i < object_list.Count; i++)
                    {
                        if (object_list[i].Key == prop_name)
                        {
                            object_list[i] = entry;
                            break;
                        }
                    }
                }
                else
                    object_list.Add(entry);

                //加进字典
                inst_object[prop_name] = value;

                //json失效
                json = null;
            }
        }

        public JsonData this[int index]
        {
            get
            {
                EnsureCollection();

                if (type == JsonType.Array)
                    return inst_array[index];

                return object_list[index].Value;
            }

            set
            {
                EnsureCollection();

                if (type == JsonType.Array)
                    inst_array[index] = value;
                else
                {
                    KeyValuePair<string, JsonData> entry = object_list[index];
                    KeyValuePair<string, JsonData> new_entry =
                        new KeyValuePair<string, JsonData>(entry.Key, value);

                    object_list[index] = new_entry;
                    inst_object[entry.Key] = value;
                }

                json = null;
            }
        }
        #endregion


        #region 构造
        public JsonData()
        {
            //EnsureDictionary();//确定字典的话不能直接new(){0,1,2}成列表了
            //jsondata的类型一旦确定则不会变化
        }

        public JsonData(bool boolean)
        {
            type = JsonType.Boolean;
            inst_boolean = boolean;
        }

        public JsonData(double number)
        {
            type = JsonType.Double;
            inst_double = number;
        }

        public JsonData(int number)
        {
            type = JsonType.Int;
            inst_int = number;
        }

        public JsonData(long number)
        {
            type = JsonType.Long;
            inst_long = number;
        }

        public JsonData(string str)
        {
            type = JsonType.String;
            inst_string = str;
        }

        public JsonData(object obj)
        {
            //是值
            if (obj is bool)
            {
                type = JsonType.Boolean;
                inst_boolean = (bool)obj;
                return;
            }

            if (obj is double)
            {
                type = JsonType.Double;
                inst_double = (double)obj;
                return;
            }

            if (obj is float)
            {
                type = JsonType.Double;
                inst_double = (double)obj;
                return;
            }

            if (obj is int)
            {
                type = JsonType.Int;
                inst_int = (int)obj;
                return;
            }

            if (obj is long)
            {
                type = JsonType.Long;
                inst_long = (long)obj;
                return;
            }

            if (obj is string)
            {
                type = JsonType.String;
                inst_string = (string)obj;
                return;
            }

            //是集合
            if (obj is JsonData)
            {
                throw new Exception(
                    "JsonData不能作为构造参数");
            }
            else
            {
                if (obj is IList list)
                {
                    var inst_list = EnsureList();
                    foreach (var item in list)
                    {
                        inst_list.Add(ToJsonData(item));
                    }
                    return;
                }
                if (obj is IDictionary dictionary)
                {
                    var inst_dic = EnsureDictionary();
                    foreach (DictionaryEntry item in dictionary)
                    {
                        string propertyName = item.Key is string key ?
                           key : Convert.ToString(item.Key, CultureInfo.InvariantCulture);

                        JsonData data = ToJsonData(item.Value);

                        inst_dic.Add(propertyName, data);

                        KeyValuePair<string, JsonData> entry =
                            new KeyValuePair<string, JsonData>(propertyName, data);
                        object_list.Add(entry);
                    }
                    return;
                }
            }

            throw new Exception(
                obj.GetType().Name + "不是支持的类型 不能转成jsonData对象");
        }

        #endregion


        #region 隐式转换
        public static implicit operator JsonData(bool data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(double data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(float data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(int data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(long data)
        {
            return new JsonData(data);
        }

        public static implicit operator JsonData(string data)
        {
            return new JsonData(data);
        }
        #endregion

        #region 显式转换
        public static explicit operator bool(JsonData data)
        {
            if (data.type != JsonType.Boolean)
                throw new InvalidCastException(
                    "JsonData实例不包含bool类型");

            return data.inst_boolean;
        }

        public static explicit operator double(JsonData data)
        {
            if (data.type != JsonType.Double)
                throw new InvalidCastException(
                    "JsonData实例不包含double类型");

            return data.inst_double;
        }

        public static explicit operator float(JsonData data) {
            if (data.type != JsonType.Double)
                throw new InvalidCastException(
                    "JsonData实例不包含double类型");

            return (float)data.inst_double;
        }

        public static explicit operator int(JsonData data)
        {
            if (data.type != JsonType.Int && data.type != JsonType.Long)
            {
                throw new InvalidCastException(
                    "JsonData实例不包含int类型");
            }

            return data.type == JsonType.Int ? data.inst_int : (int)data.inst_long;
        }

        public static explicit operator long(JsonData data)
        {
            if (data.type != JsonType.Long && data.type != JsonType.Int)
            {
                throw new InvalidCastException(
                    "JsonData实例不包含long类型");
            }

            return data.type == JsonType.Long ? data.inst_long : data.inst_int;
        }

        public static explicit operator string(JsonData data)
        {
            if (data.type != JsonType.String)
                throw new InvalidCastException(
                    "JsonData实例不包含字符串");

            return data.inst_string;
        }
        #endregion


        #region IJsonWrapper 接口实现
        bool IJsonWrapper.GetBoolean()
        {
            if (type != JsonType.Boolean)
                throw new InvalidOperationException(
                    "JsonData实例不包含bool类型");

            return inst_boolean;
        }

        double IJsonWrapper.GetDouble()
        {
            if (type != JsonType.Double)
                throw new InvalidOperationException(
                    "JsonData实例不包含double类型");

            return inst_double;
        }

        int IJsonWrapper.GetInt()
        {
            if (type != JsonType.Int)
                throw new InvalidOperationException(
                    "JsonData实例不包含int类型");

            return inst_int;
        }

        long IJsonWrapper.GetLong()
        {
            if (type != JsonType.Long)
                throw new InvalidOperationException(
                    "JsonData实例不包含long类型");

            return inst_long;
        }

        string IJsonWrapper.GetString()
        {
            if (type != JsonType.String)
                throw new InvalidOperationException(
                    "JsonData实例不包含字符串");

            return inst_string;
        }

        JsonType IJsonWrapper.GetJsonType()
        {
            return type;
        }

        void IJsonWrapper.SetBoolean(bool val)
        {
            type = JsonType.Boolean;
            inst_boolean = val;
            json = null;
        }

        void IJsonWrapper.SetDouble(double val)
        {
            type = JsonType.Double;
            inst_double = val;
            json = null;
        }

        void IJsonWrapper.SetInt(int val)
        {
            type = JsonType.Int;
            inst_int = val;
            json = null;
        }

        void IJsonWrapper.SetLong(long val)
        {
            type = JsonType.Long;
            inst_long = val;
            json = null;
        }

        void IJsonWrapper.SetString(string val)
        {
            type = JsonType.String;
            inst_string = val;
            json = null;
        }

        void IJsonWrapper.SetJsonType(JsonType type)
        {
            if (this.type == type)
                return;

            switch (type)
            {
                case JsonType.None:
                    break;

                case JsonType.Object:
                    inst_object = new Dictionary<string, JsonData>();
                    object_list = new List<KeyValuePair<string, JsonData>>();
                    break;

                case JsonType.Array:
                    inst_array = new List<JsonData>();
                    break;

                case JsonType.String:
                    inst_string = default(String);
                    break;

                case JsonType.Int:
                    inst_int = default(Int32);
                    break;

                case JsonType.Long:
                    inst_long = default(Int64);
                    break;

                case JsonType.Double:
                    inst_double = default(Double);
                    break;

                case JsonType.Boolean:
                    inst_boolean = default(Boolean);
                    break;
            }
            this.type = type;
        }

        string IJsonWrapper.ToJson()
        {
            return ToJson();
        }

        void IJsonWrapper.ToJson(JsonWriter writer)
        {
            ToJson(writer);
        }
        #endregion


        #region IList 接口实现
        int IList.Add(object value)
        {
            return Add(value);
        }

        void IList.Clear()
        {
            EnsureList().Clear();
            json = null;
        }

        bool IList.Contains(object value)
        {
            return EnsureList().Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return EnsureList().IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            EnsureList().Insert(index, value);
            json = null;
        }

        void IList.Remove(object value)
        {
            EnsureList().Remove(value);
            json = null;
        }

        void IList.RemoveAt(int index)
        {
            EnsureList().RemoveAt(index);
            json = null;
        }
        #endregion


        #region IOrderedDictionary 接口实现
        IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
        {
            EnsureDictionary();

            return new OrderedDictionaryEnumerator(
                object_list.GetEnumerator());
        }

        void IOrderedDictionary.Insert(int idx, object key, object value)
        {
            string property = (string)key;
            JsonData data = ToJsonData(value);

            this[property] = data;

            KeyValuePair<string, JsonData> entry =
                new KeyValuePair<string, JsonData>(property, data);

            object_list.Insert(idx, entry);
        }

        void IOrderedDictionary.RemoveAt(int idx)
        {
            EnsureDictionary();

            inst_object.Remove(object_list[idx].Key);
            object_list.RemoveAt(idx);
        }
        #endregion


        #region 私有方法
        /// <summary> 确定为集合   字典型或数组型 </summary>
        private ICollection EnsureCollection()
        {
            if (type == JsonType.Array)
                return (ICollection)inst_array;

            if (type == JsonType.Object)
                return (ICollection)inst_object;

            throw new InvalidOperationException(
                "必须首先初始化JsonData实例");
        }

        /// <summary> 确定为字典 </summary>
        private IDictionary EnsureDictionary()
        {
            if (type == JsonType.Object)
                return (IDictionary)inst_object;

            if (type != JsonType.None)
                throw new InvalidOperationException(
                    "JsonData实例不是一个字典");

            type = JsonType.Object;
            inst_object = new Dictionary<string, JsonData>();
            object_list = new List<KeyValuePair<string, JsonData>>();

            return (IDictionary)inst_object;
        }

        /// <summary> 确定为列表 </summary>
        private IList EnsureList()
        {
            if (type == JsonType.Array)
                return (IList)inst_array;

            if (type != JsonType.None)
                throw new InvalidOperationException(
                    "JsonData实例不是一个列表");

            type = JsonType.Array;
            inst_array = new List<JsonData>();

            return (IList)inst_array;
        }

        private JsonData ToJsonData(object obj)
        {
            if (obj == null)
                return null;

            if (obj is JsonData)
                return (JsonData)obj;

            return new JsonData(obj);
        }

        private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
        {
            if (obj == null)
            {
                writer.WriteValue(null);
                return;
            }

            if (obj.IsString)
            {
                writer.WriteValue(obj.GetString());
                return;
            }

            if (obj.IsBoolean)
            {
                writer.WriteValue(obj.GetBoolean());
                return;
            }

            if (obj.IsDouble)
            {
                writer.WriteValue(obj.GetDouble());
                return;
            }

            if (obj.IsInt)
            {
                writer.WriteValue(obj.GetInt());
                return;
            }

            if (obj.IsLong)
            {
                writer.WriteValue(obj.GetLong());
                return;
            }

            if (obj.IsArray)
            {
                writer.WriteArrayStart();
                foreach (object elem in (IList)obj)
                    WriteJson((JsonData)elem, writer);
                writer.WriteArrayEnd();

                return;
            }

            if (obj.IsObject)
            {
                writer.WriteObjectStart();

                foreach (DictionaryEntry entry in ((IDictionary)obj))
                {
                    writer.WritePropertyName((string)entry.Key);
                    WriteJson((JsonData)entry.Value, writer);
                }
                writer.WriteObjectEnd();
                return;
            }
        }
        #endregion


        public int Add(object value)
        {
            JsonData data = ToJsonData(value);
            json = null;
            return EnsureList().Add(data);
        }

        public bool Remove(object obj)
        {
            json = null;
            if (IsObject)
            {
                if (inst_object.TryGetValue((string)obj, out JsonData value))
                    return inst_object.Remove((string)obj) && object_list.Remove(new KeyValuePair<string, JsonData>((string)obj, value));
                else
                    throw new KeyNotFoundException("在JsonData对象中没有找到指定的键。");
            }
            if (IsArray)
            {
                return inst_array.Remove(ToJsonData(obj));
            }
            throw new InvalidOperationException(
                    "JsonData的实例不是对象或列表。");
        }

        public void Clear()
        {
            if (IsObject)
            {
                ((IDictionary)this).Clear();
                return;
            }

            if (IsArray)
            {
                ((IList)this).Clear();
                return;
            }
        }

        public bool Equals(JsonData x)
        {
            if (x == null)
                return false;

            if (x.type != this.type)
            {
                //针对int和long的特殊情况  int和long看做相等
                if ((x.type != JsonType.Int && x.type != JsonType.Long) || (this.type != JsonType.Int && this.type != JsonType.Long))
                {
                    return false;
                }
            }

            switch (this.type)
            {
                case JsonType.None:
                    return true;

                case JsonType.Object:
                    return this.inst_object.Equals(x.inst_object);

                case JsonType.Array:
                    return this.inst_array.Equals(x.inst_array);

                case JsonType.String:
                    return this.inst_string.Equals(x.inst_string);

                case JsonType.Int:
                    {
                        if (x.IsLong)
                        {
                            if (x.inst_long < Int32.MinValue || x.inst_long > Int32.MaxValue)
                                return false;
                            return this.inst_int.Equals((int)x.inst_long);
                        }
                        return this.inst_int.Equals(x.inst_int);
                    }

                case JsonType.Long:
                    {
                        if (x.IsInt)
                        {
                            if (this.inst_long < Int32.MinValue || this.inst_long > Int32.MaxValue)
                                return false;
                            return x.inst_int.Equals((int)this.inst_long);
                        }
                        return this.inst_long.Equals(x.inst_long);
                    }

                case JsonType.Double:
                    return this.inst_double.Equals(x.inst_double);

                case JsonType.Boolean:
                    return this.inst_boolean.Equals(x.inst_boolean);
            }

            return false;
        }
        public string ToJson()
        {
            if (json != null)
                return json;

            StringWriter sw = new StringWriter();
            JsonWriter writer = new JsonWriter(sw, false, false);
            writer.Validate = false;

            WriteJson(this, writer);
            json = sw.ToString();

            return json;
        }

        public string ToJson(bool toUnicode, bool prettyPrint)
        {
            if (toUnicode || prettyPrint)
            {
                StringWriter sw = new StringWriter();
                JsonWriter writer = new JsonWriter(sw, toUnicode, prettyPrint);
                writer.Validate = false;

                WriteJson(this, writer);//不对this.json进行赋值
                return sw.ToString();
            }
            else
            {
                return ToJson();
            }
        }

        void ToJson(JsonWriter writer)
        {
            bool old_validate = writer.Validate;

            writer.Validate = false;//无视检查  this必然是可以正确序列化

            WriteJson(this, writer);

            writer.Validate = old_validate;
        }

        public override string ToString()
        {
            switch (type)
            {
                case JsonType.Array:
                    return "JsonData Array";

                case JsonType.Boolean:
                    return inst_boolean.ToString();

                case JsonType.Double:
                    return inst_double.ToString();

                case JsonType.Int:
                    return inst_int.ToString();

                case JsonType.Long:
                    return inst_long.ToString();

                case JsonType.Object:
                    return "JsonData Object";

                case JsonType.String:
                    return inst_string;
            }
            return "Uninitialized JsonData";
        }

        public T ToObject<T>()
        {
            return Json.ToObject<T>(ToJson());
        }
        public bool TryToObject<T>(out T obj) {
            Log.Info("未完成的方法");
            obj = default;
            return false;
        }
    }

    /// <summary> 有序字典枚举器 </summary>
    internal class OrderedDictionaryEnumerator : IDictionaryEnumerator
    {
        IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;

        public object Current
        {
            get { return Entry; }
        }

        public DictionaryEntry Entry
        {
            get
            {
                KeyValuePair<string, JsonData> curr = list_enumerator.Current;
                return new DictionaryEntry(curr.Key, curr.Value);
            }
        }

        public object Key
        {
            get { return list_enumerator.Current.Key; }
        }

        public object Value
        {
            get { return list_enumerator.Current.Value; }
        }


        public OrderedDictionaryEnumerator(IEnumerator<KeyValuePair<string, JsonData>> enumerator)
        {
            list_enumerator = enumerator;
        }


        public bool MoveNext()
        {
            return list_enumerator.MoveNext();
        }

        public void Reset()
        {
            list_enumerator.Reset();
        }
    }
}
