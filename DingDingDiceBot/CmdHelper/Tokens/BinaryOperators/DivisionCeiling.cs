using System;

namespace DingDingDiceBot.CmdHelper.Tokens.BinaryOperators
{
    internal sealed class DivisionCeiling : BinaryOperator
    {
        public static readonly DivisionCeiling Token = new DivisionCeiling();

        public override int Precedence => 10;

        internal override bool IsSubOrDiv => true;

        public override string Name => "\\";

        public override long CalcCore(long a, long b) => (long)Math.Ceiling((double)a / b);

        protected override unsafe int TryGetOperator(char* str, int pos, int length, out BinaryOperator token)
        {
            token = Token;
            if (str[pos] != '\\')
            {
                return 0;
            }
            return 1;
        }
    }
}
