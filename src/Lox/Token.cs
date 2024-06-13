namespace Lox;

internal class Token
{
    private readonly TokenType _type;
    private readonly object _literal;
    private readonly int _line;

    public Token(TokenType type, string lexeme, object literal, int line)
    {
        _type = type;
        Lexeme = lexeme;
        _literal = literal;
        _line = line;
    }

    public string Lexeme { get; private set; }

    public override string ToString()
    {
        return $"{_type} {Lexeme} {_literal}";
    }
}
