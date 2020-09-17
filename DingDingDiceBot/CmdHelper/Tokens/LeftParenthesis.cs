namespace DingDingDiceBot.CmdHelper.Tokens
{
    internal sealed class LeftParenthesis : Token
    {
        public static readonly LeftParenthesis Token = new LeftParenthesis();

        public override TokenType Type => TokenType.LeftParenthesis;

        internal unsafe override void ReadToken(ParseContext context)
        {
            if (context.Str[context.Pos] != '(')
            {
                return;
            }
            context.LastTokenType = Type;
            context.Push(Token);
            context.Pos++;
        }
    }
}