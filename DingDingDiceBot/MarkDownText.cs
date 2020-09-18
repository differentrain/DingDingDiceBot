namespace DingDingDiceBot
{
    /// <summary>
    /// 表示在钉钉开发文档中定义的 <c>JSON OBJECT</c> "markdown".
    /// <para> 包含信息的标题和 <c>markdown</c> 格式正文。</para>
    /// </summary>
    /// <remarks>
    /// 详见 <see cref="ResponeJson"/> 。
    /// <para>更多资料可参考钉钉开发文档：</para>
    /// https://ding-doc.dingtalk.com/doc?spm=a2115p.8777639.0.0.205a4260i2g1Q8#/serverapi2/elzz1p
    /// </remarks>
    public sealed class MarkDownText
    {
        /// <summary>
        /// 初始化 <see cref="MarkDownText"/> 的新实例，并将 <see cref="text"/> 的值设为 <paramref name="t"/>。
        /// </summary>
        /// <param name="t">要赋予 <see cref="text"/> 的值。</param>
        internal MarkDownText(string t) => text = t;

#pragma warning disable IDE1006 // 命名样式

        /// <summary>
        /// 首屏会话透出的展示内容。
        /// </summary>
        /// <value>这里总是返回 "DiceKun", 也可修改为其他内容。</value>
        public string title => "DiceKun";

        /// <summary>
        /// markdown格式的消息内容。
        /// </summary>
        /// <value>这个值便是处理命令后返回的字符串。</value>
        public string text { get; }

#pragma warning restore IDE1006 // 命名样式
    }
}
