using System;
using System.Text;

namespace DingDingDiceBot.CmdHelper.Tokens
{
    internal sealed class RandomOperand : Token, IResultConverter
    {
        public static readonly RandomOperand Token = new RandomOperand();
        private static readonly Random s_rnds = new Random();

        private string _textDes;

        internal override TokenType Type => TokenType.RandomOperand;

        public long Value { get; private set; }

        public CalcResult ToCalcResult() => new CalcResult(Value, _textDes, 0);

        internal override unsafe void ReadToken(ParseContext context)
        {
            if (context.Str[context.Pos] != 'd' && context.Str[context.Pos] != 'D')
            {
                return;
            }
            context.Pos++;
            long dice = Int32Operand.GetABSInt32FixPos(context);
            TokenType lastToken = context._lastTokenType;
            if (lastToken == TokenType.RightParenthesis || lastToken == TokenType.RandomOperand || lastToken == TokenType.Function ||
                (dice != 2L && dice != 4L && dice != 6L && dice != 8L && dice != 10L && dice != 12L && dice != 20L && dice != 100L))
            {
                context.SetFail("错误的骰子指令。");
                return;
            }
            context._lastTokenType = TokenType.RandomOperand;
            if (lastToken != TokenType.Int32Operand)
            {
                context._containsDice = true;
                context.Enqueue(Create(1L, dice));
                return;
            }
            long r = (context.QueueEnd as Int32Operand).Value;
            if (r < 1L || r > 100L)
            {
                context.SetFail("骰子数量限制为1~100.");
                return;
            }
            context._containsDice = true;
            context.Replace(Create(r, dice));
        }

        private static RandomOperand Create(long r, long dice)
        {
            int result = 0;
            using (var sbw = InternalStringBuilderWapper.Create())
            {
                StringBuilder sb = sbw.SB;
                sb.Append($"{r}d{dice}{{");
                int i = 0;
                while (i < r)
                {
                    int j = s_rnds.Next(1, (int)dice + 1);
                    result += j;
                    sb.Append(j).Append('+');
                    i++;
                }
                sb.Remove(sb.Length - 1, 1).Append("}");
                return new RandomOperand
                {
                    Value = result,
                    _textDes = sb.ToString()
                };
            }
        }
    }
}
