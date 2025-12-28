namespace CsLox;

public class Interpreter : Expr.IVisitor<object?>
{
    public void Interpret(Expr expr)
    {
        try
        {
            Console.WriteLine(Stringify(expr.Accept(this)));
        }
        catch (RuntimeException e)
        {
            Lox.RuntimeError(e);
        }
    }


    public object? VisitAssign(Assign expr)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }
    
    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
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
}