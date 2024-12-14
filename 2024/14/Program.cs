using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace challenge
{
    static partial class Challenge14
    {
        static string[] ReadFile(string filename)
        {
            var s = Directory.GetCurrentDirectory();
            if (s.Contains("bin")) {
                s = s[..^16];
            }
            string path = $"{s}{filename}";
            
            if (!File.Exists(path)) {
                Console.WriteLine("Input file not found");
            }

            StreamReader reader = File.OpenText(path);
            string[] content = reader.ReadToEnd().Split("\r\n");

            return content;
        }

        class Headquarters {
            readonly List<Robot> robots;
            readonly int dimX;
            readonly int dimY;

            public Headquarters(string[] input, int dimX, int dimY) {
                robots = new();
                foreach (string l in input) {
                    robots.Add(new Robot(l));
                }
                this.dimX = dimX;
                this.dimY = dimY;
            }

            public int CalculateSafetyFactor(int turns) {
                int sector0 = 0;
                int sector1 = 0;
                int sector2 = 0;
                int sector3 = 0;
                
                foreach (Robot r in robots) {
                    int q = r.QuadrantAfter(turns, dimX, dimY);
                    switch (q) {
                        case 0: ++sector0; break;
                        case 1: ++sector1; break;
                        case 2: ++sector2; break;
                        case 3: ++sector3; break;
                    }
                }

                return sector0 * sector1 * sector2 * sector3;
            }

            public int FindFirstPotential(int start) {
                for (int i = start; i < 10000; ++i) {
                    if (CheckForPotentialDrawing(i)) {
                        return i;
                    }
                }
                return -1;
            }

            public bool CheckForPotentialDrawing(int turns) {
                HashSet<(int, int)> robotPositions = new();
                foreach (Robot r in robots) {
                    (int, int) p = r.GetPositionAfter(turns, dimX, dimY);
                    robotPositions.Add(p);
                }

                int count = 0;
                for (int i = 0; i < dimY; ++i) {
                    for (int j = 0; j < dimX; ++j) {
                        if (robotPositions.Contains((j,i))) {
                            ++count;
                            if (count > 7) {
                                return true;
                            }
                        } else {
                            count = 0;
                        }
                    }
                }
                return false;
            }

            public string DrawAfter(int turns) {
                HashSet<(int, int)> robotPositions = new();
                foreach (Robot r in robots) {
                    (int, int) p = r.GetPositionAfter(turns, dimX, dimY);
                    robotPositions.Add(p);
                }

                StringBuilder sb = new();
                for (int i = 0; i < dimY; ++i) {
                    for (int j = 0; j < dimX; ++j) {
                        if (robotPositions.Contains((j,i))) {
                            sb.Append('*');
                        } else {
                            sb.Append('.');
                        }
                    }
                    sb.Append('\n');
                }
                return sb.ToString();
            }
        }

        class Robot {
            readonly int startPositionX;
            readonly int startPositionY;
            readonly int velocityX;
            readonly int velocityY;

            public Robot(string input) {
                MatchCollection matches = RobotRegex().Matches(input);
                Debug.Assert(matches.Count > 0);
                startPositionX = int.Parse(matches[0].Groups[1].Value);
                startPositionY = int.Parse(matches[0].Groups[2].Value);
                velocityX = int.Parse(matches[0].Groups[3].Value);
                velocityY = int.Parse(matches[0].Groups[4].Value);
            }

            public (int, int) GetPositionAfter(int turns, int dimX, int dimY) {
                int x = (startPositionX + (turns * velocityX)) % dimX;
                int y = (startPositionY + (turns * velocityY)) % dimY;

                while (x < 0) x += dimX;
                while (y < 0) y += dimY;

                return (x, y);
            }

            // 0 1
            // 2 3
            public int QuadrantAfter(int turns, int dimX, int dimY) {
                (int, int) pos = GetPositionAfter(turns, dimX, dimY);

                if (pos.Item1 < (dimX / 2) && pos.Item2 < (dimY / 2)) return 0;
                if (pos.Item1 > (dimX / 2) && pos.Item2 < (dimY / 2)) return 1;
                if (pos.Item1 < (dimX / 2) && pos.Item2 > (dimY / 2)) return 2;
                if (pos.Item1 > (dimX / 2) && pos.Item2 > (dimY / 2)) return 3;
                return -1;
            }
        }
        
        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Headquarters exampleHQ = new(contentEx, 11, 7);
            Headquarters ebhq = new(content, 101, 103);

            Debug.Assert(PartOne(exampleHQ) == 12);
            Console.WriteLine($"Part 1 : {PartOne(ebhq)}");
            int r = PartTwo(ebhq);
            Console.WriteLine($"Part 2 : {r}");
            Console.WriteLine(ebhq.DrawAfter(r));
        }

        static Int32 PartOne(Headquarters hq)
        {
            return hq.CalculateSafetyFactor(100);
        }

        static Int32 PartTwo(Headquarters hq)
        {
            return hq.FindFirstPotential(0);
        }

        [GeneratedRegex("p=(\\d+),(\\d+) v=(-?\\d+),(-?\\d+)")]
        private static partial Regex RobotRegex();
    }
}
