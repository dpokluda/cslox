namespace CsLox;

public class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    private Environment _environment = new();
    
    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (var statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeException error)
        {
            Lox.RuntimeError(error);
        }
    }

    public object? VisitAssign(Assign expr)
    {
        var value = Evaluate(expr.Value);
        _environment.Assign(expr.Name.Lexeme, value);
        return value;
    }

    public object? VisitBinary(Binary expr)
    {
        var left = Evaluate(expr.Left);
        var right = Evaluate(expr.Right);

        double l;
        double r;
        switch (expr.Operator.Type)
        {
            case TokenType.Plus:
                if (left is double lNum && right is double rNum)
                {
                    return lNum + rNum;
                }
                if (left is string lStr && right is string rStr)
                {
                    return lStr + rStr;
                }
                throw new RuntimeException(expr.Operator, "Operands must be two numbers or two strings.");
            case TokenType.Minus:
                l = CheckNumberOperand(expr.Operator, left);
                r = CheckNumberOperand(expr.Operator, right);
                return l - r;
            case TokenType.Star:
                l = CheckNumberOperand(expr.Operator, left);
                r = CheckNumberOperand(expr.Operator, right);
                return l * r;
            case TokenType.Slash:
                l = CheckNumberOperand(expr.Operator, left);
                r = CheckNumberOperand(expr.Operator, right);
                return l / r;
            case TokenType.Greater:
                l = CheckNumberOperand(expr.Operator, left);
                r = CheckNumberOperand(expr.Operator, right);
                return l > r;
            case TokenType.GreaterEqual:
                l = CheckNumberOperand(expr.Operator, left);
                r = CheckNumberOperand(expr.Operator, right);
                return l >= r;
            case TokenType.Less:
                l = CheckNumberOperand(expr.Operator, left);
                r = CheckNumberOperand(expr.Operator, right);
                return l < r;
            case TokenType.LessEqual:
                l = CheckNumberOperand(expr.Operator, left);
                r = CheckNumberOperand(expr.Operator, right);
                return l <= r;
            case TokenType.EqualEqual:
                return Equals(left, right);
            case TokenType.BangEqual:
                return !Equals(left, right);
        }

        // unreachable
        return null;
    }

    public object? VisitCall(Call expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitGet(Get expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitGrouping(Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object? VisitLiteral(Literal expr)
    {
        return expr.Value;
    }

    public object? VisitLogical(Logical expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitSet(Set expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitSuper(Super expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitThis(This expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitUnary(Unary expr)
    {
        var right = expr.Right.Accept(this);

        switch (expr.Operator.Type)
        {
            case TokenType.Minus: 
                return -(double)right;
            case TokenType.Bang: 
                return !IsTruthy(right);
        }
        
        // unreachable
        return null;
    }
    
    public object? VisitVariable(Variable expr)
    {
        return _environment.Get(expr.Name);
    }
    
    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }
    
    private void Execute(Stmt statement)
    {
        statement.Accept(this);
    }
    
    private void ExecuteBlock(List<Stmt> exprStatements, Environment environment)
    {
        var previous = _environment;
        try
        {
            _environment = environment;

            foreach (var statement in exprStatements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = previous;
        }
    }
    
    private bool IsTruthy(object? @object)
    {
        if (@object is null)
        {
            return false;
        }

        if (@object is bool b)
        {
            return b;
        }
        
        return true;
    }
    
    private double CheckNumberOperand(Token @operator, object? @object)
    {
        if (@object is double)
        {
            return (double)@object;
        }
        
        throw new RuntimeException(@operator, "Operand must be a number.");
    }
    
    private string? Stringify(object? @object)
    {
        if (@object == null)
        {
            return "nil";
        }

        if (@object is double d)
        {
            var text = d.ToString()!;
            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }

        return @object.ToString();
    }

    public object? VisitBlock(Block expr)
    {
        ExecuteBlock(expr.Statements, new Environment(_environment));
        return null;
    }

    public object? VisitExpression(Expression expr)
    {
        Evaluate(expr.Expr);
        return null;
    }

    public object? VisitPrint(Print expr)
    {
        var value = Evaluate(expr.Expression);
        Console.WriteLine(Stringify(value));
        return null;
    }

    public object? VisitVar(Var expr)
    {
        object? value = null;
        if (expr.Initializer != null)
        {
            value = Evaluate(expr.Initializer);
        }

        _environment.Define(expr.Name.Lexeme, value);
        return null;
    }
}