using NServiceBus.Features;
using NServiceBus.Attachments;

#if FileShare
using NServiceBus.Attachments.FileShare;
#endif
#if Sql
using NServiceBus.Attachments.Sql;
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