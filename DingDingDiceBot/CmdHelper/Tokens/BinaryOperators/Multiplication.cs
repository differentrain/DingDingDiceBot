namespace DingDingDiceBot.CmdHelper.Tokens.BinaryOperators
{
    internal sealed class Multiplication : BinaryOperator
    {
        public static readonly Multiplication Token = new Multiplication();

        public override int Precedence => 10;

        public override string Name => "\\*";

        public override long CalcCore(long a, long b) => a * b;

        protected override unsafe int TryGetOperator(char* str, int pos, int length, out BinaryOperator token)
        {
            token = Token;
            if (str[pos] != '*')
            {
                return 0;
            }
            return 1;
        }
    }
}
