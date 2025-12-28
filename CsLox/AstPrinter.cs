namespace CsLox;

using System.Text;

public class AstPrinter : Expr.IVisitor<string>
{
    public string VisitAssign(Assign expr)
    {
        throw new NotImplementedException();
    }

    public string VisitBinary(Binary expr)
    {
        return Parenthize(expr.Operator.Lexeme, expr.Left, expr.Right);
    }

    public string VisitCall(Call expr)
    {
        throw new NotImplementedException();
    }

    public string VisitGet(Get expr)
    {
        throw new NotImplementedException();
    }

    public string VisitGrouping(Grouping expr)
    {
        return Parenthize("group", expr.Expression);
    }

    public string VisitLiteral(Literal expr)
    {
        if (expr.Value == null) return "nil";
        return expr.Value.ToString()!;
    }

    public string VisitLogical(Logical expr)
    {
        throw new NotImplementedException();
    }

    public string VisitSet(Set expr)
    {
        throw new NotImplementedException();
    }

    public string VisitSuper(Super expr)
    {
        throw new NotImplementedException();
    }

    public string VisitThis(This expr)
    {
        throw new NotImplementedException();
    }

    public string VisitUnary(Unary expr)
    {
        return Parenthize(expr.Operator.Lexeme, expr.Right);
    }

    public string VisitVariable(Variable expr)
    {
        throw new NotImplementedException();
    }
    
    private string Parenthize(string name, params Expr[] exprs)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("(").Append(name);
        foreach (var expr in exprs)
        {
            builder.Append(" ");
            builder.Append(expr.Accept(this));
        }
        builder.Append(")");

        return builder.ToString();
    }   
}