namespace DingDingDiceBot.CmdHelper.Tokens.BinaryOperators
{
    internal sealed class Addition : BinaryOperator
    {
        public static readonly Addition Token = new Addition();

        public override int Precedence => 11;

        public override string Name => "+";

        public override long CalcCore(long a, long b) => a + b;

        protected override unsafe int TryGetOperator(char* str, int pos, int length, out BinaryOperator token)
        {
            token = Token;
            if (str[pos] != '+')
            {
                return 0;
            }
            return 1;
        }
    }
}
