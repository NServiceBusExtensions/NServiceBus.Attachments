using Newtonsoft.Json;
using ObjectApproval;
using Xunit.Abstractions;

public class TestBase:
    XunitLoggingBase
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