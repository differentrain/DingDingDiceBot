using System;
using System.Security.Cryptography;
using System.Text;

using DingDingDiceBot.CmdHelper;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace DingDingDiceBot
{
    /// <summary>
    /// 一个静态工具类，提供利用钉钉企业私有机器人进行投骰的业务支持。
    /// </summary>
    /// <example>
    /// 这里展示了如何利用此工具类处理来自钉钉的用户聊天记录。
    /// <code>
    /// using Microsoft.AspNetCore.Http;
    /// using Microsoft.AspNetCore.Mvc;
    ///
    /// using System.Net.Mime;
    /// using System.Threading.Tasks;
    ///
    /// namespace Sample
    /// {
    ///     [ApiController]
    ///     [Route("[controller]")]
    ///     public class Roll : ControllerBase
    ///     {
    ///         [HttpPost]
    ///         [Consumes(MediaTypeNames.Application.Json)]
    ///         [ProducesResponseType(StatusCodes.Status200OK)]
    ///         [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    ///         public async Task&lt;ActionResult&lt;DingDingDiceBot.ResponeJson>> PostAsync(DingDingDiceBot.RequestJson body)
    ///         {
    ///             // 验证请求是否来自钉钉。
    ///             // DingDingDiceBot.DiceUtilities.VerifyRequestHeaders(IHeaderDictionary, string) 方法提供了验证的实现。
    ///             if (!DingDingDiceBot.DiceUtilities.VerifyRequestHeaders(HttpContext.Request.Headers, APP_SECRET))
    ///             {
    ///                 return Unauthorized();
    ///             }
    ///             // 直接读取请求的 BODY ，并返回计算结果。
    ///             return await Task.FromResult(DingDingDiceBot.DiceUtilities.GetResponeFromRequest(body));
    ///         }
    ///         // 由钉钉开放平台分配密钥。
    ///         private const string APP_SECRET = "XXXXXXXXXXXX";
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <remarks>
    /// 关于钉钉私有机器人的工作方式和部署方法详见钉钉开发文档：
    /// <para>https://ding-doc.dingtalk.com/doc?spm=a2115p.8777639.0.0.205a4260i2g1Q8#/serverapi2/elzz1p</para>
    /// <para>此静态类的所有成员都是线程安全的。</para>
    /// </remarks>
    public static class DiceUtilities
    {
        /// <summary>
        /// 根据 <c>HTTP POST</c> 请求的 <c>HTTP HEADER</c> 验证网络请求是具有授权。
        /// </summary>
        /// <param name="headers">请求的 <c>HTTP HEADER</c> 部分。</param>
        /// <param name="appSecret">由钉钉开放平台提供的 <c>App Secret</c> 。</param>
        /// <returns>如果验证通过则返回 <c>true</c>, 否则返回 <c>false</c> 。 </returns>
        /// <remarks>
        /// 当用户 <c>@</c> 目标机器人时，钉钉服务器会向开发者设置的服务器发送 <c>HTTP POST</c> 请求, 其 <c>HTTP HEADER</c> 为：
        /// <list type = "table">
        /// <item>
        /// <term>Content-Type</term>
        /// <description>application/json; charset=utf-8
        /// (本实现并未验证是否采用 <c>UTF-8</c> 编码，因为根据规范，通过 <c>JSON</c> 交换数据必须采用此编码。)</description>
        /// </item>
        /// <item>
        /// <term>timestamp</term>
        /// <description>消息发送的时间戳，单位是毫秒。此时间戳与服务器时间戳相差不应超过1小时，即3600000毫秒。</description>
        /// </item>
        /// <item>
        /// <term>sign</term>
        /// <description>签名值。计算方式为用 <c>App Secret</c> 作为Key，对 <c>timestamp</c> + "\n"(换行符) + <c>App Secret</c> 的拼接字符串进行
        /// <c>HmacSHA256</c> 计算，然后进行 <c>Base64</c> 编码。
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        public static bool VerifyRequestHeaders(IHeaderDictionary headers, string appSecret)
        {
            if (!headers.TryGetValue("Content-Type", out StringValues contentType) ||
                !headers.TryGetValue("timestamp", out StringValues timestamp) ||
                !headers.TryGetValue("sign", out StringValues sign))
            {
                return false;
            }
            if (!contentType.ToString().Contains("application/json"))
            {
                return false;
            }
            string value = timestamp.ToString();
            string s = sign.ToString();
            return !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(s) && VerifyAuthorizedAccess(timestamp, sign, appSecret);
        }

        /// <summary>
        /// 根据 <paramref name="timestamp"/>、<paramref name="sign"/> 以及目标 <paramref name="appSecret"/> 验证请求是否有权使用本服务。
        /// </summary>
        /// <param name="timestamp">消息发送的时间戳，单位是毫秒。此时间戳与服务器时间戳相差不应超过1小时，即3600000毫秒。</param>
        /// <param name="sign">签名值。计算方式为用 <paramref name="appSecret"/> 作为Key，对 <paramref name="timestamp"/> + "\n"(换行符) + <paramref name="appSecret"/> 的拼接字符串进行
        /// <see cref="HMACSHA256"/> 计算，然后进行 <c>Base64</c> 编码。</param>
        /// <param name="appSecret">由钉钉开放平台提供的 <c>App Secret</c> 。</param>
        /// <returns>如果验证通过则返回 <c>true</c>, 否则返回 <c>false</c> 。 </returns>
        /// <seealso cref="VerifyRequestHeaders(IHeaderDictionary, string)"/>
        public static bool VerifyAuthorizedAccess(string timestamp, string sign, string appSecret)
        {
            long nowTs = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (!long.TryParse(timestamp, out long oldTs) || Math.Abs(nowTs - oldTs) > 3600000L)
            {
                return false;
            }
            bool result;
            using (HMACSHA256 hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret)))
            {
                byte[] hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(timestamp + "\n" + appSecret));
                result = sign.Equals(Convert.ToBase64String(hash));
            }
            return result;
        }

        /// <summary>
        /// 解析 <see cref="RequestJson"/> 中的用户消息，并以 <see cref="ResponeJson"/> 的形式返回处理结果。
        /// </summary>
        /// <param name="json">要处理的请求。包括发送者的昵称以及命令。</param>
        /// <returns>返回 一个 <see cref="ResponeJson"/>，表示对于命令的处理结果。</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="json"/> 是 <c>null</c>。
        ///  - 或 -
        ///  <see cref="RequestJson.senderNick"/> 是 <see cref="string.Empty"/> 或 <c>null</c>。
        ///  - 或 -
        ///  <see cref="RequestJson.text"/> 是 <see cref="string.Empty"/> 或 <c>null</c>。
        /// </exception>
        public static ResponeJson GetResponeFromRequest(RequestJson json)
        {
            if (!string.IsNullOrEmpty(json?.senderNick))
            {
                TextContent text = json.text;
                if (!string.IsNullOrEmpty(text?.content))
                {
                    return new ResponeJson(json.senderNick + CommandParser.GetResults(json.text.content));
                }
            }
            throw new ArgumentException();
        }

        /// <summary>
        /// 对指定的字符串进行指令解析，并返回一个表示处理后结果的字符串。
        /// </summary>
        /// <param name="command"></param>
        /// <returns>返回 <see cref="string"/> 字符串，表示对于命令的处理结果。</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="command"/> 是 <see cref="string.Empty"/> 或 <c>null</c>。</exception>
        public static string GetResultFromCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                throw new ArgumentException();
            }
            return CommandParser.GetResults(command);
        }
    }
}
