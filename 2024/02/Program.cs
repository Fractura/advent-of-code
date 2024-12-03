using System.Data;

namespace challenge
{
    static partial class Challenge02
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
            Int32 safeLines = 0;
            foreach (string line in input) {
                var numberList = ParseLine(line);
                if (IsSafe(numberList).Item1) {
                    ++safeLines;
                }
            }
            return safeLines;
        }

        static Int32 PartTwo(string[] input)
        {
            Int32 safeLines = 0;
            foreach (string line in input) {
                var numberList = ParseLine(line);
                var r = IsSafe(numberList);
                if (r.Item1) {
                    ++safeLines;
                    continue;
                }
                // reattempt safety check with a level removed where the first problem occurred.
                // two levels are responsible for a failed safety check, so attempt this once with the "left" removed, then with the "right" removed
                // there is also a rare case where the direction has changed at index 2 which can be solved by removing the first element, ex.: (15) 13 15 16 17 20 21 24
                if (debug) Console.WriteLine($"Line {line} has problem at index {r.Item2}.");
                var numberListFirstRemoved = numberList.ToList();
                numberListFirstRemoved.RemoveAt(0);
                var numberListLeftRemoved = numberList.ToList();
                numberListLeftRemoved.RemoveAt(r.Item2 - 1);
                var numberListRightRemoved = numberList.ToList();
                numberListRightRemoved.RemoveAt(r.Item2);

                if (IsSafe(numberListFirstRemoved).Item1 || IsSafe(numberListLeftRemoved).Item1 || IsSafe(numberListRightRemoved).Item1) {
                    ++safeLines;
                } else {
                    if (debug) Console.WriteLine($"Line {line} can not be solved by removing a single level.");
                }
            }
            return safeLines;
        }

        static List<Int32> ParseLine(string line)
        {
            string[] numberStrings = line.Split(' ');
            List<int> numberList = numberStrings.Select(int.Parse).ToList();
            if (debug) Console.WriteLine($"Line = {line}");
            return numberList;
        }

        static (Boolean, Int32) IsSafe(List<Int32> numbers) {
            Boolean ascending = false;
            Boolean descending = false;
            Int32 previousNumber = -1;
            Int32 problemIndex = -1;

            foreach (Int32 number in numbers) {
                ++problemIndex;
                if (previousNumber != -1) {
                    if (debug) Console.WriteLine($"Check {previousNumber} -> {number}");
                    if (previousNumber == number) {
                        if (debug) Console.WriteLine($"Same number -> not safe.");
                        return (false, problemIndex);
                    } else if (Math.Abs(previousNumber - number) > 3) {
                        if (debug) Console.WriteLine($"Level difference > 3 -> not safe.");
                        return (false, problemIndex);
                    } else if (previousNumber < number) {
                        if (descending) {
                            if (debug) Console.WriteLine($"Higher number, but previously descending -> not safe.");
                            return (false, problemIndex);
                        } else {
                            ascending = true;
                        }
                    } else {
                        // previousNumber > number
                        if (ascending) {
                            if (debug) Console.WriteLine($"Lower number, but previously ascending -> not safe.");
                            return (false, problemIndex);
                        } else {
                            descending = true;
                        }
                    }
                    
                }
                previousNumber = number;
            }
            if (debug) Console.WriteLine($"Line safe.");
            return (true, -1);
        }
    }
}