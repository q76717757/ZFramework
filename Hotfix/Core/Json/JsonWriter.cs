/** Header
 * JsonWriter.cs
 *  json写入器
 **/


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;


namespace ZFramework
{
    internal enum Condition
    {
        CloseArray,
        CloseObject,
        StartCollection,
        Property,
        Value
    }

    /// <summary> 写入的上下文 </summary>
    internal class WriterContext
    {
        /// <summary> 当前的上文数量 逗号分割的时候会用到 </summary>
        public int Count;
        /// <summary> 本文是数组/列表 </summary>
        public bool InArray;
        /// <summary> 本文是对象/字典 </summary>
        public bool InObject;
        /// <summary> 下文期望是值 </summary>
        public bool ExpectingValue;
        /// <summary> 填充空格 格式化输出时 对齐属性名用的 </summary>
        public int Padding;
    }

    /// <summary> JSON写入器 负责组装json字符串</summary>
    public class JsonWriter
    {
        #region 字段
        /// <summary> 获取不依赖于区域性的（固定的）默认只读对象。 </summary>
        private static readonly NumberFormatInfo number_format;

        /// <summary> 当前上下文 </summary>
        private WriterContext context;
        /// <summary> 上下文栈 </summary>
        private Stack<WriterContext> ctx_stack;
        /// <summary> 已结束 </summary>
        private bool has_reached_end;
        /// <summary> 4位的十六进制char缓存 </summary>
        private char[] hex_seq;
        /// <summary> 累计缩进 </summary>
        private int indentation;
        /// <summary> 缩进单位 </summary>
        private int indent_value;
        /// <summary> StringBuilder实例 </summary>
        private StringBuilder inst_string_builder;
        /// <summary> 优质打印 </summary>
        private bool pretty_print;
        /// <summary> 验证 </summary>
        private bool validate;
        /// <summary> 属性小写 </summary>
        private bool lower_case_properties;
        /// <summary> 内部的文本编辑器 </summary>
        private TextWriter writer;
        /// <summary> 文本使用unicode编码 </summary>
        private bool unicode;
        #endregion


        #region 属性
        /// <summary> 缩进单位 </summary>
        public int IndentValue
        {
            get { return indent_value; }
            set
            {
                indentation = (indentation / indent_value) * value;
                indent_value = value;
            }
        }
        /// <summary> 对齐输出 </summary>
        public bool PrettyPrint
        {
            get { return pretty_print; }
            set { pretty_print = value; }
        }
        /// <summary> 内部的文本写入器 一般是StringWriter的实例 </summary>
        public TextWriter TextWriter
        {
            get { return writer; }
        }
        /// <summary> 校验 </summary>
        public bool Validate
        {
            get { return validate; }
            set { validate = value; }
        }
        /// <summary> 小写属性 </summary>
        public bool LowerCaseProperties
        {
            get { return lower_case_properties; }
            set { lower_case_properties = value; }
        }
        #endregion


        #region 构造
        static JsonWriter()
        {
            number_format = NumberFormatInfo.InvariantInfo;
        }
        public JsonWriter(bool toUnicode, bool prettyPrint)
        {
            inst_string_builder = new StringBuilder();
            writer = new StringWriter(inst_string_builder);
            Init(toUnicode, prettyPrint);
        }
        public JsonWriter(TextWriter writer, bool toUnicode, bool prettyPrint)
        {
            if (writer == null)
                throw new ArgumentNullException("TextWriter is Null");
            this.writer = writer;
            Init(toUnicode, prettyPrint);
        }
        public JsonWriter(StringBuilder sb, bool toUnicode, bool prettyPrint) : this(new StringWriter(sb), toUnicode, prettyPrint)
        {
            if (sb == null)
                throw new ArgumentNullException("StringBuilder is Null");
            writer = new StringWriter(sb);
            Init(toUnicode, prettyPrint);
        }

