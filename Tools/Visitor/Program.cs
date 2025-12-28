namespace Visitor;

public interface IVisitor<T> {
    T VisitLiteral(Literal expr);
    T VisitAddition(Addition expr);
    T VisitSubtraction(Subtraction expr);
}

public abstract class Expression
{
    public abstract T Accept<T>(IVisitor<T> visitor);
}

public class Literal : Expression
{
    public Literal(double value)
    {
        Value = value;
    }

    public double Value { get; }
    
    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitLiteral(this);
    }
}

public class Addition : Expression
{
    public Addition(Expression left, Expression right)
    {
        Left = left;
        Right = right;
    }
    
    public Expression Left { get; }
    public Expression Right { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitAddition(this);
    }
}

public class Subtraction : Expression
{
    public Subtraction(Expression left, Expression right)
    {
        Left = left;
        Right = right;
    }
    
    public Expression Left { get; }
    public Expression Right { get; }
    
    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitSubtraction(this);
    }
}

public class ExpressionPrintingVisitor : IVisitor<string>
{
    public string VisitLiteral(Literal expr)
    {
        return expr.Value.ToString();
    }
    
    public string VisitAddition(Addition expr)
    {
        return $"({expr.Left.Accept(this)} + {expr.Right.Accept(this)})";
    }
    
    public string VisitSubtraction(Subtraction expr)
    {
        return $"({expr.Left.Accept(this)} - {expr.Right.Accept(this)})";
    }
}

public class ExpressionEvaluatingVisitor : IVisitor<double>
{
    public double VisitLiteral(Literal expr)
    {
        return expr.Value;
    }
    
    public double VisitAddition(Addition expr)
    {
        return expr.Left.Accept(this) + expr.Right.Accept(this);
    }
    
    public double VisitSubtraction(Subtraction expr)
    {
        return expr.Left.Accept(this) - expr.Right.Accept(this);
    }
}

class Program
{
    static void Main(string[] args)
    {
        var printer = new ExpressionPrintingVisitor();
        var evaluator = new ExpressionEvaluatingVisitor();
        
        // Emulate (1 + 2) + 3
        Expression expr = new Addition(
            new Addition(
                new Literal(1.0), 
                new Literal(2.0)
                ), 
            new Literal(3.0)
            );
        
        Console.Write( expr.Accept(printer) );
        Console.WriteLine($" = {expr.Accept(evaluator)}");
        Console.WriteLine();

        // Emulate 1 - 2 = -1
        expr = new Subtraction(
            new Literal(1.0), 
            new Literal(2.0)
            ); 
        
        Console.Write( expr.Accept(printer) );
        Console.WriteLine($" = {expr.Accept(evaluator)}");
        Console.WriteLine();

        // Emulate (1 - 2) + 8 = 7
        expr = new Addition(
            new Subtraction(
                new Literal(1.0), 
                new Literal(2.0)
                ), 
            new Literal(8.0)
            );
        
        Console.Write( expr.Accept(printer) );
        Console.WriteLine($" = {expr.Accept(evaluator)}");
        Console.WriteLine();
    }
}