namespace CsLox;

public class ParseError : Exception
{
    public ParseError(Token token, string message)
    {
        Token = token;
        Message = message;
    }

    public Token Token { get; }
    public string Message { get; }
}