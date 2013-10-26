using System;

namespace Khylang.Utils
{
    public static class Ext
    {
        public static Func<TIn, TOut> Fix<TIn, TOut>(Func<Func<TIn, TOut>, TIn, TOut> action)
        {
            return p => action(Fix(action), p);
        }
    }
}