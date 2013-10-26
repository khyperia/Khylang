using Khylang.Utils;

namespace Khylang.CsParsec
{
    public delegate IEither<ParseResult<TResult>, ParseError> GenParser<TResult>(ParseState state);
}
