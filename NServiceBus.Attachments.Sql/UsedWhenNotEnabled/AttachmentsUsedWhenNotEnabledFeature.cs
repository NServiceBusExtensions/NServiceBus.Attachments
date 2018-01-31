using NServiceBus;
using NServiceBus.Features;

class AttachmentsUsedWhenNotEnabledFeature : Feature
{
    public AttachmentsUsedWhenNotEnabledFeature()
    {
        EnableByDefault();
    }

    protected override void Setup(FeatureConfigurationContext context)
    {
        if (context.Settings.TryGet<AttachmentSettings>(out var _))
        {
            return;
        }

        context.Pipeline.Register(new UsedWhenNotEnabledRegistration());
    }
}