using System.Diagnostics;
using System.Dynamic;
using System.Security;

namespace challenge
{
    static partial class Challenge06
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

        struct MapInformation {
            public Tuple<Int32, Int32> dimensions;
            public Tuple<Int32, Int32> startPosition;
            public HashSet<Tuple<Int32,Int32>> obstacles;
            public HashSet<Tuple<Int32,Int32>> visited;
            public HashSet<Tuple<Int32,Int32>> possibleNewObstructions;
            private readonly bool[] _vdarray;

            enum Direction {
                UP,
                RIGHT,
                DOWN,
                LEFT
            }

            public MapInformation(string[] input) {
                dimensions = Tuple.Create(input[0].Length, input.Length);
                obstacles = new();
                visited = new();
                startPosition = Tuple.Create(-1, -1);
                possibleNewObstructions = new();
                _vdarray = new bool[dimensions.Item1 * dimensions.Item2 * 4];
                for (int y = 0; y < dimensions.Item2; ++y) {
                    for (int x = 0; x < dimensions.Item1; ++x) {
                        if (input[y][x] == '^') {
                            startPosition = Tuple.Create(x,y);
                        }
                        if (input[y][x] == '#') {
                            obstacles.Add(Tuple.Create(x,y));
                        }
                    }
                }

                Walk();
                FindPossibleObstacles();
            }

            private readonly void Walk() {
                Direction direction = Direction.UP;
                Tuple<Int32, Int32> position = startPosition;
                while (position.Item1 != -1 && position.Item2 != -1) {
                    visited.Add(position);
                    var next = NextPosition(position.Item1, position.Item2, direction);
                    position = next.Item1;
                    direction = next.Item2;
                }
            }

            private readonly (Tuple<Int32, Int32>, Direction) NextPosition(Int32 x, Int32 y, Direction direction) {
                var end = (Tuple.Create(-1, -1), direction);
                Tuple<Int32, Int32> next = direction switch {
                    Direction.UP => Tuple.Create(x, y - 1),
                    Direction.RIGHT => Tuple.Create(x + 1, y),
                    Direction.DOWN => Tuple.Create(x, y + 1),
                    Direction.LEFT => Tuple.Create(x - 1, y),
                    _ => throw new NotImplementedException()
                };
                if (!WithinMapLimits(next)) {
                    // Reached end
                    return end;
                }

                if (obstacles.Contains(next)) {
                    Direction nextDirection = direction switch {
                        Direction.UP => Direction.RIGHT,
                        Direction.RIGHT => Direction.DOWN,
                        Direction.DOWN => Direction.LEFT,
                        Direction.LEFT => Direction.UP,
                        _ => throw new NotImplementedException()
                    };
                    return NextPosition(x, y, nextDirection);
                } else return (next, direction);
            }

            private readonly Boolean WithinMapLimits(Tuple<Int32, Int32> pos) {
                return WithinMapLimits(pos.Item1, pos.Item2);
            }

            private readonly Boolean WithinMapLimits(Int32 x, Int32 y) {
                return x >= 0 && x < dimensions.Item1 && y >= 0 && y < dimensions.Item2;
            }

            private readonly void FindPossibleObstacles() {
                // Only need to check positions that are visited in the original path.
                foreach (Tuple<Int32, Int32> pos in visited) {
                    CheckObstaclePosition(pos);
                }
            }

            private readonly void CheckObstaclePosition(Tuple<Int32, Int32> candidate) {
                if (obstacles.Contains(candidate)) {
                    // Don't try positions of existing obstacles.
                    return;
                }

                // Temporarily add the candidate to the obstacle list, then check if the walking path loops
                obstacles.Add(candidate);
                if (WalkLoopCheck()) {
                    possibleNewObstructions.Add(candidate);
                }

                obstacles.Remove(candidate);
            }

            private readonly bool WalkLoopCheck() {
                // Use preinitialized array for memory optimization.
                bool[] visitedMap = _vdarray;
                Array.Clear(visitedMap, 0, visitedMap.Length);

                Direction direction = Direction.UP;
                Tuple<Int32, Int32> position = startPosition;
                while (position.Item1 != -1 && position.Item2 != -1) {
                    int dimOffset = direction switch
                        {
                            Direction.UP => 0,
                            Direction.RIGHT => 1,
                            Direction.DOWN => 2,
                            Direction.LEFT => 3,
                            _ => throw new NotImplementedException()
                        };
                    int visitedMapPosition = position.Item2 * dimensions.Item1 * 4 + position.Item1 * 4 + dimOffset;
                    if (visitedMap[visitedMapPosition]) {
                        return true;
                    } else {
                        visitedMap[visitedMapPosition] = true;
                    }

                    var next = NextPosition(position.Item1, position.Item2, direction);
                    position = next.Item1;
                    direction = next.Item2;
                }
                return false;
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            MapInformation mapExample = new(contentEx);
            MapInformation map = new(content);

            Debug.Assert(PartOne(mapExample) == 41);
            Console.WriteLine($"Part 1 : {PartOne(map)}");
            Debug.Assert(PartTwo(mapExample) == 6);
            Console.WriteLine($"Part 2 : {PartTwo(map)}");
        }

        static Int32 PartOne(MapInformation map)
        {
            return map.visited.Count;
        }

        static Int32 PartTwo(MapInformation map)
        {
            return map.possibleNewObstructions.Count;
        }
    }
}
