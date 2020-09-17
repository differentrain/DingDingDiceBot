using System;
using System.Collections.Generic;
using System.Text;

namespace DingDingDiceBot.CmdHelper
{
	internal abstract class Token
    {
		public abstract TokenType Type { get; }

		internal abstract void ReadToken(ParseContext context);
	}
}
