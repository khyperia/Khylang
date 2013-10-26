using System;

namespace Khylang.Utils
{
    // Similar to Nullable<T>, only without the struct requirement
    public struct Maybe<T> : IMonad<Maybe<T>, T>
    {
        private readonly T _value;
        private readonly bool _hasValue;

        private Maybe(T value, bool hasValue)
        {
            _value = value;
            _hasValue = hasValue;
        }

        public static Maybe<T> Just(T value)
        {
            if (value == null)
                return None;
            return new Maybe<T>(value, true);
        }

        public static Maybe<T> None
        {
            get { return new Maybe<T>(default(T), false); }
        }

        public T Extract()
        {
            if (!_hasValue)
                throw new NullReferenceException();
            return _value;
        }

        // Monad implementation
        public Maybe<T> Bind(Func<T, Maybe<T>> action)
        {
            if (_hasValue)
                return action(_value);
            return this; // cache None value
        }

        public Maybe(T value)
        {
            _value = value;
            _hasValue = true;
        }
    }

    public static class MaybeExt
    {
        public static Maybe<T> Just<T>(this T value)
        {
            return Maybe<T>.Just(value);
        }
    }
}
