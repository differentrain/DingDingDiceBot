using System;
using System.Collections.Generic;
using System.Text;

namespace DingDingDiceBot.CmdHelper.Tokens
{
	internal sealed class RightParenthesis : Token
	{
		public static readonly RightParenthesis Token = new RightParenthesis();

		public override TokenType Type=> TokenType.RightParenthesis;

		internal unsafe override void ReadToken(ParseContext context)
		{
			if (context.Str[context.Pos] != ')')
			{
				return;
			}
			if (context.LastTokenType == TokenType.LeftParenthesis)
			{
				context.SetFail("空括号。");
				return;
			}
			while (!context.StackEmpty)
			{
				TokenType topType = context.StackTop.Type;
				if (topType == TokenType.LeftParenthesis)
				{
					context.LastTokenType = Type;
					context.Pop();
					context.Pos++;
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
