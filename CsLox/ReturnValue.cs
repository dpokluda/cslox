namespace CsLox;

internal class ReturnValue : Exception
{
    public object? Value { get; init; }
    
    public ReturnValue(object? value)
    {
        Value = value;
    }
}