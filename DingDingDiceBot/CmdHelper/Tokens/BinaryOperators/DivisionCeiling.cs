using System;

namespace DingDingDiceBot.CmdHelper.Tokens.BinaryOperators
{
    internal sealed class DivisionCeiling : BinaryOperator
    {
        public static readonly DivisionCeiling Token = new DivisionCeiling();

        public override int Precedence => PrecedenceOfMultiplicationAndDivision;

        internal override bool IsSubOrDiv => true;

        public override string Name => "\\";

        public override long CalcCore(long a, long b) => (long)Math.Ceiling((double)a / b);

        protected override int TryGetOperator(string command, int pos, int length, out BinaryOperator token)
        {
            unsafe
            {
                fixed (char* p = command)
                {
                    token = Token;
                    if (p[pos] != '\\')
                    {
                        return 0;
                    }
                    return 1;
                }
            }
        }
    }
}
