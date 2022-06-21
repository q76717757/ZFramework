/** Header
 * JsonReader.cs
 *   json读取器
 **/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ZFramework
{
    /// <summary> Json标识 </summary>
    public enum JsonToken
    {
        None,

        PropertyName,

        ObjectStart,
        ObjectEnd,

        ArrayStart,
        ArrayEnd,

        Int,
        Long,
        Double,
        String,
        Boolean,
        Null
    }

    /// <summary> JSON读取器 </summary>
    internal class JsonReader
    {
        #region 字段
        /// <summary> 解析表 第一层key是逻辑分支, 第二层key是遇到的输入  value就是对应的处理规则 </summary>
        private static readonly IDictionary<int, IDictionary<int, int[]>> parse_table;

        /// <summary> token栈 </summary>
        private Stack<int> automaton_stack;
        private int current_input;
        /// <summary> 当前标记 指示fsm返回值类型或者是具体符号=>[]{}" </summary>
        private int current_symbol;
        /// <summary> 标记读完一个json元素(key or value) </summary>
        private bool end_of_json;
        /// <summary> textReader已读完 </summary>
        private bool end_of_input;
        /// <summary> 解析器 </summary>
        private Lexer lexer;
        /// <summary> 解析器光标目前在字符串上  记录判断""的头尾 </summary>
        private bool parser_in_string;
        /// <summary> 解析器返回 </summary>
        private bool parser_return;
        /// <summary> 已开始读取 </summary>
        private bool read_started;
        private TextReader reader;
        /// <summary> 读取器是本地持有的(true=构造中创建,false=外部赋值) </summary>
        private bool reader_is_owned;
        /// <summary> 跳过没有的属性 </summary>
        private bool skip_non_members;
        /// <summary> 读到的值 </summary>
        private object token_value;
        /// <summary> Json标识 </summary>
        private JsonToken token;

        //定义正无穷
        private const string PositiveInfinity = "Infinity";//正无穷
        private const string NegativeInfinity = "-Infinity";//负无穷
        private const string NaN = "NaN";//无效数字

        #endregion

        #region 属性
        /// <summary> 允许注释 </summary>
        public bool AllowComments
        {
            get { return lexer.AllowComments; }
            set { lexer.AllowComments = value; }
        }

        /// <summary> 允许单引号字符串 </summary>
        public bool AllowSingleQuotedStrings
        {
            get { return lexer.AllowSingleQuotedStrings; }
            set { lexer.AllowSingleQuotedStrings = value; }
        }

        /// <summary> 跳过没有的属性 </summary>
        public bool SkipNonMembers
        {
            get { return skip_non_members; }
            set { skip_non_members = value; }
        }
        /// <summary> TextReader已经读完 </summary>
        public bool EndOfInput
        {
            get { return end_of_input; }
        }
        /// <summary> 标记读完一个json元素(key or value) </summary>
        public bool EndOfJson
        {
            get { return end_of_json; }
        }

        public JsonToken Token
        {
            get { return token; }
        }

        public object Value
        {
            get { return token_value; }
        }
        #endregion

        #region 构造
        static JsonReader()
        {
            parse_table = PopulateParseTable();
        }

        internal JsonReader(string json_text) : this(new StringReader(json_text), true) { }

        internal JsonReader(TextReader reader) : this(reader, false) { }

        private JsonReader(TextReader reader, bool owned)
        {
            if (reader == null)
                throw new ArgumentNullException("TextReader is Null");

            parser_in_string = false;
            parser_return = false;

            read_started = false;
            automaton_stack = new Stack<int>();
            automaton_stack.Push((int)ParserToken.End);
            automaton_stack.Push((int)ParserToken.Start);

            lexer = new Lexer(reader);

            end_of_input = false;
            end_of_json = false;

            skip_non_members = true;

            this.reader = reader;
            reader_is_owned = owned;
        }
        #endregion

        #region 静态方法

        //----------------------------------------------------------------------------------
        //                                 分析表结构
        //                                   table
        //                    ┌───────────────────────────...──────┐
        //                    │ 65543                              │ 65552  指示解析时的逻辑分支 ParserToken
        //                    │                                    │
        //                    │                                    │
        //       ┌────────────┴──────...───┐            ┌──────────┴───────...───┐
        //       │int                      │int         │int                     │int  读取时遇到的内容 (符号或者是类型标记)
        //       │                         │            │                        │
        //       │                         │            │                        │
        //      int[]                     int[]        int[]                    int[]   对应的解析逻辑分支
        //
        //-----------------------------------------------------------------------------------

        /// <summary> 填充分析表 </summary>
        private static IDictionary<int, IDictionary<int, int[]>> PopulateParseTable()
        {
            IDictionary<int, IDictionary<int, int[]>> table = new Dictionary<int, IDictionary<int, int[]>>();

            //65543
            TableAddRow(table, ParserToken.Start);
            TableAddCol(table, ParserToken.Start, '[',
                            (int)ParserToken.Array);
            TableAddCol(table, ParserToken.Start, '{',
                            (int)ParserToken.Object);

           

            //65544
            TableAddRow(table, ParserToken.Object);
            TableAddCol(table, ParserToken.Object, '{',
                            '{',
                            (int)ParserToken.ObjectPrime);

            //65545
            TableAddRow(table, ParserToken.ObjectPrime);
            TableAddCol(table, ParserToken.ObjectPrime, '"',
                            (int)ParserToken.Pair,
                            (int)ParserToken.PairRest,
                            '}');
            TableAddCol(table, ParserToken.ObjectPrime, '}',
                            '}');

            //65546
            TableAddRow(table, ParserToken.Pair);
            TableAddCol(table, ParserToken.Pair, '"',
                            (int)ParserToken.String,
                            ':',
                            (int)ParserToken.Value);

            //65547
            TableAddRow(table, ParserToken.PairRest);
            TableAddCol(table, ParserToken.PairRest, ',',
                            ',',
                            (int)ParserToken.Pair,
                            (int)ParserToken.PairRest);
            TableAddCol(table, ParserToken.PairRest, '}',
                            (int)ParserToken.Close);

           
            //65548
            TableAddRow(table, ParserToken.Array);
            TableAddCol(table, ParserToken.Array, '[',
                            '[',
                            (int)ParserToken.ArrayPrime);

            //65549
            TableAddRow(table, ParserToken.ArrayPrime);
            TableAddCol(table, ParserToken.ArrayPrime, '"',
                            (int)ParserToken.Value,
                            (int)ParserToken.ValueRest,
                            ']');
            TableAddCol(table, ParserToken.ArrayPrime, '[',
                            (int)ParserToken.Value,
                            (int)ParserToken.ValueRest,
                            ']');
            TableAddCol(table, ParserToken.ArrayPrime, ']',
                            ']');
            TableAddCol(table, ParserToken.ArrayPrime, '{',
                            (int)ParserToken.Value,
                            (int)ParserToken.ValueRest,
                            ']');
            TableAddCol(table, ParserToken.ArrayPrime, (int)ParserToken.Number,
                            (int)ParserToken.Value,
                            (int)ParserToken.ValueRest,
                            ']');
            TableAddCol(table, ParserToken.ArrayPrime, (int)ParserToken.True,
                            (int)ParserToken.Value,
                            (int)ParserToken.ValueRest,
                            ']');
            TableAddCol(table, ParserToken.ArrayPrime, (int)ParserToken.False,
                            (int)ParserToken.Value,
                            (int)ParserToken.ValueRest,
                            ']');
            TableAddCol(table, ParserToken.ArrayPrime, (int)ParserToken.Null,
                            (int)ParserToken.Value,
                            (int)ParserToken.ValueRest,
                            ']');

            //65550
            TableAddRow(table, ParserToken.Value);
            TableAddCol(table, ParserToken.Value, '"',
                            (int)ParserToken.String);
            TableAddCol(table, ParserToken.Value, '[',
                            (int)ParserToken.Array);
            TableAddCol(table, ParserToken.Value, '{',
                            (int)ParserToken.Object);
            TableAddCol(table, ParserToken.Value, (int)ParserToken.Number,
                            (int)ParserToken.Number);
            TableAddCol(table, ParserToken.Value, (int)ParserToken.True,
                            (int)ParserToken.True);
            TableAddCol(table, ParserToken.Value, (int)ParserToken.False,
                            (int)ParserToken.False);
            TableAddCol(table, ParserToken.Value, (int)ParserToken.Null,
                            (int)ParserToken.Null);

            //65551
            TableAddRow(table, ParserToken.ValueRest);
            TableAddCol(table, ParserToken.ValueRest, ',',
                            ',',
                            (int)ParserToken.Value,
                            (int)ParserToken.ValueRest);
            TableAddCol(table, ParserToken.ValueRest, ']',
                            (int)ParserToken.Close);

            //65552
            TableAddRow(table, ParserToken.String);
            TableAddCol(table, ParserToken.String, '"',
                            '"',
                            (int)ParserToken.CharSeq,
                            '"');

            return table;
        }

        private static void TableAddRow(IDictionary<int, IDictionary<int, int[]>> table, ParserToken row)
        {
            table.Add((int)row, new Dictionary<int, int[]>());
        }

        private static void TableAddCol(IDictionary<int, IDictionary<int, int[]>> table, ParserToken row, int col, params int[] symbols)
        {
            table[(int)row].Add(col, symbols);
        }

        #endregion

        #region 私有方法
        /// <summary> 处理标记 根据current_symbol切换逻辑 </summary>
        private void ProcessSymbol()
        {
            switch (current_symbol)
            {
                case '[':
                    token = JsonToken.ArrayStart;
                    parser_return = true;
                    break;
                case ']':
                    token = JsonToken.ArrayEnd;
                    parser_return = true;
                    break;
                case '{':
                    token = JsonToken.ObjectStart;
                    parser_return = true; 
                    break;
                case '}':
                    token = JsonToken.ObjectEnd;
                    parser_return = true;
                    break;
                case '"':
                    if (parser_in_string)
                    {
                        parser_in_string = false;
                        parser_return = true;
                    }
                    else
                    {
                        if (token == JsonToken.None)
                            token = JsonToken.String;
                        parser_in_string = true;
                    }
                    break;
                case (int)ParserToken.Number:
                    ProcessNumber(lexer.String_KeyOrValue);
                    parser_return = true;
                    break;
                case (int)ParserToken.True:
                    token = JsonToken.Boolean;
                    token_value = true;
                    parser_return = true;
                    break;
                case (int)ParserToken.False:
                    token = JsonToken.Boolean;
                    token_value = false;
                    parser_return = true;
                    break;
                case (int)ParserToken.Null:
                    token = JsonToken.Null;
                    parser_return = true;
                    break;
                case (int)ParserToken.CharSeq:
                    token_value = lexer.String_KeyOrValue;
                    break;
                case (int)ParserToken.Pair:
                    token = JsonToken.PropertyName;
                    break;
            }

        }

        /// <summary> 数字处理程序 </summary>
        private void ProcessNumber(string number)
        {
            if (number.IndexOf('.') != -1 || number.IndexOf('e') != -1 || number.IndexOf('E') != -1)
            {
                if (double.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out double n_double))
                {
                    token = JsonToken.Double;
                    token_value = n_double;

                    return;
                }
            }

            if (int.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out int n_int32))
            {
                token = JsonToken.Int;
                token_value = n_int32;

                return;
            }

            if (long.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out long n_int64))
            {
                token = JsonToken.Long;
                token_value = n_int64;

                return;
            }

            if (ulong.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong n_uint64))
            {
                token = JsonToken.Long;
                token_value = n_uint64;

                return;
            }

            //非常规数字出现的概率较小  放在最后处理
            if (string.Compare(number, PositiveInfinity, StringComparison.Ordinal) == 0)//正无穷
            {
                token = JsonToken.Double;
                token_value = float.PositiveInfinity;
                return;
            }
            if (string.Compare(number, NegativeInfinity, StringComparison.Ordinal) == 0)//负无穷
            {
                token = JsonToken.Double;
                token_value = float.NegativeInfinity;
                return;
            }
            if (string.Compare(number, NaN, StringComparison.Ordinal) == 0)//无效数字
            {
                token = JsonToken.Double;
                token_value = float.NaN;
                return;
            }

            //理论上不会执行，但以防万一，返回默认值Int 0
            token = JsonToken.Int;
            token_value = 0;
        }

        /// <summary> 读取经fsm格式化后的json文本,读到标记就会return  (各种符号or完整的值) </summary>
        private bool ReadToken()
        {
            if (end_of_input)
                return false;

            lexer.NextToken();

            if (lexer.EndOfInput)
            {
                Close();
                return false;
            }

            current_input = lexer.Token;

            //if (current_input < (int)ParserToken.None)
            //    ZLog.Info((char)current_input);
            //else
            //    ZLog.Info((ParserToken)current_input);
            return true;
        }
        #endregion

        /// <summary> 关闭读取器 set this.reader = null </summary>
        public void Close()
        {
            if (end_of_input)
                return;

            end_of_input = true;
            end_of_json = true;

            if (reader_is_owned)
            {
                using (reader) { }
            }
            reader = null;
        }

        public bool Read()
        {
            if (end_of_input)
                return false;

            if (end_of_json)
            {
                end_of_json = false;
                automaton_stack.Clear();
                automaton_stack.Push((int)ParserToken.End);
                automaton_stack.Push((int)ParserToken.Start);
            }

            parser_in_string = false;
            parser_return = false;

            token = JsonToken.None;
            token_value = null;

            if (!read_started)
            {
                read_started = true;

                if (!ReadToken())
                    return false;
            }


            int[] entry_symbols;

            while (true)
            {
                if (parser_return)
                {
                    if (automaton_stack.Peek() == (int)ParserToken.End)
                        end_of_json = true;
                    return true;
                }

                current_symbol = automaton_stack.Pop();
              
                ProcessSymbol();

                if (current_symbol == current_input)
                {
                    if (!ReadToken())
                    {
                        if (automaton_stack.Peek() != (int)ParserToken.End)//读不到下一个token但栈还没到底 有冗余
                            throw new Exception("输入不是正确的Json文本");

                        if (parser_return)
                            return true;

                        return false;
                    }
                    continue;
                }

                try
                {
                    entry_symbols = parse_table[current_symbol][current_input];
                }
                catch (Exception)
                {
                    if (current_input < (int)ParserToken.None)
                        throw new Exception($"输入字符串中无效的输入 '{(char)current_input}'");
                    else
                        throw new Exception($"无效标识 '{(ParserToken)current_input}'");
                }

                if (entry_symbols[0] == (int)ParserToken.Close)
                    continue;

                for (int i = entry_symbols.Length - 1; i >= 0; i--)//倒序入栈  出栈的时候正好就是数组的正序
                {
                    automaton_stack.Push(entry_symbols[i]);
                }
            }
        }

    }
}
