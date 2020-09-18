using System.Text;

using DingDingDiceBot.CmdHelper.Tokens;

namespace DingDingDiceBot.CmdHelper
{
    /// <summary>
    /// 表示一个函数。
    /// </summary>
    public abstract class FunctionToken : Token, IOperatorOrFunction
    {
        internal override TokenType Type => TokenType.Function;

        int IOperatorOrFunction.ParameterCount => ParameterCount;

        /// <summary>
        /// 所需的操作数的数量。
        /// </summary>
        public abstract int ParameterCount { get; }

        /// <summary>
        /// 函数名。用于获取 <see cref="Token"/>, 以及格式化输出。
        /// <para>不区分大小写。</para>
        /// </summary>
        public abstract string Name { get; }

        internal override unsafe void ReadToken(ParseContext context)
        {
            int length = Name.Length;
            if (string.Compare(context.Command, context.Pos, Name, 0, length, true) != 0)
            {
                return;
            }
            TokenType lastType = context._lastTokenType;
            if (lastType != TokenType.Begin && lastType != TokenType.BinaryOperator &&
                lastType != TokenType.Comma && lastType != TokenType.LeftParenthesis)
            {
                context.SetFail("错误的函数位置。");
            }
            context._lastTokenType = TokenType.Function;
            context.Pos += length;
            context.Push(this);
        }

        CalcResult IOperatorOrFunction.Calc(params CalcResult[] parameters)
        {
            long[] ls = new long[ParameterCount];

            using (var sbw = InternalStringBuilderWapper.Create())
            {
                StringBuilder sb = sbw.SB.Append(Name).Append('(');
                unsafe
                {
                    fixed (long* p = ls)
                    {
                        for (int i = 0; i < ParameterCount; i++)
                        {
                            p[i] = parameters[i].Value;
                            sb.Append(parameters[i].Text).Append(',');
                        }
                    }
                }
                sb.Remove(sb.Length - 1, 1).Append(')');
                return new CalcResult(CalcCore(ls), sb.ToString(), 0);
            }
        }

        /// <summary>
        /// 计算并返回运算结果。
        /// </summary>
        /// <param name="parameters">函数所需的参数。</param>
        /// <returns>函数的计算结果。</returns>
        protected abstract long CalcCore(params long[] parameters);
    }
}
