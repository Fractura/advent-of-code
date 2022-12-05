using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge05
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
            Regex rxDivider = new Regex(@"^(\s\d\s\s?)+$");
            Regex rxInstructions = new Regex(@"^move\s(\d+)\sfrom\s(\d+)\sto\s(\d+)\r?$");
            string[] contents = reader.ReadToEnd().Split('\n');

            int size = (contents[0].Length) / 4;
            Stack<char>[] containerStackR1 = new Stack<char>[size];
            Stack<char>[] containerStackR2 = new Stack<char>[size];

            int line = 0;
            int divLine = 0;
            while (rxDivider.Matches(contents[line]).Count == 0) {
                ++line;
            }
            divLine = line;

            while (line > 0) {
                --line;
                for (int i = 0; i < size; ++i) {
                    char container = contents[line][i*4+1];
                    if (container != ' ') {
                        containerStackR1[i] = containerStackR1[i] ?? new Stack<char>();
                        containerStackR1[i].Push(container);
                        containerStackR2[i] = containerStackR2[i] ?? new Stack<char>();
                        containerStackR2[i].Push(container);
                    }
                }
            }

            line = divLine;

            while (line < contents.Length) {
                MatchCollection matchesInstr = rxInstructions.Matches(contents[line]);
                foreach (Match m in matchesInstr) {
                    GroupCollection groups = m.Groups;
                    int amount = Int32.Parse(groups[1].Value);
                    int from = Int32.Parse(groups[2].Value) - 1;
                    int to = Int32.Parse(groups[3].Value) - 1;

                    Stack<char> temp = new Stack<char>();
                    for (int i = 0; i < amount; ++i) {
                        containerStackR1[to].Push(containerStackR1[from].Pop());
                        temp.Push(containerStackR2[from].Pop());
                    }
                    while (temp.Count != 0) {
                        containerStackR2[to].Push(temp.Pop());
                    }
                }
                ++line;
            }
            
            string result1 = "";
            foreach (Stack<char> stack in containerStackR1) {
                result1 = result1 + stack.Peek();
            }
            string result2 = "";
            foreach (Stack<char> stack in containerStackR2) {
                result2 = result2 + stack.Peek();
            }
            Console.WriteLine("Part 1 result: " + result1);
            Console.WriteLine("Part 2 result: " + result2);
        }
    }
}