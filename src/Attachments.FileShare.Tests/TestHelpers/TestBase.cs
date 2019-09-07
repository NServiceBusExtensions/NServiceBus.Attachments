using Newtonsoft.Json;
using Xunit.Abstractions;
using ObjectApproval;

public class TestBase:
    XunitApprovalBase
{
    static TestBase()
    {
        SerializerBuilder.ExtraSettings = settings =>
        {
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            settings.ContractResolver = new CustomContractResolver();
            var converters = settings.Converters;
            converters.Add(new GuidConverter());
            converters.Add(new StringConverter());
        };
    }

    public TestBase(ITestOutputHelper output) :
        base(output)
    {
    }
}