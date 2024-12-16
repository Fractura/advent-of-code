using System.Diagnostics;

namespace challenge
{
    static partial class Challenge16
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

        class Maze {
            readonly PriorityQueue<(int, int), int> queue;
            readonly int[,] map;
            readonly int[,] dirMap; // 1 = ^/N, 2 = >/E, 3 = v/S, 4 = </W
            (int, int) startingPoint;
            (int, int) finishPoint;


            public Maze(string[] input) {
                map = new int[input[0].Length, input.Length];
                dirMap = new int[input[0].Length, input.Length];
                queue = new();
                int y = 0;
                foreach (string line in input) {
                    int x = 0;
                    foreach (char c in line) {
                        switch (c) {
                            case '#':
                                map[x,y] = -1;
                                break;
                            case '.':
                                break;
                            case 'S':
                                startingPoint = (x,y);
                                dirMap[x,y] = 2;
                                queue.Enqueue(startingPoint, 0);
                                break;
                            case 'E':
                                finishPoint = (x,y);
                                break;
                        }
                        ++x;
                    }
                    ++y;
                }
                CalculatePaths();
            }

            public void CalculatePaths() {
                while (queue.Count > 0) {
                    (int, int) p = queue.Dequeue();
                    if (p.Item1 > 0 && p.Item1 < map.GetLength(0) && p.Item2 > 0 && p.Item2 < map.GetLength(1)) {
                        bool improved = false;
                        if (p.Item1 != startingPoint.Item1 || p.Item2 != startingPoint.Item2) {
                            improved = CalculateOwn(p);
                        } else {
                            QueueNeighbors(p);
                        }
                        if (improved) {
                            QueueNeighbors(p);
                        }
                    }
                }
            }

            public int CalculateNodesInBestPaths() {
                Queue<(int,int)> q = new();
                HashSet<(int,int)> v = new();
                q.Enqueue(finishPoint);
                while (q.Count > 0) {
                    (int, int) p = q.Dequeue();
                    
                    int px = p.Item1;
                    int py = p.Item2;
                    if (v.Contains((px, py))) continue;
                    v.Add((px, py));
                    int nx = px - 1;
                    int ny = py;
                    
                    if (CheckIfPrevious(px,py,nx,ny)) {
                        q.Enqueue((nx, ny));
                    }
                    nx = px + 1;
                    ny = py;
                    
                    if (CheckIfPrevious(px,py,nx,ny)) {
                        q.Enqueue((nx, ny));
                    }
                    nx = px;
                    ny = py - 1;
                    
                    if (CheckIfPrevious(px,py,nx,ny)) {
                        q.Enqueue((nx, ny));
                    }
                    nx = px;
                    ny = py + 1;
                    
                    if (CheckIfPrevious(px,py,nx,ny)) {
                        q.Enqueue((nx, ny));
                    }
                }
                return v.Count;
            }

            private bool CheckIfPrevious(int px, int py, int nx, int ny) {
                if (map[nx,ny] == -1) return false;
                int diff = map[px,py] - map[nx,ny];
                if (diff == -999) {
                    // What's this?
                    // The best route was calculated from a turn
                    // But coming straight from the other path is a path with the same cost
                    // Therefore we check if going straight from (nx,ny) would have worked

                    /* Visually
                    .......5003..... < (nx,ny) (dir = E)
                    .......↓4004.... < (px,py) (dir = N)
                    .......↓↓5005... < (px+1,py) (dir = E)
                    .......↓↓↓......
                    ......>>^>>E....
                    ........^.......
                    */

                    switch (dirMap[nx,ny]) {
                        case 1:
                            if (dirMap[px,py-1] == 1 && map[px,py-1] == map[nx,ny] + 2) return true;
                            break;
                        case 2:
                            if (dirMap[px+1,py] == 2 && map[px+1,py] == map[nx,ny] + 2) return true;
                            break;
                        case 3:
                            if (dirMap[px,py+1] == 1 && map[px,py+1] == map[nx,ny] + 2) return true;
                            break;
                        case 4:
                            if (dirMap[px-1,py] == 1 && map[px-1,py] == map[nx,ny] + 2) return true;
                            break;
                        
                    }
                }
                return diff == 1 || diff == 1001;
            }

