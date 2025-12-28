namespace CsLox;

public class RuntimeException : Exception
{
    public RuntimeException(Token token, string message) : base(message)
    {
        Token = token;
    }
        
    public Token Token { get; init; }
}