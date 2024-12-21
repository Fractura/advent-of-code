using System.Diagnostics;

// If you're reading this, this is the last chance to turn around and leave. This solution is *not* pretty.

namespace challenge
{
    static partial class Challenge21
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

        enum Numpad {
            NP_0,
            NP_1,
            NP_2,
            NP_3,
            NP_4,
            NP_5,
            NP_6,
            NP_7,
            NP_8,
            NP_9,
            NP_A
        }

        enum Arrows {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            A
        }

        static List<Arrows> MovePointerNumpad(Numpad start, Numpad end) {
            switch ((start,end)) {
                case (Numpad.NP_0, Numpad.NP_0):
                case (Numpad.NP_1, Numpad.NP_1):
                case (Numpad.NP_2, Numpad.NP_2):
                case (Numpad.NP_3, Numpad.NP_3):
                case (Numpad.NP_4, Numpad.NP_4):
                case (Numpad.NP_5, Numpad.NP_5):
                case (Numpad.NP_6, Numpad.NP_6):
                case (Numpad.NP_7, Numpad.NP_7):
                case (Numpad.NP_8, Numpad.NP_8):
                case (Numpad.NP_9, Numpad.NP_9):
                case (Numpad.NP_A, Numpad.NP_A):
                    return new() {Arrows.A};
                case (Numpad.NP_0, Numpad.NP_2):
                case (Numpad.NP_A, Numpad.NP_3):
                case (Numpad.NP_1, Numpad.NP_4):
                case (Numpad.NP_2, Numpad.NP_5):
                case (Numpad.NP_3, Numpad.NP_6):
                case (Numpad.NP_4, Numpad.NP_7):
                case (Numpad.NP_5, Numpad.NP_8):
                case (Numpad.NP_6, Numpad.NP_9):
                    return new() {Arrows.UP, Arrows.A};
                case (Numpad.NP_2, Numpad.NP_0):
                case (Numpad.NP_3, Numpad.NP_A):
                case (Numpad.NP_4, Numpad.NP_1):
                case (Numpad.NP_5, Numpad.NP_2):
                case (Numpad.NP_6, Numpad.NP_3):
                case (Numpad.NP_7, Numpad.NP_4):
                case (Numpad.NP_8, Numpad.NP_5):
                case (Numpad.NP_9, Numpad.NP_6):
                    return new() {Arrows.DOWN, Arrows.A};
                case (Numpad.NP_0, Numpad.NP_A):
                case (Numpad.NP_1, Numpad.NP_2):
                case (Numpad.NP_2, Numpad.NP_3):
                case (Numpad.NP_4, Numpad.NP_5):
                case (Numpad.NP_5, Numpad.NP_6):
                case (Numpad.NP_7, Numpad.NP_8):
                case (Numpad.NP_8, Numpad.NP_9):
                    return new() {Arrows.RIGHT, Arrows.A};
                case (Numpad.NP_A, Numpad.NP_0):
                case (Numpad.NP_2, Numpad.NP_1):
                case (Numpad.NP_3, Numpad.NP_2):
                case (Numpad.NP_5, Numpad.NP_4):
                case (Numpad.NP_6, Numpad.NP_5):
                case (Numpad.NP_8, Numpad.NP_7):
                case (Numpad.NP_9, Numpad.NP_8):
                    return new() {Arrows.LEFT, Arrows.A};
                case (Numpad.NP_0, Numpad.NP_1):
                    return new() {Arrows.UP, Arrows.LEFT, Arrows.A};
                case (Numpad.NP_2, Numpad.NP_4):
                case (Numpad.NP_3, Numpad.NP_5):
                case (Numpad.NP_5, Numpad.NP_7):
                case (Numpad.NP_A, Numpad.NP_2):
                case (Numpad.NP_6, Numpad.NP_8):
                    return new() {Arrows.LEFT, Arrows.UP, Arrows.A};
                case (Numpad.NP_1, Numpad.NP_0):
                    return new() {Arrows.RIGHT, Arrows.DOWN, Arrows.A};
                case (Numpad.NP_4, Numpad.NP_2):
                case (Numpad.NP_5, Numpad.NP_3):
                case (Numpad.NP_7, Numpad.NP_5):
                case (Numpad.NP_2, Numpad.NP_A):
                case (Numpad.NP_8, Numpad.NP_6):
                    return new() {Arrows.DOWN, Arrows.RIGHT, Arrows.A};
                case (Numpad.NP_0, Numpad.NP_3):
                case (Numpad.NP_1, Numpad.NP_5):
                case (Numpad.NP_2, Numpad.NP_6):
                case (Numpad.NP_4, Numpad.NP_8):
                case (Numpad.NP_5, Numpad.NP_9):
                    return new() {Arrows.UP, Arrows.RIGHT, Arrows.A};
                case (Numpad.NP_3, Numpad.NP_0):
                case (Numpad.NP_5, Numpad.NP_1):
                case (Numpad.NP_6, Numpad.NP_2):
                case (Numpad.NP_8, Numpad.NP_4):
                case (Numpad.NP_9, Numpad.NP_5):
                    return new() {Arrows.LEFT, Arrows.DOWN, Arrows.A};
                case (Numpad.NP_0, Numpad.NP_5):
                case (Numpad.NP_1, Numpad.NP_7):
                case (Numpad.NP_2, Numpad.NP_8):
                case (Numpad.NP_A, Numpad.NP_6):
                case (Numpad.NP_3, Numpad.NP_9):
                    return new() {Arrows.UP, Arrows.UP, Arrows.A};
                case (Numpad.NP_5, Numpad.NP_0):
                case (Numpad.NP_7, Numpad.NP_1):
                case (Numpad.NP_8, Numpad.NP_2):
                case (Numpad.NP_6, Numpad.NP_A):
                case (Numpad.NP_9, Numpad.NP_3):
                    return new() {Arrows.DOWN, Arrows.DOWN, Arrows.A};
                case (Numpad.NP_1, Numpad.NP_3):
                case (Numpad.NP_4, Numpad.NP_6):
                case (Numpad.NP_7, Numpad.NP_9):
                    return new() {Arrows.RIGHT, Arrows.RIGHT, Arrows.A};
                case (Numpad.NP_3, Numpad.NP_1):
                case (Numpad.NP_6, Numpad.NP_4):
                case (Numpad.NP_9, Numpad.NP_7):
                    return new() {Arrows.LEFT, Arrows.LEFT, Arrows.A};
                case (Numpad.NP_1, Numpad.NP_6):
                case (Numpad.NP_4, Numpad.NP_9):
                    return new() {Arrows.UP, Arrows.RIGHT, Arrows.RIGHT, Arrows.A};
                case (Numpad.NP_6, Numpad.NP_1):
                case (Numpad.NP_9, Numpad.NP_4):
                    return new() {Arrows.LEFT, Arrows.LEFT, Arrows.DOWN, Arrows.A};
                case (Numpad.NP_A, Numpad.NP_1):
                    return new() {Arrows.UP, Arrows.LEFT, Arrows.LEFT, Arrows.A};
                case (Numpad.NP_3, Numpad.NP_4):
                case (Numpad.NP_6, Numpad.NP_7):
                    return new() {Arrows.LEFT, Arrows.LEFT, Arrows.UP, Arrows.A};
                case (Numpad.NP_1, Numpad.NP_A):
                    return new() {Arrows.RIGHT, Arrows.RIGHT, Arrows.DOWN, Arrows.A};
                case (Numpad.NP_4, Numpad.NP_3):
                case (Numpad.NP_7, Numpad.NP_6):
                    return new() {Arrows.DOWN, Arrows.RIGHT, Arrows.RIGHT, Arrows.A};
                case (Numpad.NP_0, Numpad.NP_4):
                    return new() {Arrows.UP, Arrows.UP, Arrows.LEFT, Arrows.A};
                case (Numpad.NP_A, Numpad.NP_5):
                case (Numpad.NP_2, Numpad.NP_7):
                case (Numpad.NP_3, Numpad.NP_8):
                    return new() {Arrows.LEFT, Arrows.UP, Arrows.UP, Arrows.A};
                case (Numpad.NP_4, Numpad.NP_0):
                    return new() {Arrows.RIGHT, Arrows.DOWN, Arrows.DOWN, Arrows.A};
                case (Numpad.NP_5, Numpad.NP_A):
                case (Numpad.NP_7, Numpad.NP_2):
                case (Numpad.NP_8, Numpad.NP_3):
                    return new() {Arrows.DOWN, Arrows.DOWN, Arrows.RIGHT, Arrows.A};
                case (Numpad.NP_0, Numpad.NP_6):
                case (Numpad.NP_1, Numpad.NP_8):
                case (Numpad.NP_2, Numpad.NP_9):
                    return new() {Arrows.UP, Arrows.UP, Arrows.RIGHT, Arrows.A};
                case (Numpad.NP_6, Numpad.NP_0):
                case (Numpad.NP_8, Numpad.NP_1):
                case (Numpad.NP_9, Numpad.NP_2):
                    return new() {Arrows.LEFT, Arrows.DOWN, Arrows.DOWN, Arrows.A};
                case (Numpad.NP_0, Numpad.NP_8):
                case (Numpad.NP_A, Numpad.NP_9):
                    return new() {Arrows.UP, Arrows.UP, Arrows.UP, Arrows.A};
                case (Numpad.NP_8, Numpad.NP_0):
                case (Numpad.NP_9, Numpad.NP_A):
                    return new() {Arrows.DOWN, Arrows.DOWN, Arrows.DOWN, Arrows.A};
                case (Numpad.NP_0, Numpad.NP_7):
                    return new() {Arrows.UP, Arrows.UP, Arrows.UP, Arrows.LEFT, Arrows.A};
                case (Numpad.NP_A, Numpad.NP_8):
                    return new() {Arrows.LEFT, Arrows.UP, Arrows.UP, Arrows.UP, Arrows.A};
                case (Numpad.NP_7, Numpad.NP_0):
                    return new() {Arrows.RIGHT, Arrows.DOWN, Arrows.DOWN, Arrows.DOWN, Arrows.A};
                case (Numpad.NP_8, Numpad.NP_A):
                    return new() {Arrows.DOWN, Arrows.DOWN, Arrows.DOWN, Arrows.RIGHT, Arrows.A};
                case (Numpad.NP_0, Numpad.NP_9):
                    return new() {Arrows.UP, Arrows.UP, Arrows.UP, Arrows.RIGHT, Arrows.A};
                case (Numpad.NP_9, Numpad.NP_0):
                    return new() {Arrows.LEFT, Arrows.DOWN, Arrows.DOWN, Arrows.DOWN, Arrows.A};
                case (Numpad.NP_1, Numpad.NP_9):
                    return new() {Arrows.UP, Arrows.UP, Arrows.RIGHT, Arrows.RIGHT, Arrows.A};
                case (Numpad.NP_9, Numpad.NP_1):
                    return new() {Arrows.LEFT, Arrows.LEFT, Arrows.DOWN, Arrows.DOWN, Arrows.A};
                case (Numpad.NP_3, Numpad.NP_7):
                    return new() {Arrows.LEFT, Arrows.LEFT, Arrows.UP, Arrows.UP, Arrows.A};
                case (Numpad.NP_A, Numpad.NP_4):
                    return new() {Arrows.UP, Arrows.UP, Arrows.LEFT, Arrows.LEFT, Arrows.A};
                case (Numpad.NP_7, Numpad.NP_3):
                    return new() {Arrows.DOWN, Arrows.DOWN, Arrows.RIGHT, Arrows.RIGHT, Arrows.A};
                case (Numpad.NP_4, Numpad.NP_A):
                    return new() {Arrows.RIGHT, Arrows.RIGHT, Arrows.DOWN, Arrows.DOWN, Arrows.A};
                case (Numpad.NP_A, Numpad.NP_7):
                    return new() {Arrows.UP, Arrows.UP, Arrows.UP, Arrows.LEFT, Arrows.LEFT, Arrows.A};
                case (Numpad.NP_7, Numpad.NP_A):
                    return new() {Arrows.RIGHT, Arrows.RIGHT, Arrows.DOWN, Arrows.DOWN, Arrows.DOWN, Arrows.A};
                default:
                    return new();
            }
        }

