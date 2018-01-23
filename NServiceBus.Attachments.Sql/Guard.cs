using System;

static class Guard
{
    // ReSharper disable UnusedParameter.Global
    public static void AgainstNull(object value, string argumentName)
    {
        if (value == null)
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static void AgainstSqlDelimiters(string argumentName, string value)
    {
        if (value.Contains("]") || value.Contains("[") || value.Contains("`"))
        {
            throw new ArgumentException($"The argument '{value}' contains a ']', '[' or '`'. Names and schemas automatically quoted.");
        }
    }

    public static void AgainstNullOrEmpty(string value, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }
}