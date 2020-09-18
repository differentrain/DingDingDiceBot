# DingDingDiceBot
钉钉私有机器人帮助类, 用于网团投骰。

整体代码不到1000行，可执行代码不到400行，可以方便地添加到任何基于 .NET (Core) 的Web服务中。

关于钉钉企业私有机器人的说明和配置详见 [钉钉开发文档](https://ding-doc.dingtalk.com/doc?spm=a2115p.8777639.0.0.205a4260i2g1Q8#/serverapi2/elzz1p)。

## 使用方法

接收钉钉服务器的`POST`请求：

```
[HttpPost]
[Consumes(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public async Task<ActionResult<DingDingDiceBot.ResponeJson>> PostAsync(DingDingDiceBot.RequestJson body)
{
    // 验证请求是否来自钉钉。
    // DingDingDiceBot.DiceUtilities.VerifyRequestHeaders(IHeaderDictionary, string) 方法提供了验证的实现。
    // APP_SECRET 是钉钉开放平台分配密钥。
    if (!DingDingDiceBot.DiceUtilities.VerifyRequestHeaders(HttpContext.Request.Headers, APP_SECRET))
    {
        return Unauthorized();
    }
    // 直接读取请求的 BODY ，并返回计算结果。
    return await Task.FromResult(DingDingDiceBot.DiceUtilities.GetResponeFromRequest(body));
}
```
考虑到书写习惯的不同，`DiceUtilities`静态类提供了其他两个方法用于验证和计算。

> sign的计算方法：
> 
> header中的timestamp + "\n" + 机器人的appSecret 当做签名字符串，使用HmacSHA256算法计算签名，然后进行Base64 encode，得到最终的签名值。

`DingDingDiceBot.DiceUtilities.VerifyAuthorizedAccess(string, string, string)` 方法直接接收调用者提供的字符串参数 `timestamp` 和 `sign` 进行验证。

`DingDingDiceBot.DiceUtilities.GetResultFromCommand(string)` 方法直接处理调用者提供的用户输入内容，生成最终的`markdown`计算结果。

## 计算与输出规则

支持整数操作数，其运算结果也是整数。

解析器会优先匹配有效操作符和运算符，遇到无法解析的内容，则其后的内容均作为注释处理。

目前程序会自动输出`markdown`风格的文字结果，如果需要自定义，可以修改内部`CmdHelper`命名空间下的相关类。

### 有效运算符
- 加法：`+`
- 减法：`-`
- 乘法：`*`
- 除法：向下取整 `/` ，向上取整 `\`
- 投骰：`d\` 或 `D`，例如 `1d20` 。 省略前缀则表示前缀为`1`，例如 `d20` 是 `1d20`的简写。
- 分组：用 `(` 和 `)` 包围的内容优先计算。

按照C#的运算符优先级进行计算。

## 扩展二元运算符或以及增加函数功能

 详见 [项目文档](https://github.com/differentrain/DingDingDiceBot/blob/master/DingDingDiceBot/docs/DingDingDiceBot.md) 中的 [Token 类](https://github.com/differentrain/DingDingDiceBot/blob/master/DingDingDiceBot/docs/DingDingDiceBot.CmdHelper/Token.md) 。
 
## 运行效果
![效果图](https://raw.githubusercontent.com/differentrain/DingDingDiceBot/master/img.png)
