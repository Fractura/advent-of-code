using System.Diagnostics;

namespace challenge
{
    static partial class Challenge17
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

        class Computer {
            Int64 registerA;
            Int64 registerB;
            Int64 registerC;
            readonly List<int> program;
            readonly List<int> output;
            int opc = 0;

            public Computer(string[] input) {
                registerA = Int64.Parse(input[0][12..]);
                registerB = Int64.Parse(input[1][12..]);
                registerC = Int64.Parse(input[2][12..]);
                output = new();

                program = input[4][9..].Split(',').Select(int.Parse).ToList();

                RunProgram();
            }

            public Int64 FindSolutionP2() {
                // This part is input-specific!
                Int64 best = Int64.MaxValue;
                Stack<(Int64 a, int pos)> processStack = new();
                processStack.Push((1, program.Count - 2));
                while (processStack.Count > 0) {
                    var task = processStack.Pop();
                    if (task.pos == -1) {
                        best = Math.Min(best, task.a);
                    } else {
                        Int64 a = task.a * 8;
                        int expect = program[task.pos];
                        for (int i = 0; i < 8; ++i) {
                            if (CheckNumber(a+i, expect)) {
                                processStack.Push((a+i, task.pos-1));
                            }
                        }
                    }
                }
                return best;
            }

            private static bool CheckNumber(Int64 a, int expect) {
                // This part is input-specific!
                long b = a % 8;
                b ^= 6;
                long c = a / (Int64)Math.Pow(2, b);
                b ^= c;
                b ^= 7;
                return b % 8 == expect;
            }

            public void RunProgram() {
                while (opc < program.Count) {
                    int instruction = program[opc];
                    int operand = program[opc+1];
                    RunOperation(instruction, operand);
                }
            }

            private void RunOperation(int instruction, int operand) {
                try {
                    switch (instruction) {
                        case 0:
                            OperationAdv(GetComboOperand(operand));
                            break;
                        case 1:
                            OperationBxl(operand);
                            break;
                        case 2:
                            OperationBst(GetComboOperand(operand));
                            break;
                        case 3:
                            OperationJnz(operand);
                            break;
                        case 4:
                            OperationBxc();
                            break;
                        case 5:
                            OperationOut(GetComboOperand(operand));
                            break;
                        case 6:
                            OperationBdv(GetComboOperand(operand));
                            break;
                        case 7:
                            OperationCdv(GetComboOperand(operand));
                            break;
                        default:
                            break;
                    }
                } catch (InvalidOperationException) {
                    opc = int.MaxValue;
                }
                
            }

            private void OperationAdv(Int64 v) {
                registerA = Math.DivRem(registerA, (Int64)Math.Pow(2, v)).Quotient;
                opc += 2;
            }
            private void OperationBxl(Int64 v) {
                registerB ^= v;
                opc += 2;
            }
            private void OperationBst(Int64 v) {
                registerB = v % 8;
                opc += 2;
            }
            private void OperationJnz(Int64 v) {
                if (registerA != 0) {
                    opc = (int)v;
                } else opc += 2;
            }
            private void OperationBxc() {
                registerB ^= registerC;
                opc += 2;
            }
            private void OperationOut(Int64 v) {
                output.Add((int)(v % 8));
                opc += 2;
            }
            private void OperationBdv(Int64 v) {
                registerB = Math.DivRem(registerA, (Int64)Math.Pow(2, v)).Quotient;
                opc += 2;
            }
            private void OperationCdv(Int64 v) {
                registerC = Math.DivRem(registerA, (Int64)Math.Pow(2, v)).Quotient;
                opc += 2;
            }

            private Int64 GetComboOperand(int operand) {
                return operand switch {
                    0 => 0,
                    1 => 1,
                    2 => 2,
                    3 => 3,
                    4 => registerA,
                    5 => registerB,
                    6 => registerC,
                    _ => throw new InvalidOperationException()
                };
            }

            public string GetOutput() {
                return string.Join(",", output);
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Computer exampleComputer = new(contentEx);
            Computer mainComputer = new(content);

            Debug.Assert(PartOne(exampleComputer) == "4,6,3,5,6,3,5,2,1,0");
            Console.WriteLine($"Part 1 : {PartOne(mainComputer)}");
            Console.WriteLine($"Part 2 : {PartTwo(mainComputer)}");
        }

        static string PartOne(Computer c)
        {
            return c.GetOutput();
        }

        static Int64 PartTwo(Computer c)
        {
            return c.FindSolutionP2();
        }
    }
}
