using System;
using System.Collections.Generic;
using System.Linq;
using Khylang.Utils;

namespace Khylang.CsParsec
{
    public static class Parsers
    {
        /// <summary>
        /// Parser that always fails with message error
        /// </summary>
        public static GenParser<TState, T> Fail<TState, T>(string error)
        {
            return s => new ParseError<TState>(error, s).Right<ParseResult<TState, T>, ParseError<TState>>();
        }

        /// <summary>
        /// Creates a parser that does not change state and returns a constant value
        /// </summary>
        public static GenParser<TState, T> Return<TState, T>(T value)
        {
            return state => new ParseResult<TState, T>(state, value).Left<ParseResult<TState, T>, ParseError<TState>>();
        }

        /// <summary>
        /// Modifies the result value with a function
        /// </summary>
        public static GenParser<TState, TOut> FMap<TState, TIn, TOut>(this GenParser<TState, TIn> parser, Func<TIn, TOut> func)
        {
            return parser.Bind(p => Return<TState, TOut>(func(p)));
        }

        public static GenParser<TState, T> ModifyState<T, TState>(this GenParser<TState, T> parser, Func<TState, T, TState> func)
        {
            return state => parser(state).Bind(r => new ParseResult<TState, T>(
                new ParseState<TState>(r.State.S, r.State.Index, func(r.State.State, r.Result)), r.Result)
                .Left<ParseResult<TState, T>, ParseError<TState>>());
        }

        /// <summary>
        /// Creates a parser that tests with predicate, advancing the parser by it's return value, or returning errorMessage if predicate returned null
        /// </summary>
        public static GenParser<TState, Unit> When<TState>(Func<ParseState<TState>, int?> predicate, string errorMessage)
        {
            return state =>
            {
                var result = predicate(state);
                if (result.HasValue)
                    return Return<TState, Unit>(Unit.Val)(state.Add(result.Value));
                return Fail<TState, Unit>(errorMessage)(state);
            };
        }

        /// <summary>
        /// Creates a parser that consumes a constant string
        /// </summary>
        public static GenParser<TState, string> String<TState>(string s)
        {
            return When<TState>(state => state.Index + s.Length <= state.S.Length && state.S.Substring(state.Index, s.Length) == s ? s.Length : (int?)null,
                        string.Format("Expected \"{0}\"", s)).Bind(state => Return<TState, string>(s));
        }

        /// <summary>
        /// Same as String, but consumes following whitespace
        /// </summary>
        public static GenParser<TState, string> StringSpaces<TState>(string s)
        {
            return String<TState>(s).CombineLeft(Whitespace<TState>());
        }

        /// <summary>
        /// Creates a parser that matches a word boundary
        /// </summary>
        public static GenParser<TState, Unit> WordBoundary<TState>()
        {
            return When<TState>(state => state.Index == 0 ||
                                 state.Index == state.S.Length ||
                                 char.IsLetterOrDigit(state.S, state.Index - 1) != char.IsLetterOrDigit(state.S, state.Index)
                ? 0
                : (int?)null,
                "Expected word boundary");
        }

        /// <summary>
        /// Applies parsers from left to right, returning the first successful one
        /// </summary>
        public static GenParser<TState, T> Any<TState, T>(params GenParser<TState, T>[] parsers)
        {
            return state =>
            {
                var errors = new List<ParseError<TState>>();
                foreach (var result in parsers.Select(parser => parser(state)))
                {
                    if (result.IsLeft)
                        return result;
                    errors.Add(result.Right);
                }
                return Fail<TState, T>(string.Join(Environment.NewLine, errors.Select(e => e.Error)))(state);
            };
        }

        /// <summary>
        /// Applies parser repeatedly until failure, collecting results in a list
        /// </summary>
        public static GenParser<TState, List<T>> Many<TState, T>(GenParser<TState, T> parser)
        {
            return state =>
            {
                var list = new List<T>();
                while (true)
                {
                    var result = parser(state);
                    if (result.IsRight)
                        break;
                    state = result.Left.State;
                    list.Add(result.Left.Result);
                }
                return new ParseResult<TState, List<T>>(state, list).Left<ParseResult<TState, List<T>>, ParseError<TState>>();
            };
        }

