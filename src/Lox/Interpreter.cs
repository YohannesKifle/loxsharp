using System;
using System.Collections.Generic;
using static Lox.TokenType;

namespace Lox;

internal class Interpreter : IExprVisitor<object>, IStmtVisitor<Void>
{
    private Environment _environment = new();

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (Stmt statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError error)
        {
            Lox.RuntimeError(error);
        }
    }

    private void Execute(Stmt statement)
    {
        statement.Accept(this);
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

    public object VisitAssignExpr(AssignExpr expr)
    {
        object value = Evaluate(expr.Value);
        _environment.Assign(expr.Name, value);
        return value;
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

    public object VisitVariableExpr(VariableExpr expr)
    {
        return _environment.Get(expr.Name);
    }

    public object VisitLogicalExpr(LogicalExpr expr)
    {
        object left = Evaluate(expr.Left);

        if (expr.Op.Type == OR)
        {
            if (IsTruthy(left)) return left;
        }
        else
        {
            if (!IsTruthy(left)) return left;
        }

        return Evaluate(expr.Right);
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

    // object is not null or false
    private static bool IsTruthy(object obj)
    {
        if (obj == null) return false;
        return obj is not bool || (bool)obj;
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

    public Void VisitPrintStmt(PrintStmt stmt)
    {
        object value = Evaluate(stmt.Expression);
        Console.WriteLine(Stringify(value));
        return Void.Value;
    }

    public Void VisitVarStmt(VarStmt stmt)
    {
        object value = null;
        if (stmt.Initializer != null)
        {
            value = Evaluate(stmt.Initializer);
        }

        _environment.Define(stmt.Name.Lexeme, value);
        return Void.Value;
    }

    public Void VisitBlockStmt(BlockStmt stmt)
    {
        ExecuteBlock(stmt.Statements, new Environment(_environment));
        return Void.Value;
    }

    public Void VisitExpressionStmt(ExpressionStmt stmt)
    {
        Evaluate(stmt.Expression);
        return Void.Value;
    }

    public Void VisitIfStmt(IfStmt stmt)
    {
        if (IsTruthy(Evaluate(stmt.Condition)))
        {
            Execute(stmt.ThenBranch);
        }
        else if (stmt.ElseBranch != null)
        {
            Execute(stmt.ElseBranch);
        }
        return Void.Value;
    }

    public Void VisitWhileStmt(WhileStmt stmt)
    {
        while (IsTruthy(Evaluate(stmt.Condition)))
        {
            stmt.Body.Accept(this);
        }

        return Void.Value;
    }

    public Void VisitClassStmt(ClassStmt stmt)
    {
        throw new NotImplementedException();
    }

    public Void VisitFunctionStmt(FunctionStmt stmt)
    {
        throw new NotImplementedException();
    }

    public Void VisitReturnStmt(ReturnStmt stmt)
    {
        throw new NotImplementedException();
    }

    private void ExecuteBlock(List<Stmt> statements, Environment environment)
    {
        Environment previous = _environment;
        try
        {
            _environment = environment;

            foreach (Stmt statement in statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            _environment = previous;
        }
    }
}
