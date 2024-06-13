using System.Globalization;

namespace Lox.Tools.GenerateAst;

internal static class StringExtensions
{
    private static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;

    public static string ToTitleCase(this string str) => TextInfo.ToTitleCase(str);
}