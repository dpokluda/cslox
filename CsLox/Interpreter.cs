namespace CsLox;

public class Interpreter : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    private Environment _globals;
    private Environment _environment;
    
    public Interpreter()
    {
        this._globals = new Environment();
        _environment = this._globals;
        
        _globals.Define("clock", new Clock());
    }
    
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

    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }
    
    private void Execute(Stmt statement)
    {
        statement.Accept(this);
    }
    
    internal void ExecuteBlock(List<Stmt> exprStatements, Environment environment)
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
    
    public object? VisitAssignExpr(Assign expr)
    {
        var value = Evaluate(expr.Value);
        _environment.Assign(expr.Name.Lexeme, value);
        return value;
    }

    public object? VisitBinaryExpr(Binary expr)
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

    public object? VisitCallExpr(Call expr)
    {
        var callee = Evaluate(expr.Callee);
        var arguments = new List<object?>();
        foreach (var argument in expr.Arguments)
        {
            arguments.Add(Evaluate(argument));
        }
        if (!(callee is ILoxCallable))
        {
            throw new RuntimeException(expr.Paren, "Can only call functions and classes.");
        }
        
        ILoxCallable function = (ILoxCallable)callee;
        if (arguments.Count != function.Arity())
        {
            throw new RuntimeException(expr.Paren, $"Expected {function.Arity()} arguments but got {arguments.Count}.");
        }

        return function.Call(this, arguments);
    }

    public object? VisitGetExpr(Get expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitGroupingExpr(Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object? VisitLiteralExpr(Literal expr)
    {
        return expr.Value;
    }

    public object? VisitLogicalExpr(Logical expr)
    {
        var left = Evaluate(expr.Left);

        if (expr.Operator.Type == TokenType.Or)
        {
            if (IsTruthy(left)) return left;
        }
        else
        {
            if (!IsTruthy(left)) return left;
        }

        return Evaluate(expr.Right);
    }

    public object? VisitSetExpr(Set expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitSuperExpr(Super expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitThisExpr(This expr)
    {
        throw new NotImplementedException();
    }

    public object? VisitUnaryExpr(Unary expr)
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
    
    public object? VisitVariableExpr(Variable expr)
    {
        return _environment.Get(expr.Name);
    }
    
    public object? VisitBlockStmt(Block stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(_environment));
        return null;
    }

    public object? VisitExpressionStmt(Expression stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object? VisitFunctionStmt(Function stmt)
    {
        var function = new LoxFunction(stmt, _environment);
        _environment.Define(stmt.Name.Lexeme, function);
        return null;
    }

    public object? VisitIfStmt(If stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.ThenBranch);
        }
        else if (stmt.ElseBranch != null)
        {
            Execute(stmt.ElseBranch);
        }

        return null;
    }

    public object? VisitPrintStmt(Print stmt)
    {
        var value = Evaluate(stmt.Expression);
        Console.WriteLine(Stringify(value));
        return null;
    }

    public object? VisitReturnStmt(Return stmt)
    {
        object? value = null;
        if (stmt.Value != null)
        {
            value = Evaluate(stmt.Value);
        }

        throw new ReturnValue(value);
    }

    public object? VisitVarStmt(Var stmt)
    {
        object? value = null;
        if (stmt.Initializer != null)
        {
            value = Evaluate(stmt.Initializer);
        }

        _environment.Define(stmt.Name.Lexeme, value);
        return null;
    }

    public object? VisitWhileStmt(While stmt)
    {
        while (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.Body);
        }

        return null;
    }
}