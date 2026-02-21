namespace CsLox;

public class LoxFunction : ILoxCallable
{
    private readonly bool _isInitializer;
    
    public Function Declaration { get; init; }
    public Environment Closure { get; init; }
    
    public LoxFunction(Function declaration, Environment closure, bool isInitializer)
    {
        _isInitializer = isInitializer;
        Declaration = declaration;
        Closure = closure;
    }
    
    public int Arity()
    {
        return Declaration.Parameters.Count;
    }

    public object? Call(Interpreter interpreter, List<object> arguments)
    {
        var environment = new Environment(Closure);
        for (var i = 0; i < Declaration.Parameters.Count; i++)
        {
            environment.Define(Declaration.Parameters[i].Lexeme, arguments[i]);
        }
        
        try
        {
            interpreter.ExecuteBlock(Declaration.Body, environment);
        }
        catch (ReturnValue returnValue)
        {
            if (_isInitializer)
            {
                return Closure.GetAt(0, "this");
            }
            
            return returnValue.Value;
        }

        if (_isInitializer)
        {
            return Closure.GetAt(0, "this");
        }
        
        return null;
    }
    
    public override string ToString()
    {
        return $"<fn {Declaration.Name.Lexeme}>";
    }

    public LoxFunction Bind(LoxInstance instance)
    {
        var environment = new Environment(Closure);
        environment.Define("this", instance);
        return new LoxFunction(Declaration, environment, _isInitializer);
    }
}