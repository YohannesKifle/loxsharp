using System.Collections.Generic;

namespace Lox;

abstract class Expr
{
	public abstract R Accept<R>(IVisitor<R> visitor);
}

interface IVisitor<R>
{
	R VisitBinaryExpr(BinaryExpr expr);
	R VisitGroupingExpr(GroupingExpr expr);
	R VisitLiteralExpr(LiteralExpr expr);
	R VisitUnaryExpr(UnaryExpr expr);
}

internal class BinaryExpr : Expr
{
	public BinaryExpr(Expr left, Token op, Expr right)
	{
		Left = left;
		Op = op;
		Right = right;
	}

	public override R Accept<R>(IVisitor<R> visitor)
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

	public override R Accept<R>(IVisitor<R> visitor)
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

	public override R Accept<R>(IVisitor<R> visitor)
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

	public override R Accept<R>(IVisitor<R> visitor)
	{
		return visitor.VisitUnaryExpr(this);
	}

	public Token Op { get; private set; }
	public Expr Right { get; private set; }
}
