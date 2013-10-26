using System;
using System.Collections.Generic;
using Khylang.Utils;

namespace Khylang.CsParsec
{
    public static class Parsers
    {
        /// <summary>
        /// Parser that always fails with message error
        /// </summary>
        public static GenParser<T> Fail<T>(string error)
        {
            return s => new ParseError(error).Right<ParseResult<T>, ParseError>();
        }

        /// <summary>
        /// Creates a parser that does not change state and returns a constant value
        /// </summary>
        public static GenParser<T> Return<T>(T value)
        {
            return state => new ParseResult<T>(state, value).Left<ParseResult<T>, ParseError>();
        }

        /// <summary>
        /// Creates a parser that tests with predicate, advancing the parser by it's return value, or returning errorMessage if predicate returned null
        /// </summary>
        public static GenParser<Unit> When(Func<ParseState, int?> predicate, string errorMessage)
        {
            return state =>
            {
                var result = predicate(state);
                if (result.HasValue)
                    return Return(Unit.Val)(state.Add(result.Value));
                return Fail<Unit>(errorMessage)(state);
            };
        }

        /// <summary>
        /// Creates a parser that consumes a constant string
        /// </summary>
        public static GenParser<string> String(string s)
        {
            return When(state => state.Index + s.Length <= state.S.Length && state.S.Substring(state.Index, s.Length) == s ? s.Length : (int?)null,
                        string.Format("Expected \"{0}\"", s)).Bind(state => Return(s));
        }

        /// <summary>
        /// Creates a parser that matches a word boundary
        /// </summary>
        public static GenParser<Unit> WordBoundary()
        {
            return When(state => state.Index == 0 ||
                                 state.Index == state.S.Length ||
                                 char.IsLetterOrDigit(state.S, state.Index - 1) != char.IsLetterOrDigit(state.S, state.Index)
                ? 0
                : (int?)null,
                "Expected word boundary");
        }

        /// <summary>
        /// Creates a parser that consumes a single whitespace character
        /// </summary>
        public static GenParser<Unit> WhitespaceChar()
        {
            return When(p => char.IsWhiteSpace(p.S, p.Index) ? 1 : (int?)null, "Expected whitespace");
        }

        /// <summary>
        /// Applies parser repeatedly until failure, collecting results in a list
        /// </summary>
        public static GenParser<List<T>> Many<T>(GenParser<T> parser)
        {
            return state => Ext.Fix<ParseState, ParseResult<List<T>>>((f, s) =>
            {
                var result = parser(s);
                if (result.IsRight)
                    return new ParseResult<List<T>>(s, new List<T>());
                var list = f(result.Left.State);
                list.Result.Insert(0, result.Left.Result);
                return list;
            })(state).Left<ParseResult<List<T>>, ParseError>();
        }

        /// <summary>
        /// Optimized version of "Many(WhitespaceChar()).Bind(s => Return(Unit.Val));"
        /// </summary>
        /// <returns></returns>
        public static GenParser<Unit> Whitespace()
        {
            return When(s =>
            {
                var idx = 0;
                while (idx + s.Index < s.S.Length && char.IsWhiteSpace(s.S, idx + s.Index))
                    idx++;
                return idx;
            }, null);
        }

        /// <summary>
        /// Takes two parsers and returns the result of the left
        /// </summary>
        public static GenParser<TLeft> CombineLeft<TLeft, TRight>(this GenParser<TLeft> left, GenParser<TRight> right)
        {
            return left.Bind(leftResult =>
                   right.Bind(rightResult =>
                   Return(leftResult)));
        }

        /// <summary>
        /// Takes two parsers and returns the result of the right
        /// </summary>
        public static GenParser<TRight> CombineRight<TLeft, TRight>(this GenParser<TLeft> left, GenParser<TRight> right)
        {
            return left.Bind(leftResult => right);
        }

        /// <summary>
        /// Takes two parsers and returns the result of the right
        /// </summary>
        public static GenParser<TOut> CombineWith<TLeft, TRight, TOut>(this GenParser<TLeft> left, GenParser<TRight> right, Func<TLeft, TRight, TOut> func)
        {
            return left.Bind(leftResult => right.Bind(rightResult => Return(func(leftResult, rightResult))));
        }

        /// <summary>
        /// Creates a parser similar to String, but requires a word boundary at the end
        /// </summary>
        public static GenParser<string> Identifier(string s)
        {
            return String(s).CombineLeft(WordBoundary());
        }
    }
}
