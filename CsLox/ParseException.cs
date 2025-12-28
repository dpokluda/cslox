namespace CsLox;

public class ParseException : Exception
{
    public ParseException(Token token, string message) : base(message)
    {
        Token = token;
    }

    public Token Token { get; init; }
}