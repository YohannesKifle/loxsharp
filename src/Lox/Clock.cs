using System;
using System.Collections.Generic;

namespace Lox;

internal class Clock : ICallable
{
    public int Arity => 0;

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        return (double)DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000.0;
    }

    public override string ToString()
    {
        return "<native fn>";
    }
}
