using System.Diagnostics;
using System.Text;

namespace challenge
{
    static partial class Challenge15
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

        class Warehouse {
            readonly int[,] map; // Wall = -1, Empty = 0, Box = 1
            (int, int) robotPosition;
            readonly List<int> moves;

            public Warehouse(string[] input) {
                map = new int[input[0].Length, input.Length];
                moves = new();

                bool parsedMap = false;
                int y = -1;
                foreach (string line in input) {
                    ++y;

                    if (line.Length < 1) {
                        parsedMap = true;
                    }

                    if (!parsedMap) {
                        int x = 0;
                        foreach (char c in line) {
                            
                            switch (c) {
                                case '#':
                                    map[x, y] = -1;
                                    break;
                                case '.':
                                    break;
                                case '@':
                                    robotPosition = (x, y);
                                    break;
                                case 'O':
                                    map[x, y] = 1;
                                    break;
                                default:
                                    break;
                            }
                            ++x;
                        }
                    } else {
                        foreach (char c in line) {
                            switch (c) {
                                case '^':
                                    moves.Add(0);
                                    break;
                                case '>':
                                    moves.Add(1);
                                    break;
                                case 'v':
                                    moves.Add(2);
                                    break;
                                case '<':
                                    moves.Add(3);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                ProcessMoves();
            }

            public void ProcessMoves() {
                foreach (int move in moves) {
                    switch (move) {
                        case 0:
                            TryMoveUp();
                            break;
                        case 1:
                            TryMoveRight();
                            break;
                        case 2:
                            TryMoveDown();
                            break;
                        case 3:
                            TryMoveLeft();
                            break;
                        default:
                            break;
                    }
                }
            }

            private void TryMoveUp() {
                int nextX = robotPosition.Item1;
                int nextY = robotPosition.Item2 - 1;
                while (map[nextX, nextY] > 0) {
                    // skip boxes
                    --nextY;
                }

                if (map[nextX, nextY] == 0) {
                    if (map[nextX, nextY + 1] == 1) {
                        map[nextX, nextY] = 1;
                    }
                    map[robotPosition.Item1, robotPosition.Item2 - 1] = 0;
                    robotPosition = (robotPosition.Item1, robotPosition.Item2 - 1);
                }
            }

            private void TryMoveDown() {
                int nextX = robotPosition.Item1;
                int nextY = robotPosition.Item2 + 1;
                while (map[nextX, nextY] > 0) {
                    // skip boxes
                    ++nextY;
                }

                if (map[nextX, nextY] == 0) {
                    if (map[nextX, nextY - 1] == 1) {
                        map[nextX, nextY] = 1;
                    }
                    map[robotPosition.Item1, robotPosition.Item2 + 1] = 0;
                    robotPosition = (robotPosition.Item1, robotPosition.Item2 + 1);
                }
            }

            private void TryMoveRight() {
                int nextX = robotPosition.Item1 + 1;
                int nextY = robotPosition.Item2;
                while (map[nextX, nextY] > 0) {
                    // skip boxes
                    ++nextX;
                }

                if (map[nextX, nextY] == 0) {
                    if (map[nextX - 1, nextY] == 1) {
                        map[nextX, nextY] = 1;
                    }
                    map[robotPosition.Item1 + 1, robotPosition.Item2] = 0;
                    robotPosition = (robotPosition.Item1 + 1, robotPosition.Item2);
                }
            }

            private void TryMoveLeft() {
                int nextX = robotPosition.Item1 - 1;
                int nextY = robotPosition.Item2;
                while (map[nextX, nextY] > 0) {
                    // skip boxes
                    --nextX;
                }

                if (map[nextX, nextY] == 0) {
                    if (map[nextX + 1, nextY] == 1) {
                        map[nextX, nextY] = 1;
                    }
                    map[robotPosition.Item1 - 1, robotPosition.Item2] = 0;
                    robotPosition = (robotPosition.Item1 - 1, robotPosition.Item2);
                }
            }

            public Int64 CalculateScores() {
                Int64 score = 0;
                for (int y = 0; y < map.GetLength(1); ++y) {
                    for (int x = 0; x < map.GetLength(0); ++x) {
                        if (map[x, y] == 1) {
                            score += x + 100 * y;
                        }
                    }
                }
                return score;
            }
        }

        class Warehouse2 {
            readonly int[,] map; // Wall = -1, Empty = 0, Box = 1, Box-Right-Half = 2
            (int, int) robotPosition;
            readonly List<int> moves;

            public Warehouse2(string[] input) {
                map = new int[input[0].Length * 2, input.Length];
                moves = new();

                bool parsedMap = false;
                int y = -1;
                foreach (string line in input) {
                    ++y;

                    if (line.Length < 1) {
                        parsedMap = true;
                    }

                    if (!parsedMap) {
                        int x = 0;
                        foreach (char c in line) {
                            
                            switch (c) {
                                case '#':
                                    map[x, y] = -1;
                                    map[x+1, y] = -1;
                                    break;
                                case '.':
                                    break;
                                case '@':
                                    robotPosition = (x, y);
                                    break;
                                case 'O':
                                    map[x, y] = 1;
                                    map[x+1, y] = 2;
                                    break;
                                default:
                                    break;
                            }
                            x += 2;
                        }
                    } else {
                        foreach (char c in line) {
                            switch (c) {
                                case '^':
                                    moves.Add(0);
                                    break;
                                case '>':
                                    moves.Add(1);
                                    break;
                                case 'v':
                                    moves.Add(2);
                                    break;
                                case '<':
                                    moves.Add(3);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                ProcessMoves();
            }

            public string PrintMap() {
                StringBuilder sb = new();
                for (int y = 0; y < map.GetLength(1); ++y) {
                    for (int x = 0; x < map.GetLength(0); ++x) {
                        if (robotPosition.Item1 == x && robotPosition.Item2 == y) {
                            sb.Append('@');
                        } else {
                            switch (map[x,y]) {
                                case -1:
                                    sb.Append('#');
                                    break;
                                case 0:
                                    sb.Append('.');
                                    break;
                                case 1:
                                    sb.Append('[');
                                    break;
                                case 2:
                                    sb.Append(']');
                                    break;
                            }
                        }
                    }
                    sb.Append('\n');
                }
                return sb.ToString();
            }

            public void ProcessMoves() {
                int c = 0;
                foreach (int move in moves) {
                    ++c;
                    switch (move) {
                        case 0:
                            TryMoveUp();
                            break;
                        case 1:
                            TryMoveRight();
                            break;
                        case 2:
                            TryMoveDown();
                            break;
                        case 3:
                            TryMoveLeft();
                            break;
                        default:
                            break;
                    }
                }
            }

            private void TryMoveUp() {
                int nextX = robotPosition.Item1;
                int nextY = robotPosition.Item2 - 1;

                switch (map[nextX, nextY]) {
                    case -1:
                        // Wall. Stop.
                        return;
                    case 0:
                        // Empty, just move.
                        robotPosition = (nextX, nextY);
                        break;
                    case 1:
                        // Box, left half -> try move box
                        if (CanMoveBoxUp(nextX, nextY)) {
                            MoveBoxUp(nextX, nextY);
                            robotPosition = (nextX, nextY);
                        }
                        break;
                    case 2:
                        // Box, right half -> try move box
                        if (CanMoveBoxUp(nextX-1, nextY)) {
                            MoveBoxUp(nextX-1, nextY);
                            robotPosition = (nextX, nextY);
                        }
                        break;
                    default:
                        break;
                }
            }

            private bool CanMoveBoxUp(int x, int y) {
                if (map[x, y-1] == -1 || map[x+1, y-1] == -1) {
                    // Wall blocks movement
                    return false;
                }

                if (map[x, y-1] == 0 && map[x+1, y-1] == 0) {
                    // Empty spaces
                    return true;
                }

                bool lbox = map[x, y-1] == 0;
                bool rbox = map[x+1, y-1] == 0;
                if (map[x, y-1] == 1) {
                    // .[].
                    // .[].
                    // .^..
                    lbox = CanMoveBoxUp(x, y-1);
                    rbox = lbox;
                }
                if (map[x, y-1] == 2) {
                    // []..
                    // .[].
                    // .^..
                    lbox = CanMoveBoxUp(x-1, y-1);
                }
                if (map[x+1, y-1] == 1) {
                    // ..[]
                    // .[].
                    // .^..
                    rbox = CanMoveBoxUp(x+1, y-1);
                }
                return lbox && rbox;
            }

            private void MoveBoxUp(int x, int y) {
                if (map[x, y-1] == 1) {
                    // .[].
                    // .[].
                    // .^..
                    MoveBoxUp(x, y-1);
                }
                if (map[x, y-1] == 2) {
                    // []..
                    // .[].
                    // .^..
                    MoveBoxUp(x-1, y-1);
                }
                if (map[x+1, y-1] == 1) {
                    // ..[]
                    // .[].
                    // .^..
                    MoveBoxUp(x+1, y-1);
                }

                map[x, y-1] = 1;
                map[x+1, y-1] = 2;
                map[x, y] = 0;
                map[x+1, y] = 0;
            }

            private void TryMoveDown() {
                int nextX = robotPosition.Item1;
                int nextY = robotPosition.Item2 + 1;

                switch (map[nextX, nextY]) {
                    case -1:
                        // Wall. Stop.
                        return;
                    case 0:
                        // Empty, just move.
                        robotPosition = (nextX, nextY);
                        break;
                    case 1:
                        // Box, left half -> try move box
                        if (CanMoveBoxDown(nextX, nextY)) {
                            MoveBoxDown(nextX, nextY);
                            robotPosition = (nextX, nextY);
                        }
                        break;
                    case 2:
                        // Box, right half -> try move box
                        if (CanMoveBoxDown(nextX-1, nextY)) {
                            MoveBoxDown(nextX-1, nextY);
                            robotPosition = (nextX, nextY);
                        }
                        break;
                    default:
                        break;
                }
            }

            private bool CanMoveBoxDown(int x, int y) {
                if (map[x, y+1] == -1 || map[x+1, y+1] == -1) {
                    // Wall blocks movement
                    return false;
                }

                if (map[x, y+1] == 0 && map[x+1, y+1] == 0) {
                    // Empty spaces
                    return true;
                }

                bool lbox = map[x, y+1] == 0;
                bool rbox = map[x+1, y+1] == 0;
                if (map[x, y+1] == 1) {
                    // .v..
                    // .[].
                    // .[].
                    lbox = CanMoveBoxDown(x, y+1);
                    rbox = lbox;
                }
                if (map[x, y+1] == 2) {
                    // .v..
                    // .[].
                    // []..
                    lbox = CanMoveBoxDown(x-1, y+1);
                }
                if (map[x+1, y+1] == 1) {
                    // .v..
                    // .[].
                    // ..[]
                    rbox = CanMoveBoxDown(x+1, y+1);
                }
                return lbox && rbox;
            }

            private void MoveBoxDown(int x, int y) {
                if (map[x, y+1] == 1) {
                    // .v..
                    // .[].
                    // .[].
                    MoveBoxDown(x, y+1);
                }
                if (map[x, y+1] == 2) {
                    // .v..
                    // .[].
                    // []..
                    MoveBoxDown(x-1, y+1);
                }
                if (map[x+1, y+1] == 1) {
                    // .v..
                    // .[].
                    // ..[]
                    MoveBoxDown(x+1, y+1);
                }

                map[x, y+1] = 1;
                map[x+1, y+1] = 2;
                map[x, y] = 0;
                map[x+1, y] = 0;
            }

            private void TryMoveRight() {
                int nextX = robotPosition.Item1 + 1;
                int nextY = robotPosition.Item2;

                switch (map[nextX, nextY]) {
                    case -1:
                        // Wall. Stop.
                        return;
                    case 0:
                        // Empty, just move.
                        robotPosition = (nextX, nextY);
                        break;
                    case 1:
                        // Box, left half -> try move box
                        if (CanMoveBoxRight(nextX, nextY)) {
                            MoveBoxRight(nextX, nextY);
                            robotPosition = (nextX, nextY);
                        }
                        break;
                    case 2:
                        // Invalid!
                        Console.WriteLine("Invalid state detected.");
                        break;
                    default:
                        break;
                }
            }

            private bool CanMoveBoxRight(int x, int y) {
                if (map[x+2, y] == -1) {
                    // Wall blocks movement
                    return false;
                }

                if (map[x+2, y] == 0) {
                    // Empty space
                    return true;
                }

                if (map[x+2, y] == 1) {
                    return CanMoveBoxRight(x+2, y);
                }

                return false;
            }

            private void MoveBoxRight(int x, int y) {
                if (map[x+2, y] == 1) {
                    // >[][].
                    MoveBoxRight(x+2, y);
                }

                map[x+1, y] = 1;
                map[x+2, y] = 2;
                map[x, y] = 0;
            }

            private void TryMoveLeft() {
                int nextX = robotPosition.Item1 - 1;
                int nextY = robotPosition.Item2;

                switch (map[nextX, nextY]) {
                    case -1:
                        // Wall. Stop.
                        return;
                    case 0:
                        // Empty, just move.
                        robotPosition = (nextX, nextY);
                        break;
                    case 1:
                        // Invalid!
                        Console.WriteLine("Invalid state detected.");
                        break;
                    case 2:
                        // Box, right half -> try move box
                        if (CanMoveBoxLeft(nextX-1, nextY)) {
                            MoveBoxLeft(nextX-1, nextY);
                            robotPosition = (nextX, nextY);
                        }
                        break;
                    default:
                        break;
                }
            }

            private bool CanMoveBoxLeft(int x, int y) {
                if (map[x-1, y] == -1) {
                    // Wall blocks movement
                    return false;
                }

                if (map[x-1, y] == 0) {
                    // Empty space
                    return true;
                }

                if (map[x-1, y] == 2) {
                    return CanMoveBoxLeft(x-2, y);
                }

                return false;
            }

            private void MoveBoxLeft(int x, int y) {
                if (map[x-2, y] == 1) {
                    // .[][]<
                    MoveBoxLeft(x-2, y);
                }

                map[x-1, y] = 1;
                map[x, y] = 2;
                map[x+1, y] = 0;
            }

            public Int64 CalculateScores() {
                Int64 score = 0;
                for (int y = 0; y < map.GetLength(1); ++y) {
                    for (int x = 0; x < map.GetLength(0); ++x) {
                        if (map[x, y] == 1) {
                            score += x + 100 * y;
                        }
                    }
                }
                return score;
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx1 = ReadFile("example1.input");
            string[] contentEx2 = ReadFile("example2.input");
            string[] content = ReadFile("my.input");

            Warehouse example1 = new(contentEx1);
            Warehouse example2 = new(contentEx2);
            Warehouse puzzle = new(content);

            Debug.Assert(PartOne(example1) == 2028);
            Debug.Assert(PartOne(example2) == 10092);
            Console.WriteLine($"Part 1 : {PartOne(puzzle)}");

            Warehouse2 example2_2 = new(contentEx2);
            Warehouse2 puzzle_2 = new(content);
            Debug.Assert(PartTwo(example2_2) == 9021);
            Console.WriteLine($"Part 2 : {PartTwo(puzzle_2)}");
        }

        static Int64 PartOne(Warehouse warehouse)
        {
            return warehouse.CalculateScores();
        }

        static Int64 PartTwo(Warehouse2 warehouse)
        {
            return warehouse.CalculateScores();
        }
    }
}
