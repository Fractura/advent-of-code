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

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Debug.Assert(PartOne(contentEx) == -1);
            Console.WriteLine($"Part 1 : {PartOne(content)}");
            Debug.Assert(PartTwo(contentEx) == -1);
            Console.WriteLine($"Part 2 : {PartTwo(content)}");
        }

        static Int32 PartOne(string[] input)
        {
            return 0;
        }

        static Int32 PartTwo(string[] input)
        {
            return 0;
        }
    }
}
