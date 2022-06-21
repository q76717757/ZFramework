/** Header
 * Lexer.cs
 *   基于FSM有限状态机的解析器实现。 主要作用是将输入的json串进行格式化 包括unicode编码 转义 消除缩进 注释等
 **/


using System;
using System.IO;
using System.Text;

namespace ZFramework
{
    /// <summary> FSM状态机上下文 </summary>
    internal class FsmContext
    {
        /// <summary> 标记Fsm到达出口 </summary>
        public bool Return;
        /// <summary> 指示下一状态 从1算起</summary>
        public int NextState;
        /// <summary> 解析器 </summary>
        public Lexer Lexer;
        /// <summary> 当前状态 从1算起 </summary>
        public int StateStack;
    }


    /// <summary> 语法分析程序 </summary>
    internal class Lexer
    {
        #region 字段
        /// <summary> 状态处理逻辑 </summary>
        private delegate bool StateHandler(FsmContext ctx);

        /// <summary> fsm委托返回值表 取值从maxChar+1开始 值0时无意义项 </summary>
        private static readonly int[] fsm_return_table;
        /// <summary> fsm委托表表 </summary>
        private static readonly StateHandler[] fsm_handler_table;

        /// <summary> 允许注释 </summary>
        private bool allow_comments;
        /// <summary> 允许单引号字符串  </summary>
        private bool allow_single_quoted_strings;
        /// <summary> TextReander已经读完 </summary>
        private bool end_of_input;
        /// <summary> 当前上下文 </summary>
        private FsmContext fsm_context;
        /// <summary> 输入缓存 </summary>
        private int input_buffer;
        /// <summary> 输入字符 </summary>
        private int input_char;
        /// <summary> 文本读取器 </summary>
        private TextReader reader;
        /// <summary> 状态机编号 从1算起  对应fsm表中1~30状态 </summary>
        private int state;
        /// <summary> 缓存格式化中的json串 </summary>
        private StringBuilder string_buffer;
        /// <summary> 格式化出来的键或值对应的字符串 </summary>
        private string string_key_value;
        /// <summary> 标识 </summary>
        private int token;
        /// <summary> Unicode码 </summary>
        private int unichar;
        #endregion

        #region 属性
        /// <summary> 允许注释 </summary>
        public bool AllowComments
        {
            get { return allow_comments; }
            set { allow_comments = value; }
        }

        /// <summary> 允许单引号字符串 </summary>
        public bool AllowSingleQuotedStrings
        {
            get { return allow_single_quoted_strings; }
            set { allow_single_quoted_strings = value; }
        }

        /// <summary> TextReader已经读完 </summary>
        public bool EndOfInput
        {
            get { return end_of_input; }
        }

        /// <summary> 标识 </summary>
        public int Token
        {
            get { return token; }
        }
        /// <summary> 格式化得到的键或值的对应字符串 </summary>
        public string String_KeyOrValue
        {
            get { return string_key_value; }
        }
        #endregion

        #region 构造
        static Lexer()
        {
            PopulateFsmTables(out fsm_handler_table, out fsm_return_table);
        }
        internal Lexer(TextReader reader)
        {
            allow_comments = true;
            allow_single_quoted_strings = true;

            input_buffer = 0;
            string_buffer = new StringBuilder(128);
            state = 1;
            end_of_input = false;
            this.reader = reader;

            fsm_context = new FsmContext();
            fsm_context.Lexer = this;
        }
        #endregion

        
        #region 静态方法 语法分析算法
        /// <summary> 十六进制转十进制(ASCII编码)(其中A~F忽略大小写) </summary>
        private static int HexValue(int digit)
        {
            switch (digit)
            {
                case 'a':
                case 'A':
                    return 10;

                case 'b':
                case 'B':
                    return 11;

                case 'c':
                case 'C':
                    return 12;

                case 'd':
                case 'D':
                    return 13;

                case 'e':
                case 'E':
                    return 14;

                case 'f':
                case 'F':
                    return 15;

                default:
                    return digit - '0';//除了A~F的大小写外 其他字母转成正常编码
            }
        }

