using System;

namespace Lox;

public class ReturnException : Exception
{
    public object Value { private set; get; }

    public ReturnException(object value) : base(null, null)
    {
        Value = value;
    }

    public override string StackTrace => string.Empty;
}
