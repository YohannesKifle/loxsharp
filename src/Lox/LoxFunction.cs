using System.Collections.Generic;

namespace Lox;

internal class LoxFunction : ICallable
{
    public int Arity => _declaration.Params.Count;

    private readonly FunctionStmt _declaration;
    private readonly Environment _closure;

    public LoxFunction(FunctionStmt declaration, Environment closure)
    {
        _declaration = declaration;
        _closure = closure;
    }

    public object Call(Interpreter interpreter, List<object> arguments)
    {
        Environment environment = new(_closure);
        for (int i = 0; i < _declaration.Params.Count; i++)
        {
            environment.Define(_declaration.Params[i].Lexeme, arguments[i]);
        }

        try
        {
            interpreter.ExecuteBlock(_declaration.Body, environment);
        }
        catch (ReturnException returnValue)
        {
            return returnValue.Value;
        }

        return null;
    }

    public override string ToString() => $"<fn {_declaration.Name.Lexeme}>";
}
