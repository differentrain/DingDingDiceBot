namespace DingDingDiceBot.CmdHelper.Tokens
{
    internal class Comma : Token
    {
        public static readonly Comma Token = new Comma();

        internal override TokenType Type => TokenType.Comma;

        internal override unsafe void ReadToken(ParseContext context)
        {
            if (context.Str[context.Pos] != ',')
            {
                return;
            }
            TokenType lastType = context._lastTokenType;
            if (lastType != TokenType.Int32Operand &&
                lastType != TokenType.RandomOperand &&
                lastType != TokenType.RightParenthesis)
            {
                context.SetFail("错误的逗号。");
                return;
            }
            while (!context.StackEmpty && context.StackTop.Type != TokenType.LeftParenthesis)
            {
                context.Enqueue(context.Pop());
            }
            if (context.StackTop.Type != TokenType.LeftParenthesis)
            {
                context.SetFail("逗号位置错误，或不匹配的括号。");
                return;
            }
            context._lastTokenType = Type;
            ++context.Pos;
        }
    }
}
