namespace Khylang.CsParsec
{
    public struct ParseState
    {
        private readonly string _s;
        private readonly int _index;

        public ParseState(string s, int index)
        {
            _s = s;
            _index = index;
        }

        public string S
        {
            get { return _s; }
        }

        public int Index
        {
            get { return _index; }
        }

        public ParseState Add(int length)
        {
            return new ParseState(_s, _index + length);
        }
    }
}