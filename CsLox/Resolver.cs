namespace CsLox;

public class Resolver : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    private enum FunctionType
    {
        None,
        Function,
    }
    
    private readonly Interpreter _interpreter;
    private readonly Stack<Dictionary<string, bool>> _scopes = new Stack<Dictionary<string, bool>>();
    private FunctionType _currentFunction = FunctionType.None;

    public Resolver(Interpreter interpreter)
    {
        this._interpreter = interpreter;
    }

    public void Resolve(List<Stmt> statements)
    {
        foreach (var statement in statements)
        {
            Resolve(statement);
        }
    }

    private void Resolve(Stmt stmt)
    {
        stmt.Accept(this);
    }

    private void Resolve(Expr expr)
    {
        expr.Accept(this);
    }

    private void BeginScope()
    {
        _scopes.Push(new Dictionary<string, bool>());
    }

    private void EndScope()
    {
        _scopes.Pop();    
    }

    private void Declare(Token name)
    {
        if (_scopes.Count == 0)
        {
            return;
        }

        if (_scopes.Peek().ContainsKey(name.Lexeme)) 
        {
            Lox.Error(name, "Variable with this name is already declared in this scope.");
        }
        
        var scope = _scopes.Peek();
        scope[name.Lexeme] = false;
    }
    
    private void Define(Token name)
    {
        if (_scopes.Count == 0)
        {
            return;
        }
        
        var scope = _scopes.Peek();
        scope[name.Lexeme] = true;
    }
    
    private void ResolveFunction(Function function, FunctionType functionType)
    {
        FunctionType enclosingFunction = _currentFunction;
        _currentFunction = functionType;
        BeginScope();
        foreach (var param in function.Parameters)
        {
            Declare(param);
            Define(param);
        }
        Resolve(function.Body);
        EndScope();
        _currentFunction = enclosingFunction;
    }
    
    private void ResolveLocal(Expr expr, Token name)
    {
        int depth = 0;
        foreach (var scope in _scopes)
        {
            if (scope.ContainsKey(name.Lexeme))
            {
                _interpreter.Resolve(expr, depth);
                return;
            }
            depth++;
        }
        // Not found, assume global
    }

    public object? VisitAssignExpr(Assign expr)
    {
        Resolve(expr.Value);
        ResolveLocal(expr, expr.Name);
        return null;
    }

    public object? VisitBinaryExpr(Binary expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }

    public object? VisitCallExpr(Call expr)
    {
        Resolve(expr.Callee);
        foreach (var argument in expr.Arguments)
        {
            Resolve(argument);
        }
        return null;
    }

    public object? VisitGetExpr(Get expr)
    {
        Resolve(expr.Object);
        return null;
    }

    public object? VisitGroupingExpr(Grouping expr)
    {
        Resolve(expr.Expression);
        return null;
    }

    public object? VisitLiteralExpr(Literal expr)
    {
        return null;
    }

    public object? VisitLogicalExpr(Logical expr)
    {
        Resolve(expr.Left);
        Resolve(expr.Right);
        return null;
    }

    public object? VisitSetExpr(Set expr)
    {
        Resolve(expr.Value);
        Resolve(expr.Object);
        return null;
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
        Resolve(expr.Right);
        return null;
    }

    public object? VisitVariableExpr(Variable expr)
    {
        if (_scopes.Count != 0 && _scopes.Peek().TryGetValue(expr.Name.Lexeme, out var defined) && !defined)
        {
            Lox.Error(expr.Name, "Cannot read local variable in its own initializer.");
        }
        
        ResolveLocal(expr, expr.Name);
        return null;
    }

    public object? VisitBlockStmt(Block stmt)
    {
        BeginScope();
        Resolve(stmt.Statements);
        EndScope();
        return null;
    }

    public object? VisitClassStmt(Class stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);
        
        // if (stmt.Superclass != null)
        // {
        //     Resolve(stmt.Superclass);
        // }
        //
        // foreach (var method in stmt.Methods)
        // {
        //     ResolveFunction(method, FunctionType.Function);
        // }
        
        return null;
    }

    public object? VisitExpressionStmt(Expression stmt)
    {
        Resolve(stmt.Expr);
        return null;
    }

    public object? VisitFunctionStmt(Function stmt)
    {
        Declare(stmt.Name);
        Define(stmt.Name);
        
        ResolveFunction(stmt, FunctionType.Function);
        return null;
    }

    public object? VisitIfStmt(If stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.ThenBranch);
        if (stmt.ElseBranch != null)
        {
            Resolve(stmt.ElseBranch);
        }

        return null;
    }

    public object? VisitPrintStmt(Print stmt)
    {
        Resolve(stmt.Expression);
        return null;
    }

    public object? VisitReturnStmt(Return stmt)
    {
        if (_currentFunction == FunctionType.None)
        {
            Lox.Error(stmt.Keyword, "Cannot return from top-level code.");
        }
        
        if (stmt.Value != null)
        {
            Resolve(stmt.Value);
        }
        
        return null;
    }

    public object? VisitVarStmt(Var stmt)
    {
        Declare(stmt.Name);
        if (stmt.Initializer != null)
        {
            Resolve(stmt.Initializer);
        }
        
        Define(stmt.Name);
        return null;
    }

    public object? VisitWhileStmt(While stmt)
    {
        Resolve(stmt.Condition);
        Resolve(stmt.Body);
        return null;
    }
}