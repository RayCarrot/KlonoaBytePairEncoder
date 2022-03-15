using BinarySerializer;
using BinarySerializer.Klonoa.KH;

if (args.Length != 3 && args.Length != 4)
{
    ShowUsage();
    return;
}

int? blockSize = args.Length == 4 ? Int32.Parse(args[3]) : null;

switch (args[0])
{
    case "-d":
        Process(args[1], args[2], false, blockSize);
        break;

    case "-c":
        Process(args[1], args[2], true, blockSize);
        break;

    default:
        ShowUsage();
        break;
}

static void ShowUsage()
{
    Console.WriteLine($"Usage:{Environment.NewLine}" +
                      $"Compress file(s):  -c inputPath outputPath (blockSize){Environment.NewLine}" +
                      $"Decompress file(s) -d inputPath outputPath{Environment.NewLine}{Environment.NewLine}" +
                      $"Paths can be either files or folders. If it's a folder every file in the folder will be included.{Environment.NewLine}" +
                      $"Specifying the block size is optional. By default it is 1024.");
}

static void Process(string input, string output, bool compress, int? blockSize)
{
    if (input == output)
    {
        Console.WriteLine("Input and output can not be the same!");
        return;
    }

    BytePairEncoder encoder = new();

    if (blockSize != null)
        encoder.BlockSize = blockSize.Value;

    if (File.Exists(input))
    {
        ProcessFile(input, output, compress, encoder);
        Console.WriteLine("Finished");
    }
    else if (Directory.Exists(input))
    {
        foreach (string file in Directory.GetFiles(input, "*", SearchOption.TopDirectoryOnly))
            ProcessFile(file, Path.Combine(output, Path.GetFileName(file)), compress, encoder);

        Console.WriteLine("Finished");
    }
    else
    {
        Console.WriteLine("Invalid input");
    }
}

static void ProcessFile(string input, string output, bool compress, IStreamEncoder encoder)
{
    using FileStream inputStream = File.OpenRead(input);
    using FileStream outputStream = File.OpenWrite(output);

    if (compress)
        encoder.EncodeStream(inputStream, outputStream);
    else
        encoder.DecodeStream(inputStream, outputStream);

    Console.WriteLine($"{Path.GetFileName(input)} -> {Path.GetFileName(output)}");
}