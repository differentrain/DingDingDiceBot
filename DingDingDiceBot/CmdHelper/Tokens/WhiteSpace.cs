using System;
using System.Collections.Generic;
using System.Text;

namespace DingDingDiceBot.CmdHelper.Tokens
{
    internal sealed class WhiteSpace : Token
    {
        public static readonly WhiteSpace Token = new WhiteSpace();

        public override TokenType Type => TokenType.WhiteSpace;

        internal unsafe override void ReadToken(ParseContext context)
        {
            while (context.Pos < context.Length && char.IsWhiteSpace(context.Str[context.Pos]))
            {
                context.Pos++;
            }
        }
    }
}
