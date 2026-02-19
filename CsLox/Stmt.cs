//[ Appendix II Stmt
using System.Collections.Generic;

namespace CsLox;

public abstract class Stmt
{
     public interface IVisitor<T>
     {
         T VisitBlock(Block expr);
         T VisitExpression(Expression expr);
         T VisitPrint(Print expr);
         T VisitVar(Var expr);
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
        return visitor.VisitBlock(this);
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
        return visitor.VisitExpression(this);
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
        return visitor.VisitPrint(this);
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
        return visitor.VisitVar(this);
    }
}

//] Appendix II Stmt
