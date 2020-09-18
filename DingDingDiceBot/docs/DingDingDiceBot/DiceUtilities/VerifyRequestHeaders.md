# DiceUtilities.VerifyRequestHeaders method

根据 `HTTP POST` 请求的 `HTTP HEADER` 验证网络请求是具有授权。

```csharp
public static bool VerifyRequestHeaders(IHeaderDictionary headers, string appSecret)
```

| parameter | description |
| --- | --- |
| headers | 请求的 `HTTP HEADER` 部分。 |
| appSecret | 由钉钉开放平台提供的 `App Secret` 。 |

## Return Value

如果验证通过则返回 `true`, 否则返回 `false` 。

## Remarks

当用户 `@` 目标机器人时，钉钉服务器会向开发者设置的服务器发送 `HTTP POST` 请求, 其 `HTTP HEADER` 为：

**Content-Type**

application/json; charset=utf-8 (本实现并未验证是否采用 `UTF-8` 编码，因为根据规范，通过 `JSON` 交换数据必须采用此编码。)

**timestamp**

消息发送的时间戳，单位是毫秒。此时间戳与服务器时间戳相差不应超过1小时，即3600000毫秒。

**sign**

签名值。计算方式为用 `App Secret` 作为Key，对 `timestamp` + "\n"(换行符) + `App Secret` 的拼接字符串进行 `HmacSHA256` 计算，然后进行 `Base64` 编码。

## See Also

* class [DiceUtilities](../DiceUtilities.md)
* namespace [DingDingDiceBot](../../DingDingDiceBot.md)

<!-- DO NOT EDIT: generated by xmldocmd for DingDingDiceBot.dll -->