using System;
using System.Collections.Generic;
using System.Text;

namespace DingDingDiceBot.CmdHelper.Tokens
{
    internal abstract class BinaryOperator : Token
    {

        public override TokenType Type => TokenType.BinaryOperator;

        public abstract int Precedence { get; }
        public abstract string Name { get; }
        public abstract bool IsSubOrDiv { get; }


        internal override void ReadToken(ParseContext context)
        {
            var pos = context.Pos;
            var token = IsThisOperator(context);
            if (pos != context.Pos)
            {
                if (token.Name.Equals("-") &&
                    (context.LastTokenType == TokenType.Begin ||
                    context.LastTokenType == TokenType.LeftParenthesis ||
                    context.LastTokenType == TokenType.BinaryOperator))
                {
                    context.LastTokenType = TokenType.Negative;
                    return;
                }
                if (context.LastTokenType == TokenType.BinaryOperator)
                {
                    context.SetFail("连续的操作符。");
                    return;
                }
                context.LastTokenType = Type;
                UpdateBinaryOperatorNotFixPos(token, context);
            }
        }

        private static void UpdateBinaryOperatorNotFixPos(BinaryOperator token, ParseContext context)
        {
            while (!context.StackEmpty)
            {
                Token top = context.StackTop;
                if (top.Type != TokenType.BinaryOperator ||
                    token.Precedence < (top as BinaryOperator).Precedence)
                {
                    break;
                }
                context.Enqueue(context.Pop());
            }
            context.Push(token);
        }

        internal CalcResult Calc(CalcResult a, CalcResult b)
        {
            if (a.Precedence > Precedence)
            {
                a.AddParenthesis();
            }
            if (b.Precedence > Precedence || (b.Precedence == Precedence && IsSubOrDiv))
            {
                b.AddParenthesis();
            }
            return new CalcResult(CalcCore(a.Value, b.Value), $"{a.Text}{Name}{b.Text}", Precedence);
        }

        public abstract long CalcCore(long a, long b);

        protected abstract BinaryOperator IsThisOperator(ParseContext context);
    }
}
