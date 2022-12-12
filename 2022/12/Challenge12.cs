using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge12
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

            Puzzle puzzle = new Puzzle(content);

            Console.WriteLine("Part 1 result: " + puzzle.solveA());
            Console.WriteLine("Part 2 result: " + puzzle.solveB());
        }
    }

    class Puzzle {
        public string[] input {get;}
        public int[][] distanceToGoal {get;}
        int xlen {get;}
        int ylen {get;}
        public Point goal {get;}
        public Point start {get;}

        public Puzzle(String[] input) {
            this.xlen = input[0].Count();
            this.ylen = input.Count();
            int size = ylen * xlen;
            this.input = input;
            this.distanceToGoal = new int[xlen][];

            for (int x = 0; x < xlen; ++x) {
                this.distanceToGoal[x] = new int[ylen];
                for (int y = 0; y < ylen; ++y) {
                    this.distanceToGoal[x][y] = Int32.MaxValue;
                }
            }

            int targets = 2;
            for (int x = 0; x < xlen; ++x) {
                for (int y = 0; y < ylen; ++y) {
                    if (input[y][x] == 'E') {
                        goal = new Point(x,y);
                        input[y] = input[y].Replace('E', 'z');
                        --targets;
                    }
                    if (input[y][x] == 'S') {
                        start = new Point(x,y);
                        input[y] = input[y].Replace('S', 'a');
                        --targets;
                    }
                    if (targets == 0) break;
                }
                if (targets == 0) break;
            }

            calcAllDistances();
        }

        private void calcAllDistances() {
            distanceToGoal[goal.x][goal.y] = 0;

            int dist = 1;
            List<Point> next = getNavigatableNeighbors(goal);
            while (next.Count != 0) {
                List<Point> newNext = new List<Point>();
                foreach (Point p in next) {
                    if (distanceToGoal[p.x][p.y] == Int32.MaxValue) {
                        distanceToGoal[p.x][p.y] = dist;
                        newNext.AddRange(getNavigatableNeighbors(p));
                    }
                }
                next = newNext;
                ++dist;
            }
        }

        private List<Point> getNavigatableNeighbors(Point self) {
            List<Point> navigatableNeighbors = new List<Point>();
            char myValue = input[self.y][self.x];
            if (self.x < xlen-1) {
                char eastNeighbor = input[self.y][self.x+1];
                if (myValue - eastNeighbor <= 1) {
                    navigatableNeighbors.Add(new Point(self.x+1, self.y));
                }
            }

            if (self.x > 0) {
                char westNeighbor = input[self.y][self.x-1];
                if (myValue - westNeighbor <= 1) {
                    navigatableNeighbors.Add(new Point(self.x-1, self.y));
                }
            }

            if (self.y < ylen-1) {
                char southNeighbor = input[self.y+1][self.x];
                if (myValue - southNeighbor <= 1) {
                    navigatableNeighbors.Add(new Point(self.x, self.y+1));
                }
            }

            if (self.y > 0) {
                char northNeighbor = input[self.y-1][self.x];
                if (myValue - northNeighbor <= 1) {
                    navigatableNeighbors.Add(new Point(self.x, self.y-1));
                }
            }

            return navigatableNeighbors;
        }

        public int solveA() {
            return distanceToGoal[start.x][start.y];
        }

        public int solveB() {
            int bestResult = Int32.MaxValue;
            for (int x = 0; x < xlen; ++x) {
                for (int y = 0; y < ylen; ++y) {
                    if (input[y][x] == 'a') {
                        bestResult = Int32.Min(bestResult, distanceToGoal[x][y]);
                    }
                }
            }
            return bestResult;
        }

    }

    struct Point {
        public int x {get; set;}
        public int y {get; set;}

        public Point(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }
}