using System;
using System.Buffers;

namespace DingDingDiceBot.CmdHelper
{
    internal class InternalCalcsResultParametersPool:IDisposable
    {
        private static readonly ArrayPool<CalcResult> s_pool = ArrayPool<CalcResult>.Create(ParseContext.MAX_TOKEN_COUNT, 16);

        public InternalCalcsResultParametersPool(int parametersCount)
        {
            CalcResults = s_pool.Rent(parametersCount);
        }

        public CalcResult[] CalcResults { get; }

        public void Dispose()
        {
            s_pool.Return(CalcResults);
        }
    }
}
