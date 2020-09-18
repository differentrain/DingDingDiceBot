using System;

using DingDingDiceBot.CmdHelper.Tokens;

using Microsoft.Extensions.ObjectPool;

namespace DingDingDiceBot.CmdHelper
{
    /// <summary>
    /// 表示用于解析指令的上下文。
    /// </summary>
    internal sealed class ParseContext : IDisposable
    {
        private const int MAX_TOKEN_COUNT = 256;
        private const int MAX_BUFFER_SIZE = MAX_TOKEN_COUNT << 1;
        private const int STACK_MIN_INDEX = MAX_TOKEN_COUNT;

        private const string HELP_TEXT = " 查看帮助\n> ### 指令说明\n> **.h** 或 **.help** 查看帮助，不区分大小写。\n#### 指令注释：\n指令尾部的任何无法解析的字符都将被视为注释。\n#### 操作数类型：\n整数。操作数以及运算结果都是整数。\n#### 操作符类型：\n- **+** 加法\n- **-** 减法\n- **\\*** 乘法。\n- **/** 除法。向下取整。\n- **\\\\** 除法。向上取整。\n- **(** 以及 **)** 括号。例如 1\\*(2+3)\n- **d** 或 **D** 投掷指令，不区分大小写, 省略前缀默认为1。例如 1d20 可以写作 d20。骰子数量上限为 100 ，骰子可选面数为 2，4，6，8，10，12，20，100。";

        private static readonly DefaultPooledObjectPolicy<ParseContext> s_policy = new DefaultPooledObjectPolicy<ParseContext>();
        private static readonly DefaultObjectPool<ParseContext> s_pool = new DefaultObjectPool<ParseContext>(s_policy);

        private readonly Token[] _buffer = new Token[MAX_BUFFER_SIZE];

        private int _queueIndex;
        private int _stackIndex = 256;

        internal bool _containsDice;

        internal string _note;

        /// <summary>
        /// 当前字符的坐标。
        /// </summary>
        public int Pos;

        /// <summary>
        /// 获取要进行解析的字符串。
        /// </summary>
        public unsafe char* Str { get; private set; }

        /// <summary>
        /// 获取要解析的字符串的长度。
        /// </summary>
        public int Length { get; private set; }

        public string Command { get; private set; }

        /// <summary>
        /// 上一个匹配到的符号类型。
        /// </summary>
        internal TokenType _lastTokenType;

        /// <summary>
        /// 获取一个值，表示是否发生了匹配错误。
        /// </summary>
        internal bool Fail { get; private set; }

        /// <summary>
        /// 获取一个值，表示错误原因。
        /// </summary>
        internal string FailReason { get; private set; }

        /// <summary>
        /// 获取一个值，表示匹配缓冲区是否已满。
        /// </summary>
        internal bool Full => _queueIndex + _stackIndex - STACK_MIN_INDEX >= MAX_TOKEN_COUNT;

        /// <summary>
        /// 获取一个值，表示匹配缓冲区是否为空。
        /// </summary>
        internal bool Empty => _queueIndex == 0 && StackEmpty;

        /// <summary>
        /// 获取一个值，表示匹配缓冲区的栈区域是否为空。
        /// </summary>
        internal bool StackEmpty => _stackIndex == STACK_MIN_INDEX;

        /// <summary>
        /// 获取一个值，表示匹配缓冲区的栈区域的对象数。
        /// </summary>
        internal int StackCount => _stackIndex - STACK_MIN_INDEX;

        /// <summary>
        /// 获取匹配缓冲区的栈区域最顶部的对象。
        /// </summary>
        internal Token StackTop => _buffer[_stackIndex - 1];

        /// <summary>
        ///  获取匹配缓冲区的队列区域最底部的对象。
        /// </summary>
        internal Token QueueEnd => _buffer[_queueIndex - 1];

        /// <summary>
        /// 将对象压入配缓冲区的栈区域。
        /// </summary>
        /// <param name="token">要压入栈区域的<see cref="Token"/></param>
        internal void Push(Token token) => _buffer[_stackIndex++] = token;

        /// <summary>
        /// 将对象弹出配缓冲区的栈区域。
        /// </summary>
        /// <returns></returns>
        internal Token Pop() => _buffer[--_stackIndex];

        /// <summary>
        /// 将对象加入配缓冲区的队列区域。
        /// </summary>
        /// <param name="token"></param>
        internal void Enqueue(Token token) => _buffer[_queueIndex++] = token;

        /// <summary>
        /// 将配缓冲区的队列区域的最后一个数据替换为目标对象。
        /// </summary>
        /// <param name="token"></param>
        internal void Replace(Token token) => _buffer[_queueIndex - 1] = token;

        internal void SetFail(string reason)
        {
            Fail = true;
            FailReason = " 输入错误：\n> " + reason;
        }

        internal void SetHelp()
        {
            Fail = true;
            FailReason = HELP_TEXT;
        }

        internal bool Finish()
        {
            if (!StackEmpty)
            {
                Token top = Pop();
                if (top.Type == TokenType.LeftParenthesis)
                {
                    SetFail("不匹配的括号。");
                    return false;
                }
                Enqueue(top);
                while (!StackEmpty)
                {
                    Enqueue(Pop());
                }
            }
            return true;
        }

        internal CalcResult Calc()
        {
            for (int i = 0; i < _queueIndex; i++)
            {
                Token t = _buffer[i];
                if (t.Type == TokenType.Int32Operand || t.Type == TokenType.RandomOperand)
                {
                    Push((t as IResultConverter).ToCalcResult());
                }
                else
                {
                    var op = t as IOperatorOrFunction;
                    int pCount = op.ParameterCount;
                    if (StackCount < pCount)
                    {
                        return null;
                    }
                    var ps = new CalcResult[pCount--];

                    while (0 <= pCount)
                    {
                        ps[pCount] = Pop() as CalcResult;
                        pCount--;
                    }
                    Push((t as IOperatorOrFunction).Calc(ps));
                }
            }
            if (StackCount != 1)
            {
                return null;
            }
            return Pop() as CalcResult;
        }

        public static unsafe ParseContext Create(string command, char* str, int pos, int length)
        {
            ParseContext parseContext = s_pool.Get();
            parseContext.Command = command;
            parseContext.Str = str;
            parseContext.Pos = pos;
            parseContext.Length = length;
            parseContext._queueIndex = 0;
            parseContext._stackIndex = STACK_MIN_INDEX;
            parseContext._lastTokenType = TokenType.Begin;
            parseContext._note = (parseContext.FailReason = null);
            parseContext.Fail = (parseContext._containsDice = false);
            return parseContext;
        }

        /// <inheritdoc/>
        public void Dispose() => s_pool.Return(this);
    }
}
