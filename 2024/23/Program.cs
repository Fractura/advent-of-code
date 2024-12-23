using System.Diagnostics;

namespace challenge
{
    static partial class Challenge23
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

        class Network {
            readonly HashSet<(string, string)> connections;
            readonly Dictionary<string, SortedSet<string>> connectionSet;
            readonly HashSet<(string, string, string)> triplets;

            public Network(string[] input) {
                connections = new();
                connectionSet = new();
                triplets = new();
                foreach (string line in input) {
                    string[] pcs = line.Split('-');
                    string a = pcs[0];
                    string b = pcs[1];
                    if (a.CompareTo(b) < 0) {
                        connections.Add((a,b));
                    } else {
                        connections.Add((b,a));
                    }
                    if (connectionSet.ContainsKey(a)) {
                        var set = connectionSet[a];
                        set.Add(b);
                    } else {
                        connectionSet[a] = new() {a,b};
                    }
                    if (connectionSet.ContainsKey(b)) {
                        var set = connectionSet[b];
                        set.Add(a);
                    } else {
                        connectionSet[b] = new() {a,b};
                    }
                }
                FindTriplets();
            }

            private void FindTriplets() {
                connectionSet.AsParallel().ForAll(x => {
                    string a = x.Key;
                    var arr = x.Value.ToArray();
                    for (int i = 0; i < arr.Length - 1; ++i) {
                        string b = arr[i];
                        for (int j = i+1; j < arr.Length; ++j) {
                            string c = arr[j];
                            if (connections.Contains((b,c))) {
                                AddTriplet(a, b, c);
                            }
                        }
                    }
                });
            }

            private void AddTriplet(string a, string b, string c) {
                if (a == b || a == c || b == c) {
                    // We're adding "self" to the connectionSet to make part 2 a bit easier,
                    // but here, this would cause triplets like (a,b,b) to be added.
                    return;
                }
                string[] strings = {a,b,c};
                Array.Sort(strings);
                lock (triplets) {
                    triplets.Add((strings[0], strings[1], strings[2]));
                }
            }

            public int FindTripletsWithT() {
                return triplets.AsParallel().Where(x => x.Item1.StartsWith('t') || x.Item2.StartsWith('t') || x.Item3.StartsWith('t')).Count();
            }

            public string FindPassword() {
                List<SortedSet<string>> list = new();
                foreach (var triplet in triplets) {
                    SortedSet<string> set = new() {triplet.Item1, triplet.Item2, triplet.Item3};
                    list.Add(set);
                }
                
                int size = 3;
                Dictionary<string, long> checksumMap = new();
                foreach (string s in connectionSet.Keys) {
                    checksumMap[s] = s.GetHashCode();
                }
                while (list.Count > 1) {
                    List<SortedSet<string>> newList = new();
                    HashSet<long> duplicateFilter = new();
                    foreach (SortedSet<string> set in list) {
                        var x = new SortedSet<string>(connectionSet[set.First()]);
                        foreach (string s in set) {
                            x.IntersectWith(connectionSet[s]);
                        }
                        long checksum = 1;
                        foreach (string s in x) {
                            checksum *= checksumMap[s];
                        }
                        if (x.Count > size && !duplicateFilter.Contains(checksum)) {
                            duplicateFilter.Add(checksum);
                            newList.Add(x);
                        }
                    }
                    list = newList;
                    ++size;
                }

                return string.Join(",", list[0]);
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Network ex = new(contentEx);
            Network nw = new(content);

            Debug.Assert(PartOne(ex) == 7);
            Console.WriteLine($"Part 1 : {PartOne(nw)}");
            Debug.Assert(PartTwo(ex) == "co,de,ka,ta");
            Console.WriteLine($"Part 2 : {PartTwo(nw)}");
        }

        static Int32 PartOne(Network nw)
        {
            return nw.FindTripletsWithT();
        }

        static string PartTwo(Network nw)
        {
            return nw.FindPassword();
        }
    }
}
