using System;
using System.Collections.Generic;
using System.IO;

namespace Lox;

static class Lox
{
    private static bool _hadError = false;
    private static bool _hadRuntimeError = false;
    private static readonly Interpreter _interpreter = new();

    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: jlox [script]");
            System.Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    private static void RunFile(string path)
    {
        string fileContent = File.ReadAllText(Path.GetFullPath(path));
        Run(fileContent);

        if (_hadError) System.Environment.Exit(65);
        if (_hadRuntimeError) System.Environment.Exit(70);
    }

    private static void RunPrompt()
    {
        for (; ; )
        {
            Console.Write("> ");
            string line = Console.ReadLine();
            if (line == null) break;

            Run(line);
        }
    }

    private static void Run(string source)
    {
        Scanner scanner = new(source);
        List<Token> tokens = scanner.ScanTokens();

        Parser parser = new(tokens);
        List<Stmt> statements = parser.Parse();

        // Stop if there was a syntax error.
        if (_hadError) return;

        _interpreter.Interpret(statements);
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    internal static void Error(Token token, string message)
    {
        if (token.Type == TokenType.EOF)
        {
            Report(token.Line, " at end", message);
        }
        else
        {
            Report(token.Line, " at '" + token.Lexeme + "'", message);
        }
    }

    internal static void RuntimeError(RuntimeError error)
    {
        Console.Error.WriteLine(error.Message + "\n[line " + error.Token.Line + "]");
        _hadRuntimeError = true;
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        _hadError = true;
    }
}