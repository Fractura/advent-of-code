using System.Data;
using System.Diagnostics;

namespace challenge
{
    static partial class ChallengeXX
    {
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

        class StoneLine {
            public List<Int64> stones;
            public static readonly Dictionary<Int64, Boolean> isSplittable = new();
            public static readonly Dictionary<Int64, (Int64, Int64)> splitResult = new();
            public static readonly Dictionary<Int64, Int64> mulResult = new ();

            public StoneLine(string[] input) {
                stones = input[0].Split(" ").Select(Int64.Parse).ToList();
                isSplittable[0] = false;
                mulResult[0] = 1;
            }

            public static Int64 BlinkDictBased(List<Int64> stones, int times) {
                Dictionary<Int64, Int64> s0 = new();
                Dictionary<Int64, Int64> s1 = new();
                foreach (Int64 e in stones) {
                    AddToDict(s0, e, 1);
                }
                for (int i = 0; i < times; ++i) {
                    if (i % 2 == 0) {
                        ProcessDict(s0, s1);
                    } else {
                        ProcessDict(s1, s0);
                    }
                }
                if (times % 2 == 0) {
                    return GetStoneCount(s0);
                } else {
                    return GetStoneCount(s1);
                }
            }

            public static void ProcessDict(Dictionary<Int64, Int64> source, Dictionary<Int64, Int64> destination) {
                destination.Clear();
                foreach (var (v, multiplier) in source) {
                    if (!isSplittable.ContainsKey(v)) {
                        CheckSplitability(v);
                    }
                    if (isSplittable[v]) {
                        var split = splitResult[v];
                        AddToDict(destination, split, multiplier);
                    } else {
                        if (!mulResult.ContainsKey(v)) {
                            mulResult[v] = v * 2024;
                        }
                        AddToDict(destination, mulResult[v], multiplier);
                    }
                }
            }

            public static void AddToDict(Dictionary<Int64, Int64> dict, Int64 value, Int64 multiplier) {
                if (dict.ContainsKey(value)) {
                    dict[value] += multiplier;
                } else {
                    dict[value] = multiplier;
                }
            }

            public static void AddToDict(Dictionary<Int64, Int64> dict, (Int64, Int64) values, Int64 multiplier) {
                AddToDict(dict, values.Item1, multiplier);
                AddToDict(dict, values.Item2, multiplier);
            }

            public static Int64 GetStoneCount(Dictionary<Int64, Int64> dict) {
                return dict.Values.Sum(v => v);
            }

            public static void CheckSplitability(Int64 value) {
                isSplittable[value] = (int)Math.Floor(Math.Log10(value) + 1) % 2 == 0;
                if (isSplittable[value]) {
                    string numberStr = value.ToString();

                    int middleIndex = numberStr.Length / 2;
                    
                    string firstHalf = numberStr[..middleIndex];
                    string secondHalf = numberStr[middleIndex..];
                    
                    // Convert the parts back to integers
                    Int32 firstHalfInt = Int32.Parse(firstHalf);
                    Int32 secondHalfInt = Int32.Parse(secondHalf);
                    splitResult[value] = (firstHalfInt, secondHalfInt);
                }
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            StoneLine stoneLineEx = new(contentEx);
            StoneLine stoneLine = new(content);

            Debug.Assert(PartOne(stoneLineEx) == 55312);
            Console.WriteLine($"Part 1 : {PartOne(stoneLine)}");
            Console.WriteLine($"Part 2 : {PartTwo(stoneLine)}");
        }

        static Int64 PartOne(StoneLine stoneLine)
        {
            return StoneLine.BlinkDictBased(stoneLine.stones, 25);
        }

        static Int64 PartTwo(StoneLine stoneLine)
        {
            return StoneLine.BlinkDictBased(stoneLine.stones, 75);
        }
    }
}
