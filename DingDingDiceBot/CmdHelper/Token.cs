﻿namespace DingDingDiceBot.CmdHelper
{
    /// <summary>
    /// 表示解析字符串时匹配到的符号的基类。
    /// </summary>
    /// <remarks>
    /// 如果想要增加新的符号，不要直接从此处派生。
    /// <para>本项目目前仅支持添加以下内容：</para>
    /// <list type="bullet">
    /// <item>
    /// 二元运算符：从 <see cref="BinaryOperator"/> 处派生。
    /// </item>
    /// <item>
    /// 函数： 从 <see cref="FunctionToken"/> 处派生。
    /// </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// 示例：创建计算两数相除的余数的运算符以及函数。
    /// <code>
    /// using DingDingDiceBot.CmdHelper;
    ///
    /// namespace Sample
    /// {
    ///     class Program
    ///     {
    ///         static void Main(string[] args)
    ///         {
    ///             // 用 DingDingDiceBot.CmdHelper.CommandParser 方法注册有关类型。
    ///             CommandParser.RegisterToken(ModFunction.Token);
    ///             CommandParser.RegisterToken(ModOperator.Token);
    ///
    ///             //调用 DingDingDiceBot.DiceUtilities.GetResultFromCommand 方法计算字符串。
    ///             var result = DingDingDiceBot.DiceUtilities.GetResultFromCommand("10%4+mod(5,2)");
    ///
    ///             //输出 markdown 运算结果：
    ///             /* 运算结果：\n> 10%4+mod(5,2) = **3**   */
    ///
    ///         }
    ///
    ///         //用以计算余数的运算符。
    ///         public class ModOperator : BinaryOperator
    ///         {
    ///             //一个实例化的静态字段备用。
    ///             public static readonly ModOperator Token = new ModOperator();
    ///
    ///             // 常量 BinaryOperator.PrecedenceOfMultiplicationAndDivision 表示乘法和除法的优先级。
    ///             // 添加新的运算符时，它的 Precedence 值可以通过 BinaryOperator 类提供的常量来确定。
    ///             public override int Precedence => PrecedenceOfMultiplicationAndDivision;
    ///
    ///             //当解析器生成运算结果字符串时，会使用这个属性。
    ///             public override string Name => "%";
    ///
    ///             //实现运算方法。
    ///             public override long CalcCore(long a, long b) => a % b;
    ///
    ///             //检测字符串的当前位置是否是目标符号。
    ///             //如果是，则返回符号的长度，如果不是，则返回任意小于 1 的数字。
    ///             protected override int TryGetOperator(string command, int pos, int length, out BinaryOperator token)
    ///             {
    ///                 token = Token;
    ///                 if (command[pos] != '%')
    ///                 {
    ///                     return 0;
    ///                 }
    ///                 return 1;
    ///             }
    ///         }
    ///
    ///         //用以计算余数的函数。
    ///         public class ModFunction : FunctionToken
    ///         {
    ///             //一个实例化的静态字段备用。
    ///             public static readonly ModFunction Token = new ModFunction();
    ///
    ///             //所需的参数数量。
    ///             public override int ParameterCount => 2;
    ///
    ///             //函数名称，解析器会直接使用这个值进行解析。忽略大小写。
    ///             public override string Name => "mod";
    ///
    ///             protected override long CalcCore(params long[] parameters) => parameters[0] % parameters[1];
    ///
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class Token
    {
        internal abstract TokenType Type { get; }

        /// <summary>
        /// 实现符号读取的方法。
        /// </summary>
        /// <param name="context">用于解析的上下文。</param>
        internal abstract unsafe void ReadToken(ParseContext context);
    }
}
