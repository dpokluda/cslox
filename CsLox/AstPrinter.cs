namespace CsLox;

using System.Text;

public class AstPrinter : Expr.IVisitor<string>
{
    public string VisitAssignExpr(Assign expr)
    {
        throw new NotImplementedException();
    }

    public string VisitBinaryExpr(Binary expr)
    {
        return Parenthize(expr.Operator.Lexeme, expr.Left, expr.Right);
    }

    public string VisitCallExpr(Call expr)
    {
        throw new NotImplementedException();
    }

    public string VisitGetExpr(Get expr)
    {
        throw new NotImplementedException();
    }

    public string VisitGroupingExpr(Grouping expr)
    {
        return Parenthize("group", expr.Expression);
    }

    public string VisitLiteralExpr(Literal expr)
    {
        if (expr.Value == null) return "nil";
        return expr.Value.ToString()!;
    }

    public string VisitLogicalExpr(Logical expr)
    {
        throw new NotImplementedException();
    }

    public string VisitSetExpr(Set expr)
    {
        throw new NotImplementedException();
    }

    public string VisitSuperExpr(Super expr)
    {
        throw new NotImplementedException();
    }

    public string VisitThisExpr(This expr)
    {
        throw new NotImplementedException();
    }

    public string VisitUnaryExpr(Unary expr)
    {
        return Parenthize(expr.Operator.Lexeme, expr.Right);
    }

    public string VisitVariableExpr(Variable expr)
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