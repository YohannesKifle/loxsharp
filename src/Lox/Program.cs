using System;
using System.Collections.Generic;
using System.IO;

namespace Lox;

static class Lox
{
    private static bool _hadError = false;

    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: jlox [script]");
            Environment.Exit(64);
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

        if (_hadError) Environment.Exit(65);
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
        Expr expression = parser.Parse();

        // Stop if there was a syntax error.
        if (_hadError) return;

        Console.WriteLine(new AstPrinter().Print(expression));
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

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
        _hadError = true;
    }
}