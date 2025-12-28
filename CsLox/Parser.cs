namespace CsLox;

public class Parser
{
    private List<Token> _tokens;
    private int _current;
    
    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
        _current = 0;
    }

    public Expr Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseError e)
        {
            return null;
        }
    }
    
    private Expr Expression()
    {
        return Equality();
    }

    private Expr Equality()
    {
        Expr expr = Comparison();

        while (Match(TokenType.BangEqual, TokenType.EqualEqual))
        {
            Token operatorToken = Previous();
            Expr right = Comparison();
            expr = new Binary(expr, operatorToken, right);
        }

        return expr;
    }

    private Expr Comparison()
    {
        Expr expr = Term();
        while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
        {
            Token operatorToken = Previous();
            Expr right = Term();
            expr = new Binary(expr, operatorToken, right);
        }
        return expr;
    }

    private Expr Term()
    {
        Expr expr = Factor();
        while (Match(TokenType.Minus, TokenType.Plus))
        {
            Token operatorToken = Previous();
            Expr right = Factor();
            expr = new Binary(expr, operatorToken, right);
        }
        return expr;
    }

    private Expr Factor()
    {
        Expr expr = Unary();

        while (Match(TokenType.Star, TokenType.Slash))
        {
            Token operatorToken = Previous();
            Expr right = Unary();
            expr = new Binary(expr, operatorToken, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(TokenType.Bang, TokenType.Minus))
        {
            Token operatorToken = Previous();
            Expr right = Unary();
            return new Unary(operatorToken, right);
        }
        
        return Primary();
    }
    
    private Expr Primary()
    {
        if (Match(TokenType.False)) return new Literal(false);
        if (Match(TokenType.True)) return new Literal(true);
        if (Match(TokenType.Nil)) return new Literal(null);
        if (Match(TokenType.Number, TokenType.String)) return new Literal(Previous().Literal);
        
        if (Match(TokenType.LeftParen))
        {
            Expr expr = Expression();
            Consume(TokenType.RightParen, "Expect ')' after expression.");
            return new Grouping(expr);
        }
        
        throw Error(Peek(), "Expect expression.");
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }
        
        throw Error(Peek(), message);
    }
    
    private bool Match(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }
        
        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Peek()
    {
        return _tokens[_current];
    }
    
    private Token Advance()
    {
        return _tokens[_current++];
    }
    
    private Token Previous()
    {
        return _tokens[_current - 1];
    }

    private void Synchronize()
    {
        Advance();
        while (!IsAtEnd() && Previous().Type != TokenType.Semicolon) 
            Advance();

        switch (Peek().Type)
        {
            case TokenType.Class:
            case TokenType.For:
            case TokenType.Fun:
            case TokenType.If:
            case TokenType.Print:
            case TokenType.Return:
            case TokenType.Var:
            case TokenType.While:
                return;
        }

        Advance();
    }
    
    private bool IsAtEnd()
    {
        return _current >= _tokens.Count;
    }
    
    private ParseError Error(Token token, string message)
    {
        Lox.Error(token.Line, message);
        return new ParseError(token, message);
    }
}