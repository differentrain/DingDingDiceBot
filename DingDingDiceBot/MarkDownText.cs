using System;
using System.Collections.Generic;
using System.Text;

namespace DingDingDiceBot
{
	public sealed class MarkDownText
	{
		internal MarkDownText(string t)=> text = t;

#pragma warning disable IDE1006 // 命名样式
		public string title=> "DiceKun";
        public string text { get; }
#pragma warning restore IDE1006 // 命名样式
    }
}
