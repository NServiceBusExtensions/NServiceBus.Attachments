using Newtonsoft.Json;
using ObjectApproval;
using Xunit.Abstractions;

public class TestBase
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

    public TestBase(ITestOutputHelper output)
    {
        Output = output;
    }

    protected readonly ITestOutputHelper Output;
}