        /// <summary>
        /// Applies parser repeatedly until failure, collecting results in a list
        /// </summary>
        public static GenParser<TState, List<T>> Many1<TState, T>(GenParser<TState, T> parser)
        {
            return state => parser(state).Bind(initial =>
            {
                var list = new List<T> { initial.Result };
                state = initial.State;
                while (true)
                {
                    var result = parser(state);
                    if (result.IsRight)
                        break;
                    state = result.Left.State;
                    list.Add(result.Left.Result);
                }
                return new ParseResult<TState, List<T>>(state, list).Left<ParseResult<TState, List<T>>, ParseError<TState>>();
            });
        }

        /// <summary>
        /// Applies the first predicate, then repeatedly applies rest until failure
        /// </summary>
        public static GenParser<TState, string> ManyChar<TState>(Predicate<char> pred)
        {
            return ManyChar1<TState>(pred, pred);
        }

        /// <summary>
        /// Applies the first predicate, then repeatedly applies rest until failure
        /// </summary>
        public static GenParser<TState, string> ManyChar1<TState>(Predicate<char> initial, Predicate<char> rest)
        {
            return state =>
            {
                if (state.Index >= state.S.Length)
                    return Fail<TState, string>("Unexpected end-of-file")(state);
                if (!initial(state.Char))
                    return Fail<TState, string>("Unexpected character " + state.Char)(state);
                var idx = state.Index;
                do
                {
                    state = state.Add(1);
                } while (state.Index < state.S.Length && rest(state.Char));
                return new ParseResult<TState, string>(state, state.S.Substring(idx, state.Index - idx)).Left<ParseResult<TState, string>, ParseError<TState>>();
            };
        }

        /// <summary>
        /// Consumes all following whitepsace tokens
        /// </summary>
        /// <returns></returns>
        public static GenParser<TState, Unit> Whitespace<TState>()
        {
            return When<TState>(s =>
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
        public static GenParser<TState, TLeft> CombineLeft<TState, TLeft, TRight>(this GenParser<TState, TLeft> left, GenParser<TState, TRight> right)
        {
            return left.Bind(leftResult =>
                   right.Bind(rightResult =>
                   Return<TState, TLeft>(leftResult)));
        }

        /// <summary>
        /// Takes two parsers and returns the result of the right
        /// </summary>
        public static GenParser<TState, TRight> CombineRight<TState, TLeft, TRight>(this GenParser<TState, TLeft> left, GenParser<TState, TRight> right)
        {
            return left.Bind(leftResult => right);
        }

        /// <summary>
        /// Takes two parsers and returns the result of the right
        /// </summary>
        public static GenParser<TState, TOut> CombineWith<TState, TLeft, TRight, TOut>(this GenParser<TState, TLeft> left, GenParser<TState, TRight> right, Func<TLeft, TRight, TOut> func)
        {
            return left.Bind(leftResult => right.Bind(rightResult => Return<TState, TOut>(func(leftResult, rightResult))));
        }

        /// <summary>
        /// Creates a parser similar to String, but requires a word boundary at the end
        /// </summary>
        public static GenParser<TState, string> Keyword<TState>(string s)
        {
            return String<TState>(s).CombineLeft(WordBoundary<TState>());
        }

        /// <summary>
        /// Same as Keyword, but consumes following whitespace
        /// </summary>
        public static GenParser<TState, string> KeywordSpaces<TState>(string s)
        {
            return Keyword<TState>(s).CombineLeft(Whitespace<TState>());
        }

        /// <summary>
        /// Parses an identifier starting with a letter, followed by letters or digits
        /// </summary>
        public static GenParser<TState, string> Identifier<TState>()
        {
            return ManyChar1<TState>(char.IsLetter, char.IsLetterOrDigit);
        }

        /// <summary>
        /// Same as Identifier, but consumes following whitespace
        /// </summary>
        public static GenParser<TState, string> IdentifierSpaces<TState>()
        {
            return Identifier<TState>().CombineLeft(Whitespace<TState>());
        }

        public static GenParser<TState, T> Between<TState, T, TLeft, TRight>(this GenParser<TState, T> middle, GenParser<TState, TLeft> left, GenParser<TState, TRight> right)
        {
            return left.CombineRight(middle).CombineLeft(right);
        }
    }
}
