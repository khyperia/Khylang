using System;
using Khylang.CsParsec;

namespace Khylang
{
    static class Program
    {
        private static readonly GenParser<Tuple<string, string>> Parser =
            Parsers.Identifier("fzoo")
            .CombineLeft(Parsers.Whitespace())
            .CombineWith(Parsers.Identifier("<3"), Tuple.Create);

        static void Main()
        {
            Console.WriteLine(Parser.RunParser("fzoo <3"));
            Console.WriteLine(Parser.TryRunParser("fzoo </3"));
            Console.ReadLine();
        }
    }
}
