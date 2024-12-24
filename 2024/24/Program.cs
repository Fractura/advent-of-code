using System.Diagnostics;
using System.Dynamic;

namespace challenge
{
    static partial class Challenge24
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

        enum Operation {
            OR,
            XOR,
            AND
        };

        class Device {
            Dictionary<string, bool> wires;
            readonly Dictionary<string, bool> original_wires;
            readonly List<(string op1, string op2, string output, Operation op)> operations;
            readonly bool[] zHelper;

            public Device(string[] input) {
                original_wires = new();
                wires = new();
                operations = new();
                zHelper = new bool[64];

                bool partOneParsed = false;
                foreach (string line in input) {
                    if (line.Length == 0) {
                        partOneParsed = true;
                        continue;
                    }
                    string[] split = line.Split(' ');
                    if (partOneParsed) {
                        string op1 = split[0];
                        string op2 = split[2];
                        string output = split [4];
                        Operation oper = split[1] switch
                        {
                            "AND" => Operation.AND,
                            "OR" => Operation.OR,
                            "XOR" => Operation.XOR,
                            _ => throw new Exception()
                        };
                        operations.Add((op1, op2, output, oper));
                    } else {
                        string wire = split[0][..^1];
                        bool signal = split[1] switch
                        {
                            "0" => false,
                            "1" => true,
                            _ => false,
                        };
                        original_wires[wire] = signal;
                    }
                }
                ProcessOperations(operations);
            }

            public long GetZOutput() {
                long score = 0;
                for (int i = 0; i < 64; ++i) {
                    string zWire;
                    if (i < 10) {
                        zWire = "z0" + i;
                    } else {
                        zWire = "z" + i;
                    }
                    if (wires.ContainsKey(zWire)) {
                        if (wires[zWire]) {
                            zHelper[i] = true;
                            score += (long)Math.Pow(2,i);
                        } else {
                            zHelper[i] = false;
                        }
                    } else if (!wires.ContainsKey(zWire)) {
                        return score;
                    }
                }
                return score;
            }

            public string FindErrors() {
                HashSet<string> brokenWires = new();
                HashSet<string> orInputs = new();
                HashSet<string> andOutputs = new();
                operations.ForEach(operation => {
                    // z00 - z44 need to be outputs of XOR gates. (z45 is output of OR gate, as there's no x45/y45)
                    if (operation.output.StartsWith('z') && operation.op != Operation.XOR && operation.output != "z45") {
                        brokenWires.Add(operation.output);
                    }

                    // XOR gates must have either X-- and Y-- as inputs, or Z-- as outputs
                    if (operation.op == Operation.XOR
                        && !(operation.op1.StartsWith('x') || operation.op1.StartsWith('y'))
                        && !(operation.op2.StartsWith('x') || operation.op2.StartsWith('y'))
                        && !operation.output.StartsWith('z')) {
                        brokenWires.Add(operation.output);
                    }

                    // output of AND gate needs to be input of OR gate except for LSB (carry1 = x00 & y00)
                    if (operation.op == Operation.OR) {
                        orInputs.Add(operation.op1);
                        orInputs.Add(operation.op2);
                    }
                    if (operation.op == Operation.AND
                        && !(operation.op1 == "x00" || operation.op2 == "x00")) {
                        andOutputs.Add(operation.output);
                    }
                });
                foreach (string s in orInputs) {
                    if (!andOutputs.Contains(s)) {
                        brokenWires.Add(s);
                    } else {
                        andOutputs.Remove(s);
                    }
                }
                brokenWires.UnionWith(andOutputs);
                
                return string.Join(",", brokenWires.OrderBy(x => x));
            }

            private void ProcessOperations(List<(string op1, string op2, string output, Operation op)> operationList) {
                this.wires = new(original_wires);
                List<(string op1, string op2, string output, Operation op)> todo = new(operationList);
                while (todo.Count > 0) {
                    List<(string op1, string op2, string output, Operation op)> skipped = new();
                    foreach (var operation in todo) {
                        if (!ProcessOperation(operation)) {
                            skipped.Add(operation);
                        }
                    }
                    todo = skipped;
                }
            }

            private bool ProcessOperation((string, string, string, Operation) operation) {
                (string op1, string op2, string output, Operation op) = operation;
                if (wires.ContainsKey(op1) && wires.ContainsKey(op2)) {
                    switch (op) {
                        case Operation.OR:
                            wires[output] = wires[op1] || wires[op2];
                            return true;
                        case Operation.AND:
                            wires[output] = wires[op1] && wires[op2];
                            return true;
                        case Operation.XOR:
                            wires[output] = wires[op1] ^ wires[op2];
                            return true;
                        default:
                            return false;
                    }
                } else return false;
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx1 = ReadFile("example.input");
            string[] contentEx2 = ReadFile("example2.input");
            string[] content = ReadFile("my.input");

            Device ex1 = new(contentEx1);
            Device ex2 = new(contentEx2);
            Device puz = new(content);

            Debug.Assert(PartOne(ex1) == 4);
            Debug.Assert(PartOne(ex2) == 2024);
            Console.WriteLine($"Part 1 : {PartOne(puz)}");
            Console.WriteLine($"Part 2 : {PartTwo(puz)}");
        }

        static long PartOne(Device dev)
        {
            return dev.GetZOutput();
        }

        static string PartTwo(Device dev)
        {
            return dev.FindErrors();
        }
    }
}
