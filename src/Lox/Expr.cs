using System.Collections.Generic;

namespace Lox;

abstract class Expr
{
	public abstract R Accept<R>(IExprVisitor<R> visitor);
}

interface IExprVisitor<R>
{
	R VisitAssignExpr(AssignExpr expr);
	R VisitBinaryExpr(BinaryExpr expr);
    R VisitGroupingExpr(GroupingExpr expr);
	R VisitLiteralExpr(LiteralExpr expr);
	R VisitUnaryExpr(UnaryExpr expr);
	R VisitVariableExpr(VariableExpr expr);
	R VisitLogicalExpr(LogicalExpr expr);
	R VisitCallExpr(CallExpr expr);
}

internal class AssignExpr : Expr
{
    public AssignExpr(Token name, Expr value)
	{
		Name = name;
        Value = value;
    }

    public override R Accept<R>(IExprVisitor<R> visitor)
    {
        return visitor.VisitAssignExpr(this);
    }

    public Token Name { get; private set; }
    public Expr Value { get; private set; }
}

internal class BinaryExpr : Expr
{
	public BinaryExpr(Expr left, Token op, Expr right)
	{
		Left = left;
		Op = op;
		Right = right;
	}

	public override R Accept<R>(IExprVisitor<R> visitor)
	{
		return visitor.VisitBinaryExpr(this);
	}

	public Expr Left { get; private set; }
	public Token Op { get; private set; }
	public Expr Right { get; private set; }
}

internal class GroupingExpr : Expr
{
	public GroupingExpr(Expr expression)
	{
		Expression = expression;
	}

	public override R Accept<R>(IExprVisitor<R> visitor)
	{
		return visitor.VisitGroupingExpr(this);
	}

	public Expr Expression { get; private set; }
}

internal class LiteralExpr : Expr
{
	public LiteralExpr(object value)
	{
		Value = value;
	}

	public override R Accept<R>(IExprVisitor<R> visitor)
	{
		return visitor.VisitLiteralExpr(this);
	}

	public object Value { get; private set; }
}

internal class UnaryExpr : Expr
{
	public UnaryExpr(Token op, Expr right)
	{
		Op = op;
		Right = right;
	}

	public override R Accept<R>(IExprVisitor<R> visitor)
	{
		return visitor.VisitUnaryExpr(this);
	}

	public Token Op { get; private set; }
	public Expr Right { get; private set; }
}

internal class VariableExpr : Expr
{
    public VariableExpr(Token name)
    {
		Name = name;
    }

    public override R Accept<R>(IExprVisitor<R> visitor)
    {
        return visitor.VisitVariableExpr(this);
    }

    public Token Name { get; private set; }
}

internal class LogicalExpr : Expr
{
    public LogicalExpr(Expr left, Token op, Expr right)
    {
        Left = left;
        Op = op;
        Right = right;
    }

    public override R Accept<R>(IExprVisitor<R> visitor)
    {
        return visitor.VisitLogicalExpr(this);
    }

    public Expr Left { get; private set; }
    public Token Op { get; private set; }
    public Expr Right { get; private set; }
}

internal class CallExpr : Expr
{
    public CallExpr(Expr callee, Token paren, List<Expr> arguments)
	{
		Callee = callee;
        Paren = paren;
        Arguments = arguments;
    }

    public override R Accept<R>(IExprVisitor<R> visitor)
	{
        return visitor.VisitCallExpr(this);
    }

    public Expr Callee { get; private set; }
    public Token Paren { get; private set; }
    public List<Expr> Arguments { get; private set; }
}