        /// <summary> 填充Fsm委托表 </summary>
        private static void PopulateFsmTables(out StateHandler[] fsm_handler_table, out int[] fsm_return_table)
        {
            // See section A.1. of the manual for details of the finite
            // state machine.
            fsm_handler_table = new StateHandler[30] {
                State1,
                State2,
                State3,
                State4,
                State5,
                State6,
                State7,
                State8,
                State9,
                State10,
                State11,
                State12,
                State13,
                State14,
                State15,
                State16,
                State17,
                State18,
                State19,
                State20,
                State21,
                State22,
                State23,
                State24,
                State25,
                State26,
                State27,
                State28,
                State29,
                State30
            };

            fsm_return_table = new int[30] {
                (int) ParserToken.Char,
                0,
                (int) ParserToken.Number,
                (int) ParserToken.Number,
                0,
                (int) ParserToken.Number,
                0,
                (int) ParserToken.Number,
                0,
                0,
                (int) ParserToken.True,
                0,
                0,
                0,
                (int) ParserToken.False,
                0,
                0,
                (int) ParserToken.Null,
                (int) ParserToken.CharSeq,
                (int) ParserToken.Char,
                0,
                0,
                (int) ParserToken.CharSeq,
                (int) ParserToken.Char,
                0,
                0,
                0,
                0,
                (int) ParserToken.Number,
                (int) ParserToken.Number
            };
        }

        /// <summary> 转义字符处理程序 将n t r -> \n \t \r </summary>
        private static char ProcessEscChar(int esc_char)
        {
            switch (esc_char)
            {
                case '"'://双引号
                case '\''://单引号
                case '\\'://反斜杠
                case '/'://斜杠
                    return Convert.ToChar(esc_char);

                case 'n':
                    return '\n';

                case 't':
                    return '\t';

                case 'r':
                    return '\r';

                case 'b':
                    return '\b';

                case 'f':
                    return '\f';

                default:
                    //不存在的
                    return '?';
            }
        }

        /// <summary> 状态机入口 </summary>
        private static bool State1(FsmContext ctx)
        {
            while (ctx.Lexer.GetChar())
            {
                if (ctx.Lexer.input_char == ' ' ||
                    ctx.Lexer.input_char >= '\t' && ctx.Lexer.input_char <= '\r')
                    continue;//忽略无效的符号  包括 换行 空格 制表符等

                if (ctx.Lexer.input_char >= '1' && ctx.Lexer.input_char <= '9')//数字
                {
                    ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                    ctx.NextState = 3;
                    return true;
                }

                switch (ctx.Lexer.input_char)
                {
                    case '"'://字符串起止
                        ctx.NextState = 19;
                        ctx.Return = true;
                        return true;

                    case ','://终结符
                    case ':':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                        ctx.NextState = 1;
                        ctx.Return = true;
                        return true;

                    case '-'://负数
                        ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                        ctx.NextState = 2;
                        return true;

                    case '0'://0.xx 小数 或者整数0
                        ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                        ctx.NextState = 4;
                        return true;

                    case 'f'://false检查
                        ctx.NextState = 12;
                        return true;

                    case 'n'://null检查
                        ctx.NextState = 16;
                        return true;

                    case 't'://true检查
                        ctx.NextState = 9;
                        return true;

                    case '\''://单引号变换
                        if (!ctx.Lexer.allow_single_quoted_strings)
                            return false;

                        ctx.Lexer.input_char = '"';
                        ctx.NextState = 23;
                        ctx.Return = true;
                        return true;

                    case '/'://注释检查
                        if (!ctx.Lexer.allow_comments)
                            return false;

                        ctx.NextState = 25;
                        return true;

                    case 'I'://正无穷
                        ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                        ctx.NextState = 29;
                        return true;

                    case 'N'://NaN无效检查
                        ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                        ctx.NextState = 30;
                        return true;
                    default:
                        return false;
                }
            }

            return true;
        }

