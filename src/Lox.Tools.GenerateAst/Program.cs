using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lox.Tools.GenerateAst;

public static class Program
{
    public static void Main(string[] args)
    {
        // TODO: Update
        if (args.Length != -1)
        {
            Console.WriteLine("Not updated");
        }
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: generate_ast <output directory>");
            Environment.Exit(64);
        }

        string outputDir = args[0];
        DefineAst(outputDir, "Expr", new()
        {
            "BinaryExpr   : Expr left, Token op, Expr right",
            "GroupingExpr : Expr expression",
            "LiteralExpr  : object value",
            "UnaryExpr    : Token op, Expr right"
        });
    }

    private static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        string path = $"{outputDir}/{baseName}.cs";
        var builder = new StringBuilder();

        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine();
        builder.AppendLine("namespace Lox;");
        builder.AppendLine();

        builder.AppendLine($"abstract class {baseName}");
        builder.AppendLine("{");

        // The base Accept() method.
        builder.AppendLine("\tpublic abstract R Accept<R>(IVisitor<R> visitor);");
        builder.AppendLine("}");

        builder.AppendLine();
        DefineVisitor(builder, baseName, types);

        // The AST classes.
        foreach (string type in types)
        {
            string className = type.Split(':')[0].Trim();
            string fields = type.Split(":")[1].Trim();
            builder.AppendLine();
            DefineType(builder, baseName, className, fields);
        }

        File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
    }

    private static void DefineType(StringBuilder builder, string baseName, string className, string fieldList)
    {
        builder.AppendLine($"internal class {className} : {baseName}");
        builder.AppendLine("{");

        // Constructor.
        builder.AppendLine($"\tpublic {className}({fieldList})");
        builder.AppendLine("\t{");

        // Store parameters in fields.
        string[] fields = fieldList.Split(", ");
        foreach (string field in fields)
        {
            string name = field.Split(' ')[1];
            builder.AppendLine($"\t\t{name.ToTitleCase()} = {name};");
        }

        builder.AppendLine("\t}");

        // Visitor pattern.
        builder.AppendLine();
        builder.AppendLine("\tpublic override R Accept<R>(IVisitor<R> visitor)");
        builder.AppendLine("\t{");
        builder.AppendLine($"\t\treturn visitor.Visit{className}(this);");
        builder.AppendLine("\t}");

        // Fields.
        builder.AppendLine();
        foreach (string field in fields)
        {
            var fieldParts = field.Split(' ');
            builder.AppendLine($"\tpublic {fieldParts[0]} {fieldParts[1].ToTitleCase()} {{ get; private set; }}");
        }

        builder.AppendLine("}");
    }

    private static void DefineVisitor(StringBuilder builder, string baseName, List<string> types)
    {
        builder.AppendLine("interface IVisitor<R>");
        builder.AppendLine("{");

        foreach (string type in types)
        {
            string typeName = type.Split(':')[0].Trim();
            builder.AppendLine($"\tR Visit{typeName}({typeName} {baseName.ToLower()});");
        }

        builder.AppendLine("}");
    }
}