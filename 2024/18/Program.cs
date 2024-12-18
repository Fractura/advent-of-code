using System.Diagnostics;

namespace challenge
{
    static partial class Challenge18
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

        class Memoryspace {
            readonly int dim;
            readonly List<(int x, int y)> corruption;
            int[,] map;

            public Memoryspace(string[] input, int dim) {
                this.dim = dim;
                this.corruption = Parse(input);
                this.map = new int[dim, dim];
            }

            private static List<(int x, int y)> Parse(string[] input) {
                List<(int x, int y)> l = [];
                foreach (string line in input) {
                    var split = line.Split(',');
                    var x = int.Parse(split[0]);
                    var y = int.Parse(split[1]);
                    l.Add((x,y));
                }
                return l;
            }

            public int GetP1(int time) {
                map = new int[dim, dim];
                CalculateShortestPaths(time);
                return map[dim-1, dim-1];
            }

            public (int x, int y) GetP2() {
                // Yeah, it's a bit slow, there are a few optimization approaches:
                // a. Since score doesn't matter, use a simpler approach (find a path)
                // b. Track, which nodes are used for the "current best path", only calculate a new path if a node of the current best path is removed
                // c. Even without changing the algorithm itself, rather than iterating from zero, there could be a binary tree search for the solution to significantly reduce the calculated solutions.
                for (int i = 0; i < corruption.Count; ++i) {
                    if (GetP1(i) == 0) {
                        return corruption[i-1];
                    }
                }
                return (0,0);
            }

            private void CalculateShortestPaths(int time) {
                map[0,0] = 0;
                PriorityQueue<(int x, int y, int score), int> queue = new();
                queue.Enqueue((0,1,1),1);
                queue.Enqueue((1,0,1),1);
                Process(queue, time);
            }

            private void Process(PriorityQueue<(int x, int y, int score), int> queue, int time) {
                while (queue.Count > 0) {
                    var (x, y, score) = queue.Dequeue();
                    if (IsValidField(x, y, time) && map[x, y] == 0) {
                        map[x, y] = score;
                        int nextScore = score+1;
                        queue.Enqueue((x-1,y,nextScore),nextScore);
                        queue.Enqueue((x+1,y,nextScore),nextScore);
                        queue.Enqueue((x,y-1,nextScore),nextScore);
                        queue.Enqueue((x,y+1,nextScore),nextScore);
                    }
                }
            }

            private bool IsValidField(int x, int y, int time) {
                if (x < 0 || y < 0 || x >= dim || y >= dim) {
                    // Field outside area boundaries
                    return false;
                }
                var index = corruption.IndexOf((x,y));
                if (index == -1) {
                    // Field will not be corrupted
                    return true;
                }
                // If index > time, then that field is not yet corrupted
                return index >= time;
            }

        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Memoryspace mse = new(contentEx, 7);
            Memoryspace ms = new(content, 71);

            Debug.Assert(PartOne(mse, 12) == 22);
            Console.WriteLine($"Part 1 : {PartOne(ms, 1024)}");
            Debug.Assert(PartTwo(mse) == (6,1));
            Console.WriteLine($"Part 2 : {PartTwo(ms)}");
        }

        static int PartOne(Memoryspace ms, int time)
        {
            return ms.GetP1(time);
        }

        static (int,int) PartTwo(Memoryspace ms)
        {
            return ms.GetP2();
        }
    }
}
