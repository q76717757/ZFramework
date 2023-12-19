/** Header
 *  ZJson.cs
 *  提供.net对象到JSON互相转换的方法。
 **/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace ZFramework
{
    /// <summary> 属性元数据 </summary>
    internal struct PropertyMetadata
    {
        private bool isIgnore;
        private bool isField;
        private Type type;
        private MemberInfo info;

        public bool IsIgnore
        {
            get { return isIgnore; }
            set { isIgnore = value; }
        }
        public bool IsField
        {
            get { return isField; }
            set { isField = value; }
        }
        public MemberInfo Info
        {
            get { return info; }
            set { info = value; }
        }
        public Type Type {
            get { return type; }
            set { type = value; }
        }
    }

    /// <summary> 数组的元数据 </summary>
    internal struct ArrayMetadata
    {
        private Type element_type;
        private bool is_array;
        private bool is_list;

        public Type ElementType
        {
            get
            {
                if (element_type == null)
                    return typeof(JsonData);
                return element_type;
            }
            set { element_type = value; }
        }
        public bool IsArray
        {
            get { return is_array; }
            set { is_array = value; }
        }
        public bool IsList
        {
            get { return is_list; }
            set { is_list = value; }
        }
    }

    /// <summary> 对象的元数据 </summary>
    internal struct ObjectMetadata
    {
        private Type key_type;
        private Type element_type;
        private bool is_dictionary;
        private IDictionary<string, PropertyMetadata> properties;

        public Type KeyType
        {
            get { return key_type; }
            set { key_type = value; }
        }
        public Type ElementType
        {
            get
            {
                if (element_type == null)
                    return typeof(JsonData);
                return element_type;
            }
            set { element_type = value; }
        }
        public bool IsDictionary
        {
            get { return is_dictionary; }
            set { is_dictionary = value; }
        }
        public IDictionary<string, PropertyMetadata> Properties
        {
            get { return properties; }
            set { properties = value; }
        }
    }

    /// <summary> 序列化规则 </summary>
    public delegate void ExporterFunc(object obj, JsonWriter writer);
    /// <summary> 序列化规则 </summary>
    public delegate void ExporterFunc<T>(T obj, JsonWriter writer);

    /// <summary> 反序列化规则 </summary>
    public delegate object ImporterFunc(object input);
    /// <summary> 反序列化规则 </summary>
    public delegate TValue ImporterFunc<TJson, TValue>(TJson input);
    /// <summary> 容器工厂 多数用于实例一个JsonData容器 某些情况用来实例一个假容器 以更快跳过数据 </summary>
    internal delegate IJsonWrapper WrapperFactory();

    /// <summary> 提供Json字符串和.Net对象之间的转换</summary>
    public sealed class Json
    {
        #region 静态字段
        /// <summary> 最大嵌套深度 </summary>
        private static readonly int max_nesting_depth;

        /// <summary> 日期格式 </summary>
        private static readonly IFormatProvider datetime_format;

        /// <summary> 基本序列化规则表 </summary>
        private static readonly IDictionary<Type, ExporterFunc> base_exporters_table;
        /// <summary> 自定义序列化规则表 </summary>
        private static readonly IDictionary<Type, ExporterFunc> custom_exporters_table;

        /// <summary> 基本反序列化规则表 </summary>
        private static readonly IDictionary<Type, IDictionary<Type, ImporterFunc>> base_importers_table;
        /// <summary> 自定义反序列化规则表 </summary>
        private static readonly IDictionary<Type, IDictionary<Type, ImporterFunc>> custom_importers_table;

        /// <summary> 隐式转换方法表 </summary>
        private static readonly IDictionary<Type, IDictionary<Type, MethodInfo>> conv_ops;
        private static readonly object conv_ops_lock = new object();

        /// <summary> 数组的元数据表 反序列化的时候用的(json->obj)</summary>
        private static readonly IDictionary<Type, ArrayMetadata> array_metadata;
        private static readonly object array_metadata_lock = new object();

        /// <summary> 对象的元数据表 反序列化的时候用的(json->obj)</summary>
        private static readonly IDictionary<Type, ObjectMetadata> object_metadata;
        private static readonly object object_metadata_lock = new object();

        /// <summary> 序列化元数据表 序列化的时候用的(obj->json)</summary>
        private static readonly IDictionary<Type, IList<PropertyMetadata>> type_properties;
        private static readonly object type_properties_lock = new object();

        /// <summary> 全局写入器 </summary>
        private static readonly JsonWriter static_writer;
        private static readonly object static_writer_lock = new object();
        #endregion

        #region 构造
        static Json()
        {
            max_nesting_depth = 100;
            datetime_format = DateTimeFormatInfo.InvariantInfo;

            static_writer = new JsonWriter(false, false);

            conv_ops = new Dictionary<Type, IDictionary<Type, MethodInfo>>();
            array_metadata = new Dictionary<Type, ArrayMetadata>();
            object_metadata = new Dictionary<Type, ObjectMetadata>();
            type_properties = new Dictionary<Type, IList<PropertyMetadata>>();

            base_exporters_table = new Dictionary<Type, ExporterFunc>();
            custom_exporters_table = new Dictionary<Type, ExporterFunc>();

            base_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();
            custom_importers_table = new Dictionary<Type, IDictionary<Type, ImporterFunc>>();

            RegisterBaseExporters();
            RegisterBaseImporters();
        }
        #endregion

        #region 私有方法
        /// <summary> 添加数组/列表元数据(json->obj) </summary>
        private static void AddArrayMetadata(Type type)
        {
            if (array_metadata.ContainsKey(type))
                return;

            ArrayMetadata data = new ArrayMetadata();

            data.IsArray = type.IsArray;
            data.IsList = type.GetInterface("System.Collections.IList") != null;

            if (data.IsArray)
            {
                data.ElementType = type.GetElementType();
            }
            else
            {
                foreach (PropertyInfo p_info in type.GetProperties())//找索引器
                {
                    if (p_info.Name != "Item")//this[]索引器就叫Item   也可能是名字叫Item的属性
                        continue;

                    ParameterInfo[] parameters = p_info.GetIndexParameters();
                    if (parameters.Length != 1)//数组只能有一个int参
                        continue;

                    if (parameters[0].ParameterType == typeof(int))//仅有int参的必然只有一个
                    {
                        data.ElementType = p_info.PropertyType;
                        break;
                    }
                }
            }

            lock (array_metadata_lock)
            {
                try
                {
                    array_metadata.Add(type, data);
                }
                catch
                {
                    return;
                }
            }
        }

        /// <summary> 添加对象/字典元数据 反序列化的时候用的(json->obj)</summary>
        private static void AddObjectMetadata(Type type)
        {
            if (object_metadata.ContainsKey(type))
                return;

            ObjectMetadata data = new ObjectMetadata();

            data.IsDictionary = type.GetInterface("System.Collections.IDictionary") != null;

            data.Properties = new Dictionary<string, PropertyMetadata>();

            //考虑在初始化的时候就给他添上  一般最多会true两次jsonData和JsonMock   但每一个新的type都会get一次
            //实际应用的时候用到的type往往不多
            if (type.GetInterface("ZFramework.IJsonWrapper") != null)
            {
                data.KeyType = typeof(string);
                data.ElementType = typeof(JsonData);
            }
            else
            {
                //找属性 或 索引器
                foreach (PropertyInfo p_info in type.GetProperties())
                {
                    if (p_info.Name == "Item")//this[]索引器就叫Item   也可能是名字叫Item的属性
                    {
                        ParameterInfo[] parameters = p_info.GetIndexParameters();
                        if (parameters.Length != 0)//有参数就是索引器
                        {
                            if (!data.IsDictionary) //不是字典  没必要操作了  不是字典就是自定的索引器 不能保证索引器的key和所有字段都不重复,不能支持
                                continue;
                            if (parameters.Length != 1)//必须是单索引  this[key1,key2]这种是不能支持的
                                continue;

                            var paramType = parameters[0].ParameterType;
                            if (paramType == typeof(string) ||//目前支持三种key的字典
                                paramType == typeof(int) || //long或其他类型看需求再增加
                                paramType.IsEnum
                                )
                            {
                                data.KeyType = paramType;
                                data.ElementType = p_info.PropertyType;
                            }
                            continue;

                        }//没参数的不是索引器 按正经属性添加
                    }

                    PropertyMetadata p_data = new PropertyMetadata
                    {
                        Info = p_info,
                        IsField = false,
                        Type = p_info.PropertyType,
                    };
                    data.Properties.Add(p_info.Name, p_data);
                }

                //字段照单全收
                foreach (FieldInfo f_info in type.GetFields())
                {
                    PropertyMetadata p_data = new PropertyMetadata
                    {
                        Info = f_info,
                        IsField = true,
                        Type = f_info.FieldType,
                    };
                    data.Properties.Add(f_info.Name, p_data);
                }

            }
            lock (object_metadata_lock)
            {
                try
                {
                    object_metadata.Add(type, data);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }


        /// <summary> 添加属性/字段元数据 序列化的时候用的(obj->json)</summary>
        private static void AddTypeProperties(Type type)
        {
            if (type_properties.ContainsKey(type))
                return;
            IList<PropertyMetadata> props = new List<PropertyMetadata>();

            foreach (PropertyInfo p_info in type.GetProperties())//属性需要处理一下this[]索引器
            {
                if (p_info.Name == "Item")//this[key]索引器 就叫item
                {
                    ParameterInfo[] parameters = p_info.GetIndexParameters();
                    if (parameters.Length != 0)//索引器跳过 不需要序列化
                    {
                        continue;
                    }
                }

                PropertyMetadata p_data = new PropertyMetadata
                {
                    Info = p_info,
                    IsField = false,
                    IsIgnore = p_info.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length > 0
                };
                props.Add(p_data);
            }

            foreach (FieldInfo f_info in type.GetFields())//字段照单全收
            {
                PropertyMetadata p_data = new PropertyMetadata
                {
                    Info = f_info,
                    IsField = true,
                    IsIgnore = f_info.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Length > 0
                };

                props.Add(p_data);
            }

            lock (type_properties_lock)
            {
                try
                {
                    type_properties.Add(type, props);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        /// <summary> 反射出两个类型之间的隐式转换方法 </summary>
        private static MethodInfo GetConvOp(Type t1, Type t2)
        {
            lock (conv_ops_lock)
            {
                if (!conv_ops.ContainsKey(t1))
                    conv_ops.Add(t1, new Dictionary<Type, MethodInfo>());
            }

            if (conv_ops[t1].ContainsKey(t2))
                return conv_ops[t1][t2];

            MethodInfo op = t1.GetMethod(
                "op_Implicit", new Type[] { t2 });
            //Implicit是隐式转换符  这里反射出了类型的隐式转换方法 当表中没有对应规则时候 最后一步尝试隐式转换

            lock (conv_ops_lock)
            {
                try
                {
                    conv_ops[t1].Add(t2, op);
                }
                catch (Exception)
                {
                    return conv_ops[t1][t2];
                }
            }
            return op;
        }


        /// <summary> 把json转成指定类型的对象 </summary>
        private static object ReadJsonToObject(Type inst_type, JsonReader reader)
        {
            reader.Read();

            if (reader.Token == JsonToken.ArrayEnd)
                return null;

            Type underlying_type = Nullable.GetUnderlyingType(inst_type);//返回指定可以为 null 的类型的基础类型参数。
            Type value_type = underlying_type ?? inst_type;

            if (reader.Token == JsonToken.Null)
            {
                if (inst_type.IsClass || underlying_type != null)//可以null的类型
                {
                    return null;
                }
                //支持将null转成0
                if (inst_type == typeof(int))
                {
                    return 0;
                }
                if (inst_type == typeof(long))
                {
                    return 0L;
                }
                if (inst_type == typeof(double))
                {
                    return 0.0;
                }
                if (inst_type == typeof(float))
                {
                    return 0.0f;
                }
                if (inst_type.IsEnum)//空转枚举
                {
                    return 0;
                }
                throw new Exception(string.Format(
                            "不能将null赋值给类型为{0}的实例",
                            inst_type));
            }

            if (reader.Token == JsonToken.Double ||
                reader.Token == JsonToken.Int ||
                reader.Token == JsonToken.Long ||
                reader.Token == JsonToken.String ||
                reader.Token == JsonToken.Boolean)//是值
            {

                Type json_type = reader.Value.GetType();

                //确定指定类型 json_type 的实例是否能分配给value_type类型的变量。
                //也就是json_type是否继承于value_type或json_type是否实现了value_type接口(直接或者间接)
                if (value_type.IsAssignableFrom(json_type))
                    return reader.Value;

                // 先从自定义规则里找
                if (custom_importers_table.ContainsKey(json_type) &&
                    custom_importers_table[json_type].ContainsKey(
                        value_type))
                {

                    ImporterFunc importer =
                        custom_importers_table[json_type][value_type];

                    return importer(reader.Value);
                }

                // 再从基本规则里找
                if (base_importers_table.ContainsKey(json_type) &&
                    base_importers_table[json_type].ContainsKey(
                        value_type))
                {

                    ImporterFunc importer =
                        base_importers_table[json_type][value_type];

                    return importer(reader.Value);
                }

                //枚举
                if (value_type.IsEnum) {
                    return Enum.ToObject(value_type, reader.Value);
                }

                // 尝试使用隐式转换操作符
                MethodInfo conv_op = GetConvOp(value_type, json_type);
                if (conv_op != null)
                    return conv_op.Invoke(null, new object[] { reader.Value });

                // 转换失败
                throw new Exception(string.Format("不能将值'{0}'(类型{1})赋给类型{2}", reader.Value, json_type, inst_type));
            }

            object instance = null;//是对象

            if (reader.Token == JsonToken.ArrayStart)
            {
                AddArrayMetadata(inst_type);
                ArrayMetadata t_data = array_metadata[inst_type];

                if (!t_data.IsArray && !t_data.IsList)
                    throw new Exception(string.Format(
                            "类型{0}不能作为数组",
                            inst_type));

                IList list;
                Type elem_type;

                if (t_data.IsArray)//数组
                {
                    list = new ArrayList();
                }
                else//列表
                {
                    list = (IList)Activator.CreateInstance(inst_type);
                }
                elem_type = t_data.ElementType;

                list.Clear();

                while (true)//填充数组
                {
                    object item = ReadJsonToObject(elem_type, reader);
                    if (item == null && reader.Token == JsonToken.ArrayEnd)
                        break;

                    list.Add(item);
                }

                if (t_data.IsArray)
                {
                    int n = list.Count;
                    instance = Array.CreateInstance(elem_type, n);

                    for (int i = 0; i < n; i++)
                        ((Array)instance).SetValue(list[i], i);
                }
                else
                    instance = list;

            }
            else if (reader.Token == JsonToken.ObjectStart)
            {
                AddObjectMetadata(value_type);
                ObjectMetadata t_data = object_metadata[value_type];

                instance = Activator.CreateInstance(value_type);

                while (true)
                {
                    reader.Read();

                    if (reader.Token == JsonToken.ObjectEnd)
                        break;
                    string property = (string)reader.Value;
                    if (t_data.Properties.ContainsKey(property))
                    {
                        PropertyMetadata prop_data = t_data.Properties[property];

                        if (prop_data.IsField)//是字段
                        {
                            ((FieldInfo)prop_data.Info).SetValue(instance, ReadJsonToObject(prop_data.Type, reader));
                        }
                        else//是属性
                        {
                            PropertyInfo p_info = (PropertyInfo)prop_data.Info;

                            if (p_info.CanWrite)
                                p_info.SetValue(
                                    instance,
                                    ReadJsonToObject(prop_data.Type, reader),
                                    null);
                            else
                                ReadJsonToObject(prop_data.Type, reader);
                        }
                    }
                    else//没有这个属性名 就是json多传冗余的字段 或者是索引器Item 或者是jsondata嵌套 嵌套的jsondata Properties是个空字典  或者是枚举为key的字典的key
                    {
                        if (!t_data.IsDictionary) //不是字典的索引器不提供支持
                        {
                            if (!reader.SkipNonMembers)
                            {
                                throw new Exception(string.Format(
                                        "类型{0}没有属性'{1}'",
                                        inst_type, property));
                            }
                            else
                            {
                                ReadSkip(reader);
                                continue;
                            }
                        }
                        //确定是字典  或者是jsondata嵌套 jsondata也有idic接口
                        if (t_data.KeyType == typeof(string))
                        {
                            ((IDictionary)instance).Add(property, ReadJsonToObject(t_data.ElementType, reader));//嵌套jsondata也在这
                        }
                        else if (t_data.KeyType == typeof(int) && int.TryParse(property, out int intKey))
                        {
                            ((IDictionary)instance).Add(intKey, ReadJsonToObject(t_data.ElementType, reader));
                        }
                        else if (t_data.KeyType.IsEnum)//是枚举key
                        {
                            ((IDictionary)instance).Add(Enum.Parse(t_data.KeyType, property), ReadJsonToObject(t_data.ElementType, reader));
                        }
                        else
                        {
                            ReadSkip(reader);
                            continue;
                        }
                    }

                }

            }

            return instance;
        }

        /// <summary> 把json转成容器封装 </summary>
        private static IJsonWrapper ReadJsonToWrapper(WrapperFactory factory, JsonReader reader)
        {
            reader.Read();

            if (reader.Token == JsonToken.ArrayEnd || reader.Token == JsonToken.Null)
                return null;

            IJsonWrapper instance = factory();
            switch (reader.Token)
            {
                case JsonToken.ObjectStart:
                    instance.SetJsonType(JsonType.Object);
                    while (true)
                    {
                        reader.Read();

                        if (reader.Token == JsonToken.ObjectEnd)
                            break;

                        string property = (string)reader.Value;

                        ((IDictionary)instance)[property] = ReadJsonToWrapper(
                            factory, reader);
                    }
                    break;
                case JsonToken.ArrayStart:
                    instance.SetJsonType(JsonType.Array);
                    while (true)
                    {
                        IJsonWrapper item = ReadJsonToWrapper(factory, reader);
                        if (item == null && reader.Token == JsonToken.ArrayEnd)
                            break;

                        ((IList)instance).Add(item);
                    }
                    break;
                case JsonToken.Int:
                    instance.SetInt((int)reader.Value);
                    break;
                case JsonToken.Long:
                    instance.SetLong((long)reader.Value);
                    break;
                case JsonToken.Double:
                    instance.SetDouble((double)reader.Value);
                    break;
                case JsonToken.String:
                    instance.SetString((string)reader.Value);
                    break;
                case JsonToken.Boolean:
                    instance.SetBoolean((bool)reader.Value);
                    break;
                case JsonToken.None:
                case JsonToken.PropertyName:
                case JsonToken.ObjectEnd:
                case JsonToken.ArrayEnd:
                case JsonToken.Null:
                default:
                    break;
            }
            return instance;
        }

        private static void ReadSkip(JsonReader reader)
        {
            ReadJsonToWrapper(delegate { return new JsonMockWrapper(); }, reader);
        }

        private static void WriteObjectToJson(object obj, JsonWriter writer, bool writer_is_private, int depth)
        {
            if (depth > max_nesting_depth)
                throw new StackOverflowException(
                    string.Format("尝试转换类型{0}时,超过最大的嵌套深度,这可能是其内部嵌套了同type的非空属性,引起无限递归,可使用[JsonIgnore]特性将其忽略", obj.GetType()));

            if (obj == null)
            {
                writer.WriteValue(null);
                return;
            }
            if (obj is IJsonWrapper wrapper)
            {
                wrapper.ToJson(writer);
                return;
            }
            if (obj is string @string)
            {
                writer.WriteValue(@string);
                return;
            }

            if (obj is double @double)
            {
                writer.WriteValue(@double);
                return;
            }

            if (obj is float @float)
            {
                writer.WriteValue(@float);
                return;
            }

            if (obj is int @int)
            {
                writer.WriteValue(@int);
                return;
            }

            if (obj is bool @bool)
            {
                writer.WriteValue(@bool);
                return;
            }

            if (obj is long @long)
            {
                writer.WriteValue(@long);
                return;
            }

            if (obj is Array array)
            {
                writer.WriteArrayStart();
                foreach (object elem in array)
                    WriteObjectToJson(elem, writer, writer_is_private, depth + 1);
                writer.WriteArrayEnd();

                return;
            }

            if (obj is IList list)
            {
                writer.WriteArrayStart();
                foreach (object elem in list)
                    WriteObjectToJson(elem, writer, writer_is_private, depth + 1);
                writer.WriteArrayEnd();

                return;
            }

            if (obj is IDictionary dictionary)
            {
                writer.WriteObjectStart();
                foreach (DictionaryEntry entry in dictionary)
                {
                    var propertyName = entry.Key is string key ?
                        key
                        : Convert.ToString(entry.Key, CultureInfo.InvariantCulture);
                    writer.WritePropertyName(propertyName);
                    WriteObjectToJson(entry.Value, writer, writer_is_private, depth + 1);
                }
                writer.WriteObjectEnd();

                return;
            }

            Type obj_type = obj.GetType();

            // 检查是否有自定义反序列化规则
            if (custom_exporters_table.ContainsKey(obj_type))
            {
                ExporterFunc exporter = custom_exporters_table[obj_type];
                exporter(obj, writer);

                return;
            }

            // 检查基本反序列化规则
            if (base_exporters_table.ContainsKey(obj_type))
            {
                ExporterFunc exporter = base_exporters_table[obj_type];
                exporter(obj, writer);

                return;
            }

            // 检查是不是枚举
            if (obj is Enum)
            {
                Type e_type = Enum.GetUnderlyingType(obj_type);

                if (e_type == typeof(long)
                    || e_type == typeof(uint)
                    || e_type == typeof(ulong))
                    writer.WriteValue((ulong)obj);
                else
                    writer.WriteValue((int)obj);

                return;
            }

            // 都不是则确定为对象
            AddTypeProperties(obj_type);

            IList<PropertyMetadata> props = type_properties[obj_type];

            writer.WriteObjectStart();

            foreach (PropertyMetadata p_data in props)
            {
                if (p_data.IsIgnore)
                    continue;

                if (p_data.IsField)
                {
                    writer.WritePropertyName(p_data.Info.Name);
                    WriteObjectToJson(((FieldInfo)p_data.Info).GetValue(obj),
                                writer, writer_is_private, depth + 1);
                }
                else
                {
                    PropertyInfo p_info = (PropertyInfo)p_data.Info;
                    if (p_info.CanRead)
                    {
                        writer.WritePropertyName(p_data.Info.Name);
                        WriteObjectToJson(p_info.GetValue(obj, null),
                                    writer, writer_is_private, depth + 1);
                    }
                }
            }

            writer.WriteObjectEnd();
        }
        #endregion

        #region 公开方法

        public static string ToJson(object obj, bool unicode = false, bool prettyPrint = false)
        {
            lock (static_writer_lock)
            {
                static_writer.Reset(unicode, prettyPrint);
                WriteObjectToJson(obj, static_writer, true, 0);
                return static_writer.ToString();
            }
        }
        public static void ToJson(object obj, StringBuilder sb, bool unicode = false, bool prettyPrint = false) {
            WriteObjectToJson(obj, new JsonWriter(sb, unicode, prettyPrint), false, 0);
        }
        public static void ToJson(object obj, TextWriter tw, bool unicode = false, bool prettyPrint = false) {
            WriteObjectToJson(obj, new JsonWriter(tw, unicode, prettyPrint), false, 0);
        }

        public static JsonData ToObject(string json)
        {
            return (JsonData)ReadJsonToWrapper(
                delegate { return new JsonData(); }, new JsonReader(json));
        }
        public static JsonData ToObject(TextReader reader)
        {
            return (JsonData)ReadJsonToWrapper(
                delegate { return new JsonData(); }, new JsonReader(reader));
        }
        public static T ToObject<T>(string json)
        {
            JsonReader json_reader = new JsonReader(json);
            return (T)ReadJsonToObject(typeof(T), json_reader);
        }
        public static T ToObject<T>(TextReader reader)
        {
            JsonReader json_reader = new JsonReader(reader);
            return (T)ReadJsonToObject(typeof(T), json_reader);
        }

        public static bool TryToObject(string json,out JsonData obj) {
            try
            {
                obj = ToObject(json);
                return true;
            }
            catch (Exception e)
            {
                Log.Info("转换失败:" + e.Message);
                obj = null;
                return false;
            }
        }
        public static bool TryToObject<T>(string json, out T obj) {
            try
            {
                obj = ToObject<T>(json);
                return true;
            }
            catch (Exception e)
            {
                Log.Info("转换失败:" + e.Message);
                obj = default;
                return false;
            }
        }

        #endregion

        #region 规则注册相关  规则 = 指定类型进行的转换委托方法

        /// <summary> 注册全部基本序列化规则 </summary>
        private static void RegisterBaseExporters()
        {
            base_exporters_table[typeof(byte)] =
                delegate (object obj, JsonWriter writer)
                {
                    writer.WriteValue(Convert.ToInt32((byte)obj));
                };

            base_exporters_table[typeof(char)] =
                delegate (object obj, JsonWriter writer)
                {
                    writer.WriteValue(Convert.ToString((char)obj));
                };

            base_exporters_table[typeof(DateTime)] =
                delegate (object obj, JsonWriter writer)
                {
                    writer.WriteValue(Convert.ToString((DateTime)obj, datetime_format));
                };

            base_exporters_table[typeof(decimal)] =
                delegate (object obj, JsonWriter writer)
                {
                    writer.WriteValue((decimal)obj);
                };

            base_exporters_table[typeof(sbyte)] =
                delegate (object obj, JsonWriter writer)
                {
                    writer.WriteValue(Convert.ToInt32((sbyte)obj));
                };

            base_exporters_table[typeof(short)] =
                delegate (object obj, JsonWriter writer)
                {
                    writer.WriteValue(Convert.ToInt32((short)obj));
                };

            base_exporters_table[typeof(ushort)] =
                delegate (object obj, JsonWriter writer)
                {
                    writer.WriteValue(Convert.ToInt32((ushort)obj));
                };

            base_exporters_table[typeof(uint)] =
                delegate (object obj, JsonWriter writer)
                {
                    writer.WriteValue(Convert.ToUInt64((uint)obj));
                };

            base_exporters_table[typeof(ulong)] =
                delegate (object obj, JsonWriter writer)
                {
                    writer.WriteValue((ulong)obj);
                };

            base_exporters_table[typeof(DateTimeOffset)] =
                delegate (object obj, JsonWriter writer)
                {
                    writer.WriteValue(((DateTimeOffset)obj).ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz", datetime_format));
                };
        }

        /// <summary> 注册全部基本反序列化规则 </summary>
        private static void RegisterBaseImporters()
        {
            ImporterFunc importer;//反序列化规则

            importer = delegate (object input)
            {
                return Convert.ToByte((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int), typeof(byte), importer);

            importer = delegate (object input)
            {
                return Convert.ToUInt64((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int), typeof(ulong), importer);

            importer = delegate (object input)
            {
                return Convert.ToInt64((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int), typeof(long), importer);

            importer = delegate (object input)
            {
                return Convert.ToSByte((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int), typeof(sbyte), importer);

            importer = delegate (object input)
            {
                return Convert.ToInt16((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int), typeof(short), importer);

            importer = delegate (object input)
            {
                return Convert.ToUInt16((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int), typeof(ushort), importer);

            importer = delegate (object input)
            {
                return Convert.ToUInt32((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int), typeof(uint), importer);

            importer = delegate (object input)
            {
                return Convert.ToSingle((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int), typeof(float), importer);

            importer = delegate (object input)
            {
                return Convert.ToDouble((int)input);
            };
            RegisterImporter(base_importers_table, typeof(int), typeof(double), importer);

            importer = delegate (object input)
            {
                return Convert.ToDecimal((double)input);
            };
            RegisterImporter(base_importers_table, typeof(double), typeof(decimal), importer);

            importer = delegate (object input)
            {
                return Convert.ToSingle((double)input);
            };
            RegisterImporter(base_importers_table, typeof(double), typeof(float), importer);

            importer = delegate (object input)
            {
                return Convert.ToUInt32((long)input);
            };
            RegisterImporter(base_importers_table, typeof(long), typeof(uint), importer);

            importer = delegate (object input)
            {
                return Convert.ToChar((string)input);
            };
            RegisterImporter(base_importers_table, typeof(string), typeof(char), importer);

            importer = delegate (object input)
            {
                return Convert.ToDateTime((string)input, datetime_format);
            };
            RegisterImporter(base_importers_table, typeof(string), typeof(DateTime), importer);

            importer = delegate (object input)
            {
                return DateTimeOffset.Parse((string)input, datetime_format);
            };
            RegisterImporter(base_importers_table, typeof(string), typeof(DateTimeOffset), importer);
        }

        /// <summary> 注册反序列化规则 </summary>
        private static void RegisterImporter(IDictionary<Type, IDictionary<Type, ImporterFunc>> table, Type json_type, Type value_type, ImporterFunc importer)
        {
            if (!table.ContainsKey(json_type))
                table.Add(json_type, new Dictionary<Type, ImporterFunc>());
            table[json_type][value_type] = importer;
        }



        /// <summary> 注册自定义序列化规则 </summary>
        public void RegisterExporter<T>(ExporterFunc<T> exporter)
        {
            ExporterFunc exporter_wrapper =
                delegate (object obj, JsonWriter writer)
                {
                    exporter((T)obj, writer);
                };
            custom_exporters_table[typeof(T)] = exporter_wrapper;
        }
        /// <summary> 注册自定义反序列化规则 </summary>
        public void RegisterImporter<TJson, TValue>(ImporterFunc<TJson, TValue> importer)
        {
            ImporterFunc importer_wrapper =
                delegate (object input)
                {
                    return importer((TJson)input);
                };
            RegisterImporter(custom_importers_table, typeof(TJson), typeof(TValue), importer_wrapper);
        }
        /// <summary> 清空自定义序列化规则 </summary>
        public void UnregisterExporters() => custom_exporters_table.Clear();
        /// <summary> 清空自定义反序列化规则 </summary>
        public void UnregisterImporters() => custom_importers_table.Clear();
        #endregion
    }

    /// <summary> 跳过序列化 只作用于序列化过程 不影响反序列化过程</summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class JsonIgnoreAttribute : Attribute { }
}
