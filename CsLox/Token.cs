namespace CsLox;

public class Token
{
    public Token(TokenType type, string lexeme, object? literal, int line)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        Line = line;
    }
    
    public TokenType Type { get; init; }
    public string Lexeme { get; init; } = string.Empty;
    public object? Literal { get; init; } = null;
    public int Line { get; init; } = 0;
    
    public override string ToString() => $"{Type} {Lexeme} {Literal}";
}