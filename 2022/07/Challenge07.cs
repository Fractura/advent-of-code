using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge07
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

            var result1 = 0;
            var result2 = Int32.MaxValue;

            Stack<string> cwd = new Stack<string>();
            Dictionary<string, Directory> dirMap = new Dictionary<string, Directory>();

            cwd.Push("");
            Directory rootDir = new Directory(null);
            dirMap.Add("/", rootDir);

            Regex regexCd = new Regex(@"^\$\scd\s(\w+)$");
            Regex regexCdUp = new Regex(@"^\$\scd\s\.\.$");
            Regex regexLs = new Regex(@"^\$\sls$");
            Regex regexDirListing = new Regex(@"^dir\s(\w+)$");
            Regex regexFileListing = new Regex(@"(\d+)\s([\w\.]+)$");
            
            foreach (string line in content) {
                if (line.Equals("$ cd /")) {
                    // Ignore first line lol
                } else if (line[0] == '$') {
                    Match? matchCd = regexCd.Matches(line).Cast<Match>().FirstOrDefault();
                    Match? matchCdUp = regexCdUp.Matches(line).Cast<Match>().FirstOrDefault();
                    Match? matchLs = regexLs.Matches(line).Cast<Match>().FirstOrDefault();
                    if (matchCd != null) {
                        GroupCollection groups = matchCd.Groups;
                        string dirname = groups[1].Value;

                        cwd.Push(dirname);
                    } else if (matchCdUp != null && cwd.Count > 1) {
                        cwd.Pop();
                    } else if (matchLs != null) {
                        // Directory listing next.
                    } else {
                        Console.WriteLine("Parsing input failed: " + line);
                    }
                } else {
                    Match? matchDirListing = regexDirListing.Matches(line).Cast<Match>().FirstOrDefault();
                    Match? matchFileListing = regexFileListing.Matches(line).Cast<Match>().FirstOrDefault();
                    if (matchDirListing != null) {
                        GroupCollection groups = matchDirListing.Groups;
                        string dirname = groups[1].Value;

                        string path = "";
                        foreach (string d in cwd) {
                            path = d + "/" + path;
                        }

                        Directory? parent;
                        Directory newDirectory;
                        if (dirMap.TryGetValue(path, out parent)) {
                            newDirectory = new Directory(parent);
                        } else {
                            Console.WriteLine("Couldn't find directory: " + path);
                            return;
                        }
                        dirMap.Add(path + dirname + "/", newDirectory);
                    } else if (matchFileListing != null) {
                        GroupCollection groups = matchFileListing.Groups;
                        int size = Int32.Parse(groups[1].Value);
                        string file = groups[2].Value;

                        string path = "";
                        foreach (string d in cwd) {
                            path = d + "/" + path;
                        }

                        Directory? current;
                        if (dirMap.TryGetValue(path, out current)) {
                            current.add(size);
                        } else {
                            Console.WriteLine("Couldn't find directory: " + path);
                        }
                    } else {
                        Console.WriteLine("Parsing input failed: " + line);
                    }
                }
            }

            // Calculate result 1:
            foreach (Directory d in dirMap.Values) {
                if (d.cumulativeDirSize <= 100000) {
                    result1 += d.cumulativeDirSize;
                }
            }

            // Calculate result 2:
            int filesystem_space = 70000000;
            int required_space = 30000000;
            int used_space = rootDir.cumulativeDirSize;
            int ideal_size_needed = used_space - (filesystem_space - required_space);
            foreach (Directory d in dirMap.Values) {
                if (d.cumulativeDirSize > ideal_size_needed) {
                    result2 = Int32.Min(d.cumulativeDirSize, result2);
                }
            }

            Console.WriteLine("Part 1 result: " + result1);
            Console.WriteLine("Part 2 result: " + result2);
        }
    }

    class Directory {
        public int cumulativeDirSize { get; set; }
        public Directory? parent { get; set; }

        public Directory(Directory? par) {
            cumulativeDirSize = 0;
            parent = par;
        }

        public void add(int size) {
            cumulativeDirSize += size;
            if (parent != null) parent.add(size);
        }

    }
}