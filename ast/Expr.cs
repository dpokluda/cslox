//[ Appendix II Expr
using System.Collections.Generic;

namespace CsLox;

public abstract class Expr
{
     public interface IVisitor<T>
     {
         T VisitAssignExpr(Assign expr);
         T VisitBinaryExpr(Binary expr);
         T VisitCallExpr(Call expr);
         T VisitGetExpr(Get expr);
         T VisitGroupingExpr(Grouping expr);
         T VisitLiteralExpr(Literal expr);
         T VisitLogicalExpr(Logical expr);
         T VisitSetExpr(Set expr);
         T VisitSuperExpr(Super expr);
         T VisitThisExpr(This expr);
         T VisitUnaryExpr(Unary expr);
         T VisitVariableExpr(Variable expr);
     }

    public abstract T Accept<T>(IVisitor<T> visitor);
}

public class Assign : Expr
{
    public Assign(Token name, Expr value)
    {
        Name = name;
        Value = value;
    }

    public Token Name { get; init; }
    public Expr Value { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitAssignExpr(this);
    }
}

public class Binary : Expr
{
    public Binary(Expr left, Token operator_, Expr right)
    {
        Left = left;
        Operator = operator_;
        Right = right;
    }

    public Expr Left { get; init; }
    public Token Operator { get; init; }
    public Expr Right { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitBinaryExpr(this);
    }
}

public class Call : Expr
{
    public Call(Expr callee, Token paren, List<Expr> arguments)
    {
        Callee = callee;
        Paren = paren;
        Arguments = arguments;
    }

    public Expr Callee { get; init; }
    public Token Paren { get; init; }
    public List<Expr> Arguments { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitCallExpr(this);
    }
}

public class Get : Expr
{
    public Get(Expr object_, Token name)
    {
        Object = object_;
        Name = name;
    }

    public Expr Object { get; init; }
    public Token Name { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitGetExpr(this);
    }
}

public class Grouping : Expr
{
    public Grouping(Expr expression)
    {
        Expression = expression;
    }

    public Expr Expression { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitGroupingExpr(this);
    }
}

public class Literal : Expr
{
    public Literal(Object? value)
    {
        Value = value;
    }

    public Object? Value { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitLiteralExpr(this);
    }
}

public class Logical : Expr
{
    public Logical(Expr left, Token operator_, Expr right)
    {
        Left = left;
        Operator = operator_;
        Right = right;
    }

    public Expr Left { get; init; }
    public Token Operator { get; init; }
    public Expr Right { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitLogicalExpr(this);
    }
}

public class Set : Expr
{
    public Set(Expr object_, Token name, Expr value)
    {
        Object = object_;
        Name = name;
        Value = value;
    }

    public Expr Object { get; init; }
    public Token Name { get; init; }
    public Expr Value { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitSetExpr(this);
    }
}

public class Super : Expr
{
    public Super(Token keyword, Token method)
    {
        Keyword = keyword;
        Method = method;
    }

    public Token Keyword { get; init; }
    public Token Method { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitSuperExpr(this);
    }
}

public class This : Expr
{
    public This(Token keyword)
    {
        Keyword = keyword;
    }

    public Token Keyword { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitThisExpr(this);
    }
}

public class Unary : Expr
{
    public Unary(Token operator_, Expr right)
    {
        Operator = operator_;
        Right = right;
    }

    public Token Operator { get; init; }
    public Expr Right { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitUnaryExpr(this);
    }
}

public class Variable : Expr
{
    public Variable(Token name)
    {
        Name = name;
    }

    public Token Name { get; init; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitVariableExpr(this);
    }
}

//] Appendix II Expr
