using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace challenge
{
    static partial class Challenge09
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

        class FileBlock {
            public int fileId;
            public int size;

            public FileBlock(int f, int s) {
                fileId = f;
                size = s;
            }

            public override string ToString()
            {
                return $"id={fileId},sz={size}";
            }
        }

        struct Filesystem {
            public int[] fileblocks;
            public int[] freeblocks;
            public long checksum1;
            public List<FileBlock> defragmentedSystem;
            public long checksum2;

            public Filesystem(string input)
            {
                fileblocks = new int[input.Length / 2 + 1];
                freeblocks = new int[input.Length / 2];
                defragmentedSystem = new();

                for (int i = 0; i < input.Length; ++i)
                {
                    if (i % 2 == 0)
                    {
                        int size = (int)char.GetNumericValue(input[i]);
                        fileblocks[i / 2] = size;
                        defragmentedSystem.Add(new FileBlock(i/2, size));
                    }
                    else
                    {
                        int size = (int)char.GetNumericValue(input[i]);
                        freeblocks[i / 2] = size;
                        if (size > 0) {
                            defragmentedSystem.Add(new FileBlock(-1, size));
                        }
                    }
                }

                checksum1 = MoveFilesAndCalculateChecksum1(input);
                checksum2 = MoveFilesAndCalculateChecksum2();
            }

            private long MoveFilesAndCalculateChecksum1(string input)
            {
                long checksum = 0;
                Boolean moving = false;
                int blockIndex = 0;
                int freeFileIndex = 0;
                int freeFileBlocksRemain = freeblocks[0];
                int forwardFileIndex = 0;
                int forwardFileBlocksRemain = fileblocks[0];
                int backwardFileIndex = input.Length / 2;
                int backwardFileBlocksRemain = fileblocks[input.Length / 2];
                while (true)
                {
                    int fileId;
                    if (moving)
                    {
                        fileId = backwardFileIndex;
                        --backwardFileBlocksRemain;

                        if (backwardFileBlocksRemain == 0)
                        {
                            --backwardFileIndex;
                            backwardFileBlocksRemain = fileblocks[backwardFileIndex];
                        }

                        --freeFileBlocksRemain;

                        if (freeFileBlocksRemain == 0)
                        {
                            ++freeFileIndex;
                            freeFileBlocksRemain = freeblocks[freeFileIndex];
                            moving = false;
                        }

                    }
                    else
                    {
                        fileId = forwardFileIndex;
                        --forwardFileBlocksRemain;
                        if (forwardFileBlocksRemain == 0)
                        {
                            ++forwardFileIndex;
                            forwardFileBlocksRemain = fileblocks[forwardFileIndex];
                            if (freeFileBlocksRemain == 0)
                            {
                                // Free space between files is 0
                                ++freeFileIndex;
                                freeFileBlocksRemain = freeblocks[freeFileIndex];
                            }
                            else
                            {
                                moving = true;
                            }
                        }
                    }

                    checksum += blockIndex * fileId;
                    ++blockIndex;

                    if (forwardFileIndex == backwardFileIndex)
                    {
                        while (backwardFileBlocksRemain > 0)
                        {
                            checksum += blockIndex * backwardFileIndex;
                            ++blockIndex;
                            --backwardFileBlocksRemain;
                        }

                        return checksum;
                    }

                }
            }

            private readonly long MoveFilesAndCalculateChecksum2()
            {
                long checksum = 0;
                for (int file = fileblocks.Length - 1; file > 0; --file) {
                    int originalIndex = defragmentedSystem.FindIndex(0, defragmentedSystem.Count, fb => fb.fileId == file);
                    int size = defragmentedSystem[originalIndex].size;
                    int destination = defragmentedSystem.FindIndex(0, originalIndex, b => b.size >= size && b.fileId == -1);

                    if (destination != -1) {
                        RemoveFileBlock(file);
                        InsertFileBlock(destination, file, size);
                    }
                }

                int b = 0;
                foreach (FileBlock fb in defragmentedSystem) {
                    for (int i = 0; i < fb.size; ++i) {
                        if (fb.fileId > 0) {
                            checksum += b * fb.fileId;
                        }
                        ++b;
                    }
                }

                return checksum;
            }

            private readonly void InsertFileBlock(int destination, int fileId, int size) {
                FileBlock freeSpaceBlock = defragmentedSystem[destination];
                if (freeSpaceBlock.size == size) {
                    freeSpaceBlock.fileId = fileId;
                } else {
                    freeSpaceBlock.size -= size;
                    FileBlock newBlock = new(fileId, size);
                    defragmentedSystem.Insert(destination, newBlock);
                }
            }

            private readonly void RemoveFileBlock(int id) {
                int index = defragmentedSystem.FindLastIndex(fb => fb.fileId == id);
                FileBlock previous = defragmentedSystem[index-1];
                FileBlock remove = defragmentedSystem[index];
                FileBlock next = new (0, 0);
                if (defragmentedSystem.Count > index+1) {
                    next = defragmentedSystem[index+1];
                }

                // set fileid to empty
                remove.fileId = -1;

                // merge with free space after
                if (next.fileId == -1) {
                    remove.size += next.size;
                    defragmentedSystem.Remove(next);
                }

                // merge with free space before
                if (previous.fileId == -1) {
                    remove.size += previous.size;
                    defragmentedSystem.Remove(previous);
                }
            }
        }

        static void Main(string[] args)
        {
            string[] contentEx = ReadFile("example.input");
            string[] content = ReadFile("my.input");

            Filesystem fsEx = new(contentEx[0]);
            Filesystem fs = new(content[0]);

            Debug.Assert(PartOne(fsEx) == 1928);
            Console.WriteLine($"Part 1 : {PartOne(fs)}");
            Debug.Assert(PartTwo(fsEx) == 2858);
            Console.WriteLine($"Part 2 : {PartTwo(fs)}");
        }

        static long PartOne(Filesystem fs)
        {
            return fs.checksum1;
        }

        static long PartTwo(Filesystem fs)
        {
            return fs.checksum2;
        }
    }
}
