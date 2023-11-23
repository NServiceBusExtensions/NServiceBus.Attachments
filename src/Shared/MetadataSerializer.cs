using Serializer = System.Text.Json.JsonSerializer;

namespace NServiceBus.Attachments
#if FileShare
.FileShare
#elif Sql
.Sql
#endif
#if Raw
.Raw
#endif
;

/// <summary>
/// Converts a dictionary of metadata to/from json.
/// </summary>
public static class MetadataSerializer
{
    /// <summary>
    /// An empty <see cref="IReadOnlyDictionary{TKey,TValue}"/>
    /// </summary>
    public readonly static IReadOnlyDictionary<string, string> EmptyMetadata = new Dictionary<string, string>();

    /// <summary>
    /// Serialize <paramref name="instance"/> to json.
    /// </summary>
    public static string? Serialize(IReadOnlyDictionary<string, string>? instance)
    {
        if (instance is null)
        {
            return null;
        }

        return Serializer.Serialize(instance);
    }

    /// <summary>
    /// Deserialize <paramref name="json"/> to a <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
    /// </summary>
    public static IReadOnlyDictionary<string, string> Deserialize(string? json)
    {
        if (json is null)
        {
            return EmptyMetadata;
        }

        return Serializer.Deserialize<Dictionary<string, string>>(json)!;
    }

    /// <summary>
    ///
    /// </summary>
    public static async Task<IReadOnlyDictionary<string, string>> Deserialize(
        Stream stream,
        Cancel cancel = default) =>
        (await Serializer.DeserializeAsync<Dictionary<string, string>>(stream, cancellationToken: cancel))!;

    /// <summary>
    ///
    /// </summary>
    public static Task Serialize(
        Stream stream,
        IReadOnlyDictionary<string, string> metadata,
        Cancel cancel = default) =>
        Serializer.SerializeAsync(stream, metadata, cancellationToken: cancel);

    internal static Dictionary<string, string> AppendEncoding(Encoding encoding, IReadOnlyDictionary<string, string>? metadata)
    {
        Dictionary<string, string> dictionary;
        if (metadata == null)
        {
            dictionary = [];
        }
        else
        {
            dictionary = metadata.ToDictionary(_ => _.Key, _ => _.Value);
        }

        dictionary.Add("encoding", encoding.WebName);
        return dictionary;
    }

    internal static Encoding GetEncoding(Encoding? encoding, IReadOnlyDictionary<string, string> metadata)
    {
        if (metadata.TryGetValue("encoding", out var encodingString))
        {
            return Encoding.GetEncoding(encodingString);
        }

        if (encoding == null)
        {
            return encoding.Default();
        }

        return encoding;
    }
}