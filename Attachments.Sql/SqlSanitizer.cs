using System.Text;

static class SqlSanitizer
{
    private static string quoteSuffix = "]";
    private static string quotePrefix = "[";

    public static string Sanitize(string unquotedIdentifier)
    {
        var builder = new StringBuilder();
        if (!string.IsNullOrEmpty(quotePrefix))
        {
            builder.Append(quotePrefix);
        }

        if (string.IsNullOrEmpty(quoteSuffix))
        {
            builder.Append(unquotedIdentifier);
        }
        else
        {
            builder.Append(unquotedIdentifier.Replace(quoteSuffix, quoteSuffix + quoteSuffix));
            builder.Append(quoteSuffix);
        }

        return builder.ToString();
    }
}