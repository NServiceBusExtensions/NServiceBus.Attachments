#if(NET472)
using Newtonsoft.Json;
using ObjectApproval;
#endif
using Xunit.Abstractions;

public class TestBase
{
    static TestBase()
    {
#if(NET472)
        var jsonSerializer = ObjectApprover.JsonSerializer;
        jsonSerializer.DefaultValueHandling = DefaultValueHandling.Ignore;
        jsonSerializer.ContractResolver = new CustomContractResolver();
        var converters = jsonSerializer.Converters;
        converters.Add(new GuidConverter());
        converters.Add(new StringConverter());
#endif
    }

    public TestBase(ITestOutputHelper output)
    {
        Output = output;
    }

    protected readonly ITestOutputHelper Output;
}