            private bool CalculateOwn((int,int) point) {
                if (map[point.Item1, point.Item2] < 0) {
                    return false;
                }

                int score = int.MaxValue;
                int direction = 0;
                // from west
                int neighbor_score = map[point.Item1 - 1, point.Item2];
                int neighbor_direction = dirMap[point.Item1 - 1, point.Item2];
                if (neighbor_direction != 0) {
                    int turnPenalty = (neighbor_direction % 2 == 1) ? 1000 : 0;
                    int step = neighbor_score + 1 + turnPenalty;
                    if (step < score) {
                        direction = 2;
                        score = step;
                    }
                }

                // from east
                neighbor_score = map[point.Item1 + 1, point.Item2];
                neighbor_direction = dirMap[point.Item1 + 1, point.Item2];
                if (neighbor_direction != 0) {
                    int turnPenalty = (neighbor_direction % 2 == 1) ? 1000 : 0;
                    int step = neighbor_score + 1 + turnPenalty;
                    if (step < score) {
                        direction = 4;
                        score = step;
                    }
                }

                // from north
                neighbor_score = map[point.Item1, point.Item2 - 1];
                neighbor_direction = dirMap[point.Item1, point.Item2 - 1];
                if (neighbor_direction != 0) {
                    int turnPenalty = (neighbor_direction % 2 == 0) ? 1000 : 0;
                    int step = neighbor_score + 1 + turnPenalty;
                    if (step < score) {
                        direction = 3;
                        score = step;
                    }
                }

                // from south
                neighbor_score = map[point.Item1, point.Item2 + 1];
                neighbor_direction = dirMap[point.Item1, point.Item2 + 1];
                if (neighbor_direction != 0) {
                    int turnPenalty = (neighbor_direction % 2 == 0) ? 1000 : 0;
                    int step = neighbor_score + 1 + turnPenalty;
                    if (step < score) {
                        direction = 1;
                        score = step;
                    }
                }

                // check if better than previous
                if (map[point.Item1, point.Item2] == 0 || score < map[point.Item1, point.Item2]) {
                    map[point.Item1, point.Item2] = score;
                    dirMap[point.Item1, point.Item2] = direction;
                    return true;
                }
                return false;
            }

            private void QueueNeighbors((int,int) point) {
                int s = map[point.Item1, point.Item2];
                queue.Enqueue((point.Item1-1, point.Item2), s);
                queue.Enqueue((point.Item1+1, point.Item2), s);
                queue.Enqueue((point.Item1, point.Item2-1), s);
                queue.Enqueue((point.Item1, point.Item2+1), s);
            }

            public Int32 GetScore() {
                return map[finishPoint.Item1, finishPoint.Item2];
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx1 = ReadFile("example1.input");
            string[] contentEx2 = ReadFile("example2.input");
            string[] content = ReadFile("my.input");

            Maze mazeExample1 = new(contentEx1);
            Maze mazeExample2 = new(contentEx2);
            Maze maze = new(content);

            Debug.Assert(PartOne(mazeExample1) == 7036);
            Debug.Assert(PartOne(mazeExample2) == 11048);
            Console.WriteLine($"Part 1 : {PartOne(maze)}");
            Debug.Assert(PartTwo(mazeExample1) == 45);
            Debug.Assert(PartTwo(mazeExample2) == 64);
            Console.WriteLine($"Part 1 : {PartTwo(maze)}");
        }

        static Int32 PartOne(Maze maze)
        {
            return maze.GetScore();
        }

        static Int32 PartTwo(Maze maze)
        {
            return maze.CalculateNodesInBestPaths();
        }
    }
}
