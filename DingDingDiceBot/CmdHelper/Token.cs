namespace DingDingDiceBot.CmdHelper
{
    internal abstract class Token
    {
        public abstract TokenType Type { get; }

        internal abstract void ReadToken(ParseContext context);
    }
}