using System;

namespace Khylang.CsParsec
{
    public struct ParseError
    {
        private readonly string _error;

        public ParseError(string error)
        {
            _error = error;
        }

        public string Error
        {
            get { return _error; }
        }

        public Exception Exception
        {
            get { return new ParseException(this); }
        }

        public override string ToString()
        {
            return string.Format("ParseError{{{0}}}", _error);
        }
    }
}