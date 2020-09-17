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