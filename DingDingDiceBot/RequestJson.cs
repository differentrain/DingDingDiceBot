using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DingDingDiceBot
{

	public sealed class RequestJson
	{
#pragma warning disable IDE1006 // 命名样式
		[Required]
		public string senderNick { get; set; }
		[Required]
        public TextContent text { get; set; }
#pragma warning restore IDE1006 // 命名样式
    }
}
