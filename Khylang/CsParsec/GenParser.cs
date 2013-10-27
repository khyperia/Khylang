using Khylang.Utils;

namespace Khylang.CsParsec
{
    public delegate IEither<ParseResult<TState, TResult>, ParseError<TState>> GenParser<TState, TResult>(ParseState<TState> state);
}
