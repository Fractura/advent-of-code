namespace challenge
{
    static partial class Challenge04
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
            string[] contentEx1 = ReadFile("example.input");
            string[] contentEx2 = ReadFile("example2.input");
            string[] content = ReadFile("my.input");

            Byte[,] matrixEx1 = CreateMatrixFromInput(contentEx1);
            Byte[,] matrixEx2 = CreateMatrixFromInput(contentEx2);
            Byte[,] matrix = CreateMatrixFromInput(content);

            Console.WriteLine($"Example part 1: {PartOne(matrixEx1)}");
            Console.WriteLine($"Part 1 : {PartOne(matrix)}");
            Console.WriteLine($"Example part 2: {PartTwo(matrixEx2)}");
            Console.WriteLine($"Part 2 : {PartTwo(matrix)}");
        }

        static Byte[,] CreateMatrixFromInput(string[] input) {
            Byte[,] matrix = new Byte[input.Length, input[0].Length];
            for (int y = 0; y < input.Length; ++y) {
                char[] chars = input[y].ToCharArray();
                for (int x = 0; x < input[y].Length; ++x) {
                    matrix[y, x] = chars[x] switch
                    {
                        'X' => 1,
                        'M' => 2,
                        'A' => 3,
                        'S' => 4,
                        _ => 0,
                    };
                }
            }
            return matrix;
        }

        static Int32 PartOne(Byte[,] matrix)
        {
            int yLen = matrix.GetLength(0);
            int xLen = matrix.GetLength(1);
            int count = 0;
            for (int y = 0; y < yLen; ++y) {
                for (int x = 0; x < xLen; ++x) {
                    if (matrix[y, x] == 1) {
                        // UP
                        if (y > 2 && matrix[y-1, x] == 2 && matrix[y-2, x] == 3 && matrix[y-3, x] == 4) {
                            ++count;
                        }

                        // UP-RIGHT
                        if (y > 2 && x < xLen - 3 && matrix[y-1, x+1] == 2 && matrix[y-2, x+2] == 3 && matrix[y-3, x+3] == 4) {
                            ++count;
                        }

                        // RIGHT
                        if (x < xLen - 3 && matrix[y, x+1] == 2 && matrix[y, x+2] == 3 && matrix[y, x+3] == 4) {
                            ++count;
                        }

                        // DOWN-RIGHT
                        if (y < yLen - 3 && x < xLen - 3 && matrix[y+1, x+1] == 2 && matrix[y+2, x+2] == 3 && matrix[y+3, x+3] == 4) {
                            ++count;
                        }

                        // DOWN
                        if (y < yLen - 3 && matrix[y+1, x] == 2 && matrix[y+2, x] == 3 && matrix[y+3, x] == 4) {
                            ++count;
                        }

                        // DOWN-LEFT
                        if (y < yLen - 3 && x > 2 && matrix[y+1, x-1] == 2 && matrix[y+2, x-2] == 3 && matrix[y+3, x-3] == 4) {
                            ++count;
                        }

                        // LEFT
                        if (x > 2 && matrix[y, x-1] == 2 && matrix[y, x-2] == 3 && matrix[y, x-3] == 4) {
                            ++count;
                        }

                        // UP-LEFT
                        if (y > 2 && x > 2 && matrix[y-1, x-1] == 2 && matrix[y-2, x-2] == 3 && matrix[y-3, x-3] == 4) {
                            ++count;
                        }
                    }
                }
            }
            return count;
        }

        static Int32 PartTwo(Byte[,] matrix)
        {
            int yLen = matrix.GetLength(0);
            int xLen = matrix.GetLength(1);
            int count = 0;
            for (int y = 1; y < yLen - 1; ++y) {
                for (int x = 1; x < xLen - 1; ++x) {
                    if (matrix[y, x] == 3) {
                        // M . M
                        // . A .
                        // S . S
                        if (matrix[y-1, x-1] == 2 && matrix[y-1, x+1] == 2 && matrix[y+1, x-1] == 4 && matrix[y+1, x+1] == 4) {
                            ++count;
                        }

                        // M . S
                        // . A .
                        // M . S
                        if (matrix[y-1, x-1] == 2 && matrix[y-1, x+1] == 4 && matrix[y+1, x-1] == 2 && matrix[y+1, x+1] == 4) {
                            ++count;
                        }

                        // S . S
                        // . A .
                        // M . M
                        if (matrix[y-1, x-1] == 4 && matrix[y-1, x+1] == 4 && matrix[y+1, x-1] == 2 && matrix[y+1, x+1] == 2) {
                            ++count;
                        }

                        // S . M
                        // . A .
                        // S . M
                        if (matrix[y-1, x-1] == 4 && matrix[y-1, x+1] == 2 && matrix[y+1, x-1] == 4 && matrix[y+1, x+1] == 2) {
                            ++count;
                        }
                    }
                }
            }
            return count;
        }
    }
}
