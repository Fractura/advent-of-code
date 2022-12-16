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
            List<(int, int, int, int)> commands = new List<(int, int, int, int)>();
            foreach (string line in content) {
                commands.Add(parseCommand(line));
            }

            List<Range> ranges = new List<Range>();
            int yToCheck = 2000000;
            HashSet<int> beaconsAtYToCheck = new HashSet<int>();
            int minX = Int32.MaxValue;
            int maxX = Int32.MinValue;
            foreach ((int, int, int, int) command in commands) {
                //Console.WriteLine(command);
                
                Range range = convertToRangeAt(yToCheck, command.Item1, command.Item2, command.Item3, command.Item4);
                //Console.WriteLine("Range: " + range.min + " -> " + range.max);
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

            List<BeaconRange> beaconRanges = new List<BeaconRange>();
            foreach ((int, int, int, int) command in commands) {
                beaconRanges.Add(convertToBeaconRange(command.Item1, command.Item2, command.Item3, command.Item4));
            }

            Point solution = new Point(-1, -1);

            int lowerBounds = 0;
            int upperBounds = 4000000;
            HashSet<Point> relevantPoints = new();
            for (int i = 0; i < beaconRanges.Count; i++) {
                var br_1 = beaconRanges[i];

                for(int j = i+1; j < beaconRanges.Count; j++) {
                    var br_2 = beaconRanges[j];
                    
                    if(distance(new Point(br_1.x, br_1.y), new Point(br_2.x, br_2.y)) == br_1.r + br_2.r + 2) {
                        int endy = Math.Min(br_1.y + br_1.r, br_2.y + br_2.r);
                        int starty = Math.Max(br_1.y - br_1.r, br_2.y - br_2.r);

                        int startx = Math.Max(br_1.x - br_1.r, br_2.x - br_2.r);
                        int endx = Math.Min(br_1.x + br_1.r, br_2.x + br_2.r);

                        for (int y = starty; y < endy; y++) {
                            int x1 = br_1.x + (br_1.r + 1 - Math.Abs(y - br_1.y));
                            int x2 = br_1.x - (br_1.r + 1 - Math.Abs(y - br_1.y));

                            if (x1 >= lowerBounds && x1 <= upperBounds && x1 >= startx && x1 <= endx) {
                                relevantPoints.Add(new Point(x1, y));
                            }
                            if (x2 >= lowerBounds && x2 <= upperBounds && x2 >= startx && x2 <= endx) {
                                relevantPoints.Add(new Point(x2, y));
                            }
                        }
                    }
                }
            }
            foreach (Point p in relevantPoints) {
                Boolean possibleMatch = true;
                foreach (BeaconRange br in beaconRanges) {
                    if (withinBeaconRange(p.x, p.y, br)) {
                        possibleMatch = false;
                        break;
                    }
                }
                if (possibleMatch) {
                    solution = p;
                    break;
                }
            }

            Int64 result2 = solution.x * 4000000 + solution.y;
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

        static BeaconRange convertToBeaconRange(int xCenter, int yCenter, int xBeacon, int yBeacon) {
            int r = calculateRadius(xCenter, yCenter, xBeacon, yBeacon);
            return new BeaconRange(xCenter, yCenter, r);
        }

        static Int64 distance(Point a, Point b) {
            return Int64.Abs(a.x - b.x) + Int64.Abs(a.y - b.y);
        }

        static Boolean withinBeaconRange(Int64 x, Int64 y, BeaconRange br) {
            return distance(new Point(br.x, br.y), new Point(x, y)) <= br.r;
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

    struct Point {
        public Int64 x;
        public Int64 y;
        public Point(Int64 x, Int64 y) {
            this.x = x; this.y = y;
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

    struct BeaconRange {
        public int x;
        public int y;
        public int r;

        public BeaconRange(int x, int y, int r) {
            this.x = x; this.y = y; this.r = r;
        }
    }
}