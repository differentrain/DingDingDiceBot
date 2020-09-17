using DingDingDiceBot.CmdHelper.Tokens;

using Microsoft.Extensions.ObjectPool;

using System;

namespace DingDingDiceBot.CmdHelper
{
    internal sealed class ParseContext : IDisposable
    {
        private const int MAX_TOKEN_COUNT = 256;
        private const int MAX_BUFFER_SIZE = MAX_TOKEN_COUNT << 1;
        private const int STACK_MIN_INDEX = MAX_TOKEN_COUNT;

        private const string HELP_TEXT = " 查看帮助\n> ### 指令说明\n> **.h** 或 **.help** 查看帮助，不区分大小写。\n#### 指令注释：\n指令尾部的任何无法解析的字符都将被视为注释。\n#### 操作数类型：\n整数。操作数以及运算结果都是整数。\n#### 操作符类型：\n- **+** 加法\n- **-** 减法\n- **\\*** 乘法。\n- **/** 除法。向下取整。\n- **\\\\** 除法。向上取整。\n- **(** 以及 **)** 括号。例如 1\\*(2+3)\n- **d** 或 **D** 投掷指令，不区分大小写, 省略前缀默认为1。例如 1d20 可以写作 d20。骰子数量上限为 100 ，骰子可选面数为 2，4，6，8，10，12，20，100。";

        private static readonly DefaultPooledObjectPolicy<ParseContext> _policy = new DefaultPooledObjectPolicy<ParseContext>();
        private static readonly DefaultObjectPool<ParseContext> _pool = new DefaultObjectPool<ParseContext>(ParseContext._policy);

        private readonly Token[] _buffer = new Token[MAX_BUFFER_SIZE];

        public unsafe char* Str;
        public int Length;
        public int Pos;

        private int _queueIndex;
        private int _stackIndex = 256;

        public bool ContainsDice;
        public TokenType LastTokenType;
        public string Note;

        public bool Fail { get; private set; }
        public string FailReason { get; private set; }

        public bool Full => _queueIndex + _stackIndex - STACK_MIN_INDEX >= MAX_TOKEN_COUNT;
        public bool Empty => _queueIndex == 0 && StackEmpty;
        public bool StackEmpty => _stackIndex == STACK_MIN_INDEX;
        public int StackCount => _stackIndex - STACK_MIN_INDEX;

        public Token StackTop => _buffer[_stackIndex - 1];
        public Token QueueEnd => _buffer[_queueIndex - 1];

        public void Push(Token token) => _buffer[_stackIndex++] = token;

        public Token Pop() => _buffer[--_stackIndex];

        public void Enqueue(Token token) => _buffer[_queueIndex++] = token;

        public void Replace(Token token) => _buffer[_queueIndex - 1] = token;

        public void SetFail(string reason)
        {
            Fail = true;
            FailReason = " 输入错误：\n> " + reason;
        }

        public void SetHelp()
        {
            Fail = true;
            FailReason = HELP_TEXT;
        }

        public bool Finish()
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

        public CalcResult Calc()
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
                    if (StackCount < 2)
                    {
                        return null;
                    }
                    CalcResult b = Pop() as CalcResult;
                    CalcResult a = Pop() as CalcResult;
                    Push((t as BinaryOperator).Calc(a, b));
                }
            }
            if (StackCount != 1)
            {
                return null;
            }
            return Pop() as CalcResult;
        }

        public unsafe static ParseContext Create(char* str, int pos, int length)
        {
            ParseContext parseContext = _pool.Get();
            parseContext.Str = str;
            parseContext.Pos = pos;
            parseContext.Length = length;
            parseContext._queueIndex = 0;
            parseContext._stackIndex = STACK_MIN_INDEX;
            parseContext.LastTokenType = TokenType.Begin;
            parseContext.Note = (parseContext.FailReason = null);
            parseContext.Fail = (parseContext.ContainsDice = false);
            return parseContext;
        }

        public void Dispose() => _pool.Return(this);
    }
}