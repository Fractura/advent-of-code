using System.Diagnostics;

namespace challenge
{
    static partial class Challenge19
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

        class Onsen {
            readonly List<string> towelPatterns;
            readonly List<string> designs;

            public Onsen(string[] input) {
                towelPatterns = input[0].Replace(" ", string.Empty).Split(',').ToList();
                designs = new();
                for (int i = 2; i < input.Length; ++i) {
                    designs.Add(input[i]);
                }
            }

            public int GetSolvableDesignCount() {
                return designs.AsParallel().Count(Solvable);
            }

            private bool Solvable(string design) {
                SortedSet<int> possiblePositions = new()
                {
                    0
                };
                while (possiblePositions.Count > 0) {
                    int position = possiblePositions.First();
                    possiblePositions.Remove(position);
                    if (position == design.Length) {
                        return true;
                    }

                    foreach (string validSequence in towelPatterns.AsParallel().Where(design[position..].StartsWith)) {
                        possiblePositions.Add(position + validSequence.Length);
                    }
                }
                
                return false;
            }

            public Int64 GetAllPossibleDesignOptions() {
                return designs.AsParallel().Sum(GetPossibleDesignOptions);
            }

            private Int64 GetPossibleDesignOptions(string design) {
                Int64[] positionReachableCombinations = new Int64[design.Length+1];
                positionReachableCombinations[0] = 1;
                for (int i = 0; i < design.Length; ++i) {
                    foreach (string validSequence in towelPatterns.AsParallel().Where(design[i..].StartsWith)) {
                        positionReachableCombinations[i + validSequence.Length] += positionReachableCombinations[i];
                    }
                }
                return positionReachableCombinations[design.Length];
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Onsen example = new(contentEx);
            Onsen puzzle = new(content);

            Debug.Assert(PartOne(example) == 6);
            Console.WriteLine($"Part 1 : {PartOne(puzzle)}");
            Debug.Assert(PartTwo(example) == 16);
            Console.WriteLine($"Part 2 : {PartTwo(puzzle)}");
        }

        static Int32 PartOne(Onsen onsen)
        {
            return onsen.GetSolvableDesignCount();
        }

        static Int64 PartTwo(Onsen onsen)
        {
            return onsen.GetAllPossibleDesignOptions();
        }
    }
}
