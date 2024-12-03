using System.Data;
using System.Text.RegularExpressions;

namespace challenge
{
    static partial class Challenge01
    {
        static readonly bool debug = false;

        static string ReadFile(string filename)
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
            string content = reader.ReadToEnd();

            return content;
        }

        static void Main(string[] args)
        {
            string contentEx1 = ReadFile("example.input");
            string contentEx2 = ReadFile("example2.input");
            string content = ReadFile("my.input");

            Console.WriteLine($"Example part 1: {PartOne(contentEx1)}");
            Console.WriteLine($"Part 1 : {PartOne(content)}");
            Console.WriteLine($"Example part 2: {PartTwo(contentEx2)}");
            Console.WriteLine($"Part 2 : {PartTwo(content)}");
        }

        static Int32 PartOne(string input)
        {
            Int32 sum = 0;
            MatchCollection matches = MulInstructionRegex().Matches(input);
            if (matches.Count == 0) {
                // Can't parse
                Console.WriteLine($"Could not parse or no valid expressions found in input.");
                return sum;
            }
            foreach (GroupCollection group in matches.Select(match => match.Groups)) {
                Int32 factorLeft = int.Parse(group[1].Value);
                Int32 factorRight = int.Parse(group[2].Value);
                sum += factorLeft * factorRight;
                if (debug) Console.WriteLine($"Added {factorLeft} x {factorRight}, sum now: {sum}.");
            }
            return sum;
        }

        static Int32 PartTwo(string input)
        {
            Int32 sum = 0;
            Boolean enabled = true;
            MatchCollection matches = GenInstructionRegex().Matches(input);
            if (matches.Count == 0) {
                // Can't parse
                Console.WriteLine($"Could not parse or no valid expressions found in input.");
                return sum;
            }
            foreach (GroupCollection group in matches.Select(match => match.Groups)) {
                if (group[2].Success && enabled) {
                    Int32 factorLeft = int.Parse(group[2].Value);
                    Int32 factorRight = int.Parse(group[3].Value);
                    sum += factorLeft * factorRight;
                    if (debug) Console.WriteLine($"Added {factorLeft} x {factorRight}, sum now: {sum}.");
                } else if (group[1].Value.Equals("do()")) {
                    enabled = true;
                    if (debug) Console.WriteLine($"Detected \"do()\".");
                } else if (group[1].Value.Equals("don't()")) {
                    enabled = false;
                    if (debug) Console.WriteLine($"Detected \"don't()\".");
                } else {
                    if (debug) Console.WriteLine($"Detected factorial, but processing is disabled by a previous \"don't()\".");
                }
            }
            return sum;
        }

        [GeneratedRegex("mul\\((\\d+),(\\d+)\\)")]
        private static partial Regex MulInstructionRegex();

        [GeneratedRegex("(mul\\((\\d+),(\\d+)\\)|do\\(\\)|don't\\(\\))")]
        private static partial Regex GenInstructionRegex();
    }
}
