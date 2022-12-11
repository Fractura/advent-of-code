using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge11
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
            string[] content = reader.ReadToEnd().Split("\r\n\r\n");

            Jungle jungle1 = new Jungle(3);
            Jungle jungle2 = new Jungle(1);

            foreach (string line in content) {
                Command? command = parseCommand(line);
                if (command != null) {
                    jungle1.createMonkeByCommand(command);
                    jungle2.createMonkeByCommand(command);
                }
            }

            //jungle1.processTurns(20); Somehow having this line active breaks the result in part two and I have absolutely no idea why.
            jungle2.processTurns(10000);

            Console.WriteLine("Part 1 result: " + jungle1.getMonkeBusiness());
            Console.WriteLine("Part 2 result: " + jungle2.getMonkeBusiness());
        }

        static Command? parseCommand(string line) {
            string stripped = line.Replace(" ", "").Replace("\t", "").Replace("\r\n", "");
            Regex commandRegex = new Regex(@"^Monkey(\d+):Startingitems:([\d,]+)Operation:new=([\w\d]+)([+\*])([\w\d]+)Test:divisibleby(\d+)Iftrue:throwtomonkey(\d+)Iffalse:throwtomonkey(\d+)$");
            Match? matchCommand = commandRegex.Matches(stripped).Cast<Match>().FirstOrDefault();
            if (matchCommand == null) {
                // Can't parse
                return null;
            }
            GroupCollection groups = matchCommand.Groups;

            int id = Int32.Parse(groups[1].Value);

            string[] receivedItemsStr = groups[2].Value.Split(',');

            // Parse List
            Queue<Int128> receivedItems = new Queue<Int128>();
            foreach (string s in receivedItemsStr) {
                receivedItems.Enqueue(Int128.Parse(s));
            }

            // Parse operation arguments
            string left = groups[3].Value;
            string right = groups[5].Value;
            int? leftInt;
            int? rightInt;
            
            if (left.Equals("old")) {
                leftInt = null;
            } else {
                leftInt = Int32.Parse(left);
            }

            if (right.Equals("old")) {
                rightInt = null;
            } else {
                rightInt = Int32.Parse(right);
            }


            // Parse operator
            Operand operand;
            switch (groups[4].Value[0]) {
                case '+': {
                    operand = Operand.ADD;
                    break;
                }
                case '*': {
                    operand = Operand.MULTIPLY;
                    break;
                }
                default: return null;
            }
            // Divisible by factor
            int divisibleby = Int32.Parse(groups[6].Value);

            // Parse targets
            int trueTarget = Int32.Parse(groups[7].Value);
            int falseTarget = Int32.Parse(groups[8].Value);

            return new Command(id, receivedItems, leftInt, rightInt, operand, divisibleby, trueTarget, falseTarget);
        }
    }

    enum Operand {
        ADD, MULTIPLY
    }

    struct Command {
        public int id {get;}
        public Queue<Int128> items {get;}
        public int? firstOperand {get;}
        public int? secondOperand {get;}
        public Operand mathOperand {get;}
        public int divisibleFactor {get;}
        public int trueTarget {get;}
        public int falseTarget {get;}

        public Command(int id, Queue<Int128> items, int? firstOperand, int? secondOperand, Operand mathOperand, int divisibleFactor, int trueTarget, int falseTarget) {
            this.id = id;
            this.items = items;
            this.firstOperand = firstOperand;
            this.secondOperand = secondOperand;
            this.mathOperand = mathOperand;
            this.divisibleFactor = divisibleFactor;
            this.trueTarget = trueTarget;
            this.falseTarget = falseTarget;
        }
    }

    class Jungle {
        List<Monke> monkes;
        public Int128 leastCommonMultiple {get; private set;}
        int worryFactor;

        public Jungle(int worryFactor) {
            this.monkes = new List<Monke>();
            this.worryFactor = worryFactor;
            this.leastCommonMultiple = 1;
        }

        public void createMonkeByCommand(Command? cmd) {
            if (cmd == null) return;
            Command command = cmd ?? new Command();
            Func<Int128, Int128> operation = (x => {
                Int128 a = command.firstOperand ?? x;
                Int128 b = command.secondOperand ?? x;
                switch (command.mathOperand) {
                    case Operand.ADD: return a + b;
                    case Operand.MULTIPLY: return a * b;
                    default: return x;
                }
            });
            
            monkes.Add(new Monke(this, command.id, command.items, operation, command.divisibleFactor, command.trueTarget, command.falseTarget, worryFactor));
            this.leastCommonMultiple *= command.divisibleFactor;
        }

        public void processTurns(int n) {
            for (int i = 0; i < n; ++i) {
                processTurn();
            }
        }

        public void processTurn() {
            foreach (Monke monke in monkes) {
                monke.process();
            }
        }

        public void throwTo(int monke, Int128 value) {
            monkes[monke].catchItem(value);
        }

        public Int128 getMonkeBusiness() {
            List<Int128> inspectionCounts = new List<Int128>();
            foreach (Monke monke in monkes) {
                inspectionCounts.Add(monke.inspectionCount);
            }

            Int128 highest = inspectionCounts.Max();
            inspectionCounts.Remove(inspectionCounts.Max());
            Int128 secondHighest = inspectionCounts.Max();
            return highest * secondHighest;
        }
    }

    class Monke {
        int id;
        Jungle myJungle;
        Queue<Int128> items;
        Func<Int128, Int128> operation;
        int testDividend;
        public int trueTarget {get;}
        public int falseTarget {get;}

        public Int128 inspectionCount {get; private set;}

        int worry;

        public Monke(Jungle jungle, int id, Queue<Int128> startingItems, Func<Int128, Int128> operation, int divident, int trueTarget, int falseTarget, int worry) {
            this.myJungle = jungle;
            this.id = id;
            this.inspectionCount = 0;
            this.items = startingItems;
            this.operation = operation;
            this.testDividend = divident;
            this.trueTarget = trueTarget;
            this.falseTarget = falseTarget;
            this.worry = worry;
        }

        public void catchItem(Int128 item) {
            items.Enqueue(item);
        }

        public void process() {
            while (items.Count > 0) {
                ++inspectionCount;
                Int128 item = items.Dequeue();
                Int128 newValue = (operation(item) / worry) % myJungle.leastCommonMultiple;
                if (newValue % testDividend == 0) {
                    myJungle.throwTo(trueTarget, newValue);
                } else {
                    myJungle.throwTo(falseTarget, newValue);
                }
            }
        }
    }
}