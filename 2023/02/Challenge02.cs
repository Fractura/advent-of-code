using System.Data;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

namespace challenge
{
    static partial class Challenge02
    {
        static bool debug = true;

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
            Console.WriteLine($"Example: P1={resultEx.Item1}, P2={resultEx.Item2}, matches expected={resultEx.Item1 == 8 && resultEx.Item2 == 2286}");
            Console.WriteLine($"Input  : P1={resultIn.Item1}, P2={resultIn.Item2}");
        }

        static (Int32, Int32) CalculateScores(string[] input)
        {
            Int32 sum1 = 0;
            Int32 sum2 = 0;
            foreach (string line in input) {
                Int32 id = GetGameId(line);
                bool valid = IsValidGame(line);
                if (valid) sum1 += id;
                if (debug) Console.WriteLine($"P1 -- ID: {id}, Valid: {valid}, Sum: {sum1} (line={line})");

                Int32 value = GetGamePower(line);
                sum2 += value;
                if (debug) Console.WriteLine($"P2 -- Value: {value}, Sum: {sum2}, (line={line})");
            }
            return (sum1, sum2);
        }
        
        static Int32 GetGameId(string line) {
            Match? match = idRegex().Match(line);
            return Int32.Parse(match.Groups[1].Value);
        }

        static bool IsValidGame(string line) {
            return (CheckRed(line) && CheckGreen(line) && CheckBlue(line));
        }

        static bool CheckRed(string line) {
            return CheckLine(line, redRegex(), 12);
        }

        static bool CheckGreen(string line) {
            return CheckLine(line, greenRegex(), 13);
        }

        static bool CheckBlue(string line) {
            return CheckLine(line, blueRegex(), 14);
        }

        static bool CheckLine(string line, Regex rx, Int32 max) {
            MatchCollection matches = rx.Matches(line);
            if (matches.Count == 0) {
                // Can't parse
                Console.WriteLine($"Could not parse line: {line}");
                return false;
            }
            foreach (Match match in matches.AsEnumerable()) {
                if (Int32.Parse(match.Groups[2].Value) > max) return false;
            }
            return true;
        }

        static Int32 GetGamePower(string line) {
            return GetMinColorCount(line, redRegex()) * GetMinColorCount(line, greenRegex()) * GetMinColorCount(line, blueRegex());
        }

        static Int32 GetMinColorCount(string line, Regex rx) {
            Int32 minimumCount = 0;
            MatchCollection matches = rx.Matches(line);
            if (matches.Count == 0) {
                // Can't parse
                Console.WriteLine($"Could not parse line: {line}");
                return minimumCount;
            }
            foreach (Match match in matches.AsEnumerable()) {
                minimumCount = Math.Max(minimumCount, Int32.Parse(match.Groups[2].Value));
            }
            return minimumCount;
        }

        [GeneratedRegex("^Game (\\d+):.*$")]
        private static partial Regex idRegex();

        [GeneratedRegex("(?=((\\d+) red))")]
        private static partial Regex redRegex();

        [GeneratedRegex("(?=((\\d+) green))")]
        private static partial Regex greenRegex();

        [GeneratedRegex("(?=((\\d+) blue))")]
        private static partial Regex blueRegex();
    }
}
