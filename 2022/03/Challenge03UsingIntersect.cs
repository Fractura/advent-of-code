namespace challenge
{
    class Challenge03UsingIntersect
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
            int result1 = 0;
            int result2 = 0;
            while ((line = reader.ReadLine()) != null) {
                result1 += getResultPart1(line);
                string? line2 = reader.ReadLine();
                string? line3 = reader.ReadLine();

                if (line2 == null) break;
                result1 += getResultPart1(line2);
                if (line3 == null) break;
                result1 += getResultPart1(line3);

                string common = string.Join("", line.Intersect(line2.Intersect(line3)));

                result2 += getValueForCharacter(common[0]);
            }
            Console.WriteLine("Part 1 result: " + result1);
            Console.WriteLine("Part 2 result: " + result2);
        }

        static int getResultPart1(string str) {
            string firstHalf = str.Substring(0, str.Length/2);
            string secondHalf = str.Substring(str.Length/2);
            return getValueForCharacter(string.Join("", firstHalf.Intersect(secondHalf))[0]);
        }

        static int getValueForCharacter(char character) {
            int value = character - '@';
            // Swap value of Upper/Lowercase, because the task has the lowercase characters as values 1-26 and uppercase is first in ascii table
            if (value > 26) {
                // Letter is lowercase, subtract 6 to account for the 6 characters between A-Z and a-z
                value = value - 6 - 26;
            } else {
                // Letter is uppercase
                value = value + 26;
            }
            return value;
        }
    }
}