        static List<Arrows> MovePointerArrowpad(Arrows start, Arrows end) {
            switch ((start,end)) {
                case (Arrows.A, Arrows.A):
                case (Arrows.UP, Arrows.UP):
                case (Arrows.LEFT, Arrows.LEFT):
                case (Arrows.DOWN, Arrows.DOWN):
                case (Arrows.RIGHT, Arrows.RIGHT):
                    return new() {Arrows.A};
                case (Arrows.DOWN, Arrows.UP):
                case (Arrows.RIGHT, Arrows.A):
                    return new() {Arrows.UP, Arrows.A};
                case (Arrows.UP, Arrows.DOWN):
                case (Arrows.A, Arrows.RIGHT):
                    return new() {Arrows.DOWN, Arrows.A};
                case (Arrows.UP, Arrows.A):
                case (Arrows.DOWN, Arrows.RIGHT):
                case (Arrows.LEFT, Arrows.DOWN):
                    return new() {Arrows.RIGHT, Arrows.A};
                case (Arrows.A, Arrows.UP):
                case (Arrows.RIGHT, Arrows.DOWN):
                case (Arrows.DOWN, Arrows.LEFT):
                    return new() {Arrows.LEFT, Arrows.A};
                case (Arrows.RIGHT, Arrows.UP):
                    return new() {Arrows.LEFT, Arrows.UP, Arrows.A};
                case (Arrows.UP, Arrows.RIGHT):
                    return new() {Arrows.DOWN, Arrows.RIGHT, Arrows.A};
                case (Arrows.LEFT, Arrows.UP):
                    return new() {Arrows.RIGHT, Arrows.UP, Arrows.A};
                case (Arrows.DOWN, Arrows.A):
                    return new() {Arrows.UP, Arrows.RIGHT, Arrows.A};
                case (Arrows.UP, Arrows.LEFT):
                    return new() {Arrows.DOWN, Arrows.LEFT, Arrows.A};
                case (Arrows.A, Arrows.DOWN):
                    return new() {Arrows.LEFT, Arrows.DOWN, Arrows.A};
                case (Arrows.LEFT, Arrows.RIGHT):
                    return new() {Arrows.RIGHT, Arrows.RIGHT, Arrows.A};
                case (Arrows.RIGHT, Arrows.LEFT):
                    return new() {Arrows.LEFT, Arrows.LEFT, Arrows.A};
                case (Arrows.A, Arrows.LEFT):
                    return new() {Arrows.DOWN, Arrows.LEFT, Arrows.LEFT, Arrows.A};
                case (Arrows.LEFT, Arrows.A):
                    return new() {Arrows.RIGHT, Arrows.RIGHT, Arrows.UP, Arrows.A};
                default:
                    return new();
            }
        }

