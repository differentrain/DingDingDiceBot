namespace DingDingDiceBot.CmdHelper.Tokens
{
    internal sealed class WhiteSpace : Token
    {
        public static readonly WhiteSpace Token = new WhiteSpace();

        internal override TokenType Type => TokenType.WhiteSpace;

        internal override unsafe void ReadToken(ParseContext context)
        {
            while (context.Pos < context.Length && char.IsWhiteSpace(context.Str[context.Pos]))
            {
                context.Pos++;
            }
        }
    }
}
