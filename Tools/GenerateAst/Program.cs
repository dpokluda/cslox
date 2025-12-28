namespace GenerateAst;

using System.CommandLine;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var output = new Option<DirectoryInfo>("--output") { Description = "Path to the output directory" };

        var rootCommand = new RootCommand("Generates AST classes for the Lox interpreter.")
        {
            output
        };

        rootCommand.SetAction(RunCommandAsync);

        return await rootCommand.Parse(args).InvokeAsync();
    }

    private static async Task<int> RunCommandAsync(ParseResult parseResult)
    {
        var output = parseResult.GetValue<DirectoryInfo>("--output");
        output.Create();
        Console.WriteLine($"Directory '{output.FullName}' created.");
        
        var generator = new AstGenerator();
        generator.DefineAst(
            [
                "using System.Collections.Generic;",
                "",
                "namespace CsLox;"
            ],
            "Expr",
            output.FullName,
            [
                "Assign   : Token name, Expr value",
                "Binary   : Expr left, Token operator, Expr right",
                "Call     : Expr callee, Token paren, List<Expr> arguments",
                "Get      : Expr object, Token name",
                "Grouping : Expr expression",
                "Literal  : Object? value",
                "Logical  : Expr left, Token operator, Expr right",
                "Set      : Expr object, Token name, Expr value",
                "Super    : Token keyword, Token method",
                "This     : Token keyword",
                "Unary    : Token operator, Expr right",
                "Variable : Token name"
            ]);
        
        ConsoleEx.WriteLine(ConsoleColor.Green, "Finished.");
        return 0;
    }
}