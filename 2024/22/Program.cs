using System.Diagnostics;

namespace challenge
{
    static partial class Challenge22
    {
        static string[] ReadFile(string filename)
        {
            var s = Directory.GetCurrentDirectory();
            if (s.Contains("bin")) {
                s = s[..^16];
            }
            string path = $"{s}\\{filename}";
            
            if (!File.Exists(path)) {
                Console.WriteLine("Input file not found");
            }

            StreamReader reader = File.OpenText(path);
            string[] content = reader.ReadToEnd().Split("\r\n");

            return content;
        }

        class Market {
            readonly List<long> secretNumbers;

            public Market(string[] input) {
                secretNumbers = new();
                foreach (string line in input) {
                    secretNumbers.Add(long.Parse(line));
                }
            }

            public long GetSumOfSecretNumbers(int iteration) {
                long sum = 0;
                foreach (long n in secretNumbers) {
                    sum += GetSecretNumber(n, iteration);
                }
                return sum;
            }

            readonly static Dictionary<long, long> sncache = new();
            readonly static Dictionary<long, long> changecache = new();

            public static long GetSecretNumber(long initial, int iteration) {
                long sn = initial;
                
                for (int i = 0; i < iteration; ++i) {
                    if (sncache.ContainsKey(sn)) {
                        sn = sncache[sn];
                        continue;
                    }
                    long numberToMix = sn * 64;
                    long mixed = Mix(sn, numberToMix);
                    long pruned = Prune(mixed);
                    long div = pruned / 32;
                    mixed = Mix(pruned, div);
                    pruned = Prune(mixed);
                    long mul = pruned * 2048;
                    mixed = Mix(pruned, mul);
                    pruned = Prune(mixed);
                    sncache[sn] = pruned;
                    changecache[pruned] = pruned % 10 - sn % 10;
                    sn = pruned;
                }
                return sn;
            }

            public long TryAllSequences() {
                List<(int,int,int,int)> sequences = new();
                for (int a = -9; a < 10; ++a) {
                    for (int b = -9; b < 10; ++b) {
                        for (int c = -9; c < 10; ++c) {
                            for (int d = 0; d < 10; ++d) {
                                if (a+b+c+d > 0 && a+b+c+d < 10) {
                                    sequences.Add((a,b,c,d));
                                }
                            }
                        }
                    }
                }
                foreach (long sn in secretNumbers) {
                    Precache(sn);
                }
                return sequences.AsParallel().Select(GetPriceSum).Max();
            }

            public long GetPriceSum((int,int,int,int) sequence) {
                long sum = 0;
                foreach (long sn in secretNumbers) {
                    sum += GetPriceCached(sn, sequence);
                }
                return sum;
            }

            readonly static Dictionary<(long initial, long iteration), (long c, (int,int,int,int) seq)> p2cache = new();
            readonly static Dictionary<(long initial, (int,int,int,int) seq), long> p2rcache = new();
            readonly static Dictionary<long, long[]> p2sncache = new();

            private static void Precache(long sn) {
                if (!p2sncache.ContainsKey(sn)) {
                    p2sncache.Add(sn, new long[2001]);
                    // preload via sncache
                    p2sncache[sn][2000] = GetSecretNumber(sn, 2000);
                    for (int i = 0; i < 2000; ++i) {
                        p2sncache[sn][i] = GetSecretNumber(sn, i);
                    }
                }
                
                for (int i = 4; i < 2000; ++i) {
                    var ch_1 = (int)changecache[p2sncache[sn][i-3]];
                    var ch_2 = (int)changecache[p2sncache[sn][i-2]];
                    var ch_3 = (int)changecache[p2sncache[sn][i-1]];
                    var ch_4 = (int)changecache[p2sncache[sn][i]];
                    var c = p2sncache[sn][i] % 10;
                    p2cache[(sn,i)] = (c, (ch_1,ch_2,ch_3,ch_4));
                    if (!p2rcache.ContainsKey((sn, (ch_1,ch_2,ch_3,ch_4)))) {
                        p2rcache[(sn, (ch_1,ch_2,ch_3,ch_4))] = c;
                    }
                }
            }

            public static long GetPriceCached(long sn, (int,int,int,int) sequence) {
                if (p2rcache.ContainsKey((sn, sequence))) {
                    return p2rcache[(sn, sequence)];
                }
                return 0;
            }

            private static long Mix(long sn, long mix) {
                return sn ^ mix;
            }

            private static long Prune(long sn) {
                return sn % 16777216;
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] contentEx2 = ReadFile("example2.input");
            string[] content = ReadFile("my.input");

            Market ex = new(contentEx);
            Market ex2 = new(contentEx2);
            Market m = new(content);

            Debug.Assert(Market.GetSecretNumber(123, 1) == 15887950);
            Debug.Assert(Market.GetSecretNumber(123, 2) == 16495136);
            Debug.Assert(Market.GetSecretNumber(123, 3) == 527345);
            Debug.Assert(Market.GetSecretNumber(123, 4) == 704524);
            Debug.Assert(Market.GetSecretNumber(123, 5) == 1553684);
            Debug.Assert(Market.GetSecretNumber(123, 6) == 12683156);
            Debug.Assert(Market.GetSecretNumber(123, 7) == 11100544);
            Debug.Assert(Market.GetSecretNumber(123, 8) == 12249484);
            Debug.Assert(Market.GetSecretNumber(123, 9) == 7753432);
            Debug.Assert(Market.GetSecretNumber(123, 10) == 5908254);
            Debug.Assert(PartOne(ex) == 37327623);
            Console.WriteLine($"Part 1 : {PartOne(m)}");
            Debug.Assert(PartTwo(ex2) == 23);
            Console.WriteLine($"Part 2 : {PartTwo(m)}");
        }

        static long PartOne(Market m)
        {
            return m.GetSumOfSecretNumbers(2000);
        }

        static long PartTwo(Market m)
        {
            return m.TryAllSequences();
        }
    }
}
