using System.Collections.Generic;
namespace Lox;

abstract class Stmt
{
    public abstract R Accept<R>(IStmtVisitor<R> visitor);
}

interface IStmtVisitor<R>
{
    R VisitBlockStmt(BlockStmt stmt);
    R VisitClassStmt(ClassStmt stmt);
    R VisitExpressionStmt(ExpressionStmt stmt);
    R VisitFunctionStmt(FunctionStmt stmt);
    R VisitIfStmt(IfStmt stmt);
    R VisitPrintStmt(PrintStmt stmt);
    R VisitReturnStmt(ReturnStmt stmt);
    R VisitVarStmt(VarStmt stmt);
    R VisitWhileStmt(WhileStmt stmt);
}

internal class BlockStmt : Stmt
{
    public List<Stmt> Statements { get; private set; }

    public BlockStmt(List<Stmt> statements)
    {
        Statements = statements;
    }

    public override R Accept<R>(IStmtVisitor<R> visitor)
    {
        return visitor.VisitBlockStmt(this);
    }
}

internal class ClassStmt : Stmt
{
    public Token Name { get; private set; }
    public VariableExpr Superclass { get; private set; }
    public List<FunctionStmt> Methods { get; private set; }

    public ClassStmt(Token name, VariableExpr superclass, List<FunctionStmt> methods)
    {
        Name = name;
        Superclass = superclass;
        Methods = methods;
    }

    public override R Accept<R>(IStmtVisitor<R> visitor)
    {
        return visitor.VisitClassStmt(this);
    }
}

internal class ExpressionStmt : Stmt
{
    public Expr Expression { get; private set; }

    public ExpressionStmt(Expr expression)
    {
        Expression = expression;
    }

    public override R Accept<R>(IStmtVisitor<R> visitor)
    {
        return visitor.VisitExpressionStmt(this);
    }
}

internal class FunctionStmt : Stmt
{
    public Token Name { get; private set; }
    public List<Token> Params { get; private set; }
    public List<Stmt> Body { get; private set; }

    public FunctionStmt(Token name, List<Token> parameters, List<Stmt> body)
    {
        Name = name;
        Params = parameters;
        Body = body;
    }

    public override R Accept<R>(IStmtVisitor<R> visitor)
    {
        return visitor.VisitFunctionStmt(this);
    }
}

internal class IfStmt : Stmt
{
    public Expr Condition { get; private set; }
    public Stmt ThenBranch { get; private set; }
    public Stmt ElseBranch { get; private set; }

    public IfStmt(Expr condition, Stmt thenBranch, Stmt elseBranch)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public override R Accept<R>(IStmtVisitor<R> visitor)
    {
        return visitor.VisitIfStmt(this);
    }
}

internal class PrintStmt : Stmt
{
    public Expr Expression { get; private set; }

    public PrintStmt(Expr expression)
    {
        Expression = expression;
    }

    public override R Accept<R>(IStmtVisitor<R> visitor)
    {
        return visitor.VisitPrintStmt(this);
    }
}

internal class ReturnStmt : Stmt
{
    public Expr Value { get; private set; }
    public Token Keyword { get; private set; }

    public ReturnStmt(Token keyword, Expr value)
    {
        Keyword = keyword;
        Value = value;
    }

    public override R Accept<R>(IStmtVisitor<R> visitor)
    {
        return visitor.VisitReturnStmt(this);
    }
}

internal class VarStmt : Stmt
{
    public Expr Initializer { get; private set; }
    public Token Name { get; private set; }

    public VarStmt(Token name, Expr initializer) {
        Name = name;
        Initializer = initializer;
    }

    public override R Accept<R>(IStmtVisitor<R> visitor)
    {
        return visitor.VisitVarStmt(this);
    }
}

internal class WhileStmt : Stmt
{
    public Expr Condition { get; private set; }
    public Stmt Body { get; private set; }

    public WhileStmt(Expr condition, Stmt body)
    {
        Condition = condition;
        Body = body;
    }

    public override R Accept<R>(IStmtVisitor<R> visitor)
    {
        return visitor.VisitWhileStmt(this);
    }
}