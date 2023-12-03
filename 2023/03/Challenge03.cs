using System.Data;
using System.Runtime.ExceptionServices;
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
            Console.WriteLine($"Example: P1={resultEx.Item1}, P2={resultEx.Item2}, matches expected={resultEx.Item1 == 4361 && resultEx.Item2 == 467835}");
            Console.WriteLine($"Input  : P1={resultIn.Item1}, P2={resultIn.Item2}");
        }

        static (Int32, Int32) CalculateScores(string[] input)
        {
            Int32 sum1 = CalculatePartA(input);
            Int32 sum2 = CalculatePartB(input);
            return (sum1, sum2);
        }

        static Int32 CalculatePartA(string[] input) {
            Int32 sum = 0;
            for (Int32 lineNumber = 0; lineNumber < input.Length; ++lineNumber) {
                MatchCollection matchCollection = numberRegex().Matches(input[lineNumber]);

                if (debug) Console.WriteLine($"Line {lineNumber} has {matchCollection.Count} numbers.");
                foreach (Match m in matchCollection.AsEnumerable()) {
                    Int32 value = Int32.Parse(m.Value);
                    Int32 x_begin = m.Index;
                    Int32 x_len = m.Length;

                    bool symbolFound = HasSymbol(input, x_begin, x_len, lineNumber);
                    if (symbolFound) {
                        sum += value;
                    }

                    if (debug) Console.WriteLine($"Element {value}, hasSymbol: {symbolFound}, sum: {sum}");
                }
            }
            return sum;
        }

        static bool HasSymbol(string[] input, Int32 x_begin, Int32 len, Int32 y_center) {
            Int32 y_start = (y_center == 0) ? 0 : y_center - 1;
            Int32 y_end = Math.Min(input.Length - 1, y_center + 1);
            Int32 x_start = (x_begin == 0) ? 0 : x_begin - 1;
            Int32 x_end = Math.Min(input[0].Length - 1, x_begin + len + 1);
            
            for (Int32 y = y_start; y <= y_end; ++y) {
                string cut = input[y].Substring(x_start, x_end - x_start);
                Regex r = illegalSymbolRegex();
                Match? match = r.Match(cut);
                if (match.Success) return true;
            }
            return false;
        }

        static Int32 CalculatePartB(string[] input) {
            Int32 sum = 0;
            for (Int32 lineNumber = 0; lineNumber < input.Length; ++lineNumber) {
                MatchCollection matchCollection = gearRegex().Matches(input[lineNumber]);

                if (debug) Console.WriteLine($"Line {lineNumber} has {matchCollection.Count} potential gears.");
                foreach (Match m in matchCollection.AsEnumerable()) {
                    Int32 x_pos = m.Index;
                    Int32 y_pos = lineNumber;

                    Int32 gearValue = GetGearValue(input, x_pos, y_pos);
                    if (debug) Console.WriteLine($"Gear at [{x_pos},{y_pos}] has value {gearValue}.");
                    sum += gearValue;
                }
            }
            return sum;
        }

        static Int32 GetGearValue(string[] input, Int32 x, Int32 y) {
            Int32[] x_valid = {x-1, x, x+1};
            Int32[] y_valid = {y-1, y, y+1};
            Int32 v1 = 0;
            Int32 v2 = 0;

            foreach (Int32 lineNumber in y_valid) {
                if (lineNumber == -1 || lineNumber == input.Length) {
                    continue;
                }

                MatchCollection matchCollection = numberRegex().Matches(input[lineNumber]);
                foreach (Match m in matchCollection.AsEnumerable()) {
                    Int32 value = Int32.Parse(m.Value);
                    Int32 x_begin = m.Index;
                    Int32 x_len = m.Length;

                    bool adjacent = false;
                    for (x = x_begin; x < x_begin + x_len; ++x) {
                        if (x_valid.Contains(x)) {
                            adjacent = true;
                            break;
                        }
                    }

                    if (debug) Console.WriteLine($"At [{lineNumber},{x_begin}]: {value}, adjacent:{adjacent}");
                    if (adjacent) {
                        if (v1 == 0) {
                            v1 = value;
                        } else {
                            v2 = value;
                            break;
                        }
                    }
                }
            }
            return v1*v2;
        }

        [GeneratedRegex("(\\d+)")]
        private static partial Regex numberRegex();
        [GeneratedRegex("\\*")]
        private static partial Regex gearRegex();
        [GeneratedRegex("[^0-9\\.]")]
        private static partial Regex illegalSymbolRegex();
    }
}
