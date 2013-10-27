namespace Khylang.CsParsec
{
    public struct ParseResult<TState, T>
    {
        private readonly ParseState<TState> _state;
        private readonly T _result;

        public ParseResult(ParseState<TState> state, T result)
        {
            _state = state;
            _result = result;
        }

        public ParseState<TState> State
        {
            get { return _state; }
        }

        public T Result
        {
            get { return _result; }
        }
    }
}