namespace CsLox;

public class LoxClass : ILoxCallable
{
    public string Name { get; init; }
    // public LoxClass? Superclass { get; init; }

    public LoxClass(string name/*, LoxClass? superclass*/)
    {
        Name = name;
        // Superclass = superclass;
    }
    
    public override string ToString()
    {
        return Name;
    }

    public int Arity()
    {
        return 0;
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        var instance = new LoxInstance(this);
        // var initializer = FindMethod("init");
        // if (initializer != null)
        // {
        //     initializer.Bind(instance).Call(interpreter, arguments);
        // }
        return instance;
    }
}