namespace DingDingDiceBot.CmdHelper
{
    /// <summary>
    /// 表示一个读取到的符号。
    /// </summary>
    public abstract class Token
    {
        internal abstract TokenType Type { get; }

        /// <summary>
        /// 实现符号读取的方法。
        /// </summary>
        /// <param name="context">用于解析的上下文。</param>
        internal abstract unsafe void ReadToken(ParseContext context);
    }
}
