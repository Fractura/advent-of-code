namespace challenge
{
    class Challenge03
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
            string? line;
            Boolean[] characterPresentPart1 = new Boolean[53];
            Boolean[] characterPresentPart2a = new Boolean[53]; // tracks present characters in line
            Boolean[] characterPresentPart2b = new Boolean[53];  // tracks present characters in all 3 group members
            int result1 = 0;
            int result2 = 0;
            int lineNumber = 0;
            Boolean part1DoneForThisLine = false;
            while ((line = reader.ReadLine()) != null) {
                string firstHalf = line.Substring(0, line.Length/2);
                string secondHalf = line.Substring(line.Length/2);

                for (int i = 0; i < firstHalf.Length; ++i) {
                    int value = getValueForCharacter(firstHalf.ElementAt(i));
                    characterPresentPart1[value] = true;
                    characterPresentPart2a[value] = true;
                }

                for (int i = 0; i < secondHalf.Length; ++i) {
                    int value = getValueForCharacter(secondHalf.ElementAt(i));
                    if (characterPresentPart1[value]) {
                        if (!part1DoneForThisLine) {
                            result1 += value;
                            part1DoneForThisLine = true;
                        }
                    }
                    characterPresentPart2a[value] = true;
                }

                if (lineNumber % 3 == 0) {
                    Array.Copy(characterPresentPart2a, characterPresentPart2b, 53);
                } else {
                    for (int i = 1; i < 53; ++i) {
                        characterPresentPart2b[i] = characterPresentPart2b[i] && characterPresentPart2a[i];
                    }
                }

                if (lineNumber % 3 == 2) {
                    for (int i = 1; i < 53; ++i) {
                        if (characterPresentPart2b[i]) {
                            result2 += i;
                            break;
                        }
                    }
                }

                Array.Fill<Boolean>(characterPresentPart1, false);
                Array.Fill<Boolean>(characterPresentPart2a, false);
                ++lineNumber;
                part1DoneForThisLine = false;
            }
            Console.WriteLine("Part 1 result: " + result1);
            Console.WriteLine("Part 2 result: " + result2);
        }

        static int getValueForCharacter(char character) {
            int value = character - '@';
            // Swap value of Upper/Lowercase, because the task has the lowercase characters as values 1-26 and uppercase is first in ascii table
            if (value > 26) {
                // Letter is lowercase, subtract 6 to account for the 6 characters between A-Z and a-z
                value = value - 6 - 26;
            } else {
                // Letter is uppercase
                value = value + 26;
            }
            return value;
        }
    }
}

