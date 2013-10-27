using System;

namespace Khylang.CsParsec
{
    public class ParseException<TState> : Exception
    {
        public ParseException(ParseError<TState> s)
            : base(s.Error)
        {
        }
    }
}