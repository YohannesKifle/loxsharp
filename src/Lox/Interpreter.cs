using System;
using static Lox.TokenType;

namespace Lox;

internal class Interpreter : IVisitor<object>
{
    public void Interpret(Expr expression)
    {
        try
        {
            object value = Evaluate(expression);
            Console.WriteLine(Stringify(value));
        }
        catch (RuntimeError error)
        {
            Lox.RuntimeError(error);
        }
    }

    private static string Stringify(object obj)
    {
        if (obj == null) return "nil";

        if (obj is double)
        {
            string text = obj.ToString();
            return text;
        }

        return obj.ToString();
    }

    public object VisitBinaryExpr(BinaryExpr expr)
    {
        object left = Evaluate(expr.Left);
        object right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case BANG_EQUAL: return !IsEqual(left, right);
            case EQUAL_EQUAL: return IsEqual(left, right);
            case GREATER:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left > (double)right;
            case GREATER_EQUAL:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left >= (double)right;
            case LESS:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left < (double)right;
            case LESS_EQUAL:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left <= (double)right;
            case MINUS:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left - (double)right;
            case PLUS:
                if (left is double lDbl && right is double rDbl)
                {
                    return lDbl + rDbl;
                }

                if (left is string lStr && right is string rStr)
                {
                    return lStr + rStr;
                }
                throw new RuntimeError(expr.Op, "Operands must be two numbers or two strings.");
            case SLASH:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left / (double)right;
            case STAR:
                CheckNumberOperands(expr.Op, left, right);
                return (double)left * (double)right;
        }

        // Unreachable.
        return null;
    }

    public object VisitGroupingExpr(GroupingExpr expr)
    {
        return Evaluate(expr);
    }

    public object VisitLiteralExpr(LiteralExpr expr)
    {
        return expr.Value;
    }

    public object VisitUnaryExpr(UnaryExpr expr)
    {
        object right = Evaluate(expr.Right);

        switch (expr.Op.Type)
        {
            case MINUS:
                CheckNumberOperand(expr.Op, right);
                return -(double)right;
            case BANG: return !IsTruthy(right);
        }

        return null;
    }

    private static void CheckNumberOperand(Token op, object operand)
    {
        if (operand is double) return;
        throw new RuntimeError(op, "Operand must be a number.");
    }

    private static void CheckNumberOperands(Token op, object left, object right)
    {
        if (left is double && right is double) return;
        throw new RuntimeError(op, "Operands must be numbers.");
    }

    private static bool IsTruthy(object obj)
    {
        if (obj == null) return false;
        if (obj is bool) return (bool)obj;

        return true;
    }

    private object Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    private static bool IsEqual(object left, object right)
    {
        if (left == null && right == null) return true;
        if (left == null) return false;

        return left.Equals(right);
    }
}
