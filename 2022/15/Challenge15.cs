using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge15
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

            List<Range> ranges = new List<Range>();
            int yToCheck = 2000000;
            HashSet<int> beaconsAtYToCheck = new HashSet<int>();
            int minX = Int32.MaxValue;
            int maxX = Int32.MinValue;
            foreach (string line in content) {
                (int, int, int, int) command = parseCommand(line);
                Console.WriteLine(command);
                
                Range range = convertToRangeAt(yToCheck, command.Item1, command.Item2, command.Item3, command.Item4);
                Console.WriteLine("Range: " + range.min + " -> " + range.max);
                if (range.min != -1) {
                    ranges.Add(range);
                }

                // Account for actual beacons in range.
                if (command.Item4 == yToCheck) {
                    beaconsAtYToCheck.Add(command.Item3);
                }

                maxX = Int32.Max(maxX, range.max);
                minX = Int32.Min(minX, range.min);
            }

            int result1 = 0;
            for (int i = minX; i <= maxX; ++i) {
                foreach (Range r in ranges) {
                    if (i >= r.min && i <= r.max) {
                        ++result1;
                        break;
                    }
                }
            }
            result1 -= beaconsAtYToCheck.Count();

            int x2 = -1;
            int y2 = -1;
            int yCheck = 0;
            while (x2 == -1 && y2 == -1) {
                List<Range> ranges2 = new List<Range>();
                foreach (string line in content) {
                    (int, int, int, int) command = parseCommand(line);
                    
                    Range range = convertToRangeAt(yCheck, command.Item1, command.Item2, command.Item3, command.Item4);
                    if (range.min != -1) {
                        ranges.Add(range);
                    }

                    maxX = 4000000;
                    minX = 0;

                    for (int i = minX; i <= maxX; ++i) {
                        Boolean found = false;
                        foreach (Range r in ranges) {
                            if (i >= r.min && i <= r.max) {
                                found = true;
                                break;
                            }
                        }
                        if (!found) {
                            x2 = i;
                            y2 = yCheck;
                        }
                    }
                }
                ++yCheck;
            }

            int result2 = x2 * 4000000 + y2;
            Console.WriteLine("Part 1 result: " + result1);
            Console.WriteLine("Part 2 result: " + result2);
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