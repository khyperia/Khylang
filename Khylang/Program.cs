using System;
using Khylang.CsParsec;
using Khylang.Utils;

namespace Khylang
{
    static class Program
    {
        private static readonly GenParser<Unit, Tuple<string, string>> Parser =
            Parsers.KeywordSpaces<Unit>("fzoo").CombineWith(Parsers.KeywordSpaces<Unit>("<3"), Tuple.Create);

        static void Main()
        {
            Console.WriteLine(Parser.RunParser("fzoo <3", Unit.Val));
            Console.WriteLine(Parser.TryRunParser("fzoo </3", Unit.Val));
            while (true)
            {
                var line = Console.ReadLine();
                if (line == null)
                    break;
                Console.WriteLine(KhylangParser.Parse(line));
            }
        }
    }
}
