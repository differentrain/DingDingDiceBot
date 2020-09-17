using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DingDingDiceBot
{
    public class TextContent
    {
        [Required]
#pragma warning disable IDE1006 // 命名样式
        public string content { get; set; }
#pragma warning restore IDE1006 // 命名样式
    }
}
