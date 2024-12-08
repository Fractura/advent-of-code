using System.Diagnostics;

namespace challenge
{
    static partial class Challenge08
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

        struct AntennaMap {
            public Dictionary<char, List<(Int32, Int32)>> antennaPositions;
            public HashSet<(Int32, Int32)> antinodes;
            public HashSet<(Int32, Int32)> antinodes2;
            public Int32 xDim;
            public Int32 yDim;

            public AntennaMap(string[] input) {
                yDim = input.Length;
                xDim = input[0].Length;
                antennaPositions = new();
                antinodes = new();
                antinodes2 = new();

                for (int y = 0; y < yDim; ++y) {
                    for (int x = 0; x < xDim; ++x) {
                        char c = input[y][x];
                        if (c == '.') {
                            continue;
                        }
                        if (antennaPositions.ContainsKey(c)) {
                            antennaPositions[c].Add((x,y));
                        } else {
                            List<(Int32, Int32)> list = new()
                            {
                                (x, y)
                            };
                            antennaPositions[c] = list;
                        }
                    }
                }

                FindAntinodesP1();
                FindAntinodesP2();
            }

            private readonly void FindAntinodesP1() {
                foreach (List<(Int32, Int32)> antennaList in antennaPositions.Values) {
                    for (int a = 0; a < antennaList.Count; ++a) {
                        Int32 x_a = antennaList[a].Item1;
                        Int32 y_a = antennaList[a].Item2;
                        for (int b = a+1; b < antennaList.Count; ++b) {
                            Int32 x_b = antennaList[b].Item1;
                            Int32 y_b = antennaList[b].Item2;

                            Int32 x_delta = x_b - x_a;
                            Int32 y_delta = y_b - y_a;

                            Int32 x_antinode_1 = x_a - x_delta;
                            Int32 y_antinode_1 = y_a - y_delta;

                            Int32 x_antinode_2 = x_b + x_delta;
                            Int32 y_antinode_2 = y_b + y_delta;

                            if (WithinDim(x_antinode_1, y_antinode_1)) {
                                antinodes.Add((x_antinode_1, y_antinode_1));
                            }

                            if (WithinDim(x_antinode_2, y_antinode_2)) {
                                antinodes.Add((x_antinode_2, y_antinode_2));
                            }
                        }
                    }
                }
            }

            private readonly void FindAntinodesP2() {
                foreach (List<(Int32, Int32)> antennaList in antennaPositions.Values) {
                    for (int a = 0; a < antennaList.Count; ++a) {
                        Int32 x_a = antennaList[a].Item1;
                        Int32 y_a = antennaList[a].Item2;
                        antinodes2.Add((x_a, y_a));
                        for (int b = a+1; b < antennaList.Count; ++b) {
                            Int32 x_b = antennaList[b].Item1;
                            Int32 y_b = antennaList[b].Item2;

                            Int32 x_delta = x_b - x_a;
                            Int32 y_delta = y_b - y_a;

                            Int32 x_line = x_b;
                            Int32 y_line = y_b;
                            while(WithinDim(x_line, y_line)) {
                                antinodes2.Add((x_line, y_line));
                                x_line += x_delta;
                                y_line += y_delta;
                            }

                            x_line = x_a - x_delta;
                            y_line = y_a - y_delta;
                            while(WithinDim(x_line, y_line)) {
                                antinodes2.Add((x_line, y_line));
                                x_line -= x_delta;
                                y_line -= y_delta;
                            }
                        }
                    }
                }
            }

            public readonly Boolean WithinDim(Int32 x, Int32 y) {
                return x >= 0 && y >= 0 && x < xDim && y < yDim;
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            AntennaMap antennaMapEx = new(contentEx);
            AntennaMap antennaMap = new(content);

            Debug.Assert(PartOne(antennaMapEx) == 14);
            Console.WriteLine($"Part 1 : {PartOne(antennaMap)}");
            Debug.Assert(PartTwo(antennaMapEx) == 34);
            Console.WriteLine($"Part 2 : {PartTwo(antennaMap)}");
        }

        static Int32 PartOne(AntennaMap map)
        {
            return map.antinodes.Count;
        }

        static Int32 PartTwo(AntennaMap map)
        {
            return map.antinodes2.Count;
        }
    }
}
