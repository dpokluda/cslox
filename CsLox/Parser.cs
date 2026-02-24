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

    public List<Stmt> Parse()
    {
        var statements = new List<Stmt>();
        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }
        
        return statements;
    }

    private Stmt ExpressionStatement()
    {
        Expr expr = Expression();
        Consume(TokenType.Semicolon, "Expect ';' after expression.");
        return new Expression(expr);
    }
    
    private Stmt Function(string kind)
    {
        Token name = Consume(TokenType.Identifier, $"Expect {kind} name.");
        Consume(TokenType.LeftParen, $"Expect '(' after {kind} name.");
        var parameters = new List<Token>();
        if (!Check(TokenType.RightParen))
        {
            do
            {
                if (parameters.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 parameters.");
                }
                parameters.Add(Consume(TokenType.Identifier, "Expect parameter name."));
            } while (Match(TokenType.Comma));
        }
        Consume(TokenType.RightParen, $"Expect ')' after parameters.");

        Consume(TokenType.LeftBrace, $"Expect '{{' before {kind} body.");
        List<Stmt> body = Block();
        return new Function(name, parameters, body);
    }
    
    private List<Stmt> Block()
    {
        var statements = new List<Stmt>();

        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            statements.Add(Declaration() ?? throw new InvalidOperationException());
        }

        Consume(TokenType.RightBrace, "Expect '}' after block.");
        return statements;
    }

    private Expr Expression()
    {
        return Assignment();
    }

    private Expr Assignment()
    {
        Expr expr = Or();

        if (Match(TokenType.Equal))
        {
            Token equals = Previous();
            Expr value = Assignment();

            if (expr is Variable variable)
            {
                Token name = variable.Name;
                return new Assign(name, value);
            }
            else if (expr is Get get)
            {
                return new Set(get.Object, get.Name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private Expr Or()
    {
        Expr expr = And();

        while (Match(TokenType.Or))
        {
            Token operatorToken = Previous();
            Expr right = And();
            expr = new Logical(expr, operatorToken, right);
        }

        return expr;
    }

    private Expr And()
    {
        Expr expr = Equality();

        while (Match(TokenType.And))
        {
            Token operatorToken = Previous();
            Expr right = Equality();
            expr = new Logical(expr, operatorToken, right);
        }

        return expr;
    }

    private Stmt Declaration()
    {
        try
        {
            if (Match(TokenType.Class))
            {
                return ClassDeclaration();
            }
            
            if (Match(TokenType.Fun))
            {
                return Function("function");
            }
            
            if (Match(TokenType.Var))
            {
                return VarDeclaration();
            }
            
            return Statement();
        }
        catch (ParseException)
        {
            Synchronize();
            return null;
        }
    }

    private Stmt ClassDeclaration()
    {
        Token name = Consume(TokenType.Identifier, "Expect class name.");

        Variable? superclass = null;
        if (Match(TokenType.Less))
        {
            Consume(TokenType.Identifier, "Expect superclass name.");
            superclass = new Variable(Previous());
        }

        Consume(TokenType.LeftBrace, "Expect '{' before class body.");

        var methods = new List<Function>();
        while (!Check(TokenType.RightBrace) && !IsAtEnd())
        {
            methods.Add((Function)Function("method"));
        }

        Consume(TokenType.RightBrace, "Expect '}' after class body.");

        return new Class(name, superclass, methods);
    }

    private Stmt Statement()
    {
        if (Match(TokenType.For))
        {
            return ForStatement();
        }
        
        if (Match(TokenType.If))
        {
            return IfStatement();
        }
        
        if (Match(TokenType.Print))
        {
            return PrintStatement();
        }
        
        if (Match(TokenType.Return))
        {
            return ReturnStatement();
        }
        
        if (Match(TokenType.While))
        {
            return WhileStatement();
        }
        
        if (Match(TokenType.LeftBrace))
        {
            return new Block(Block());
        }
        
        return ExpressionStatement();
    }

    private Stmt ForStatement()
    {
        Consume(TokenType.LeftParen, "Expect '(' after 'for'.");

        Stmt? initializer;
        if (Match(TokenType.Semicolon))
        {
            initializer = null;
        }
        else if (Match(TokenType.Var))
        {
            initializer = VarDeclaration();
        }
        else
        {
            initializer = ExpressionStatement();
        }

        Expr? condition = null;
        if (!Check(TokenType.Semicolon))
        {
            condition = Expression();
        }
        Consume(TokenType.Semicolon, "Expect ';' after loop condition.");

        Expr? increment = null;
        if (!Check(TokenType.RightParen))
        {
            increment = Expression();
        }
        Consume(TokenType.RightParen, "Expect ')' after for clauses.");

        Stmt body = Statement();

        if (increment != null)
        {
            body = new Block(new List<Stmt>
            {
                body,
                new Expression(increment)
            });
        }

        if (condition == null)
        {
            condition = new Literal(true);
        }
        body = new While(condition, body);

        if (initializer != null)
        {
            body = new Block(new List<Stmt>
            {
                initializer,
                body
            });
        }

        return body;
    }

    private Stmt IfStatement()
    {
        Consume(TokenType.LeftParen, "Expect '(' after 'if'.");
        Expr condition = Expression();
        Consume(TokenType.RightParen, "Expect ')' after if condition.");

        Stmt thenBranch = Statement();
        Stmt? elseBranch = null;
        if (Match(TokenType.Else))
        {
            elseBranch = Statement();
        }

        return new If(condition, thenBranch, elseBranch);
    }

    private Stmt PrintStatement()
    {
        Expr value = Expression();
        Consume(TokenType.Semicolon, "Expect ';' after value.");
        return new Print(value);
    }
    
    private Stmt ReturnStatement()
    {
        Token keyword = Previous();
        Expr? value = null;
        if (!Check(TokenType.Semicolon))
        {
            value = Expression();
        }

        Consume(TokenType.Semicolon, "Expect ';' after return value.");
        return new Return(keyword, value);
    }
    
    private Stmt VarDeclaration()
    {
        Token name = Consume(TokenType.Identifier, "Expect variable name.");

        Expr? initializer = null;
        if (Match(TokenType.Equal))
        {
            initializer = Expression();
        }

        Consume(TokenType.Semicolon, "Expect ';' after variable declaration.");
        return new Var(name, initializer);
    }

    private Stmt WhileStatement()
    {
        Consume(TokenType.LeftParen, "Expect '(' after 'while'.");
        Expr condition = Expression();
        Consume(TokenType.RightParen, "Expect ')' after condition.");
        Stmt body = Statement();

        return new While(condition, body);
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
        
        return Call();
    }
    
    private Expr Call()
    {
        Expr expr = Primary();

        while (true)
        {
            if (Match(TokenType.LeftParen))
            {
                expr = FinishCall(expr);
            }
            else if (Match(TokenType.Dot))
            {
                Token name = Consume(TokenType.Identifier, "Expect property name after '.'.");
                expr = new Get(expr, name);
            }
            else
            {
                break;
            }
        }

        return expr;
    }

    private Expr FinishCall(Expr callee)
    {
        var arguments = new List<Expr>();
        if (!Check(TokenType.RightParen))
        {
            do
            {
                if (arguments.Count >= 255)
                {
                    Error(Peek(), "Can't have more than 255 arguments.");
                }
                arguments.Add(Expression());
            } while (Match(TokenType.Comma));
        }

        Token paren = Consume(TokenType.RightParen, "Expect ')' after arguments.");

        return new Call(callee, paren, arguments);
    }

    private Expr Primary()
    {
        if (Match(TokenType.False)) return new Literal(false);
        if (Match(TokenType.True)) return new Literal(true);
        if (Match(TokenType.Nil)) return new Literal(null);
        if (Match(TokenType.Number, TokenType.String))
        {
            return new Literal(Previous().Literal);
        }
        
        if (Match(TokenType.Super))
        {
            Token keyword = Previous();
            Consume(TokenType.Dot, "Expect '.' after 'super'.");
            Token method = Consume(TokenType.Identifier, "Expect superclass method name.");
            return new Super(keyword, method);
        }
        
        if (Match(TokenType.This))
        {
            return new This(Previous());
        }
        
        if (Match(TokenType.Identifier))
        {
            return new Variable(Previous());
        }
        
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
    
    private ParseException Error(Token token, string message)
    {
        Lox.Error(token.Line, message);
        return new ParseException(token, message);
    }
}