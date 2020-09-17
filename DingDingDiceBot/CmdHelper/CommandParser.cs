using DingDingDiceBot.CmdHelper.Tokens;
using DingDingDiceBot.CmdHelper.Tokens.BinaryOperators;

namespace DingDingDiceBot.CmdHelper
{
    internal static class CommandParser
    {
        public static string GetResults(string command)
        {
            int length = command.Length;
            unsafe
            {
                fixed (char* str = command)
                {
                    using (ParseContext context = ParseContext.Create(str, 0, length))
                    {
                        for (int currentPos = 0; currentPos < length; currentPos = context.Pos)
                        {
                            if (context.Full)
                            {
                                return " 无法处理：\n> 指令符号长度超限。";
                            }
                            if (context.LastTokenType == TokenType.Negative)
                            {
                                Int32Operand.Token.ReadToken(context);
                            }
                            else
                            {
                                CheckAllToken(command, length, context);
                                if (context.Note != null)
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
                            if (!string.IsNullOrEmpty(context.Note))
                            {
                                return string.Format("  **{0}：**\n> {1}=**{2}**", context.Note, result.Text, result.Value);
                            }
                            else if (context.ContainsDice)
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

        private static readonly Token[] _tokens = new Token[]
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
            Int32Operand.Token
        };

        private static void CheckAllToken(string command, int length, ParseContext context)
        {
            int currentPos = context.Pos;
            int newPos = currentPos;
            for (int i = 0; i < _tokens.Length; i++)
            {
                _tokens[i].ReadToken(context);
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
            if (currentPos == newPos)
            {
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
                    context.Note = command.Substring(num, num2).TrimEnd();
                }
                context.SetFail("未读取到有效指令。输入 **.help** 或 **.h** 查看帮助，不区分大小写。");
            }
        }
    }
}