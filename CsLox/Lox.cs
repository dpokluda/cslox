using System.CommandLine;

namespace CsLox;

class Lox
{
    private static bool hadError = false;
    
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
            RunPrompt();
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

    private static async Task RunFileAsync(string path)
    {
        string content = await File.ReadAllTextAsync(path);
        Run(content);

        if (hadError)
        {
            Environment.Exit(65);
        }
    }

    private static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();
            if (line?.Trim().IsWhiteSpace() == true) 
            {
                break;
            }
            Run(line);
        }
        
        hadError = false;
    }

    private static void Run(string source)
    {
        var scanner = new Scanner(source);
        var tokens = scanner.ScanTokens();
        
        var parser = new Parser(tokens);
        var expr = parser.Parse();
        
        if (hadError) return;
        
        var printer = new AstPrinter();
        Console.WriteLine(expr.Accept(printer));
    }
    
    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }
    
    public static void Error(Token token, string message)
    {
        if (token.Type == TokenType.Eof)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }
    
    private static void Report(int line, string what, string message)
    {
        Console.WriteLine($"[line {line}] Error{what}: {message}");
        hadError = true;
    }
}
