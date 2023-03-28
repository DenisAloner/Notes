using System.Globalization;
using EFCore.NamingConventions.Internal;

namespace Notes.CoreService.Extensions;

public static class StringExtensions
{
    private static readonly CamelCaseNameRewriter CamelCaseFormatter = new(CultureInfo.InvariantCulture);

    public static string ToCamelCase(this string value) => CamelCaseFormatter.RewriteName(value);
}