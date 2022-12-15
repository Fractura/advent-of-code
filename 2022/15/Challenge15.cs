using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge15
    {
        static List<Range> ranges = new List<Range>();

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
                (int, int, int, int) command = parseCommand(line);
                Console.WriteLine(command);
                Range range = convertToRangeAt(10, command.Item1, command.Item2, command.Item3, command.Item4);
                if (range.min != -1) {
                    ranges.Add(range);
                }
            }

            Console.WriteLine("Part 1 result: ???");
            Console.WriteLine("Part 2 result: ???");
        }

        static (int, int, int, int) parseCommand(string line) {
            Regex commandRegex = new Regex(@"^Sensor at x=(\-?\d+), y=(\-?\d+): closest beacon is at x=(\-?\d+), y=(\-?\d+)$");
            Match? matchCommand = commandRegex.Matches(line).Cast<Match>().FirstOrDefault();
            if (matchCommand == null) {
                // Can't parse
                return (0, 0, 0, 0);
            }
            GroupCollection groups = matchCommand.Groups;
            return (Int32.Parse(groups[1].Value), Int32.Parse(groups[2].Value), Int32.Parse(groups[3].Value), Int32.Parse(groups[4].Value));
        }

        static Range convertToRangeAt(int y, int xCenter, int yCenter, int xBeacon, int yBeacon) {
            int range = calculateRadius(xCenter, yCenter, xBeacon, yBeacon);
            int affectedXCount = range - Math.Abs(yCenter - y);
            if (affectedXCount < 0) return new Range(-1,-1);
            else {
                int left = xCenter - affectedXCount;
                int right = xCenter + affectedXCount;
                return new Range(left, right);
            }
        }

        static int calculateRadius(int xCenter, int yCenter, int xBeacon, int yBeacon) {
            return Math.Abs(xCenter - xBeacon) + Math.Abs(yCenter - yBeacon);
        }
    }

    struct Range {
        public int min;
        public int max;

        public Range(int min, int max) {
            this.min = min;
            this.max = max;
        }
    }
}