namespace DingDingDiceBot.CmdHelper.Tokens
{
    internal sealed class RightParenthesis : Token
    {
        public static readonly RightParenthesis Token = new RightParenthesis();

        internal override TokenType Type => TokenType.RightParenthesis;

        internal override unsafe void ReadToken(ParseContext context)
        {
            if (context.Str[context.Pos] != ')')
            {
                return;
            }
            TokenType lastToken = context._lastTokenType;
            if (lastToken == TokenType.LeftParenthesis)
            {
                context.SetFail("空括号。");
                return;
            }

            if (lastToken != TokenType.RandomOperand && lastToken != TokenType.Int32Operand && lastToken != TokenType.RightParenthesis)
            {
                context.SetFail("错误的右括号。");
                return;
            }

            while (!context.StackEmpty)
            {
                TokenType topType = context.StackTop.Type;
                if (topType == TokenType.LeftParenthesis)
                {
                    context._lastTokenType = Type;
                    context.Pop();
                    context.Pos++;
                    if (!context.StackEmpty && context.StackTop.Type == TokenType.Function)
                    {
                        context.Enqueue(context.Pop());
                    }
                    return;
                }
                if (topType != TokenType.BinaryOperator)
                {
                    break;
                }
                context.Enqueue(context.Pop());
            }
            context.SetFail("不匹配的括号。");
        }
    }
}
