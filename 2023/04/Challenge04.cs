using System.Text.RegularExpressions;

namespace challenge
{
    static partial class Challenge03
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

            var resultEx = CalculateScores(contentEx);
            var resultIn = CalculateScores(content);
            Console.WriteLine($"Example: P1={resultEx.Item1}, P2={resultEx.Item2}, matches expected={resultEx.Item1 == 13 && resultEx.Item2 == 30}");
            Console.WriteLine($"Input  : P1={resultIn.Item1}, P2={resultIn.Item2}");
        }

        static (Int32, Int32) CalculateScores(string[] input) {
            Int32 sum1 = 0;
            Int32[] cardAmounts = new Int32[input.Length];
            Array.Fill(cardAmounts, 1);
            for (Int32 lineNumber = 0; lineNumber < input.Length; ++lineNumber) {
                Match? match = playRegex().Match(input[lineNumber]);

                if (!match.Success) {
                    Console.WriteLine($"Failed parsing line {lineNumber}");
                    continue;
                }

                List<Int32> winning = GetNumbers(match.Groups["winning"].Value);
                List<Int32> playing = GetNumbers(match.Groups["playing"].Value);
                Int32 intersect = winning.Intersect(playing).Count();

                // Part 1
                Int32 value = (intersect == 0) ? 0 : Convert.ToInt32(Math.Pow(2, intersect-1));
                sum1 += value;

                // Part 2
                int limit = Math.Min(input.Length, lineNumber + 1 + intersect);
                for (int i = lineNumber+1; i < limit; ++i) {
                    cardAmounts[i] += cardAmounts[lineNumber];
                }

                if (debug) {
                    Console.WriteLine($"Line: {lineNumber}, Intersect: {intersect}, Value: {value}, Sum1: {sum1}");
                }
            }
            return (sum1, cardAmounts.Sum());
        }

        static List<Int32> GetNumbers(string input) {
            List<Int32> values = new();
            string[] splits = input.Split(' ');
            foreach (string s in splits) {
                if (s.Length > 0) {
                    values.Add(Int32.Parse(s));
                }
            }
            return values;
        }

        [GeneratedRegex("^Card +\\d+:(?<winning>.*)\\|(?<playing>.*)$")]
        private static partial Regex playRegex();
    }
}
