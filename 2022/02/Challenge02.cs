namespace challenge
{
    class Challenge02
    {
        static void Main(string[] args)
        {
            SortedDictionary<string, int> outcomesA = new SortedDictionary<string, int>
            {
                {"A X", 1 + 3},
                {"A Y", 2 + 6},
                {"A Z", 3 + 0},
                {"B X", 1 + 0},
                {"B Y", 2 + 3},
                {"B Z", 3 + 6},
                {"C X", 1 + 6},
                {"C Y", 2 + 0},
                {"C Z", 3 + 3}
            };
            SortedDictionary<string, int> outcomesB = new SortedDictionary<string, int>
            {
                {"A X", 3 + 0},
                {"A Y", 1 + 3},
                {"A Z", 2 + 6},
                {"B X", 1 + 0},
                {"B Y", 2 + 3},
                {"B Z", 3 + 6},
                {"C X", 2 + 0},
                {"C Y", 3 + 3},
                {"C Z", 1 + 6}
            };
            string filename;
            if (args.Length < 1) {
                filename = "./input.txt";
            } else {
                filename = (string) (args.GetValue(0) ?? "./input.txt");
            }

            if (!File.Exists(filename)) {
                Console.WriteLine("Input file not found");
                return;
            }

            StreamReader reader = File.OpenText(filename);
            string? line;
            int resultA = 0;
            int resultB = 0;
            while ((line = reader.ReadLine()) != null) {
                resultA = resultA + outcomesA.GetValueOrDefault(line, 0);
                resultB = resultB + outcomesB.GetValueOrDefault(line, 0);
            }
            Console.WriteLine($"Calculated result A is: {resultA}");
            Console.WriteLine($"Calculated result B is: {resultB}");
        }
    }
}