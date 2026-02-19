//[ Appendix II Stmt
using System.Collections.Generic;

namespace CsLox;

public abstract class Stmt
{
     public interface IVisitor<T>
     {
         T VisitBlockStmt(Block stmt);
         T VisitExpressionStmt(Expression stmt);
         T VisitIfStmt(If stmt);
         T VisitPrintStmt(Print stmt);
         T VisitVarStmt(Var stmt);
         T VisitWhileStmt(While stmt);
     }

    public abstract T Accept<T>(IVisitor<T> visitor);
}

public class Block : Stmt
{
    public Block(List<Stmt> statements)
    {
        Statements = statements;
    }

    public List<Stmt> Statements { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitBlockStmt(this);
    }
}

public class Expression : Stmt
{
    public Expression(Expr expr)
    {
        Expr = expr;
    }

    public Expr Expr { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitExpressionStmt(this);
    }
}

public class If : Stmt
{
    public If(Expr condition, Stmt thenBranch, Stmt? elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public Expr Condition { get; init; }
    public Stmt ThenBranch { get; init; }
    public Stmt? ElseBranch { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitIfStmt(this);
    }
}

public class Print : Stmt
{
    public Print(Expr expression)
    {
        Expression = expression;
    }

    public Expr Expression { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitPrintStmt(this);
    }
}

public class Var : Stmt
{
    public Var(Token name, Expr? initializer)
    {
        Name = name;
        Initializer = initializer;
    }

    public Token Name { get; init; }
    public Expr? Initializer { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitVarStmt(this);
    }
}

public class While : Stmt
{
    public While(Expr condition, Stmt body)
    {
        Condition = condition;
        Body = body;
    }

    public Expr Condition { get; init; }
    public Stmt Body { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitWhileStmt(this);
    }
}

//] Appendix II Stmt
