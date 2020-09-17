using DingDingDiceBot.CmdHelper;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace DingDingDiceBot
{
	public static class DiceUtilities
	{
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
			return !string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(s) && VerifyRequestHeaders(timestamp, sign, appSecret);
		}

		public static bool VerifyRequestHeaders(string timestamp, string sign, string appSecret)
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
