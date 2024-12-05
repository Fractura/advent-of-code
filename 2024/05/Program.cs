using System;

namespace challenge
{
    static partial class Challenge05
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

        struct Ruleset {
            public Dictionary<Int32, SortedSet<Int32>> pageOrderingRules;
            public List<List<Int32>> updatePages;
            public List<List<Int32>> correctlyOrderedUpdatePages;
            public List<List<Int32>> incorrectlyOrderedUpdatePages;

            public Ruleset(Dictionary<Int32, SortedSet<Int32>> pageOrderingRules, List<List<Int32>> updatePages) {
                this.pageOrderingRules = pageOrderingRules;
                this.updatePages = updatePages;
                var updatePagesChecked = checkUpdatePages();
                this.correctlyOrderedUpdatePages = updatePagesChecked.Item1;
                this.incorrectlyOrderedUpdatePages = updatePagesChecked.Item2;
            }

            private (List<List<Int32>>,List<List<Int32>>) checkUpdatePages() {
                List<List<Int32>> correct = new();
                List<List<Int32>> incorrect = new();
                foreach (List<Int32> updatePage in updatePages) {
                    Boolean pageValid = true;
                    List<Int32> processed = new();
                    foreach (Int32 entry in updatePage) {
                        if (!pageValid) {
                            break;
                        }
                        foreach (Int32 processedPage in processed) {
                            if (pageOrderingRules.ContainsKey(entry) && pageOrderingRules[entry].Contains(processedPage)) {
                                pageValid = false;
                                break;
                            }
                        }
                        processed.Add(entry);
                    }

                    if (pageValid) {
                        correct.Add(updatePage);
                    } else {
                        incorrect.Add(updatePage);
                    }
                }

                return (correct, incorrect);
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Ruleset rulesetExample = ParseInput(contentEx);
            Ruleset ruleset = ParseInput(content);

            Console.WriteLine($"Example part 1: {PartOne(rulesetExample)}");
            Console.WriteLine($"Part 1 : {PartOne(ruleset)}");
            Console.WriteLine($"Example part 2: {PartTwo(rulesetExample)}");
            Console.WriteLine($"Part 2 : {PartTwo(ruleset)}");
        }

        static Ruleset ParseInput(string[] input) {
            Dictionary<Int32, SortedSet<Int32>> pageOrderingRules = new();
            List<List<Int32>> updatePages = new();
            Boolean finishedParsingOrderingRules = false;
            foreach (string line in input) {
                if (line.Length < 1) {
                    finishedParsingOrderingRules = true;
                    continue;
                }
                if (finishedParsingOrderingRules) {
                    List<Int32> ints = line.Split(",").Select(Int32.Parse).ToList();
                    updatePages.Add(ints);
                } else {
                    string[] values = line.Split("|");
                    Int32 left = int.Parse(values[0]);
                    Int32 right = int.Parse(values[1]);
                    if (pageOrderingRules.ContainsKey(left)) {
                        pageOrderingRules[left].Add(right);
                    } else {
                        pageOrderingRules[left] = new SortedSet<Int32> {right};
                    }
                }
            }
            return new Ruleset(pageOrderingRules, updatePages);
        }

        static Int32 PartOne(Ruleset ruleset)
        {
            Int32 count = 0;
            foreach (List<Int32> update in ruleset.correctlyOrderedUpdatePages) {
                count += GetMiddlePageNumber(update);
            }
            return count;
        }

        static Int32 PartTwo(Ruleset ruleset)
        {
            Int32 count = 0;
            foreach (List<Int32> update in ruleset.incorrectlyOrderedUpdatePages) {
                count += GetMiddlePageNumber(OrderUpdatePage(update, ruleset));
            }
            return count;
        }

        static Int32 GetMiddlePageNumber(List<Int32> pages) {
            if (pages.Count == 0) {
                return 0;
            } else return pages[pages.Count / 2];
        }

        static List<Int32> OrderUpdatePage(List<Int32> pages, Ruleset ruleset) {
            List<Int32> newPage = new();
            List<Int32> copy = pages.ToList();
            while (copy.Count > 0) {
                Int32 next = FindFirst(copy, ruleset);
                newPage.Add(next);
                copy.Remove(next);
            }

            return newPage;
        }

        static Int32 FindFirst(List<Int32> pages, Ruleset ruleset) {
            foreach (Int32 candidate in pages) {
                Boolean validCandidate = true;
                foreach (Int32 other in pages) {
                    if (ruleset.pageOrderingRules.ContainsKey(other) && ruleset.pageOrderingRules[other].Contains(candidate)) {
                        validCandidate = false;
                        break;
                    }
                }
                if (validCandidate) {
                    return candidate;
                }
            }
            Console.WriteLine($"Error during processing: cannot find valid candidate for list {pages}.");
            return -1;
        }
    }
}
