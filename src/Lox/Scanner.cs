using System.Collections;
using System.Collections.Generic;
using static Lox.TokenType;

namespace Lox;

internal class Scanner
{
    private readonly string _source;
    private readonly List<Token> _tokens = [];
    private int _start = 0;
    private int _current = 0;
    private int _line = 1;
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        {"and", AND},
        {"class", CLASS},
        {"else", ELSE},
        {"false", FALSE},
        {"for", FOR},
        {"fun", FUN},
        {"if", IF},
        {"nil", NIL},
        {"or", OR},
        {"print", PRINT},
        {"return", RETURN},
        {"super", SUPER},
        {"this", THIS},
        {"true", TRUE},
        {"var", VAR},
        {"while", WHILE},
    };
    
    public Scanner(string source)
    {
        _source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            // We are at the beginning of the next lexeme.
            _start = _current;
            ScanToken();
        }

        _tokens.Add(new Token(EOF, "", null, _line));
        return _tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(':
                AddToken(LEFT_PAREN);
                break;
            case ')':
                AddToken(RIGHT_PAREN);
                break;
            case '{':
                AddToken(LEFT_BRACE);
                break;
            case '}':
                AddToken(RIGHT_BRACE);
                break;
            case ',':
                AddToken(COMMA);
                break;
            case '.':
                AddToken(DOT);
                break;
            case '-':
                AddToken(MINUS);
                break;
            case '+':
                AddToken(PLUS);
                break;
            case ';':
                AddToken(SEMICOLON);
                break;
            case '*':
                AddToken(STAR);
                break;
            case '!':
                AddToken(Match('=') ? BANG_EQUAL : BANG);
                break;
            case '=':
                AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? LESS_EQUAL : LESS);
                break;
            case '>':
                AddToken(Match('=') ? GREATER_EQUAL : GREATER);
                break;
            case '/':
                if (Match('/'))
                {
                    // A comment goes until the end of the line.
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                }
                else
                {
                    AddToken(SLASH);
                }
                break;
            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace.
                break;
            case '\n':
                _line++;
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
                    Lox.Error(_line, "Unexpected character.");
                }
                break;
        }
    }

    private static bool IsAlphaNumeric(char c) => IsAlpha(c) || IsDigit(c);
    private static bool IsDigit(char c) => c is >= '0' and <= '9';
    private static bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') ||
            (c >= 'A' && c <= 'Z') ||
            c == '_';
    }

    private void Number()
    {
        while (IsDigit(Peek())) Advance();

        // Look for a fractional part.
        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            // Consume the "."
            Advance();

            while (IsDigit(Peek())) Advance();
        }
        AddToken(NUMBER, double.Parse(_source[_start.._current]));
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek()))
        {
            Advance();
        }

        string text = _source[_start.._current];
        if (!Keywords.TryGetValue(text, out TokenType type))
        {
            type = IDENTIFIER;
        }

        AddToken(type);
    }

    private void String()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') _line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(_line, "Unterminated string.");
            return;
        }

        // The closing ".
        Advance();

        // Trim the surrounding quotes.
        string value = _source[(_start + 1)..(_current - 1)];
        AddToken(STRING, value);
    }

    private char Advance()
    {
        return _source[_current++];
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return _source[_current];
    }

    private char PeekNext()
    {
        if (_current + 1 >= _source.Length) return '\0';
        return _source[_current + 1];
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object literal)
    {
        string text = _source[_start.._current];
        _tokens.Add(new Token(type, text, literal, _line));
    }

    private bool IsAtEnd()
    {
        return _current >= _source.Length;
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (_source[_current] != expected) return false;

        _current++;
        return true;
    }
}