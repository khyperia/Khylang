﻿using System;

namespace Khylang.Utils
{
    public interface IEither<out TLeft, out TRight>
    {
        bool IsLeft { get; }
        bool IsRight { get; }
        TLeft Left { get; }
        TRight Right { get; }
    }

    public static class EitherExt
    {
        public static IEither<TLeft, TRight> Left<TLeft, TRight>(this TLeft value)
        {
            return new LeftEither<TLeft, TRight>(value);
        }

        public static IEither<TLeft, TRight> Right<TLeft, TRight>(this TRight value)
        {
            return new RightEither<TLeft, TRight>(value);
        }

        public static IEither<TLeftOut, TRight> Bind<TLeftIn, TLeftOut, TRight>(this IEither<TLeftIn, TRight> value, Func<TLeftIn, IEither<TLeftOut, TRight>> func)
        {
            return value.IsLeft ? func(value.Left) : new RightEither<TLeftOut, TRight>(value.Right);
        }
    }

    public class WrongEitherException : Exception
    {
        public WrongEitherException() : base("Incorrect Either")
        {
        }
    }

    public struct LeftEither<TLeft, TRight> : IEither<TLeft, TRight>
    {
        private readonly TLeft _value;

        public LeftEither(TLeft value)
        {
            _value = value;
        }

        public bool IsLeft { get { return true; } }
        public bool IsRight { get { return false; } }
        public TLeft Left { get { return _value; } }
        public TRight Right { get { throw new WrongEitherException(); } }
    }

    public struct RightEither<TLeft, TRight> : IEither<TLeft, TRight>
    {
        private readonly TRight _value;

        public RightEither(TRight value)
        {
            _value = value;
        }

        public bool IsLeft { get { return true; } }
        public bool IsRight { get { return false; } }
        public TLeft Left { get { throw new WrongEitherException(); } }
        public TRight Right { get { return _value; } }
    }
}
