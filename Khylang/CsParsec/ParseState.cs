namespace Khylang.CsParsec
{
    public struct ParseState<TState>
    {
        private readonly string _s;
        private readonly int _index;
        private readonly TState _state;

        public ParseState(string s, int index, TState state)
        {
            _s = s;
            _index = index;
            _state = state;
        }

        public string S
        {
            get { return _s; }
        }

        public int Index
        {
            get { return _index; }
        }

        public char Char
        {
            get { return _s[_index]; }
        }

        public TState State
        {
            get { return _state; }
        }

        public ParseState<TState> Add(int length)
        {
            return new ParseState<TState>(_s, _index + length, State);
        }

        public override string ToString()
        {
            return string.Format("State{{{0}}}", _index);
        }
    }
}
