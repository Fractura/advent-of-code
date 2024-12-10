using System.Diagnostics;

namespace challenge
{
    static partial class Challenge10
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

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Int32[,] mapEx = CreateMap(contentEx);
            Int32[,] map = CreateMap(content);

            Debug.Assert(PartOne(mapEx) == 36);
            Console.WriteLine($"Part 1 : {PartOne(map)}");
            Debug.Assert(PartTwo(mapEx) == 81);
            Console.WriteLine($"Part 2 : {PartTwo(map)}");
        }

        static Int32[,] CreateMap(string[] input) {
            int dim = input.Length;
            Int32[,] map = new int[dim,dim];
            for (int y = 0; y < dim; ++y) {
                char[] chars = input[y].ToCharArray();
                for (int x = 0; x < dim; ++x) {
                    map[x,y] = (Int32) char.GetNumericValue(chars[x]);
                }
            }
            return map;
        }

        static Boolean InMap(Int32[,] map, Int32 x, Int32 y) {
            return x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1);
        }

        static Int32 CheckDistinctPaths(Int32[,] map, Int32 x, Int32 y) {
            if (map[x,y] == 0) {
                Int32 sum = 0;
                if (InMap(map,x-1,y)) sum += CheckDistinctPath(map, x-1, y, 0);
                if (InMap(map,x+1,y)) sum += CheckDistinctPath(map, x+1, y, 0);
                if (InMap(map,x,y-1)) sum += CheckDistinctPath(map, x, y-1, 0);
                if (InMap(map,x,y+1)) sum += CheckDistinctPath(map, x, y+1, 0);
                return sum;
            } else return 0;
        }

        static HashSet<(Int32, Int32)> temporaryResultMap = [];

        static Int32 CheckReachableNines(Int32[,] map, Int32 x, Int32 y) {
            if (map[x,y] == 0) {
                temporaryResultMap = [];
                if (InMap(map,x-1,y)) CheckPath(map, x-1, y, 0);
                if (InMap(map,x+1,y)) CheckPath(map, x+1, y, 0);
                if (InMap(map,x,y-1)) CheckPath(map, x, y-1, 0);
                if (InMap(map,x,y+1)) CheckPath(map, x, y+1, 0);
                return temporaryResultMap.Count;
            } else return 0;
        }

        static Int32 CheckDistinctPath(Int32[,] map, Int32 x, Int32 y, Int32 previous) {
            if (map[x,y] == previous+1) {
                if (previous == 8) return 1;
                Int32 sum = 0;
                if (InMap(map,x-1,y)) sum += CheckDistinctPath(map, x-1, y, previous+1);
                if (InMap(map,x+1,y)) sum += CheckDistinctPath(map, x+1, y, previous+1);
                if (InMap(map,x,y-1)) sum += CheckDistinctPath(map, x, y-1, previous+1);
                if (InMap(map,x,y+1)) sum += CheckDistinctPath(map, x, y+1, previous+1);
                return sum;
            } else return 0;
        }

        static void CheckPath(Int32[,] map, Int32 x, Int32 y, Int32 previous) {
            if (map[x,y] == previous+1) {
                if (previous == 8) {
                    temporaryResultMap.Add((x,y));
                    return;
                }
                if (InMap(map,x-1,y)) CheckPath(map, x-1, y, previous+1);
                if (InMap(map,x+1,y)) CheckPath(map, x+1, y, previous+1);
                if (InMap(map,x,y-1)) CheckPath(map, x, y-1, previous+1);
                if (InMap(map,x,y+1)) CheckPath(map, x, y+1, previous+1);
            }
        }

        static Int32 PartOne(Int32[,] map)
        {
            Int32 score = 0;
            for (int y = 0; y < map.GetLength(1); ++y) {
                for (int x = 0; x < map.GetLength(0); ++x) {
                    score += CheckReachableNines(map,x,y);
                }
            }
            return score;
        }

        static Int32 PartTwo(Int32[,] map)
        {
            Int32 score = 0;
            for (int y = 0; y < map.GetLength(1); ++y) {
                for (int x = 0; x < map.GetLength(0); ++x) {
                    score += CheckDistinctPaths(map,x,y);
                }
            }
            return score;
        }
    }
}
