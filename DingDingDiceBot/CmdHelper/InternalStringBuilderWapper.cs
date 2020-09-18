using System;
using System.Text;

using Microsoft.Extensions.ObjectPool;

namespace DingDingDiceBot.CmdHelper
{
    internal class InternalStringBuilderWapper : IDisposable
    {
        private static readonly DefaultPooledObjectPolicy<InternalStringBuilderWapper> s_policy = new DefaultPooledObjectPolicy<InternalStringBuilderWapper>();
        private static readonly DefaultObjectPool<InternalStringBuilderWapper> s_pool = new DefaultObjectPool<InternalStringBuilderWapper>(s_policy);

        public StringBuilder SB { get; } = new StringBuilder(1024);

        public static InternalStringBuilderWapper Create() => s_pool.Get();

        public void Dispose()
        {
            SB.Clear();
            s_pool.Return(this);
        }
    }
}
