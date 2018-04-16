using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace NServiceBus.Attachments
#if FileShare
    .FileShare
#elif Sql
    .Sql
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
        public readonly static IReadOnlyDictionary<string, string> EmptyMetadata= new Dictionary<string, string>();

        /// <summary>
        /// Serialize <paramref name="instance"/> to json.
        /// </summary>
        public static string Serialize(IReadOnlyDictionary<string, string> instance)
        {
            if (instance == null)
            {
                return null;
            }
            var serializer = BuildSerializer();
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, instance);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Deserialize <paramref name="json"/> to a <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
        /// </summary>
        public static IReadOnlyDictionary<string, string> Deserialize(string json)
        {
            if (json == null)
            {
                return EmptyMetadata;
            }
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return Deserialize(stream);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public static IReadOnlyDictionary<string, string> Deserialize(Stream stream)
        {
            var serializer = BuildSerializer();
            return (IReadOnlyDictionary<string, string>) serializer.ReadObject(stream);
        }
        /// <summary>
        ///
        /// </summary>
        public static void Serialize(Stream stream, IReadOnlyDictionary<string, string> metadata)
        {
            var serializer = BuildSerializer();
            serializer.WriteObject(stream, metadata);
        }

        static DataContractJsonSerializer BuildSerializer()
        {
            var settings = new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true
            };
            return new DataContractJsonSerializer(typeof(Dictionary<string, string>), settings);
        }
    }
}