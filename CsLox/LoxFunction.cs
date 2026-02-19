namespace CsLox;

public class LoxFunction : ILoxCallable
{
    public Function Declaration { get; init; }
    public Environment Closure { get; init; }
    
    public LoxFunction(Function declaration, Environment closure)
    {
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
            return returnValue.Value;
        }
        
        return null;
    }
    
    public override string ToString()
    {
        return $"<fn {Declaration.Name.Lexeme}>";
    }
}