namespace CsLox;

public class Environment
{
    public Environment? Enclosing { get; init; }
    
    private readonly Dictionary<string, object?> _values = new();
    
    public Environment()
    {
        Enclosing = null;
    }
    
    public Environment(Environment enclosing)
    {
        Enclosing = enclosing;
    }
    
    public void Define(string name, object? value)
    {
        _values[name] = value;
    }
    
    public object? GetAt(int distance, string name)
    {
        return Ancestor(distance)._values[name];
    }
    
    public void AssignAt(int distance, Token name, object? value)
    {
        Ancestor(distance)._values[name.Lexeme] = value;
    }

    private Environment Ancestor(int distance)
    {
        Environment environment = this;
        for (int i = 0; i < distance; i++)
        {
            if (environment.Enclosing == null)
            {
                throw new InvalidOperationException("No enclosing environment.");
            }
            environment = environment.Enclosing;
        }
        
        return environment;
    }

    public object? Get(Token name)
    {
        if (_values.TryGetValue(name.Lexeme, out var value))
        {
            return value;
        }
        
        if (Enclosing != null)
        {
            return Enclosing.Get(name);
        }
        
        throw new RuntimeException(name, $"Undefined variable '{name.Lexeme}'.");
    }

    public void Assign(Token name, object? value)
    {
        if (_values.ContainsKey(name.Lexeme))
        {
            _values[name.Lexeme] = value;
            return;
        }
        
        if (Enclosing != null)
        {
            Enclosing.Assign(name, value);
            return;
        }
        
        throw new RuntimeException(null, $"Undefined variable '{name.Lexeme}'.");
    }
}