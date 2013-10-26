using System;

namespace Khylang.CsParsec
{
    public class ParseException : Exception
    {
        public ParseException(ParseError s)
            : base(s.Error)
        {
        }
    }
}