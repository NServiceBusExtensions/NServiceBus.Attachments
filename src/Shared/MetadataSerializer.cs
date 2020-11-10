using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
{
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
            if (instance == null)
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
            if (json == null)
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
            CancellationToken cancellation = default)
        {
            return (await Serializer.DeserializeAsync<Dictionary<string, string>>(stream, cancellationToken: cancellation))!;
        }

        /// <summary>
        ///
        /// </summary>
        public static Task Serialize(
            Stream stream,
            IReadOnlyDictionary<string, string> metadata,
            CancellationToken cancellation = default)
        {
            return Serializer.SerializeAsync(stream, metadata, cancellationToken: cancellation);
        }
    }
}