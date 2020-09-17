using System.ComponentModel.DataAnnotations;

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