        /// <summary> 数字头处理程序 </summary>
        private static bool State2(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            if (ctx.Lexer.input_char >= '1' && ctx.Lexer.input_char <= '9')//非0开头的数字 x.xxx
            {
                ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                ctx.NextState = 3;
                return true;
            }

            switch (ctx.Lexer.input_char)
            {
                case '0'://0开头数字  0.xxx
                    ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                    ctx.NextState = 4;
                    return true;
                case 'I'://负无穷
                    ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                    ctx.NextState = 29;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary> 数字处理程序(整数部分) </summary>
        private static bool State3(FsmContext ctx)
        {
            while (ctx.Lexer.GetChar())
            {
                if (ctx.Lexer.input_char >= '0' && ctx.Lexer.input_char <= '9')
                {
                    ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                    continue;
                }

                if (ctx.Lexer.input_char == ' ' ||
                    ctx.Lexer.input_char >= '\t' && ctx.Lexer.input_char <= '\r')
                {
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;
                }

                switch (ctx.Lexer.input_char)
                {
                    case ',':
                    case ']':
                    case '}':
                        ctx.Lexer.UngetChar();
                        ctx.Return = true;
                        ctx.NextState = 1;
                        return true;

                    case '.':
                        ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                        ctx.NextState = 5;
                        return true;

                    case 'e':
                    case 'E':
                        ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                        ctx.NextState = 7;
                        return true;

                    default:
                        return false;
                }
            }
            return true;
        }

        /// <summary> 数字处理程序(小数部分) </summary>
        private static bool State4(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            if (ctx.Lexer.input_char == ' ' ||
                ctx.Lexer.input_char >= '\t' && ctx.Lexer.input_char <= '\r')
            {
                ctx.Return = true;
                ctx.NextState = 1;
                return true;
            }

            switch (ctx.Lexer.input_char)
            {
                case ',':
                case ']':
                case '}':
                    ctx.Lexer.UngetChar();
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;

                case '.':
                    ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                    ctx.NextState = 5;
                    return true;

                case 'e':
                case 'E':
                    ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                    ctx.NextState = 7;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 小数处理程序 </summary>
        private static bool State5(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            if (ctx.Lexer.input_char >= '0' && ctx.Lexer.input_char <= '9')
            {
                ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                ctx.NextState = 6;
                return true;
            }

            return false;
        }

        /// <summary> double数字识别程序 </summary>
        private static bool State6(FsmContext ctx)
        {
            while (ctx.Lexer.GetChar())
            {
                if (ctx.Lexer.input_char >= '0' && ctx.Lexer.input_char <= '9')
                {
                    ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                    continue;
                }

                if (ctx.Lexer.input_char == ' ' ||
                    ctx.Lexer.input_char >= '\t' && ctx.Lexer.input_char <= '\r')
                {
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;
                }

                switch (ctx.Lexer.input_char)
                {
                    case ',':
                    case ']':
                    case '}':
                        ctx.Lexer.UngetChar();
                        ctx.Return = true;
                        ctx.NextState = 1;
                        return true;

                    case 'e':
                    case 'E':
                        ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                        ctx.NextState = 7;
                        return true;

                    default:
                        return false;
                }
            }

            return true;
        }

        /// <summary> double数字指数正负检查程序 </summary>
        private static bool State7(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            if (ctx.Lexer.input_char >= '0' && ctx.Lexer.input_char <= '9')
            {
                ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                ctx.NextState = 8;
                return true;
            }

            switch (ctx.Lexer.input_char)
            {
                case '+':
                case '-':
                    ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                    ctx.NextState = 8;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> double数字指数填充程序 </summary>
        private static bool State8(FsmContext ctx)
        {
            while (ctx.Lexer.GetChar())
            {
                if (ctx.Lexer.input_char >= '0' && ctx.Lexer.input_char <= '9')
                {
                    ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                    continue;
                }

                if (ctx.Lexer.input_char == ' ' ||
                    ctx.Lexer.input_char >= '\t' && ctx.Lexer.input_char <= '\r')//9~13 -> 水平制表 换行 垂直制表 换页 回车
                {
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;
                }

                switch (ctx.Lexer.input_char)//终结符号
                {
                    case ',':
                    case ']':
                    case '}':
                        ctx.Lexer.UngetChar();
                        ctx.Return = true;
                        ctx.NextState = 1;
                        return true;

                    default:
                        return false;
                }
            }

            return true;
        }

        /// <summary> 小写字母r检查程序 </summary>
        private static bool State9(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case 'r':
                    ctx.NextState = 10;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 小写字母u检查程序 </summary>
        private static bool State10(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case 'u':
                    ctx.NextState = 11;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 小写字母e检查程序 </summary>
        private static bool State11(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case 'e':
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 小写字母a检查程序 </summary>
        private static bool State12(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case 'a':
                    ctx.NextState = 13;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 小写字母l检查程序 </summary>
        private static bool State13(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case 'l':
                    ctx.NextState = 14;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 小写字母s检查程序 </summary>
        private static bool State14(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case 's':
                    ctx.NextState = 15;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 小写字母e判断程序 </summary>
        private static bool State15(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case 'e':
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 小写字母u检查程序  用于判断null </summary>
        private static bool State16(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case 'u':
                    ctx.NextState = 17;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 小写字母l检查程序  用于判断null </summary>
        private static bool State17(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case 'l':
                    ctx.NextState = 18;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary> 小写字母l检查程序 用于判断null </summary>
        private static bool State18(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case 'l':
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 双引号和反斜杠检查程序 </summary>
        private static bool State19(FsmContext ctx)
        {
            while (ctx.Lexer.GetChar())
            {
                switch (ctx.Lexer.input_char)
                {
                    case '"':
                        ctx.Lexer.UngetChar();
                        ctx.Return = true;
                        ctx.NextState = 20;
                        return true;

                    case '\\':
                        ctx.StateStack = 19;
                        ctx.NextState = 21;
                        return true;

                    default:
                        ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                        continue;
                }
            }

            return true;
        }

        /// <summary> 双引号检查程序 return char == "? </summary>
        private static bool State20(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case '"':
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 转义处理程序 n r t -> \n \r \t  </summary>
        private static bool State21(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case 'u':
                    ctx.NextState = 22;//unicode还原
                    return true;

                case '"':
                case '\'':
                case '/':
                case '\\':
                case 'b':
                case 'f':
                case 'n':
                case 'r':
                case 't':
                    ctx.Lexer.string_buffer.Append(ProcessEscChar(ctx.Lexer.input_char));
                    ctx.NextState = ctx.StateStack;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> Unicode编码还原 \uXXXX -> Char </summary>
        private static bool State22(FsmContext ctx)
        {
            int counter = 0;
            int mult = 4096;

            ctx.Lexer.unichar = 0;

            while (ctx.Lexer.GetChar())
            {
                if (ctx.Lexer.input_char >= '0' && ctx.Lexer.input_char <= '9' ||
                    ctx.Lexer.input_char >= 'A' && ctx.Lexer.input_char <= 'F' ||
                    ctx.Lexer.input_char >= 'a' && ctx.Lexer.input_char <= 'f')//属于16进制 0~f范围内
                {

                    ctx.Lexer.unichar += HexValue(ctx.Lexer.input_char) * mult; //4096 / 256 / 16 / 1

                    counter++;
                    mult /= 16;

                    if (counter == 4)//4位的16进制符号  \uXXXX
                    {
                        ctx.Lexer.string_buffer.Append(
                            Convert.ToChar(ctx.Lexer.unichar));
                        ctx.NextState = ctx.StateStack;
                        return true;
                    }

                    continue;
                }

                return false;
            }

            return true;
        }

        /// <summary> 反斜杠和单引号 检查程序 </summary>
        private static bool State23(FsmContext ctx)
        {
            while (ctx.Lexer.GetChar())
            {
                switch (ctx.Lexer.input_char)
                {
                    case '\'':
                        ctx.Lexer.UngetChar();
                        ctx.Return = true;
                        ctx.NextState = 24;
                        return true;

                    case '\\':
                        ctx.StateStack = 23;
                        ctx.NextState = 21;
                        return true;

                    default:
                        ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                        continue;
                }
            }

            return true;
        }

        /// <summary> 单引号转双引号处理程序 \' -> " </summary>
        private static bool State24(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case '\'':
                    ctx.Lexer.input_char = '"';
                    ctx.Return = true;
                    ctx.NextState = 1;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 星号 斜杠检查程序 </summary>
        private static bool State25(FsmContext ctx)
        {
            ctx.Lexer.GetChar();

            switch (ctx.Lexer.input_char)
            {
                case '*':
                    ctx.NextState = 27;
                    return true;

                case '/':
                    ctx.NextState = 26;
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> 换行符检查程序 </summary>
        private static bool State26(FsmContext ctx)
        {
            while (ctx.Lexer.GetChar())
            {
                if (ctx.Lexer.input_char == '\n')
                {
                    ctx.NextState = 1;
                    return true;
                }
            }

            return true;
        }

        /// <summary> /*注释头处理程序 </summary>
        private static bool State27(FsmContext ctx)
        {
            while (ctx.Lexer.GetChar())
            {
                if (ctx.Lexer.input_char == '*')
                {
                    ctx.NextState = 28;
                    return true;
                }
            }

            return true;
        }
        /// <summary> */注释尾处理程序 </summary>
        private static bool State28(FsmContext ctx)
        {
            while (ctx.Lexer.GetChar())
            {
                if (ctx.Lexer.input_char == '*')
                    continue;

                if (ctx.Lexer.input_char == '/')
                {
                    ctx.NextState = 1;
                    return true;
                }

                ctx.NextState = 27;
                return true;
            }

            return true;
        }

        /// <summary> Infinity无穷数字处理程序 </summary>
        private static bool State29(FsmContext ctx)
        {
            int index = 0;
            while (ctx.Lexer.GetChar())
            {
                switch (ctx.Lexer.input_char)
                {
                    case 'n':
                        if (index == 0 || index == 3)
                        {
                            index++;
                            break;
                        }
                        else
                            return false;
                    case 'f':
                        if (index == 1)
                        {
                            index++;
                            break;
                        }
                        else
                            return false;
                    case 'i':
                        if (index == 2 || index == 4)
                        {
                            index++;
                            break;
                        }
                        else
                            return false;
                    case 't':
                        if (index == 5)
                        {
                            index++;
                            break;
                        }
                        else
                            return false;
                    case 'y':
                        if (index == 6)
                        {
                            ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                            ctx.NextState = 1;
                            ctx.Return = true;
                            return true;
                        }
                        return false;
                    default:
                        return false;
                }
                ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
            }
            return false;
        }

        /// <summary> NaN无效数字处理程序 </summary>
        private static bool State30(FsmContext ctx)
        {
            int index = 0;
            while (ctx.Lexer.GetChar())
            {
                switch (ctx.Lexer.input_char)
                {
                    case 'a':
                        if (index == 0)
                        {
                            index++;
                            break;
                        }
                        else
                            return false;
                    case 'N':
                        if (index == 1)
                        {
                            ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
                            ctx.NextState = 1;
                            ctx.Return = true;
                            return true;
                        }
                        return false;
                    default:
                        return false;
                }
                ctx.Lexer.string_buffer.Append((char)ctx.Lexer.input_char);
            }
            return false;
        }
        #endregion


        /// <summary> 取出下一个char到input_char  成功返回true 失败返回false </summary>
        private bool GetChar()
        {
            if ((input_char = NextChar()) != -1)
                return true;

            end_of_input = true;
            return false;
        }
        /// <summary> 获取下一个字符  如果缓存有则从缓存拿 </summary>
        private int NextChar()
        {
            if (input_buffer != 0)//有被回退的char
            {
                int tmp = input_buffer;
                input_buffer = 0;

                return tmp;
            }
            return reader.Read();
        }

        /// <summary> 将char退回到缓存 </summary>
        private void UngetChar()
        {
            input_buffer = input_char;
        }

        /// <summary> 继续运行fsm 直到fsm再次返回 </summary>
        public bool NextToken()
        {
            StateHandler handler;
            fsm_context.Return = false;

            while (true)//把本文投入状态机  直到状态机到达出口再跳出
            {
                handler = fsm_handler_table[state - 1];

                if (!handler(fsm_context))
                {
                    throw new Exception($"输入字符串中无效字符 '{(char)input_char}'  (状态码{state})");
                }

                if (end_of_input)
                    return false;

                if (fsm_context.Return)
                {
                    string_key_value = string_buffer.ToString();
                    string_buffer.Remove(0, string_buffer.Length);

                    token = fsm_return_table[state - 1];

                    if (token == (int)ParserToken.Char)
                        token = input_char;

                    state = fsm_context.NextState;

                    return true;
                }

                state = fsm_context.NextState;
            }
        }

    }
}
