using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Xunit.Abstractions;
using ObjectApproval;

public abstract class TestBase :
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

    protected TestBase(ITestOutputHelper output, [CallerFilePath] string sourceFilePath = "") :
        base(output, sourceFilePath)
    {
    }
}