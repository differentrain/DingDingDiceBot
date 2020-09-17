using System;
using System.Collections.Generic;
using System.Text;

namespace DingDingDiceBot
{
	public sealed class ResponeJson
	{
		internal ResponeJson(string t)=> markdown = new MarkDownText(t);

#pragma warning disable IDE1006 // 命名样式
        public string msgtype=> "markdown";
        public MarkDownText markdown { get; }
#pragma warning restore IDE1006 // 命名样式
	}
}
