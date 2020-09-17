using System;

namespace DingDingDiceBot.CmdHelper.Tokens.BinaryOperators
{
    internal sealed class DivisionCeiling : BinaryOperator
    {
        public static readonly DivisionCeiling Token = new DivisionCeiling();

        public override int Precedence => 10;

        public override bool IsSubOrDiv => false;

        public override string Name => "\\";

        public override long CalcCore(long a, long b) => (long)Math.Ceiling((double)a / b);

        protected unsafe override BinaryOperator IsThisOperator(ParseContext context)
        {
            if (context.Str[context.Pos] != '\\')
            {
                return null;
            }
            context.Pos++;
            return Token;
        }
    }
}