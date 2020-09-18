using System;
using System.Buffers;

namespace DingDingDiceBot.CmdHelper
{
    internal struct InternalLongParametersPool : IDisposable
    {
        private static readonly ArrayPool<long> s_pool = ArrayPool<long>.Create(ParseContext.MAX_TOKEN_COUNT, 16);

        public InternalLongParametersPool(int parametersCount)
        {
            LongArray = s_pool.Rent(parametersCount);
        }

        public long[] LongArray { get; } 

        public void Dispose()
        {
            s_pool.Return(LongArray);
        }
    }
}
