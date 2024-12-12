using System.Diagnostics;
using System.Formats.Asn1;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace challenge
{
    static partial class Challenge12
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

        class Garden
        {
            readonly char[,] garden;
            readonly (int, int) dim;
            readonly HashSet<char> plantTypes;

            public Garden(string[] input) {
                dim = (input[0].Length, input.Length);
                garden = new char[dim.Item1, dim.Item2];
                plantTypes = [];
                for (int y = 0; y < dim.Item2; ++y) {
                    char[] charArray = input[y].ToCharArray();
                    for (int x = 0; x < dim.Item1; ++x) {
                        char plantType = charArray[x];
                        garden[x,y] = plantType;
                        plantTypes.Add(plantType);
                    }
                }
            }

            private static Boolean WithinGardenDimensions(Garden g, int x, int y) {
                return x >= 0 && y >= 0 && x < g.garden.GetLength(0) && y < g.garden.GetLength(1);
            }

            public static Int32 GetCost(Garden g, bool discount) {
                Int32 sum = 0;
                foreach (char type in g.plantTypes) {
                    sum += GetCostForPlantType(g, type, discount);
                }
                return sum;
            }

            private static Int32 GetCostForPlantType(Garden g, char plantType, bool discount) {
                return GetAllRegionsCost(g, plantType, discount);
            }

            static Boolean[,] visited = new bool[0,0];
            static Boolean[,] northFences = new bool[0,0];
            static Boolean[,] eastFences = new bool[0,0];
            static Boolean[,] southFences = new bool[0,0];
            static Boolean[,] westFences = new bool[0,0];

            private static Int32 GetAllRegionsCost(Garden g, char plantType, bool discount) {
                Int32 sum = 0;
                visited = new bool[g.dim.Item1, g.dim.Item2];
                for (int x = 0; x < g.dim.Item1; ++x) {
                    for (int y = 0; y < g.dim.Item2; ++y) {
                        if (g.garden[x,y] == plantType && !visited[x,y]) {
                            (Int32, Int32) costs = GetRegionCost(g, plantType, x, y, discount);
                            //Console.WriteLine($"Cost of region, type {plantType}: {costs.Item2} * {costs.Item1} = {costs.Item1 * costs.Item2}");
                            sum += costs.Item1 * costs.Item2;
                        }
                    }
                }
                return sum;
            }

            private static (Int32, Int32) GetRegionCost(Garden g, char plantType, int x, int y, bool discount) {
                northFences = new bool[g.dim.Item1,g.dim.Item2];
                eastFences = new bool[g.dim.Item1,g.dim.Item2];
                southFences = new bool[g.dim.Item1,g.dim.Item2];
                westFences = new bool[g.dim.Item1,g.dim.Item2];
                var res = GetRegionCostWithoutDiscount(g, plantType, x, y);
                if (discount) {
                    int discountedFences = 0;
                    discountedFences += GetNSFences(g, northFences);
                    discountedFences += GetNSFences(g, southFences);
                    discountedFences += GetWEFences(g, westFences);
                    discountedFences += GetWEFences(g, eastFences);
                    return (discountedFences, res.Item2);
                } else {
                    return res;
                }
            }

            private static (Int32, Int32) GetRegionCostWithoutDiscount(Garden g, char plantType, int x, int y) {
                if (visited[x,y]) {
                    return (0,0);
                }
                Int32 fence = 0;
                Int32 area = 1;
                visited[x,y] = true;
                if (WithinGardenDimensions(g, x-1, y)) {
                    if (g.garden[x-1,y] != plantType) {
                        westFences[x,y] = true;
                        fence += 1;
                    } else {
                        var c = GetRegionCostWithoutDiscount(g, plantType, x-1, y);
                        fence += c.Item1;
                        area += c.Item2;
                    }
                } else {
                    westFences[x,y] = true;
                    fence += 1;
                }

                if (WithinGardenDimensions(g, x+1, y)) {
                    if (g.garden[x+1,y] != plantType) {
                        eastFences[x,y] = true;
                        fence += 1;
                    } else {
                        var c = GetRegionCostWithoutDiscount(g, plantType, x+1, y);
                        fence += c.Item1;
                        area += c.Item2;
                    }
                } else {
                    eastFences[x,y] = true;
                    fence += 1;
                }

                if (WithinGardenDimensions(g, x, y-1)) {
                    if (g.garden[x,y-1] != plantType) {
                        northFences[x,y] = true;
                        fence += 1;
                    } else {
                        var c = GetRegionCostWithoutDiscount(g, plantType, x, y-1);
                        fence += c.Item1;
                        area += c.Item2;
                    }
                } else {
                    northFences[x,y] = true;
                    fence += 1;
                }

                if (WithinGardenDimensions(g, x, y+1)) {
                    if (g.garden[x,y+1] != plantType) {
                        southFences[x,y] = true;
                        fence += 1;
                    } else {
                        var c = GetRegionCostWithoutDiscount(g, plantType, x, y+1);
                        fence += c.Item1;
                        area += c.Item2;
                    }
                } else {
                    southFences[x,y] = true;
                    fence += 1;
                }
                return (fence, area);
            }
            
            private static Int32 GetNSFences(Garden g, Boolean[,] fences) {
                int c = 0;
                for (int y = 0; y < g.dim.Item2; ++y) {
                    bool p = false;
                    for (int x = 0; x < g.dim.Item1; ++x) {
                        if (!p && fences[x,y]) {
                            ++c;
                        }
                        p = fences[x,y];
                    }
                }
                return c;
            }

            private static Int32 GetWEFences(Garden g, Boolean[,] fences) {
                int c = 0;
                for (int x = 0; x < g.dim.Item1; ++x) {
                    bool p = false;
                    for (int y = 0; y < g.dim.Item2; ++y) {
                        if (!p && fences[x,y]) {
                            ++c;
                        }
                        p = fences[x,y];
                    }
                }
                return c;
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx1 = ReadFile("example.input");
            string[] contentEx2 = ReadFile("example2.input");
            string[] content = ReadFile("my.input");

            Garden ex1 = new(contentEx1);
            Garden ex2 = new(contentEx2);
            Garden g = new(content);

            Debug.Assert(PartOne(ex1) == 140);
            Debug.Assert(PartOne(ex2) == 1930);
            Console.WriteLine($"Part 1 : {PartOne(g)}");
            Debug.Assert(PartTwo(ex1) == 80);
            Debug.Assert(PartTwo(ex2) == 1206);
            Console.WriteLine($"Part 2 : {PartTwo(g)}");
        }

        static Int32 PartOne(Garden g)
        {
            return Garden.GetCost(g, false);
        }

        static Int32 PartTwo(Garden g)
        {
            return Garden.GetCost(g, true);
        }
    }
}