        static Numpad Parse(char c) {
            return c switch
            {
                '0' => Numpad.NP_0,
                '1' => Numpad.NP_1,
                '2' => Numpad.NP_2,
                '3' => Numpad.NP_3,
                '4' => Numpad.NP_4,
                '5' => Numpad.NP_5,
                '6' => Numpad.NP_6,
                '7' => Numpad.NP_7,
                '8' => Numpad.NP_8,
                '9' => Numpad.NP_9,
                'A' => Numpad.NP_A,
                _ => Numpad.NP_A
            };
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Debug.Assert(PartOne(contentEx) == 126384);
            Console.WriteLine($"Part 1 : {PartOne(content)}");
            Console.WriteLine($"Part 2 : {PartTwo(content)}");
        }

        static Int32 PartOne(string[] input)
        {
            Int32 score = 0;
            for (int i = 0; i < input.Length; ++i) {
                string line = input[i];
                int inputLength = 0;
                int numeric = int.Parse(line[..^1]);
                Numpad current = Numpad.NP_A;
                Numpad next;
                for (int next_index = 0; next_index < line.Length; ++next_index) {
                    next = Parse(line[next_index]);
                    var inputToFinalRobot = MovePointerNumpad(current, next);
                    Arrows currentDr1 = Arrows.A;
                    Arrows nextDr1;
                    for (int idr1 = 0; idr1 < inputToFinalRobot.Count; ++idr1) {
                        nextDr1 = inputToFinalRobot[idr1];
                        var inputToDirectionalRobot1 = MovePointerArrowpad(currentDr1, nextDr1);
                        Arrows currentDr2 = Arrows.A;
                        Arrows nextDr2;
                        for (int idr2 = 0; idr2 < inputToDirectionalRobot1.Count; ++idr2) {
                            nextDr2 = inputToDirectionalRobot1[idr2];
                            var inputToDirectionalRobot2 = MovePointerArrowpad(currentDr2, nextDr2);
                            inputLength += inputToDirectionalRobot2.Count;
                            currentDr2 = nextDr2;
                        }
                        currentDr1 = nextDr1;
                    }
                    current = next;
                }
                Console.WriteLine($"{line} -> {inputLength} * {numeric} = {inputLength * numeric}");
                score += inputLength * numeric;
            }
            return score;
        }

