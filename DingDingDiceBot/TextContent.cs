using System.ComponentModel.DataAnnotations;

namespace DingDingDiceBot
{
    /// <summary>
    /// 表示请求的文本内容。
    /// </summary>
    public class TextContent
    {
        /// <summary>
        /// 用户发来的纯文本格式消息正文。
        /// </summary>
        [Required]
#pragma warning disable IDE1006 // 命名样式
        public string content { get; set; }

#pragma warning restore IDE1006 // 命名样式
    }
}
