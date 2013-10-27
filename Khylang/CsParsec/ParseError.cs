using System;

namespace Khylang.CsParsec
{
    public struct ParseError<TState>
    {
        private readonly string _error;
        private readonly ParseState<TState> _state;

        public ParseError(string error, ParseState<TState> state)
        {
            _error = error;
            _state = state;
        }

        public string Error
        {
            get { return _error; }
        }

        public ParseState<TState> State
        {
            get { return _state; }
        }

        public Exception Exception
        {
            get { return new ParseException<TState>(this); }
        }

        public override string ToString()
        {
            return string.Format("ParseError{{{0} {1}}}", _state, _error);
        }
    }
}