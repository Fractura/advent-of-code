using System.Diagnostics;
using System.Text.RegularExpressions;

namespace challenge
{
    static partial class Challenge13
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

        class Arcade
        {
            private readonly List<Machine> machines;

            public Arcade(string[] input) {
                machines = [];
                Int64 ax = 0;
                Int64 ay = 0;
                Int64 bx = 0;
                Int64 by = 0;
                Int64 wcx = 0;
                Int64 wcy = 0;
                for (int i = 0; i < input.Length; ++i) {
                    string line = input[i];
                    if (i % 4 == 0) {
                        MatchCollection matches = ButtonRegex().Matches(line);
                        Debug.Assert(matches.Count > 0);
                        ax = Int64.Parse(matches[0].Groups[1].Value);
                        ay = Int64.Parse(matches[0].Groups[2].Value);
                    } else if (i % 4 == 1) {
                        MatchCollection matches = ButtonRegex().Matches(line);
                        Debug.Assert(matches.Count > 0);
                        bx = Int64.Parse(matches[0].Groups[1].Value);
                        by = Int64.Parse(matches[0].Groups[2].Value);
                    } else if (i % 4 == 2) {
                        MatchCollection matches = PrizeRegex().Matches(line);
                        Debug.Assert(matches.Count > 0);
                        wcx = Int64.Parse(matches[0].Groups[1].Value);
                        wcy = Int64.Parse(matches[0].Groups[2].Value);
                        machines.Add(new Machine((ax,ay),(bx,by),(wcx,wcy)));
                    }
                }
            }

            public Int64 SolveAll1() {
                Int64 sum = 0;
                foreach (Machine m in machines) {
                    sum += m.Solve(0);
                }
                return sum;
            }

            public Int64 SolveAll2() {
                Int64 sum = 0;
                foreach (Machine m in machines) {
                    sum += m.Solve(10000000000000);
                }
                return sum;
            }
        }

        class Machine((Int64, Int64) a, (Int64, Int64) b, (Int64, Int64) win)
        {
            (Int64, Int64) buttonA = a;
            (Int64, Int64) buttonB = b;
            (Int64, Int64) winCondition = win;

            // public Int64 Solve(Int64 limit) {
            //     Int64 best = Int64.MaxValue;
            //     for (Int64 i = 0; i < limit; ++i) {
            //         if (buttonA.Item1 * i > winCondition.Item1 || buttonA.Item2 * i > winCondition.Item2) {
            //             break;
            //         }
            //         for (Int64 j = 0; j < limit; ++j) {
            //             if (buttonA.Item1 * i + buttonB.Item1 * j == winCondition.Item1 && buttonA.Item2 * i + buttonB.Item2 * j == winCondition.Item2)
            //             {
            //                 best = Math.Min(i*3+j, best);
            //             }
            //             if (buttonA.Item1 * i + buttonB.Item1 * j > winCondition.Item1 | buttonA.Item2 * i + buttonB.Item2 * j > winCondition.Item2)
            //             {
            //                 break;
            //             }
            //         }
            //     }

            //     if (best == Int64.MaxValue) {
            //         return 0;
            //     } else {
            //         return best;
            //     }
            // }

            public Int64 Solve(Int64 offset) {
                Int64 best = Int64.MaxValue;
                Int64 i2_delta = Int64.MaxValue;
                (Int64,Int64) offsetWinCondition = (winCondition.Item1 + offset, winCondition.Item2 + offset);
                // Highest possible button A count
                Int64 A_count = offsetWinCondition.Item1 / buttonA.Item1;
                Int64 max_rep = buttonB.Item1;
                while ((offsetWinCondition.Item1 - A_count * buttonA.Item1) % buttonB.Item1 != 0 && max_rep > 0) {
                    --A_count;
                    --max_rep;
                }
                if ((offsetWinCondition.Item1 - A_count * buttonA.Item1) % buttonB.Item1 != 0) {
                    return 0; // Cannot find a combination where position can be aligned to target X value.
                }
                Int64 B_count = (offsetWinCondition.Item1 - A_count * buttonA.Item1) / buttonB.Item1;
                if (A_count * buttonA.Item2 + B_count * buttonB.Item2 == offsetWinCondition.Item2) {
                    best = Math.Min(A_count*3+B_count, best);
                } else {
                    i2_delta = Math.Abs(offsetWinCondition.Item2 - (A_count * buttonA.Item2 + B_count * buttonB.Item2));
                }

                // Find interval of A_count where a matching B_count can be found
                Int64 intervalA = 1;
                while (intervalA * buttonA.Item1 % buttonB.Item1 != 0) {
                    ++intervalA;
                }

                // Number of B buttons equivalent to A button interval
                Int64 intervalB = intervalA * buttonA.Item1 / buttonB.Item1;

                // Make one step towards the next solution where X position fits
                A_count -= intervalA;
                B_count += intervalB;
                if (A_count * buttonA.Item2 + B_count * buttonB.Item2 == offsetWinCondition.Item2) {
                    best = Math.Min(A_count*3+B_count, best);
                } else {
                    // check if delta of Y position improved
                    Int64 new_delta = Math.Abs(offsetWinCondition.Item2 - (A_count * buttonA.Item2 + B_count * buttonB.Item2));
                    if (new_delta < i2_delta) {
                        // check if delta can be used to find correct solution
                        Int64 delta_step = i2_delta - new_delta;
                        if (new_delta % delta_step == 0) {
                            Int64 steps = new_delta / delta_step;
                            A_count -= steps * intervalA;
                            B_count += steps * intervalB;
                            if (A_count * buttonA.Item2 + B_count * buttonB.Item2 == offsetWinCondition.Item2) {
                                best = Math.Min(A_count*3+B_count, best);
                            }
                        }
                    }
                }

                if (best == Int64.MaxValue) {
                    return 0;
                } else {
                    return best;
                }
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Arcade ex = new(contentEx);
            Arcade ar = new(content);

            Debug.Assert(PartOne(ex) == 480);
            Console.WriteLine($"Part 1 : {PartOne(ar)}");
            Console.WriteLine($"Part 2 : {PartTwo(ar)}");
        }

        static Int64 PartOne(Arcade arcade)
        {
            return arcade.SolveAll1();
        }

        static Int64 PartTwo(Arcade arcade)
        {
            return arcade.SolveAll2();
        }

        [GeneratedRegex("Button [A|B]: X\\+(\\d+), Y\\+(\\d+)")]
        private static partial Regex ButtonRegex();
        [GeneratedRegex("Prize: X=(\\d+), Y=(\\d+)")]
        private static partial Regex PrizeRegex();
    }
}