        static Int64 PartTwo(string[] input)
        {
            Int64 score = 0;
            for (int i = 0; i < input.Length; ++i) {
                string line = input[i];
                Int64 inputLength = 0;
                Int64 numeric = Int64.Parse(line[..^1]);
                Numpad current = Numpad.NP_A;
                Numpad next;
                for (int next_index = 0; next_index < line.Length; ++next_index) {
                    next = Parse(line[next_index]);
                    var inputToFinalRobot = MovePointerNumpad(current, next);
                    Arrows currentDr1 = Arrows.A;
                    Arrows nextDr1;
                    for (int idr1 = 0; idr1 < inputToFinalRobot.Count; ++idr1) {
                        nextDr1 = inputToFinalRobot[idr1];
                        inputLength += GetInputsForNRobots(currentDr1, nextDr1, 24);
                        currentDr1 = nextDr1;
                    }
                    current = next;
                }
                Console.WriteLine($"{line} -> {inputLength} * {numeric} = {inputLength * numeric}");
                score += inputLength * numeric;
            }
            return score;
        }

        readonly static Dictionary<(Arrows, Arrows, int), Int64> cache = new();

        static Int64 GetInputsForNRobots(Arrows current, Arrows next, int iter) {
            Int64 score = 0;
            if (iter <= 0) {
                return MovePointerArrowpad(current, next).Count;
            } else {
                var inputs = MovePointerArrowpad(current, next);
                Arrows currentNextRobot = Arrows.A;
                Arrows nextNextRobot;
                int ni = iter - 1;
                for (int i = 0; i < inputs.Count; ++i) {
                    nextNextRobot = inputs[i];
                    long r;
                    if (cache.ContainsKey((currentNextRobot, nextNextRobot, ni))) {
                        r = cache[(currentNextRobot, nextNextRobot, ni)];
                    } else {
                        r = GetInputsForNRobots(currentNextRobot, nextNextRobot, ni);
                        cache[(currentNextRobot, nextNextRobot, ni)] = r;
                    }
                    score += r;
                    currentNextRobot = nextNextRobot;
                }
            }
            return score;
        }
    }
}
