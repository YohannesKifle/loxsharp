using System;
using System.Collections.Generic;
using static Lox.TokenType;

namespace Lox;

internal class Parser
{
    private readonly List<Token> _tokens;
    private int _current = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public List<Stmt> Parse()
    {
        List<Stmt> statements = new();
        while (!IsAtEnd())
        {
            statements.Add(Declaration());
        }

        return statements;
    }

    private Stmt Declaration()
    {
        try
        {
            if (Match(VAR)) return VarDeclaration();

            return Statement();
        }
        catch (ParseError)
        {
            Synchronize();
            return null;
        }
    }

    private Stmt VarDeclaration()
    {
        Token name = Consume(IDENTIFIER, "Expect variable name.");

        Expr initializer = null;
        if (Match(EQUAL))
        {
            initializer = Expression();
        }

        Consume(SEMICOLON, "Expect ';' after variable declaration.");
        return new VarStmt(name, initializer);
    }

    private Stmt Statement()
    {
        if (Match(FOR)) return ForStatement();
        if (Match(IF)) return IfStatement();
        if (Match(PRINT)) return PrintStatement();
        if (Match(WHILE)) return WhileStatement();
        if (Match(LEFT_BRACE)) return new BlockStmt(Block());

        return ExpressionStatement();
    }

    private List<Stmt> Block()
    {
        List<Stmt> statements = [];

        while (!Check(RIGHT_BRACE) && !IsAtEnd())
        {
            statements.Add(Declaration());
        }

        Consume(RIGHT_BRACE, "Expect '}' after block.");
        return statements;
    }

    private Stmt ForStatement()
    {
        Consume(LEFT_PAREN, "Expect '(' after 'for'.");
        Stmt initializer;
        if (Match(SEMICOLON))
        {
            initializer = null;
        }
        else if (Match(VAR))
        {
            initializer = VarDeclaration();
        }
        else
        {
            initializer = ExpressionStatement();
        }

        Expr condition = null;
        if (!Check(SEMICOLON))
        {
            condition = Expression();
        }
        Consume(SEMICOLON, "Expect ';' after loop condition.");

        Expr increment = null;
        if (!Check(RIGHT_PAREN))
        {
            increment = Expression();
        }
        Consume(RIGHT_PAREN, "Expect ')' after for clauses.");

        Stmt body = Statement();
        if (increment != null)
        {
            body = new BlockStmt([body, new ExpressionStmt(increment)]);
        }

        condition ??= new LiteralExpr(true);
        body = new WhileStmt(condition, body);

        if (initializer != null)
        {
            body = new BlockStmt([initializer, body]);
        }

        return body;
    }

    private Stmt IfStatement()
    {
        Consume(LEFT_PAREN, "Expect '(' after 'if'.");
        Expr condition = Expression();
        Consume(RIGHT_PAREN, "Expect ')' after if condition.");
        Stmt thenBranch = Statement();
        Stmt elseBranch = null;
        if (Match(ELSE))
        {
            elseBranch = Statement();
        }

        return new IfStmt(condition, thenBranch, elseBranch);
    }

    private Stmt PrintStatement()
    {
        Expr value = Expression();
        Consume(SEMICOLON, "Expect ';' after value.");
        return new PrintStmt(value);
    }

    private Stmt WhileStatement()
    {
        Consume(LEFT_PAREN, "Expect '(' after 'while'.");
        Expr condition = Expression();
        Consume(RIGHT_PAREN, "Expect ')' after condition.");
        Stmt body = Statement();

        return new WhileStmt(condition, body);
    }

    private Stmt ExpressionStatement()
    {
        Expr expr = Expression();
        Consume(SEMICOLON, "Expect ';' after expression.");
        return new ExpressionStmt(expr);
    }

    private Expr Expression()
    {
        return Assignment();
    }

    private Expr Assignment()
    {
        Expr expr = Or();

        if (Match(EQUAL))
        {
            Token equals = Previous();
            Expr value = Assignment();

            if (expr is VariableExpr variableExpr)
            {
                Token name = variableExpr.Name;
                return new AssignExpr(name, value);
            }

            Error(equals, "Invalid assignment target.");
        }

        return expr;
    }

    private Expr Or()
    {
        Expr expr = And();

        while (Match(OR))
        {
            Token op = Previous();
            Expr right = And();
            expr = new LogicalExpr(expr, op, right);
        }

        return expr;
    }

    private Expr And()
    {
        Expr expr = Equality();

        while (Match(AND))
        {
            Token op = Previous();
            Expr right = Equality();
            expr = new LogicalExpr(expr, op, right);
        }

        return expr;
    }

    private Expr Equality()
    {
        Expr expr = Comparison();

        while (Match(BANG_EQUAL, EQUAL_EQUAL))
        {
            Token op = Previous();
            Expr right = Comparison();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr Comparison()
    {
        Expr expr = Term();

        while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
        {
            Token op = Previous();
            Expr right = Term();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr Term()
    {
        Expr expr = Factor();

        while (Match(MINUS, PLUS))
        {
            Token op = Previous();
            Expr right = Term();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr Factor()
    {
        Expr expr = Unary();

        while (Match(SLASH, STAR))
        {
            Token op = Previous();
            Expr right = Term();
            expr = new BinaryExpr(expr, op, right);
        }

        return expr;
    }

    private Expr Unary()
    {
        if (Match(BANG, MINUS))
        {
            Token op = Previous();
            Expr right = Unary();
            return new UnaryExpr(op, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(FALSE)) return new LiteralExpr(false);
        if (Match(TRUE)) return new LiteralExpr(true);
        if (Match(NIL)) return new LiteralExpr(null);

        if (Match(NUMBER, STRING))
        {
            return new LiteralExpr(Previous().Literal);
        }

        if (Match(IDENTIFIER))
        {
            return new VariableExpr(Previous());
        }

        if (Match(LEFT_PAREN))
        {
            Expr expr = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after expression.");
            return new GroupingExpr(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }

    private void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            if (Previous().Type == SEMICOLON) return;

            switch (Peek().Type)
            {
                case CLASS:
                case FUN:
                case VAR:
                case FOR:
                case IF:
                case WHILE:
                case PRINT:
                case RETURN:
                    return;
            }

            Advance();
        }
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();

        throw Error(Peek(), message);
    }

    private ParseError Error(Token token, string message)
    {
        Lox.Error(token, message);
        return new ParseError();
    }

    private bool Match(params TokenType[] types)
    {
        foreach (TokenType type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        }

        return false;
    }

    private bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    private Token Advance()
    {
        if (!IsAtEnd()) _current++;
        return Previous();
    }

    private bool IsAtEnd()
    {
        return Peek().Type == EOF;
    }

    private Token Peek()
    {
        return _tokens[_current];
    }

    private Token Previous()
    {
        return _tokens[_current - 1];
    }

}

internal class ParseError : Exception { }