        /// <summary> 构造初始化方法 </summary>
        private void Init(bool toUnicode, bool prettyPrint)
        {
            has_reached_end = false;
            hex_seq = new char[4];
            indentation = 0;
            indent_value = 4;
            pretty_print = prettyPrint;
            unicode = toUnicode;
            validate = true;
            lower_case_properties = false;


            ctx_stack = new Stack<WriterContext>();
            context = new WriterContext();
        }
        #endregion


        #region 私有方法

        /// <summary> 校验即将写入的下文的合理性 </summary>
        private void DoValidation(Condition cond)
        {
            if (!context.ExpectingValue)
                context.Count++;

            if (!validate)
                return;

            if (has_reached_end)
                throw new Exception(
                    "已经编写了一个完整的JSON");

            switch (cond)
            {
                case Condition.CloseArray:
                    if (!context.InArray)
                        throw new Exception(
                            writer.ToString() + "不能在这里闭合一个数组");
                    break;

                case Condition.CloseObject:
                    if (!context.InObject || context.ExpectingValue)
                    {
                        throw new Exception(
                            "不能在这里闭合对象");
                    }
                    break;

                case Condition.StartCollection:
                    if (context.InObject && !context.ExpectingValue)
                        throw new Exception(
                            "下文应是一个属性");
                    break;

                case Condition.Property:
                    if (!context.InObject || context.ExpectingValue)
                    {
                        throw new Exception(
                            "不能在这里添加属性");
                    }
                    break;

                case Condition.Value:
                    if (!context.InArray && (!context.InObject || !context.ExpectingValue))
                        throw new Exception(
                            "不能在这里添加值");
                    break;
            }
        }

        /// <summary> 转十六进制(Unicode编码) </summary>
        private static void IntToHex(int n, char[] hex)
        {
            int num;
            for (int i = 0; i < 4; i++)
            {
                num = n % 16;

                if (num < 10)
                    hex[3 - i] = (char)('0' + num);
                else
                    hex[3 - i] = (char)('A' + (num - 10));

                n >>= 4;
            }
        }

        /// <summary> 缩排+ </summary>
        private void Indent()
        {
            if (pretty_print)
                indentation += indent_value;
        }

        /// <summary> 缩排- </summary>
        private void Unindent()
        {
            if (pretty_print)
                indentation -= indent_value;
        }

        /// <summary> 输出字符串(带缩进格式化) </summary>
        private void PutValueStr(string str)
        {
            if (pretty_print && !context.ExpectingValue)
                for (int i = 0; i < indentation; i++)
                    writer.Write(' ');
            writer.Write(str);
        }
        /// <summary> 输出字符串 </summary>
        private void PutStringStr(string str)
        {
            PutValueStr(string.Empty);

            writer.Write('"');
            int n = str.Length;
            for (int i = 0; i < n; i++)
            {
                switch (str[i])
                {
                    case '\n':
                        writer.Write("\\n");
                        continue;

                    case '\r':
                        writer.Write("\\r");
                        continue;

                    case '\t':
                        writer.Write("\\t");
                        continue;

                    case '"':
                    case '\\':
                        writer.Write('\\');
                        writer.Write(str[i]);
                        continue;

                    case '\f':
                        writer.Write("\\f");
                        continue;

                    case '\b':
                        writer.Write("\\b");
                        continue;
                }

                if (str[i] >= 32 && str[i] <= 126)//ASCII码32~126之间一共95个常规字符
                {
                    writer.Write(str[i]);
                    continue;
                }

                if (unicode)//转成unicode编码  \uXXXX
                {
                    IntToHex(str[i], hex_seq);
                    writer.Write("\\u");
                    writer.Write(hex_seq);
                }
                else //保持utf-8编码
                {
                    writer.Write(str[i]);
                }
            }
            writer.Write('"');
        }

        /// <summary> 换行 根据上文数量添加逗号  如果是闭合过程则不添加逗号 </summary>
        private void PutNewline(bool add_comma = true)
        {
            if (add_comma && !context.ExpectingValue && context.Count > 1)
                writer.Write(',');

            if (pretty_print && !context.ExpectingValue)
                writer.Write(Environment.NewLine);
        }

        #endregion

        public override string ToString()
        {
            return inst_string_builder.ToString();
        }

