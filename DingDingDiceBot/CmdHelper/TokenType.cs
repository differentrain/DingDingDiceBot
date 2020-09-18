namespace DingDingDiceBot.CmdHelper
{
    /// <summary>
    /// 表示 <see cref="Token"/> 的所属类型。
    /// </summary>
    internal enum TokenType
    {
        /// <summary>
        /// 表示匹配开始。任何  <see cref="Token"/> 的派生类都不应使用此类型。
        /// </summary>
        Begin,

        /// <summary>
        /// 表示32位整数。
        /// </summary>
        Int32Operand,

        /// <summary>
        /// 表示一个骰子指令。
        /// </summary>
        RandomOperand,

        /// <summary>
        /// 表示一个二元运算符。
        /// </summary>
        BinaryOperator,

        /// <summary>
        /// 左括号。
        /// </summary>
        LeftParenthesis,

        /// <summary>
        /// 右括号。
        /// </summary>
        RightParenthesis,

        /// <summary>
        /// 负号。
        /// </summary>
        Negative,

        /// <summary>
        /// 逗号。
        /// </summary>
        Comma,

        /// <summary>
        /// 函数。
        /// </summary>
        Function,

        /// <summary>
        /// 空格。
        /// </summary>
        WhiteSpace,

        /// <summary>
        /// 最后的计算结果。
        /// </summary>
        Result
    }
}
