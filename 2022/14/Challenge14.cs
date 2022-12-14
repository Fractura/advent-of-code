using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge14
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

            Reservoir r = new Reservoir();
            r.applyInputToMap(content);
            Reservoir2 r2 = new Reservoir2();
            r2.applyInputToMap(content);

            Console.WriteLine("Part 1 result: " + r.dropSandUntilYouNoLongerCan());
            Console.WriteLine("Part 2 result: " + r2.dropSandUntilYouNoLongerCan());
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

    class Reservoir {
        int[][] map = new int[200][]; // 0 = empty, 1 = rock, 2 = sand
        int xOffset = 400;
        
        public Reservoir() {
            for (int i = 0; i < 200; ++i) {
                map[i] = new int[200];
            }
        }

        public int dropSandUntilYouNoLongerCan() {
            int count = 0;
            while (dropSand()) {
                ++count;
                drawMap();
            }

            return count;
        }

        private Boolean dropSand() {
            int x = 100;
            int y = 0;
            while (true) {
                if (y == 199) {
                    return false;
                } else if (map[x][y+1] == 0) {
                    // drops straight down
                    ++y;
                } else if (map[x-1][y+1] == 0) {
                    // step down and to the left
                    --x;
                    ++y;
                } else if (map[x+1][y+1] == 0) {
                    // step down and to the right
                    ++x;
                    ++y;
                } else {
                    // stop
                    map[x][y] = 2;
                    return true;
                }
            }
        }

        public void applyInputToMap(string[] input) {
            foreach (string line in input) {
                string[] formationPoints = line.Split(" -> ");
                int x = 0;
                int y = 0;
                foreach (string point in formationPoints) {
                    string[] split = point.Split(",");
                    int targetX = Int32.Parse(split[0]) - xOffset;
                    int targetY = Int32.Parse(split[1]);

                    if (x == 0 && y == 0) {
                        x = targetX;
                        y = targetY;
                        map[x][y] = 1;
                    } else {
                        if (targetX == x) {
                            if (targetY > y) {
                                while (y < targetY) {
                                    ++y;
                                    map[x][y] = 1;
                                }
                            } else {
                                while (y > targetY) {
                                    --y;
                                    map[x][y] = 1;
                                }
                            }
                        } else {
                            if (targetX > x) {
                                while (x < targetX) {
                                    ++x;
                                    map[x][y] = 1;
                                }
                            } else {
                                while (x > targetX) {
                                    --x;
                                    map[x][y] = 1;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void drawMap() {
            String sw = "";
            for (int y = 0; y < 200; ++y) {
                for (int x = 0; x < 200; ++x) {
                    switch (map[x][y]) {
                        case 0: sw += "."; break;
                        case 1: sw += "#"; break;
                        case 2: sw += "o"; break;
                        default: break;
                    }
                }
                sw += "\n";
            }
            File.WriteAllText("output.txt", sw);
        }
    }

    class Reservoir2 {
        int[][] map = new int[1000][]; // 0 = empty, 1 = rock, 2 = sand
        
        public Reservoir2() {
            for (int i = 0; i < 1000; ++i) {
                map[i] = new int[200];
            }
        }

        public int dropSandUntilYouNoLongerCan() {
            int count = 0;
            while (dropSand()) {
                ++count;
                //drawMap();
            }

            return count;
        }

        private Boolean dropSand() {
            int x = 500;
            int y = 0;
            while (true) {
                if (map[500][0] > 0) {
                    return false;
                } else if (map[x][y+1] == 0) {
                    // drops straight down
                    ++y;
                } else if (map[x-1][y+1] == 0) {
                    // step down and to the left
                    --x;
                    ++y;
                } else if (map[x+1][y+1] == 0) {
                    // step down and to the right
                    ++x;
                    ++y;
                } else {
                    // stop
                    map[x][y] = 2;
                    return true;
                }
            }
        }

        public void applyInputToMap(string[] input) {
            int highestY = 0;
            foreach (string line in input) {
                string[] formationPoints = line.Split(" -> ");
                int x = 0;
                int y = 0;
                foreach (string point in formationPoints) {
                    string[] split = point.Split(",");
                    int targetX = Int32.Parse(split[0]);
                    int targetY = Int32.Parse(split[1]);

                    if (x == 0 && y == 0) {
                        x = targetX;
                        y = targetY;
                        map[x][y] = 1;
                        if (y > highestY) highestY = y;
                    } else {
                        if (targetX == x) {
                            if (targetY > y) {
                                while (y < targetY) {
                                    ++y;
                                    map[x][y] = 1;
                                    if (y > highestY) highestY = y;
                                }
                            } else {
                                while (y > targetY) {
                                    --y;
                                    map[x][y] = 1;
                                    if (y > highestY) highestY = y;
                                }
                            }
                        } else {
                            if (targetX > x) {
                                while (x < targetX) {
                                    ++x;
                                    map[x][y] = 1;
                                    if (y > highestY) highestY = y;
                                }
                            } else {
                                while (x > targetX) {
                                    --x;
                                    map[x][y] = 1;
                                    if (y > highestY) highestY = y;
                                }
                            }
                        }
                    }
                }
            }
            for (int x = 0; x < 1000; ++x) {
                map[x][highestY+2] = 1;
            }
        }

        public void drawMap() {
            String sw = "";
            for (int y = 0; y < 200; ++y) {
                for (int x = 0; x < 1000; ++x) {
                    switch (map[x][y]) {
                        case 0: sw += "."; break;
                        case 1: sw += "#"; break;
                        case 2: sw += "o"; break;
                        default: break;
                    }
                }
                sw += "\n";
            }
            File.WriteAllText("output.txt", sw);
        }
    }
}