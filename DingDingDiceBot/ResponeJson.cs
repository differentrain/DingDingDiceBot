namespace DingDingDiceBot
{
    /// <summary>
    /// 一个类，表示返回的 <c>JSON</c> 结构。
    /// </summary>
    /// <remarks>
    /// 钉钉开发文档中为此结构定义了更多的字段，以及 <see cref="ResponeJson.msgtype"/> 类型。
    /// <para>https://ding-doc.dingtalk.com/doc?spm=a2115p.8777639.0.0.205a4260i2g1Q8#/serverapi2/elzz1p</para>
    /// </remarks>
    public sealed class ResponeJson
    {
        /// <summary>
        /// 初始化 <see cref="ResponeJson"/> 的新实例，并将 <see cref="markdown"/> 属性的 <see cref="MarkDownText.text"/> 的值设为 <paramref name="t"/>。
        /// </summary>
        /// <param name="t"></param>
        internal ResponeJson(string t) => markdown = new MarkDownText(t);

#pragma warning disable IDE1006 // 命名样式

        /// <summary>
        /// 获取一个值，表示返回给调用方的数据类型。
        /// </summary>
        /// <value>这里总是返回 "markdown" 。</value>
        public string msgtype => "markdown";

        /// <summary>
        /// 获取一个值，包含信息的标题和 <c>markdown</c> 格式正文。
        /// </summary>
        public MarkDownText markdown { get; }

#pragma warning restore IDE1006 // 命名样式
    }
}
