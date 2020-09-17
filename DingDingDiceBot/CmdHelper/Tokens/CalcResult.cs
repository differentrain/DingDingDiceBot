using System;

namespace DingDingDiceBot.CmdHelper.Tokens
{
    internal sealed class CalcResult : Token
    {
        public CalcResult(long value, string text, int precedence)
        {
            Value = value;
            Text = text;
            Precedence = precedence;
        }

        public override TokenType Type => TokenType.Result;

        public int Precedence { get; }
        public long Value { get; }
        public string Text { get; private set; }

        public void AddParenthesis() => Text = $"({Text})";

        internal override void ReadToken(ParseContext context) => throw new NotImplementedException();
    }
}