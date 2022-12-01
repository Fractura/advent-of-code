namespace challenge
{
    class Challenge01
    {
        static void Main(string[] args)
        {
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
            int currentValue = 0;
            List<int> results = new List<int>();
            while ((line = reader.ReadLine()) != null) {
                if (line.Length == 0) {
                    results.Add(currentValue);
                    currentValue = 0;
                } else {
                    currentValue = currentValue + int.Parse(line);
                }
            }
            results.Sort();
            Console.WriteLine($"Calculated top result is: {results.Last()}");
            List<int> topThree = results.GetRange(results.Count()-3, 3);
            Console.WriteLine($"Calculated top-three sum result is: {topThree.Sum()}");
        }
    }
}