using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge16
    {
        public static Dictionary<String, Valve> valveMap = new Dictionary<String, Valve>();

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

            // Create object
            
            String startingKey = "";
            foreach (string line in content) {
                var command = parseCommand(line);
                if (valveMap.Count == 0) startingKey = command.key;
                valveMap.Add(command.key, command.value);
            }

            calculatePath();

            Valve? startingValve;
            valveMap.TryGetValue(startingKey, out startingValve);
            
            if (startingValve == null) throw new Exception();
            Int64 result1 = startingValve.calculate1();

            Console.WriteLine("Part 1 result: " + result1);
            Console.WriteLine("Part 2 result: ???");
        }

        static (String key, Valve value) parseCommand(string line) {
            Regex commandRegex = new Regex(@"^Valve ([A-Z]{2}) has flow rate=(\d+); tunnels? leads? to valves? ([A-Z\,\s]+)$");
            Match? matchCommand = commandRegex.Matches(line).Cast<Match>().FirstOrDefault();
            if (matchCommand == null) {
                // Can't parse
                throw new Exception();
            }
            GroupCollection groups = matchCommand.Groups;
            String key = groups[1].Value;
            int pressure = Int32.Parse(groups[2].Value);
            String listOfTunnels = groups[3].Value;
            List<String> tunnels = listOfTunnels.Split(", ").ToList();
            return (key, new Valve(key, pressure, tunnels));
        }

        static void calculatePath() {
            foreach (Valve v in valveMap.Values) {
                v.paths[v.myKey] = 0;
                calculatePaths(v, v.myKey);
            }
        }

        static void calculatePaths(Valve v, String target) {
            HashSet<String> visited = new HashSet<String>();
            HashSet<String> available = new HashSet<String>();
            Valve? current = v;
            while (current != null && visited.Count < valveMap.Count) {
                visited.Add(current.myKey);
                int dist = v.paths[current.myKey] + 1;
                foreach (String reachable in current.tunnels) {
                    available.Add(reachable);
                    if (v.paths.ContainsKey(reachable)) {
                        v.paths[reachable] = Math.Min(v.paths[reachable], dist);
                    } else {
                        v.paths[reachable] = dist;
                    }
                }
                current = valveMap.Values.Where(next => !visited.Contains(next.myKey) && available.Contains(next.myKey)).FirstOrDefault();
            }
        }
    }

    class Valve {
        public String myKey {get; private set;}
        public int pressure {get; private set;}
        public List<String> tunnels {get; private set;}
        public Dictionary<String, int> paths {get; set;}

        public Valve(String key, int pressure, List<String> tunnels) {
            this.myKey = key;
            this.pressure = pressure;
            this.tunnels = tunnels;
            this.paths = new Dictionary<String, int>();
        }

        public Int64 calculate1() {
            int remainingMinutes = 30;
            IEnumerable<Valve> usefulValves = Challenge16.valveMap.Values.Where(v => v.pressure > 0);
            return calculate1(remainingMinutes, usefulValves);
        }

        public Int64 calculate1(int remainingTime, IEnumerable<Valve> remainingValves) {
            Int64 best = 0;
            --remainingTime;
            foreach (Valve n in remainingValves) {
                int leftOverTime = remainingTime - this.paths[n.myKey];
                if (leftOverTime > 0) {
                    Int64 gain = leftOverTime * n.pressure + n.calculate1(leftOverTime, remainingValves.Where(next => next.myKey != n.myKey));
                    best = Int64.Max(best, gain);
                }
            }
            return best;
        }

    }
}