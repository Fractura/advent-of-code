using System.Text.RegularExpressions;

namespace challenge
{
    class Challenge04
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
            int result1 = 0;
            int result2 = 0;
            Regex rx = new Regex(@"(\d+)\-(\d+),(\d+)\-(\d+)");
            string contents = reader.ReadToEnd();
            MatchCollection matches = rx.Matches(contents);

            foreach (Match match in matches) {
                GroupCollection groups = match.Groups;
                Range a = new Range(Int32.Parse(groups[1].Value), Int32.Parse(groups[2].Value));
                Range b = new Range(Int32.Parse(groups[3].Value), Int32.Parse(groups[4].Value));
                if (isWithin(a,b)) ++result1;
                if (hasOverlap(a,b)) ++result2;
            }
            Console.WriteLine("Part 1 result: " + result1);
            Console.WriteLine("Part 2 result: " + result2);
        }

        static Boolean isWithin(Range a, Range b) {
            // a in b or b in a
            return (a.Start.Value >= b.Start.Value && a.End.Value <= b.End.Value) || (b.Start.Value >= a.Start.Value && b.End.Value <= a.End.Value);
        }

        static Boolean hasOverlap(Range a, Range b) {
            // not (a starts after b ends OR b starts after a ends)
            return !((a.Start.Value > b.End.Value) || (b.Start.Value > a.End.Value));
        }
    }
}

