namespace Khylang.CsParsec
{
    public struct ParseResult<T>
    {
        private readonly ParseState _state;
        private readonly T _result;

        public ParseResult(ParseState state, T result)
        {
            _state = state;
            _result = result;
        }

        public ParseState State
        {
            get { return _state; }
        }

        public T Result
        {
            get { return _result; }
        }
    }
}