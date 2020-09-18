using System;
using System.Collections.Generic;

using DingDingDiceBot.CmdHelper.Tokens;
using DingDingDiceBot.CmdHelper.Tokens.BinaryOperators;

namespace DingDingDiceBot.CmdHelper
{
    /// <summary>
    /// 表示解析字符串的服务。
    /// </summary>
    public static class CommandParser
    {
        internal static string GetResults(string command)
        {
            int length = command.Length;
            unsafe
            {
                fixed (char* str = command)
                {
                    using (ParseContext context = ParseContext.Create(command, str, 0, length))
                    {
                        for (int currentPos = 0; currentPos < length; currentPos = context.Pos)
                        {
                            if (context.Full)
                            {
                                return " 无法处理：\n> 指令符号长度超限。";
                            }
                            if (context._lastTokenType == TokenType.Negative)
                            {
                                Int32Operand.Token.ReadToken(context);
                            }
                            else
                            {
                                CheckAllToken(command, length, context);
                                if (context._note != null)
                                {
                                    break;
                                }
                            }
                            if (context.Fail)
                            {
                                return context.FailReason;
                            }
                        }
                        if (!context.Finish())
                        {
                            return context.FailReason;
                        }

                        CalcResult result = context.Calc();
                        if (result != null)
                        {
                            if (!string.IsNullOrEmpty(context._note))
                            {
                                return string.Format("  **{0}：**\n> {1}=**{2}**", context._note, result.Text, result.Value);
                            }
                            else if (context._containsDice)
                            {
                                return string.Format(" 投骰结果：\n> {0} = **{1}**", result.Text, result.Value);
                            }
                            else
                            {
                                return string.Format(" 运算结果：\n> {0} = **{1}**", result.Text, result.Value);
                            }
                        }
                        return " 输入错误\n> 表达式不正确。输入 **.help** 或 **.h** 查看帮助，不区分大小写。";
                    }
                }
            }
        }

        private static readonly List<Token> s_tokens = new List<Token>(32)
        {
            WhiteSpace.Token,
            LeftParenthesis.Token,
            RightParenthesis.Token,
            Addition.Token,
            Subtraction.Token,
            Multiplication.Token,
            DivisionFloor.Token,
            DivisionCeiling.Token,
            RandomOperand.Token,
            Int32Operand.Token,
        };

        private static bool s_has_function = false;

        /// <summary>
        /// 将一个新的 <see cref="Token"/> 类型注册到匹配服务中。
        /// </summary>
        /// <param name="token">一个自定义的 <see cref="Token"/> 。</param>
        /// <exception cref="ArgumentNullException"> <paramref name="token"/> 是 <c>null</c> .</exception>
        public static void RegisterToken(Token token)
        {
            if (token == null)
            {
                throw new ArgumentNullException();
            }

            if (!s_has_function && token.Type == TokenType.Function)
            {
                s_tokens.Add(Comma.Token);
                s_has_function = true;
            }
            s_tokens.Add(token);
        }

        private static void CheckAllToken(string command, int length, ParseContext context)
        {
            int currentPos = context.Pos;
            int newPos = currentPos;

            for (int i = 0; i < s_tokens.Count; i++)
            {
                s_tokens[i].ReadToken(context);
                if (context.Fail)
                {
                    return;
                }
                newPos = context.Pos;
                if (currentPos != newPos)
                {
                    break;
                }
            }
            if (currentPos != newPos)
            {
                return;
            }
            if (context.Empty)
            {
                int left = length - currentPos;
                if (left >= 4)
                {
                    if (string.Compare(command, currentPos, ".h", 0, 4, true) == 0)
                    {
                        context.SetHelp();
                        return;
                    }
                }
                else if (left >= 2 && string.Compare(command, currentPos, ".help", 0, 2, true) == 0)
                {
                    context.SetHelp();
                    return;
                }
            }
            else
            {
                int num = currentPos;
                int num2 = length - num;
                context._note = command.Substring(num, num2).TrimEnd();
            }
            context.SetFail("未读取到有效指令。输入 **.help** 或 **.h** 查看帮助，不区分大小写。");
        }
    }
}
