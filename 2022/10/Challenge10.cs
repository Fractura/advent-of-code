using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge10
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

            CathodeRayTube tube = new CathodeRayTube();

            foreach (string line in content) {
                (Command, int) command = parseCommand(line);
                tube.processCommand(command.Item1, command.Item2);
            }

            Console.WriteLine("Part 1 result: " + tube.getSignalStrengthSum(20, 40));
            Console.WriteLine("Part 2 result: ");
            tube.printCrt();
        }

        static (Command, int) parseCommand(string line) {
            if (line.Equals("noop")) return (Command.NOOP, 0);

            Regex commandRegex = new Regex(@"^(\w+)\s(\-?\d+)$");
            Match? matchCommand = commandRegex.Matches(line).Cast<Match>().FirstOrDefault();
            if (matchCommand == null) {
                return (0, 0);
            }
            GroupCollection groups = matchCommand.Groups;
            Command cmd;
            if (groups[1].Value.Equals("addx")) {
                cmd = Command.ADDX;
            } else {
                cmd = Command.NOOP;
            }
            int amount = Int32.Parse(groups[2].Value);
            return (cmd, amount);
        }
    }

    enum Command {
        ADDX, NOOP
    }

    class CathodeRayTube {
        public int register {get; private set;}
        public int cycle {get; private set;}
        public List<int> signalStrength {get;}
        public string crt {get; private set;}

        public CathodeRayTube() {
            this.register = 1;
            this.cycle = 0;
            this.crt = "";
            signalStrength = new List<int>();
            addCurrentSignalStrength();
        }

        public void processCommand(Command command, int argument) {
            switch (command) {
                case Command.NOOP: {
                    addCycle();
                    break;
                }
                case Command.ADDX: {
                    addCycle();
                    addCycle();
                    register += argument;
                    break;
                }
            }
        }

        private void addCycle() {
            ++cycle;
            addCurrentSignalStrength();

            addPixel();
            Console.WriteLine("Cycle {0}: X={1}", cycle, register);
        }

        private void addPixel() {
            int spritePosition = register;
            int drawPosition = (cycle-1) % 40;

            if (Math.Abs(spritePosition - drawPosition) <= 1) {
                crt += '#';
            } else {
                crt += '.';
            }

            if (cycle % 40 == 0) {
                crt += '\n';
            }
        }

        private void addCurrentSignalStrength() {
            signalStrength.Add(cycle * register);
        }

        public int getSignalStrengthSum(int start, int interval) {
            int sum = 0;
            for (int i = start; i < signalStrength.Count; i += interval) {
                sum += signalStrength[i];
            }
            return sum;
        }

        public void printCrt() {
            Console.WriteLine(crt);
        }
    }
}