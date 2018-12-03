using NServiceBus.Features;

#if FileShare
using NServiceBus.Attachments.FileShare;
#elif Sql
using NServiceBus.Attachments.Sql;
#else
using NServiceBus.Attachments;
#endif

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