using System.Threading.Tasks;
using NServiceBus.Attachments;
using NServiceBus.Installation;
using NServiceBus.Settings;

class NeedToInstallSomething : INeedToInstallSomething
{
    AttachmentSettings installerSettings;

    public NeedToInstallSomething(ReadOnlySettings settings)
    {
        installerSettings = settings.GetOrDefault<AttachmentSettings>();
    }

    public async Task Install(string identity)
    {
        if (installerSettings == null || installerSettings.DisableInstaller)
        {
            return;
        }

        using (var connection = installerSettings.ConnectionBuilder())
        {
            await connection.OpenAsync().ConfigureAwait(false);
            Installer.CreateTable(connection, installerSettings.Schema, installerSettings.TableName);
        }
    }
}