        /// <summary> 重置写入器 </summary>
        public void Reset(bool toUnicode, bool prettyPrint)
        {
            pretty_print = prettyPrint;
            this.unicode = toUnicode;
            has_reached_end = false;

            ctx_stack.Clear();
            context = new WriterContext();
            ctx_stack.Push(context);

            inst_string_builder.Remove(0, inst_string_builder.Length);
        }
        public void WriteValue(bool boolean)
        {
            DoValidation(Condition.Value);
            PutNewline();

            PutValueStr(boolean ? "true" : "false");

            context.ExpectingValue = false;
        }

        public void WriteValue(decimal number)
        {
            DoValidation(Condition.Value);
            PutNewline();

            PutValueStr(Convert.ToString(number, number_format));

            context.ExpectingValue = false;
        }

        public void WriteValue(double number)
        {
            DoValidation(Condition.Value);
            PutNewline();

            string str = Convert.ToString(number, number_format);
            PutValueStr(str);

            if (str.IndexOf('.') == -1 &&
                str.IndexOf('E') == -1)
                writer.Write(".0");

            context.ExpectingValue = false;
        }

        public void WriteValue(float number)
        {
            DoValidation(Condition.Value);
            PutNewline();

            string str = Convert.ToString(number, number_format);
            PutValueStr(str);

            context.ExpectingValue = false;
        }

        public void WriteValue(int number)
        {
            DoValidation(Condition.Value);
            PutNewline();

            PutValueStr(Convert.ToString(number, number_format));

            context.ExpectingValue = false;
        }

        public void WriteValue(long number)
        {
            DoValidation(Condition.Value);
            PutNewline();

            PutValueStr(Convert.ToString(number, number_format));

            context.ExpectingValue = false;
        }

        public void WriteValue(string str)
        {
            DoValidation(Condition.Value);
            PutNewline();

            if (str == null)
                PutValueStr("null");
            else
                PutStringStr(str);

            context.ExpectingValue = false;
        }

        public void WriteValue(ulong number)
        {
            DoValidation(Condition.Value);
            PutNewline();

            PutValueStr(Convert.ToString(number, number_format));

            context.ExpectingValue = false;
        }

        public void WriteArrayStart()
        {
            DoValidation(Condition.StartCollection);
            PutNewline();

            PutValueStr("[");

            context = new WriterContext();
            context.InArray = true;
            ctx_stack.Push(context);

            Indent();
        }
        public void WriteArrayEnd()
        {
            DoValidation(Condition.CloseArray);
            PutNewline(false);

            ctx_stack.Pop();
            if (ctx_stack.Count == 0)
                has_reached_end = true;
            else
            {
                context = ctx_stack.Peek();
                context.ExpectingValue = false;
            }

            Unindent();
            PutValueStr("]");
        }

        public void WriteObjectStart()
        {
            DoValidation(Condition.StartCollection);
            PutNewline();

            PutValueStr("{");

            context = new WriterContext();
            context.InObject = true;
            ctx_stack.Push(context);

            Indent();
        }
        public void WriteObjectEnd()
        {
            DoValidation(Condition.CloseObject);
            PutNewline(false);

            ctx_stack.Pop();
            if (ctx_stack.Count == 0)
                has_reached_end = true;
            else
            {
                context = ctx_stack.Peek();
                context.ExpectingValue = false;
            }

            Unindent();
            PutValueStr("}");
        }

        /// <summary> 写入{属性名:} </summary>
        public void WritePropertyName(string property_name)
        {
            DoValidation(Condition.Property);
            PutNewline();
            string propertyName = (property_name == null || !lower_case_properties)
                ? property_name
                : property_name.ToLowerInvariant();

            PutStringStr(propertyName);

            if (pretty_print)
            {
                if (propertyName.Length > context.Padding)
                    context.Padding = propertyName.Length;

                for (int i = context.Padding - propertyName.Length;
                     i >= 0; i--)
                    writer.Write(' ');

                writer.Write(": ");
            }
            else
                writer.Write(':');

            context.ExpectingValue = true;
        }
    }
}
