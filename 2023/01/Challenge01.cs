using System.Data;
using System.Text.RegularExpressions;

namespace challenge
{
    static partial class Challenge01
    {
        static bool debug = false;

        static string[] ReadFile(string filename)
        {
            var s = Directory.GetCurrentDirectory();
            if (s.Contains("bin")) {
                s = s[..^16];
            }
            string path = $"{s}{filename}";
            
            if (!File.Exists(path)) {
                Console.WriteLine("Input file not found");
            }

            StreamReader reader = File.OpenText(path);
            string[] content = reader.ReadToEnd().Split("\r\n");

            return content;
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Console.WriteLine($"Example: {getTotalValue(contentEx)}");
            Console.WriteLine($"Part 1 : {getTotalValue(content)}");
            Console.WriteLine($"Part 2 : {getTotalValueWords(content)}");
        }

        static Int32 getTotalValue(string[] input)
        {
            Int32 sum = 0;
            foreach (string line in input) {
                (Int32,Int32) digits = getFirstAndLast(line);
                Int32 value = concatAndMerge(digits);
                sum += value;
                if (debug) Console.WriteLine($"LnA={line},digits={digits.Item1},{digits.Item2},value={value},sum={sum}");
            }
            return sum;
        }

        static Int32 getTotalValueWords(string[] input)
        {
            Int32 sum = 0;
            foreach (string line in input) {
                (Int32,Int32) digits = getFirstAndLastWords(line);
                Int32 value = concatAndMerge(digits);
                sum += value;
                if (debug) Console.WriteLine($"LnB={line},digits={digits.Item1},{digits.Item2},value={value},sum={sum}");
            }
            return sum;
        }

        static (Int32, Int32) getFirstAndLast(string line) {
            Regex commandRegex = digitRegex();
            Match? matchCommand = commandRegex.Matches(line).Cast<Match>().FirstOrDefault();
            if (matchCommand == null) {
                // Can't parse
                Console.WriteLine($"Could not parse line: {line}");
                return (0,0);
            }
            GroupCollection groups = matchCommand.Groups;
            if (groups[2].Length == 0) {
                Int32 v = Int32.Parse(groups[1].ToString());
                return (v,v);
            } else {
                Int32 l = Int32.Parse(groups[2].ToString());
                Int32 r = Int32.Parse(groups[3].ToString());
                return (l,r);
            }
        }

        static (Int32, Int32) getFirstAndLastWords(string line) {
            Regex commandRegex = wordRegex();
            Match? matchCommand = commandRegex.Matches(line).Cast<Match>().FirstOrDefault();
            if (matchCommand == null) {
                // Can't parse
                Console.WriteLine($"Could not parse line: {line}");
                return (0,0);
            }
            GroupCollection groups = matchCommand.Groups;
            if (groups[2].Length == 0) {
                Int32 v = ParseWord(groups[1].ToString());
                return (v,v);
            } else {
                Int32 l = ParseWord(groups[2].ToString());
                Int32 r = ParseWord(groups[3].ToString());
                return (l,r);
            }
        }

        static Int32 concatAndMerge((Int32, Int32) value) {
            return Int32.Parse($"{value.Item1}{value.Item2}");
        }

        static Int32 ParseWord(string word) {
            return word switch
            {
                "0" or "zero" => 0,
                "1" or "one" => 1,
                "2" or "two" => 2,
                "3" or "three" => 3,
                "4" or "four" => 4,
                "5" or "five" => 5,
                "6" or "six" => 6,
                "7" or "seven" => 7,
                "8" or "eight" => 8,
                "9" or "nine" => 9,
                _ => 0,
            };
        }

        [GeneratedRegex("^\\D*((\\d).*(\\d)|\\d)\\D*$")]
        private static partial Regex digitRegex();

        [GeneratedRegex("^\\D*?((\\d|zero|one|two|three|four|five|six|seven|eight|nine).*(\\d|zero|one|two|three|four|five|six|seven|eight|nine)|(\\d|zero|one|two|three|four|five|six|seven|eight|nine))\\D*$")]
        private static partial Regex wordRegex();
    }
}
