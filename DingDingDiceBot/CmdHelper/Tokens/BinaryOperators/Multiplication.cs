using System;
using System.Collections.Generic;
using System.Text;

namespace DingDingDiceBot.CmdHelper.Tokens.BinaryOperators
{
    internal sealed class Multiplication : BinaryOperator
    {
        public static readonly Multiplication Token = new Multiplication();

        public override int Precedence => 10;

        public override bool IsSubOrDiv => false;

        public override string Name => "\\*";

        public override long CalcCore(long a, long b) => a * b;

        protected unsafe override BinaryOperator IsThisOperator(ParseContext context)
        {
            if (context.Str[context.Pos] != '*')
            {
                return null;
            }
            context.Pos++;
            return Token;
        }
    }
}
