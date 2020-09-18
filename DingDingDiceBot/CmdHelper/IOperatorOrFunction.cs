namespace DingDingDiceBot.CmdHelper
{
    /// <summary>
    /// 表示这是一个函数或运算符。
    /// </summary>
    internal interface IOperatorOrFunction
    {
        /// <summary>
        /// 所需的操作数的数量。
        /// </summary>
        int ParameterCount { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        CalcResult Calc(params CalcResult[] parameters);
    }
}
