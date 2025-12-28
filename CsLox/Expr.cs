//[ Appendix II Expr
using System.Collections.Generic;

namespace CsLox;

public abstract class Expr
{
    public abstract T Accept<T>(IVisitor<T> visitor);
}

public interface IVisitor<T>
{
    T VisitAssign(Assign expr);
    T VisitBinary(Binary expr);
    T VisitCall(Call expr);
    T VisitGet(Get expr);
    T VisitGrouping(Grouping expr);
    T VisitLiteral(Literal expr);
    T VisitLogical(Logical expr);
    T VisitSet(Set expr);
    T VisitSuper(Super expr);
    T VisitThis(This expr);
    T VisitUnary(Unary expr);
    T VisitVariable(Variable expr);
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
        return visitor.VisitAssign(this);
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
        return visitor.VisitBinary(this);
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
        return visitor.VisitCall(this);
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
        return visitor.VisitGet(this);
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
        return visitor.VisitGrouping(this);
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
        return visitor.VisitLiteral(this);
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
        return visitor.VisitLogical(this);
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
        return visitor.VisitSet(this);
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
        return visitor.VisitSuper(this);
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
        return visitor.VisitThis(this);
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
        return visitor.VisitUnary(this);
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
        return visitor.VisitVariable(this);
    }
}

//] Appendix II Expr
