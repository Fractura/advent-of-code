using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge08
    {
        static void Main(string[] args)
        {
            string filename;
            if (args.Length < 1) {
                filename = "./input.txt";
            } else {
                filename = (string) (args.GetValue(0) ?? "./input.txt");
            }

            if (!File.Exists(filename)) {
                Console.WriteLine("Input file not found");
                return;
            }

            StreamReader reader = File.OpenText(filename);
            string[] content = reader.ReadToEnd().Split("\r\n");

            int rows = content.Length;
            int cols = content[0].Length;

            int[] treeHeight = new int[rows * cols];
            Boolean[] treeVisible = new Boolean[rows * cols];
            int[] scenicScore = new int[rows * cols];

            for (int i = 0; i < rows; ++i) {
                for (int j = 0; j < cols; ++j) {
                    treeHeight[i*cols + j] = Int32.Parse("" + content[i].ElementAt(j));
                }
            }

            int blockingHeight = -1;
            for (int i = 0; i < rows; ++i) {
                blockingHeight = -1;
                // left to right
                for (int j = 0; j < cols; ++j) {
                    int tree = treeHeight[i*cols + j];
                    if (tree > blockingHeight) {
                        blockingHeight = tree;
                        treeVisible[i*cols + j] = true;
                        if (tree == 9) break;
                    }
                }
                blockingHeight = -1;
                // right to left
                for (int j = cols-1; j >= 0; --j) {
                    int tree = treeHeight[i*cols + j];
                    if (tree > blockingHeight) {
                        blockingHeight = tree;
                        treeVisible[i*cols + j] = true;
                        if (tree == 9) break;
                    }
                }
            }
            blockingHeight = -1;
            for (int j = 0; j < cols; ++j) {
                blockingHeight = -1;
                // top to bottom
                for (int i = 0; i < rows; ++i) {
                    int tree = treeHeight[i*cols + j];
                    if (tree > blockingHeight) {
                        blockingHeight = tree;
                        treeVisible[i*cols + j] = true;
                        if (tree == 9) break;
                    }
                }
                blockingHeight = -1;
                // bottom to top
                for (int i = rows-1; i >= 0; --i) {
                    int tree = treeHeight[i*cols + j];
                    if (tree > blockingHeight) {
                        blockingHeight = tree;
                        treeVisible[i*cols + j] = true;
                        if (tree == 9) break;
                    }
                }
            }

            for (int i = 0; i < rows; ++i) {
                for (int j = 0; j < cols; ++j) {
                    int tree = treeHeight[i*cols + j];
                    int scoreL = 0;
                    int scoreR = 0;
                    int scoreU = 0;
                    int scoreD = 0;

                    for (int left = i-1; left >= 0; --left) {
                        ++scoreL;
                        if (treeHeight[left*cols + j] >= tree) {
                            break;
                        }
                    }
                    for (int right = i+1; right < cols; ++right) {
                        ++scoreR;
                        if (treeHeight[right*cols + j] >= tree) {
                            break;
                        }
                    }
                    for (int up = j-1; up >= 0; --up) {
                        ++scoreU;
                        if (treeHeight[i*cols + up] >= tree) {
                            break;
                        }
                    }
                    for (int down = j+1; down < rows; ++down) {
                        ++scoreD;
                        if (treeHeight[i*cols + down] >= tree) {
                            break;
                        }
                    }
                    scenicScore[i*cols + j] = scoreL * scoreR * scoreU * scoreD;
                }
            }

            var result1 = treeVisible.Where(b => b == true).Count();
            var result2 = scenicScore.Max();

            Console.WriteLine("Part 1 result: " + result1);
            Console.WriteLine("Part 2 result: " + result2);
        }
    }
}