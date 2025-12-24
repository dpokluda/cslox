using System.CommandLine;

namespace CsLox;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<FileInfo?>("--file") { Description = "Script file to execute" };
        var interactiveOption = new Option<bool>("--interactive") { Description = "Run in interactive REPL mode" };

        var rootCommand = new RootCommand("C# implementation of Lox language from Crafting Interpreters book.")
        {
            fileOption,
            interactiveOption
        };

        rootCommand.SetAction(RunCommandAsync);

        return await rootCommand.Parse(args).InvokeAsync();
    }

    private static async Task<int> RunCommandAsync(ParseResult parseResult)
    {
        var file = parseResult.GetValue<FileInfo>("--file");
        var isInteractive = parseResult.GetValue<bool>("--interactive");
        if (file != null)
        {
            ConsoleEx.WriteLine(ConsoleColor.Yellow, $"Running script from file {file.FullName}...");
            await RunFileAsync(file.FullName);
        }
        else if (isInteractive)
        {
            ConsoleEx.WriteLine(ConsoleColor.Yellow, "Running in interactive REPL mode...");
            await RunPromptAsync();
        }
        else
        {
            Console.WriteLine("Usage: CsLox [--file <path>] [--interactive]");
            Console.WriteLine("Use --help for more information.");
            return 1;
        }

        ConsoleEx.WriteLine(ConsoleColor.Green, "Finished.");
        return 0;
    }

    static async Task RunFileAsync(string path)
    {
        string content = await File.ReadAllTextAsync(path);
        await RunAsync(content);
    }

    static async Task RunPromptAsync()
    {
        // display "> " prompt and read input line by line and execute RunAsync on each line
        while (true)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();
            if (line?.Trim().IsWhiteSpace() == true) 
            {
                break;
            }
            await RunAsync(line);
        }
    }

    static async Task RunAsync(string source)
    {
        var scanner = new Scanner(source);
        var tokens = await scanner.ScanTokensAsync();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }
}
