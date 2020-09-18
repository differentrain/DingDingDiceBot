using System;

namespace DingDingDiceBot.CmdHelper
{
    /// <summary>
    /// 表示计算的结果。
    /// </summary>
    internal sealed class CalcResult : Token
    {
        internal CalcResult(long value, string text, int precedence)
        {
            Value = value;
            Text = text;
            Precedence = precedence;
        }

        internal override TokenType Type => TokenType.Result;

        internal int Precedence { get; }

        /// <summary>
        /// 计算的结果
        /// </summary>
        public long Value { get; }

        /// <summary>
        /// 计算结果的字符串形式。
        /// </summary>
        public string Text { get; private set; }

        internal void AddParenthesis() => Text = $"({Text})";

        internal override void ReadToken(ParseContext context) => throw new NotImplementedException();
    }
}
