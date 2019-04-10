﻿using Newtonsoft.Json;
using ObjectApproval;
using Xunit.Abstractions;

public class TestBase:
    XunitLoggingBase
{
    public TestBase(ITestOutputHelper output) :
        base(output)
    {
    }
    
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
}