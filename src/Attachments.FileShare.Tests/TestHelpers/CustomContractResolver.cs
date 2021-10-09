using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VerifyTests;

public class CustomContractResolver :
    DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);

        property.SkipEmptyCollections(member);

        return property;
    }
}