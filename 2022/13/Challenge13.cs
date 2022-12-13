using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge13
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
            string[] pairs = reader.ReadToEnd().Split("\r\n\r\n");

            int result1 = 0;
            int result2 = 1;
            int index = 1;
            List<string> sortedPackets = new List<string>();
            foreach (string pair in pairs) {
                string[] split = pair.Split("\r\n");
                if (evaluateSignalPair(split[0], split[1], false)) result1 += index;
                ++index;
                sortedPackets.Add(split[0]);
                sortedPackets.Add(split[1]);
            }

            sortedPackets.Add("[[2]]");
            sortedPackets.Add("[[6]]");
            sortedPackets.Sort(new StringEvaluator());
            result2 = (sortedPackets.IndexOf("[[2]]")+1) * (sortedPackets.IndexOf("[[6]]")+1);

            Console.WriteLine("Part 1 result: " + result1);
            Console.WriteLine("Part 2 result: " + result2);
        }

        static Boolean evaluateSignalPair(string left, string right, bool debug) {
            if (debug) Console.WriteLine("- Compare " + left + " vs " + right + " :start");
            int result = evaluateSignalPair(getElements(left), getElements(right), debug);
            if (debug) Console.WriteLine("- Compare " + left + " vs " + right + " : " + result);
            return result > 0;
        }

        public static int evaluateSignalPair(List<string> left, List<string> right, bool debug) {
            if (debug) Console.WriteLine("Compare " + readableOf(left) + " vs " + readableOf(right));
            int index = 0;
            while (!(left.Count == index && right.Count == index)) {
                if (left.Count == index) {
                    if (debug) Console.WriteLine("Left side ran out of items, so inputs are in the right order");
                    return +1;
                }
                if (right.Count == index) {
                    if (debug) Console.WriteLine("Right side ran out of items, so inputs are not in the right order");
                    return -1;
                }
                if (!left[index].Contains('[') && !right[index].Contains('[')) {
                    int leftInt = Int32.Parse(left[index]);
                    int rightInt = Int32.Parse(right[index]);
                    if (debug) Console.WriteLine("Compare " + leftInt + " vs " +rightInt);
                    if (leftInt < rightInt) {
                        if (debug) Console.WriteLine("Left side is smaller, so inputs are in the right order");
                        return +1;
                    }
                    if (rightInt < leftInt) {
                        if (debug) Console.WriteLine("Right side is smaller, so inputs are not in the right order");
                        return -1;
                    }
                } else {
                    List<string> leftList;
                    List<string> rightList;

                    if (left[index].StartsWith('[')) {
                        leftList = getElements(left[index]);
                    } else {
                        leftList = getElements("[" + left[index] + "]");
                    }

                    if (right[index].StartsWith('[')) {
                        rightList = getElements(right[index]);
                    } else {
                        rightList = getElements("[" + right[index] + "]");
                    }

                    int res = evaluateSignalPair(leftList, rightList, debug);
                    if (res != 0) return res;
                }

                ++index;
            }
            return 0;
        }

        public static List<string> getElements(string str) {
            // Catch case empty list ("[]")
            if (str.Equals("[]")) return new List<string>();
            // Strip [ and ]
            str = str.Substring(1, str.Length-2);
            if (str.Contains('[')) {
                int dep = 0;
                int iStart = 0;
                List<string> strings = new List<string>();
                for (int i = 0; i < str.Length; ++i) {
                    if (str[i] == '[') {
                        if (dep == 0) {
                            iStart = i;
                        }
                        ++dep;
                    } else if (str[i] == ']') {
                        --dep;
                    } else if (dep == 0 && str[i] == ',') {
                        strings.Add(str.Substring(iStart, i-iStart));
                        iStart = i+1;
                    }
                }
                strings.Add(str.Substring(iStart, str.Length-iStart));
                return strings;
            } else {
                return str.Split(',').ToList();
            }
        }

        static string readableOf(List<string> strings) {
            string output = "[";
            foreach (string s in strings) output = output + s + ",";
            output = output.Remove(output.Length-1);
            output = output + "]";
            return output;
        }
    }

    public class StringEvaluator : Comparer<string> {
        public override int Compare(string? a, string? b) {
            string _a = a ?? "";
            string _b = b ?? "";
            return -Challenge13.evaluateSignalPair(Challenge13.getElements(_a), Challenge13.getElements(_b), false);
        }
    }
}