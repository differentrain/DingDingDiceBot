namespace DingDingDiceBot.CmdHelper.Tokens
{
    internal sealed class LeftParenthesis : Token
    {
        public static readonly LeftParenthesis Token = new LeftParenthesis();

        internal override TokenType Type => TokenType.LeftParenthesis;

        internal override unsafe void ReadToken(ParseContext context)
        {
            if (context.Str[context.Pos] != '(')
            {
                return;
            }
            TokenType lastToken = context._lastTokenType;
            if (lastToken == TokenType.Int32Operand || lastToken == TokenType.RandomOperand || lastToken == TokenType.RightParenthesis)
            {
                context.SetFail("错误的左括号位置。");
                return;
            }

            context._lastTokenType = Type;
            context.Push(Token);
            context.Pos++;
        }
    }
}
