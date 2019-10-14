using System.Threading.Tasks;
using NServiceBus.Attachments.Sql;
using NServiceBus.Installation;
using NServiceBus.Settings;

class NeedToInstallSomething :
    INeedToInstallSomething
{
    AttachmentSettings settings;

    public NeedToInstallSomething(ReadOnlySettings settings)
    {
        this.settings = settings.GetOrDefault<AttachmentSettings>();
    }

    public async Task Install(string identity)
    {
        if (settings == null || settings.InstallerDisabled)
        {
            return;
        }

        await using var connection = await settings.ConnectionFactory();
        await Installer.CreateTable(connection, settings.Table);
    }
}