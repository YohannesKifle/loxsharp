using System;
using System.Text;

namespace Lox;

internal class AstPrinter : IVisitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    public string VisitAssignExpr(AssignExpr expr)
    {
        throw new NotImplementedException();
    }

    public string VisitBinaryExpr(BinaryExpr expr)
    {
        return Parenthesize(expr.Op.Lexeme,
                            expr.Left, expr.Right);
    }

    public string VisitGroupingExpr(GroupingExpr expr)
    {
        return Parenthesize("group", expr.Expression);
    }

    public string VisitLiteralExpr(LiteralExpr expr)
    {
        if (expr.Value == null) return "nil";
        return expr.Value.ToString();
    }

    public string VisitUnaryExpr(UnaryExpr expr)
    {
        return Parenthesize(expr.Op.Lexeme, expr.Right);
    }

    public string VisitVariableExpr(VariableExpr expr)
    {
        throw new NotImplementedException();
    }

    private string Parenthesize(string name, params Expr[] exprs)
    {
        StringBuilder builder = new();

        builder.Append('(').Append(name);
        foreach (Expr expr in exprs)
        {
            builder.Append(' ');
            builder.Append(expr.Accept(this));
        }
        builder.Append(')');

        return builder.ToString();
    }
}

public static class LoxSample
{

    // Sample program for testing AstPrinter
    public static void AstPrinterMain()
    {
        Expr expression = new BinaryExpr(
            new UnaryExpr(
                new Token(TokenType.MINUS, "-", null, 1),
                new LiteralExpr(123)),
            new Token(TokenType.STAR, "*", null, 1),
            new GroupingExpr(new LiteralExpr(45.67)));

        Console.WriteLine(new AstPrinter().Print(expression));
    }
}