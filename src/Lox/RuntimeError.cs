using System;

namespace Lox;

internal class RuntimeError : Exception
{
    public RuntimeError(Token token, string message) : base(message)
    {
        Token = token;
    }

    public Token Token { get; private set; }
}