using System;
using Khylang.Utils;

namespace Khylang.CsParsec
{
    public static class ParsecMonad
    {
        public static GenParser<TState, TResult> Bind<TState, TValue, TResult>(this GenParser<TState, TValue> ths, Func<TValue, GenParser<TState, TResult>> func)
        {
            return p => ths(p).Bind(s => func(s.Result)(s.State));
        }

        public static T RunParser<TState, T>(this GenParser<TState, T> parser, string s, TState state)
        {
            var result = TryRunParser(parser, s, state);
            if (result.IsRight)
                throw result.Right.Exception;
            return result.Left;
        }

        public static IEither<T, ParseError<TState>> TryRunParser<TState, T>(this GenParser<TState, T> parser, string s, TState state)
        {
            var result = parser(new ParseState<TState>(s, 0, state));
            if (result.IsRight)
                return result.Right.Right<T, ParseError<TState>>();
            var resultState = result.Left;
            if (resultState.State.Index != s.Length)
                return new ParseError<TState>("Expected end of file", resultState.State).Right<T, ParseError<TState>>();
            return resultState.Result.Left<T, ParseError<TState>>();
        }
    }
}