using System;

namespace Khylang.Utils
{
    interface IMonad<TMonad, out TContained> where TMonad : IMonad<TMonad, TContained>
    {
        TMonad Bind(Func<TContained, TMonad> action);
    }

    static class Monad
    {
        public static TMonad Return<TMonad, TContained>(TContained value) where TMonad : IMonad<TMonad, TContained>
        {
            return (TMonad)Activator.CreateInstance(typeof(TMonad), value);
        }
    }
}
