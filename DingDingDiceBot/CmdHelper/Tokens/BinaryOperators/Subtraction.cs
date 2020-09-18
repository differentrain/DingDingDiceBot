namespace DingDingDiceBot.CmdHelper.Tokens.BinaryOperators
{
    internal sealed class Subtraction : BinaryOperator
    {
        public static readonly Subtraction Token = new Subtraction();

        public override int Precedence => PrecedenceOfAdditionAndSubtraction;

        public override string Name => "-";

        internal override bool IsSubOrDiv => true;

        public override long CalcCore(long a, long b) => a - b;

        protected override int TryGetOperator(string command, int pos, int length, out BinaryOperator token)
        {
            unsafe
            {
                fixed (char* p = command)
                {
                    token = Token;
                    if (p[pos] != '-')
                    {
                        return 0;
                    }
                    return 1;
                }
            }
        }
    }
}
