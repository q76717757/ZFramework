/** Header
 * ParserToken.cs
 *  lexer和解析器使用的标记的内部表示。
 **/


namespace ZFramework
{
    /// <summary> 解析器标记 有多种用途  (37~42 fam返回类型)  (43~52 解析表标记) </summary>
    internal enum ParserToken
    {
        None = System.Char.MaxValue + 1,  // = 65536

        // 标记fsm返回值的类型   37 ~ 42
        Number,
        True,
        False,
        Null,
        CharSeq,
        Char,

        // 解析器标记  43 ~ 52
        Start,
        Object,
        ObjectPrime,
        Pair,
        PairRest,
        Array,
        ArrayPrime,
        Value,
        ValueRest,
        String,

        // 输入结束   53
        End,

        // 闭合     54
        Close
    }
}
