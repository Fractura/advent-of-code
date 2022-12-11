using System.Text.RegularExpressions;

namespace challenge
{
    class ChallengeTemplate
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
            string[] content = reader.ReadToEnd().Split("\r\n");

            // Create object

            foreach (string line in content) {
                //var command = parseCommand(line);
                //object.process
            }

            Console.WriteLine("Part 1 result: ???");
            Console.WriteLine("Part 2 result: ???");
        }

        static void parseCommand(string line) {
            Regex commandRegex = new Regex(@"^(group1)(group2)$");
            Match? matchCommand = commandRegex.Matches(line).Cast<Match>().FirstOrDefault();
            if (matchCommand == null) {
                // Can't parse
                return;
            }
            GroupCollection groups = matchCommand.Groups;
        }
    }
}