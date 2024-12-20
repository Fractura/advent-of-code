using System.Diagnostics;

namespace challenge
{
    static partial class Challenge20
    {
        static string[] ReadFile(string filename)
        {
            var s = Directory.GetCurrentDirectory();
            if (s.Contains("bin")) {
                s = s[..^16];
            }
            string path = $"{s}\\{filename}";
            
            if (!File.Exists(path)) {
                Console.WriteLine("Input file not found");
            }

            StreamReader reader = File.OpenText(path);
            string[] content = reader.ReadToEnd().Split("\r\n");

            return content;
        }

        class Racetrack {
            readonly (int x, int y) start;
            readonly int[,] map;
            readonly Dictionary<int, int> cheatEfficiency;
            readonly Dictionary<int, int> cheatEfficiency2;

            public Racetrack(string[] input) {
                map = new int[input.Length, input.Length];
                start = (-1,-1);
                cheatEfficiency = new();
                cheatEfficiency2 = new();

                for (int y = 0; y < input.Length; ++y) {
                    string line = input[y];
                    for (int x = 0; x < input.Length; ++x) {
                        switch (line[x]) {
                            case '#':
                                map[x,y] = -1;
                                break;
                            case 'S':
                                start = (x,y);
                                break;
                        }
                    }
                }
                CalculateSteps();
            }

            private bool InMap((int x, int y) p) {
                return p.x >= 0 && p.y >= 0 && p.x < map.GetLength(0) && p.y < map.GetLength(1);
            }

            private void CalculateSteps() {
                Stack<(int x, int y, int score)> stack = new();
                stack.Push((start.x,start.y,1));
                while (stack.Count > 0) {
                    var (x, y, score) = stack.Pop();
                    map[x,y] = score;
                    (int x, int y) next = (x+1, y);
                    int nextScore = score + 1;
                    if (InMap(next) && map[next.x, next.y] == 0) {
                        stack.Push((next.x, next.y, nextScore));
                    }
                    next = (x-1, y);
                    if (InMap(next) && map[next.x, next.y] == 0) {
                        stack.Push((next.x, next.y, nextScore));
                    }
                    next = (x, y+1);
                    if (InMap(next) && map[next.x, next.y] == 0) {
                        stack.Push((next.x, next.y, nextScore));
                    }
                    next = (x, y-1);
                    if (InMap(next) && map[next.x, next.y] == 0) {
                        stack.Push((next.x, next.y, nextScore));
                    }
                }
                
                
            }

            public int GetPart1(int minEff) {
                for (int y = 0; y < map.GetLength(1); ++y) {
                    for (int x = 0; x < map.GetLength(0); ++x) {
                        CalculatePotentialCheats((x,y));
                    }
                }
                return cheatEfficiency.Where(x => x.Key >= minEff).Select(x => x.Value).Sum();
            }

            public int GetPart2(int minEff, int range) {
                for (int y = 0; y < map.GetLength(1); ++y) {
                    for (int x = 0; x < map.GetLength(0); ++x) {
                        CalculatePotentialCheats2((x,y), range, minEff);
                    }
                }
                return cheatEfficiency2.Select(x => x.Value).Sum();
            }

            private void CalculatePotentialCheats((int x, int y) p) {
                if (!InMap(p)) {
                    return;
                }
                int thisScore = map[p.x,p.y];
                if (thisScore == -1) {
                    return;
                }
                (int x, int y) next = (p.x+2, p.y);
                if (InMap(next) && map[next.x, next.y] - thisScore > 2) {
                    AddToCheatEfficiencyDict(map[next.x, next.y] - thisScore - 2);
                }
                next = (p.x-2, p.y);
                if (InMap(next) && map[next.x, next.y] - thisScore > 2) {
                    AddToCheatEfficiencyDict(map[next.x, next.y] - thisScore - 2);
                }
                next = (p.x, p.y+2);
                if (InMap(next) && map[next.x, next.y] - thisScore > 2) {
                    AddToCheatEfficiencyDict(map[next.x, next.y] - thisScore - 2);
                }
                next = (p.x, p.y-2);
                if (InMap(next) && map[next.x, next.y] - thisScore > 2) {
                    AddToCheatEfficiencyDict(map[next.x, next.y] - thisScore - 2);
                }
            }

            private void CalculatePotentialCheats2((int x, int y) p, int range, int minEff) {
                int thisScore = map[p.x,p.y];
                if (thisScore == -1) {
                    return;
                }
                for (int r = 2; r <= range; ++r) {
                    for (int xy_axis = 0; xy_axis < r; ++xy_axis) {
                        int xn = p.x + (r-xy_axis);
                        int yn = p.y + xy_axis;
                        if (InMap((xn, yn))) {
                            int ce = map[xn, yn] - thisScore - r;
                            AddToCheatEfficiencyDict2(ce, minEff);
                        }

                        xn = p.x - xy_axis;
                        yn = p.y + (r-xy_axis);
                        if (InMap((xn, yn))) {
                            int ce = map[xn, yn] - thisScore - r;
                            AddToCheatEfficiencyDict2(ce, minEff);
                        }

                        xn = p.x - (r-xy_axis);
                        yn = p.y - xy_axis;
                        if (InMap((xn, yn))) {
                            int ce = map[xn, yn] - thisScore - r;
                            AddToCheatEfficiencyDict2(ce, minEff);
                        }

                        xn = p.x + xy_axis;
                        yn = p.y - (r-xy_axis);
                        if (InMap((xn, yn))) {
                            int ce = map[xn, yn] - thisScore - r;
                            AddToCheatEfficiencyDict2(ce, minEff);
                        }
                    }
                }
            }

            private void AddToCheatEfficiencyDict(int gain) {
                if (cheatEfficiency.ContainsKey(gain)) {
                    cheatEfficiency[gain] += 1;
                } else {
                    cheatEfficiency[gain] = 1;
                }
            }

            private void AddToCheatEfficiencyDict2(int gain, int minEff) {
                if (gain < minEff) {
                    return;
                }
                if (cheatEfficiency2.ContainsKey(gain)) {
                    cheatEfficiency2[gain] += 1;
                } else {
                    cheatEfficiency2[gain] = 1;
                }
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Racetrack example = new(contentEx);
            Racetrack main = new(content);

            Debug.Assert(PartOne(example, 40) == 2);
            Console.WriteLine($"Part 1 : {PartOne(main, 100)}");
            Debug.Assert(PartTwo(example, 70, 20) == 41);
            Console.WriteLine($"Part 2 : {PartTwo(main, 100, 20)}");
        }

        static Int32 PartOne(Racetrack rt, int minEff)
        {
            return rt.GetPart1(minEff);
        }

        static Int32 PartTwo(Racetrack rt, int minEff, int range)
        {
            return rt.GetPart2(minEff, range);
        }
    }
}
