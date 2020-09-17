using System;

namespace DingDingDiceBot.CmdHelper.Tokens
{
    internal sealed class Int32Operand : Token, IResultConverter
    {
        public static readonly Int32Operand Token = new Int32Operand();
        private const int MAX_LEN = 10;

        public override TokenType Type => TokenType.Int32Operand;
        public long Value { get; private set; }

        public CalcResult ToCalcResult() => new CalcResult(Value, Value.ToString(), 0);

        public override string ToString() => Value.ToString();

        internal override void ReadToken(ParseContext context)
        {
            var lastToken = context.LastTokenType;
            var isNegative = context.LastTokenType == TokenType.Negative;
            var value = GetABSInt32FixPos(context);
            if (value <= -1L)
            {
                if (isNegative)
                {
                    context.SetFail("错误的“-”号。");
                }
                return;
            }
            if (lastToken == TokenType.Int32Operand)
            {
                context.SetFail("多余的常数操作符。");
                return;
            }
            context.LastTokenType = Type;
            context.Enqueue(new Int32Operand
            {
                Value = (isNegative ? (0L - value) : value)
            });
        }

        internal unsafe static long GetABSInt32FixPos(ParseContext context)
        {
            char* str = context.Str;
            var pos = context.Pos;
            var length = context.Length;
            var i = 0;
            var limit = Math.Min(length - pos, 10);
            byte* buf = stackalloc byte[limit];
            while (i < limit && str[i + pos] >= '0' && str[i + pos] <= '9')
            {
                buf[i] = (byte)(str[i + pos] - '0');
                i++;
            }
            if (i > 0)
            {
                context.Pos += i;
                var j = i - 1;
                var value = (long)buf[j--];
                var k = 10;
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