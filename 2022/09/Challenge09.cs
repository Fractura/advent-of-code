using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge09
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

            Snake snake1 = new Snake(2);
            Snake snake2 = new Snake(10);

            foreach (string line in content) {
                (Direction, int) command = parseCommand(line);
                snake1.moveHead(command.Item1, command.Item2);
                snake2.moveHead(command.Item1, command.Item2);
            }

            Console.WriteLine("Part 1 result: " + snake1.getVisitedCount());
            Console.WriteLine("Part 2 result: " + snake2.getVisitedCount());
        }

        static (Direction, int) parseCommand(string line) {
            Regex commandRegex = new Regex(@"^([DLRU])\s(\d+)$");
            Match? matchCommand = commandRegex.Matches(line).Cast<Match>().FirstOrDefault();
            if (matchCommand == null) {
                return (Direction.DOWN, 0);
            }
            GroupCollection groups = matchCommand.Groups;
            Direction direction;
            switch (groups[1].Value[0]) 
            {
                case 'L': direction = Direction.LEFT; break;
                case 'R': direction = Direction.RIGHT; break;
                case 'U': direction = Direction.UP; break;
                default: direction = Direction.DOWN; break;
            }
            int amount = Int32.Parse(groups[2].Value);
            return (direction, amount);
        }
    }

    enum Direction {
        LEFT, RIGHT, UP, DOWN
    }

    class Snake {
        Position[] snake;
        HashSet<Position> visited = new HashSet<Position>();

        public Snake(int size) {
            this.snake = new Position[size];
            visited.Add(snake.Last());
        }

        public int getVisitedCount() {
            return visited.Count;
        }

        public void moveHead(Direction d, int amount) {
            moveHead(d, amount, false);
        }

        public void moveHead(Direction d, int amount, Boolean debug) {
            for (int i = 0; i < amount; ++i) {
                snake[0].move(d);
                moveTailIfNecessary();
                visited.Add(snake.Last());
            }
            if (debug) {
                Console.WriteLine(String.Format("Line '{0}' resulted in Head at {1} and Tail at {2}. (visited={3})", (d, amount), snake.First().toString(), snake.Last().toString(), visited.Count));
            }
        }

        void moveTailIfNecessary() {
            for (int i = 0; i < snake.Length-1; ++i) {
                snake[i+1] = moveTailIfNecessary(snake[i], snake[i+1]);
            }
        }

        // returns new position for trailing element because it seems this is call-by-value
        Position moveTailIfNecessary(Position leading, Position trailing) {
            
            if (Math.Abs(leading.x - trailing.x) > 1) {
                if (leading.x < trailing.x) {
                    trailing.move(Direction.LEFT);
                } else {
                    trailing.move(Direction.RIGHT);
                }
                if (leading.y != trailing.y) {
                    if (leading.y < trailing.y) {
                        trailing.move(Direction.DOWN);
                    } else {
                        trailing.move(Direction.UP);
                    }
                }
            }
            if (Math.Abs(leading.y - trailing.y) > 1) {
                if (leading.y < trailing.y) {
                    trailing.move(Direction.DOWN);
                } else {
                    trailing.move(Direction.UP);
                }
                if (leading.x != trailing.x) {
                    if (leading.x < trailing.x) {
                        trailing.move(Direction.LEFT);
                    } else {
                        trailing.move(Direction.RIGHT);
                    }
                }
            }
            return trailing;
        }
    }

    struct Position {
        public int x {get; set;}
        public int y {get; set;}

        public Position () {
            this.x = 0;
            this.y = 0;
        }

        public Position (int x, int y) {
            this.x = x;
            this.y = y;
        }

        public string toString() {
            return String.Format("[{0},{1}]", x, y);
        }

        public void move(Direction d) {
            switch (d) {
                case Direction.LEFT: --x; break;
                case Direction.RIGHT: ++x; break;
                case Direction.UP: ++y; break;
                case Direction.DOWN: --y; break;
            }
        }
    }
}