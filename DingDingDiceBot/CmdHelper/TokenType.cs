using System;
using System.Collections.Generic;
using System.Text;

namespace DingDingDiceBot.CmdHelper
{
    internal enum TokenType
    {
        Begin,
        Int32Operand,
        RandomOperand,
        BinaryOperator,
        LeftParenthesis,
        RightParenthesis,
        Negative,
        WhiteSpace,
        Result
    }
}
