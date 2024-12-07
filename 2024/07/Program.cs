using System.Diagnostics;

namespace challenge
{
    static partial class Challenge07
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

        struct InputElement {
            public Int64 left;
            public List<Int64> right;
            private readonly string s;

            public InputElement(string input) {
                string[] split = input.Split(':');
                this.left = Int64.Parse(split[0]);
                this.right = split[1].Split(' ')[1..].Select(Int64.Parse).ToList();
                this.s = input;
            }

            public override readonly String ToString() {
                return s;
            }
        }

        static List<InputElement> ParseInputElements(string[] input) {
            List<InputElement> inputElements = new();
            foreach (string inputElement in input) {
                inputElements.Add(new InputElement(inputElement));
            }
            return inputElements;
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            var elementsEx = ParseInputElements(contentEx);
            var elements = ParseInputElements(content);

            Debug.Assert(PartOne(elementsEx) == 3749);
            Console.WriteLine($"Part 1 : {PartOne(elements)}");
            Debug.Assert(PartTwo(elementsEx) == 11387);
            Console.WriteLine($"Part 2 : {PartTwo(elements)}");
        }

        static Boolean IsSolvableOne(InputElement input) {
            Int64 expectedResult = input.left;
            List<Int64> values = input.right;
            int permutation = (int) Math.Pow(2, values.Count - 1) - 1;
            while (permutation >= 0) {
                int pV = permutation;
                Int64 calculation = values[0];
                for (int i = 1; i < values.Count; ++i) {
                    if (pV % 2 == 0) {
                        calculation += values[i];
                    } else {
                        calculation *= values[i];
                    }
                    pV /= 2;
                }
                if (calculation == expectedResult) {
                    return true;
                }
                --permutation;
            }
            return false;
        }

        static Boolean IsSolvableTwo(InputElement input) {
            Int64 expectedResult = input.left;
            List<Int64> values = input.right;
            int permutation = (int) Math.Pow(3, values.Count - 1) - 1;
            while (permutation >= 0) {
                int pV = permutation;
                Int64 calculation = values[0];
                for (int i = 1; i < values.Count; ++i) {
                    if (pV % 3 == 0) {
                        calculation += values[i];
                    } else if (pV % 3 == 1) {
                        calculation *= values[i];
                    } else {
                        calculation = Int64.Parse(""+calculation+""+values[i]);
                    }
                    pV /= 3;
                }
                if (calculation == expectedResult) {
                    return true;
                }
                --permutation;
            }
            return false;
        }

        static Int64 PartOne(List<InputElement> input)
        {
            Int64 result = 0;
            foreach (InputElement element in input.AsParallel().Where(IsSolvableOne)) {
                result += element.left;
            }
            return result;
        }

        static Int64 PartTwo(List<InputElement> input)
        {
            Int64 result = 0;
            foreach (InputElement element in input.AsParallel().Where(IsSolvableTwo)) {
                result += element.left;
            }
            return result;
        }
    }
}
