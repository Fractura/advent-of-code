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

            Console.WriteLine($"Example: {GetTotalValue(contentEx, digitRegex())}");
            Console.WriteLine($"Part 1 : {GetTotalValue(content, digitRegex())}");
            Console.WriteLine($"Part 2 : {GetTotalValue(content, wordRegex())}");
        }

        static Int32 GetTotalValue(string[] input, Regex rx)
        {
            Int32 sum = 0;
            foreach (string line in input) {
                (Int32,Int32) digits = GetFirstAndLast(line, rx);
                Int32 value = ConcatAndMerge(digits);
                sum += value;
                if (debug) Console.WriteLine($"LnA={line},digits={digits.Item1},{digits.Item2},value={value},sum={sum}");
            }
            return sum;
        }

        static (Int32, Int32) GetFirstAndLast(string line, Regex rx) {
            MatchCollection matches = rx.Matches(line);
            if (matches.Count == 0) {
                // Can't parse
                Console.WriteLine($"Could not parse line: {line}");
                return (0,0);
            }
            Int32 first = ParseWord(matches[0].Groups[1].Value);
            Int32 last = ParseWord(matches[^1].Groups[1].Value);
            return (first,last);
        }

        static Int32 ConcatAndMerge((Int32, Int32) value) {
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

        [GeneratedRegex("\\d")]
        private static partial Regex digitRegex();

        [GeneratedRegex("(?=(\\d|zero|one|two|three|four|five|six|seven|eight|nine))")]
        private static partial Regex wordRegex();
    }
}
