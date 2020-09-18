namespace DingDingDiceBot.CmdHelper.Tokens.BinaryOperators
{
    internal sealed class Multiplication : BinaryOperator
    {
        public static readonly Multiplication Token = new Multiplication();

        public override int Precedence => PrecedenceOfMultiplicationAndDivision;

        public override string Name => "\\*";

        public override long CalcCore(long a, long b) => a * b;

        protected override int TryGetOperator(string command, int pos, int length, out BinaryOperator token)
        {
            unsafe
            {
                fixed (char* p = command)
                {
                    token = Token;
                    if (p[pos] != '*')
                    {
                        return 0;
                    }
                    return 1;
                }
            }
        }
    }
}
