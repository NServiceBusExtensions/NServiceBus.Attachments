﻿using NServiceBus.Attachments.Sql;
using NServiceBus.Installation;
using NServiceBus.Settings;

class NeedToInstallSomething :
    INeedToInstallSomething
{
    AttachmentSettings? settings;

    public NeedToInstallSomething(IReadOnlySettings settings) =>
        this.settings = settings.GetOrDefault<AttachmentSettings?>();

    public async Task Install(string identity, Cancellation cancellation = default)
    {
        if (settings == null || settings.InstallerDisabled)
        {
            return;
        }

        using var connection = await settings.ConnectionFactory(cancellation);
        await Installer.CreateTable(connection, settings.Table, cancellation);
    }
}