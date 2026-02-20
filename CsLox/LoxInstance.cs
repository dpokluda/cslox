namespace CsLox;

public class LoxInstance
{
    public LoxClass Class { get; init; }
    
    private readonly Dictionary<string, object?> _fields = new();
    
    
    public LoxInstance(LoxClass @class)
    {
        Class = @class;
    }
    
    public object? Get(Token name)
    {
        if (_fields.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }
        
        // var method = Class.FindMethod(name.Lexeme);
        // if (method != null)
        // {
        //     return method.Bind(this);
        // }
        
        throw new RuntimeException(name, $"Undefined property '{name.Lexeme}'.");
    }

    public override string ToString()
    {
        return $"{Class.Name} instance";
    }

    public void Set(Token name, object? value)
    {
        _fields[name.Lexeme] = value;
    }
}