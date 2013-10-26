using System;
using Khylang.Utils;

namespace Khylang.CsParsec
{
    public static class ParsecMonad
    {
        public static GenParser<TResult> Bind<TValue, TResult>(this GenParser<TValue> ths, Func<TValue, GenParser<TResult>> func)
        {
            return p => ths(p).Bind(s => func(s.Result)(s.State));
        }

        public static T RunParser<T>(this GenParser<T> parser, string s)
        {
            var result = TryRunParser(parser, s);
            if (result.IsRight)
                throw result.Right.Exception;
            return result.Left;
        }

        public static IEither<T, ParseError> TryRunParser<T>(this GenParser<T> parser, string s)
        {
            var result = parser(new ParseState(s, 0));
            if (result.IsRight)
                return result.Right.Right<T, ParseError>();
            var state = result.Left;
            if (state.State.Index != s.Length)
                return new ParseError("Expected end of file").Right<T, ParseError>();
            return state.Result.Left<T, ParseError>();
        }
    }
}