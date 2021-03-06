﻿namespace DingDingDiceBot.CmdHelper
{
    /// <summary>
    /// 表示一个二元运算符。
    /// </summary>
    /// <remarks>
    ///  通过继承 <see cref="BinaryOperator"/> 以添加二元运算符的例子详见 <see cref="Token"/> 类的说明
    /// </remarks>
    public abstract class BinaryOperator : Token, IOperatorOrFunction
    {
        internal override TokenType Type => TokenType.BinaryOperator;

        /// <summary>
        /// 表示乘法和除法运算符的优先级。
        /// </summary>
        public const int PrecedenceOfMultiplicationAndDivision = 10;

        /// <summary>
        /// 表示加法和减法运算符的优先级。
        /// </summary>
        public const int PrecedenceOfAdditionAndSubtraction = PrecedenceOfMultiplicationAndDivision + 1;

        /// <summary>
        /// 运算符的优先级。
        /// </summary>
        /// <remarks>
        /// 内置的乘法、除法运算符的优先级被设置为 10。<see cref="PrecedenceOfMultiplicationAndDivision"/> 。
        /// <para>加法和减法运算符的优先级被设置为 11。<see cref="PrecedenceOfAdditionAndSubtraction"/> 。</para>
        /// <para>如果要添加新的二元运算符，请根据这两个值确定新的运算符的优先级。</para>
        /// </remarks>
        public abstract int Precedence { get; }

        internal virtual bool IsSubOrDiv => false;

        /// <summary>
        /// 表示二元运算符的名称。它将被用作生成最后的输出算式。
        /// </summary>
        public abstract string Name { get; }

        /// <inheritdoc/>
        int IOperatorOrFunction.ParameterCount => 2;

        /// <inheritdoc/>
        internal override void ReadToken(ParseContext context)
        {
            unsafe
            {
                int tokenLength = TryGetOperator(context.Command, context.Pos, context.Length, out BinaryOperator token);
                if (tokenLength > 0)
                {
                    context.Pos += tokenLength;
                    TokenType tokenType = context._lastTokenType;

                    if (tokenType == TokenType.Begin ||
                        tokenType == TokenType.LeftParenthesis ||
                        tokenType == TokenType.BinaryOperator ||
                        tokenType == TokenType.Comma)
                    {
                        if (token.Name.Equals("-"))
                        {
                            context._lastTokenType = TokenType.Negative;
                            return;
                        }
                    }
                    else if (tokenType == TokenType.Function)
                    {
                        context.SetFail("错误的操作符或函数。");
                        return;
                    }
                    else
                    {
                        context._lastTokenType = Type;
                        UpdateBinaryOperatorNotFixPos(token, context);
                        return;
                    }
                    context.SetFail("操作符位置不正确。");
                }
            }
        }

        private static void UpdateBinaryOperatorNotFixPos(BinaryOperator token, ParseContext context)
        {
            while (!context.StackEmpty)
            {
                Token top = context.StackTop;
                if (top.Type != TokenType.BinaryOperator ||
                    token.Precedence < (top as BinaryOperator).Precedence)
                {
                    break;
                }
                context.Enqueue(context.Pop());
            }
            context.Push(token);
        }

        CalcResult IOperatorOrFunction.Calc(params CalcResult[] parameters)
        {
            CalcResult a = parameters[0];
            CalcResult b = parameters[1];
            if (a.Precedence > Precedence)
            {
                a.AddParenthesis();
            }
            if (b.Precedence > Precedence || (b.Precedence == Precedence && IsSubOrDiv))
            {
                b.AddParenthesis();
            }
            return new CalcResult(CalcCore(a.Value, b.Value), $"{a.Text}{Name}{b.Text}", Precedence);
        }

        /// <summary>
        /// 通过此运算符进行计算并返回结果。
        /// </summary>
        /// <param name="a">第一个操作数。</param>
        /// <param name="b">第二个操作数。</param>
        /// <returns>计算的结果</returns>
        public abstract long CalcCore(long a, long b);

        /// <summary>
        /// 检测字符串的当前位置是不是目标操作符。
        /// </summary>
        /// <param name="command">要检测的字符串。</param>
        /// <param name="pos">当前位置。</param>
        /// <param name="length">字符串长度。</param>
        /// <param name="token">成功则返回字符串所表示的 <see cref="Token"/> 。</param>
        /// <returns>如果是目标操作符，返回操作符的长度，否则返回一个小于1的数。</returns>
        protected abstract int TryGetOperator(string command, int pos, int length, out BinaryOperator token);
    }
}
