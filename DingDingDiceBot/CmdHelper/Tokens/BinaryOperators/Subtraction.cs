namespace DingDingDiceBot.CmdHelper.Tokens.BinaryOperators
{
    internal sealed class Subtraction : BinaryOperator
    {
        public static readonly Subtraction Token = new Subtraction();

        public override int Precedence => 11;

        public override bool IsSubOrDiv => false;

        public override string Name => "-";

        public override long CalcCore(long a, long b) => a - b;

        protected unsafe override BinaryOperator IsThisOperator(ParseContext context)
        {
            if (context.Str[context.Pos] != '-')
            {
                return null;
            }
            context.Pos++;
            return Token;
        }
    }
}