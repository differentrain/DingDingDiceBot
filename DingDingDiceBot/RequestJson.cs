using System.ComponentModel.DataAnnotations;

namespace DingDingDiceBot
{
    /// <summary>
    /// 一个类，表示 <c>HTTP POST</c> 请求的 <c>JSON</c> 结构。
    /// </summary>
    /// <remarks>
    /// 当用户 <c>@</c> 目标机器人时，钉钉服务器会向开发者设置的服务器发送 <c>HTTP POST</c> 请求, 其 <c>BODY</c> 是一个 <c>JSON</c> 结构。
    /// <para>次类就是 <c>JSON</c> 结构的简化版，只要求最关键的信息。</para>
    /// 钉钉开发文档中为此结构定义了更多的字段：
    /// <para>https://ding-doc.dingtalk.com/doc?spm=a2115p.8777639.0.0.205a4260i2g1Q8#/serverapi2/elzz1p</para>
    /// </remarks>
    public sealed class RequestJson
    {
#pragma warning disable IDE1006 // 命名样式

        /// <summary>
        /// 发送消息的用户昵称。
        /// </summary>
        [Required]
        public string senderNick { get; set; }

        /// <summary>
        /// 获取一个值，存储着用户发送的消息文本。
        /// </summary>
        [Required]
        public TextContent text { get; set; }

#pragma warning restore IDE1006 // 命名样式
    }
}
