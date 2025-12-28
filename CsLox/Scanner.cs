namespace CsLox;

public class Scanner
{
  private static Dictionary<string, TokenType> s_keywords = new()
  { 
    {"and", TokenType.And},
    {"class", TokenType.Class},
    {"else", TokenType.Else},
    {"false", TokenType.False},
    {"for", TokenType.For},
    {"fun", TokenType.Fun},
    {"if", TokenType.If},
    {"nil", TokenType.Nil},
    {"or", TokenType.Or},
    {"print", TokenType.Print},
    {"return", TokenType.Return},
    {"super", TokenType.Super},
    {"this", TokenType.This},
    {"true", TokenType.True},
    {"var", TokenType.Var},
    {"while", TokenType.While}
  };
  
  private string _source;
  private List<Token> _tokens;
  private int _start;
  private int _current;
  private int _line;

  public Scanner(string source)
  {
    _source = source;
    _tokens = new List<Token>();
    _start = 0;
    _current = 0;
    _line = 1;
  }

  public List<Token> ScanTokens()
  {
    while (!IsAtEnd())
    {
      _start = _current;
      ScanToken();
    }
    
    return _tokens;
  }
  
  private bool IsAtEnd()
  {
    return _current >= _source.Length;
  }
  
  private void ScanToken()
  {
    char c = Advance();
    switch (c)
    {
      case '(':
        AddToken(TokenType.LeftParen);
        break;
      case ')':
        AddToken(TokenType.RightParen);
        break;
      case '{':
        AddToken(TokenType.LeftBrace);
        break;
      case '}':
        AddToken(TokenType.RightBrace);
        break;
      case ',':
        AddToken(TokenType.Comma);
        break;
      case '.':
        AddToken(TokenType.Dot);
        break;
      case '-':
        AddToken(TokenType.Minus);
        break;
      case '+':
        AddToken(TokenType.Plus);
        break;
      case ';':
        AddToken(TokenType.Semicolon);
        break;
      case '*':
        AddToken(TokenType.Star);
        break;
      case '!': 
        AddToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
        break;
      case '=': 
        AddToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
        break;
      case '<': 
        AddToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
        break;
      case '>': 
        AddToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
        break;
      case '/':
        if (Match('/'))
        {
          // A comment goes until the end of the line.
          while (Peek() != '\n' && !IsAtEnd())
          {
            _current++;
          }
        }
        else
        {
          AddToken(TokenType.Slash);
        }
        break;
      case ' ':
      case '\r':
      case '\t':
        // Ignore whitespace.
        break;
      case '"':
        String();
        break;
      default: 
        if (IsDigit(c))
        {
          Number();
        }
        else if (IsAlpha(c))
        {
          Identifier();
        }
        else
        {
          Lox.Error(_line, $"Unexpected character: '{c}'.");
        }
        break;
    }
  }
  
  private char Advance()
  {
    return _source[_current++]; 
  }

  private char Peek()
  {
    if (IsAtEnd())
    {
      return '\0';
    }
    
    return _source[_current];
  }
  
  private bool Match(char expected)
  {
    if (IsAtEnd())
    {
      return false;
    }

    if (Peek() != expected)
    {
      return false;
    }
    
    _current++;
    return true;
  }

  private bool IsDigit(char c)
  {
    return '0' <= c && c <= '9';
  }
  
  private bool IsAlpha(char c)
  {
    return ('a' <= c && c <= 'z') || ('A' <= c && c <= 'Z') || c == '_';
  }
  
  private bool IsAlphaNumeric(char c)
  {
    return IsAlpha(c) || IsDigit(c);
  }
  
  private char PeekNext()
  {
    if (_current + 1 >= _source.Length)
    {
      return '\0';
    }
    return _source[_current + 1];
  }
  
  private void String()
  {
    while (Peek() != '"' && !IsAtEnd())
    {
      if (Peek() == '\n')
      {
        _line++;
      }
      _current++;
    }

    if (IsAtEnd())
    {
      Lox.Error(_line, "Unterminated string.");
      return;
    }

    // The closing ".
    _current++;

    // Trim the surrounding quotes.
    string value = _source.Substring(_start + 1, _current - _start - 2);
    AddToken(TokenType.String, value);
  }

  private void Number()
  {
    while (IsDigit(Peek()))
    {
      _current++;
    }
    
    // Look for a fractional part.
    if (Peek() == '.' && IsDigit(PeekNext()))
    {
      // Consume the "."
      _current++;

      while (IsDigit(Peek()))
      {
        _current++;
      }
    }
    
    AddToken(TokenType.Number, double.Parse(_source.Substring(_start, _current - _start)));
  }
  
  private void Identifier()
  {
    while (IsAlphaNumeric(Peek()))
    {
      _current++;
    }
    
    string text = _source.Substring(_start, _current - _start);
    if (s_keywords.ContainsKey(text))
    {
      AddToken(s_keywords[text]);
    }
    else 
    {
      AddToken(TokenType.Identifier);
    }
  }

  private void AddToken(TokenType type)
  {
    AddToken(type, null);
  }
  
  private void AddToken(TokenType type, object? literal)
  {
    _tokens.Add(new Token(type, _source.Substring(_start, _current - _start), literal, _line));
  }
}
