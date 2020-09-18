using System;

namespace DingDingDiceBot.CmdHelper.Tokens
{
    internal sealed class Int32Operand : Token, IResultConverter
    {
        public static readonly Int32Operand Token = new Int32Operand();
        private const int MAX_LEN = 10;

        internal override TokenType Type => TokenType.Int32Operand;
        public long Value { get; private set; }

        public CalcResult ToCalcResult() => new CalcResult(Value, Value.ToString(), 0);

        public override string ToString() => Value.ToString();

        internal override void ReadToken(ParseContext context)
        {
            TokenType lastToken = context._lastTokenType;
            bool isNegative = context._lastTokenType == TokenType.Negative;
            long value = GetABSInt32FixPos(context);
            if (value <= -1L)
            {
                if (isNegative)
                {
                    context.SetFail("错误的“-”号。");
                }
                return;
            }
            if (lastToken == TokenType.Int32Operand || lastToken == TokenType.RandomOperand ||
                lastToken == TokenType.Function || lastToken == TokenType.RightParenthesis)
            {
                context.SetFail("常数操作符位置错误。");
                return;
            }
            context._lastTokenType = Type;
            context.Enqueue(new Int32Operand
            {
                Value = (isNegative ? (0L - value) : value)
            });
        }

        internal static unsafe long GetABSInt32FixPos(ParseContext context)
        {
            char* str = context.Str;
            int pos = context.Pos;
            int length = context.Length;
            int i = 0;
            int limit = Math.Min(length - pos, 10);
            byte* buf = stackalloc byte[limit];
            while (i < limit && str[i + pos] >= '0' && str[i + pos] <= '9')
            {
                buf[i] = (byte)(str[i + pos] - '0');
                i++;
            }
            if (i > 0)
            {
                context.Pos += i;
                int j = i - 1;
                long value = (long)buf[j--];
                int k = 10;
                while (j >= 0)
                {
                    value += buf[j] * k;
                    k *= 10;
                    j--;
                }
                return value;
            }
            return -1L;
        }
    }
}
