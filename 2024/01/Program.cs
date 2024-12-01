using System.Data;
using System.Text.RegularExpressions;

namespace challenge
{
    static partial class Challenge01
    {
        static readonly bool debug = false;

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

            Console.WriteLine($"Example part 1: {PartOne(contentEx)}");
            Console.WriteLine($"Part 1 : {PartOne(content)}");
            Console.WriteLine($"Example part 2: {PartTwo(contentEx)}");
            Console.WriteLine($"Part 2 : {PartTwo(content)}");
        }

        static Int32 PartOne(string[] input)
        {
            Int32 sum = 0;
            (List<Int32>, List<Int32>) lists = ParseInput(input);
            List<Int32> left = lists.Item1;
            List<Int32> right = lists.Item2;
            left.Sort();
            right.Sort();
            for (int i = 0; i < left.Count && i < right.Count; ++i) {
                sum += Math.Abs(left[i] - right[i]);
                if (debug) Console.WriteLine($"L: {left[i]}, R: {right[i]}, SUM: {sum}");
            }
            return sum;
        }

        static Int32 PartTwo(string[] input)
        {
            Int32 sum = 0;
            (List<Int32>, List<Int32>) lists = ParseInput(input);
            List<Int32> left = lists.Item1;
            List<Int32> right = lists.Item2;

            // Dictionary to store the counts
            Dictionary<Int32, Int32> counts = new();
            foreach (Int32 item in right) {
                if (counts.ContainsKey(item)) {
                    ++counts[item];
                } else {
                    counts[item] = 1;
                }
            }

            for (int i = 0; i < left.Count; ++i) {
                if (counts.ContainsKey(left[i])) {
                    sum += left[i] * counts[left[i]];
                    if (debug) Console.WriteLine($"L: {left[i]}, Occ: {right[i]}, SUM: {sum}");
                } else {
                    if (debug) Console.WriteLine($"L: {left[i]}, Occ: 0, SUM: {sum}");
                }
            }
            return sum;
        }

        static (List<Int32>, List<Int32>) ParseInput(string[] input) {
            List<Int32> left = new();
            List<Int32> right = new();
            foreach (string line in input) {
                (Int32, Int32) values = ParseLine(line);
                left.Add(values.Item1);
                right.Add(values.Item2);
                if (debug) Console.WriteLine($"LnA={line},digits={values.Item1},{values.Item2}");
            }
            return (left, right);
        }

        static (Int32, Int32) ParseLine(string line)
        {
            MatchCollection matches = inputRegex().Matches(line);
            if (matches.Count == 0) {
                // Can't parse
                Console.WriteLine($"Could not parse line: {line}");
                return (0,0);
            }
            Int32 left = int.Parse(matches[0].Groups[1].Value);
            Int32 right = int.Parse(matches[0].Groups[2].Value);
            return (left, right);
        }

        [GeneratedRegex("(\\d+)\\s+(\\d+)")]
        private static partial Regex inputRegex();
    }
}