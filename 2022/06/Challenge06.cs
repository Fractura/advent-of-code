namespace challenge
{
    class Challenge06
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
            string content = reader.ReadToEnd();

            int result1;
            int result2;

            int index = 0;
            int len = 4;
            while (true) {
                string substring = content.Substring(index, len);
                if (!hasDuplicates(substring)) {
                    result1 = index + len;
                    break;
                } else ++index;
            }
            len = 14;
            while (true) {
                string substring = content.Substring(index, len);
                if (!hasDuplicates(substring)) {
                    result2 = index + len;
                    break;
                } else ++index;
            }

            Console.WriteLine("Part 1 result: " + result1);
            Console.WriteLine("Part 2 result: " + result2);
        }

        static Boolean hasDuplicates(string input) {
            HashSet<char> set = new HashSet<char>();
            foreach (char c in input) {
                set.Add(c);
            }
            return set.Count() < input.Length;
        }
    }
}