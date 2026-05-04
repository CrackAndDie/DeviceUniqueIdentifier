using DeviceUniqueIdentifier.Utils;
using System.Diagnostics;
using System.Management;
using System.Text;

namespace DeviceUniqueIdentifier.Platforms
{
    internal class MacOs : PlatformBase
    {
        public override string GetRawDeviceData(List<string> additionalInfo = null)
        {
            return GetRawDeviceDataAsync(additionalInfo).GetAwaiter().GetResult();
        }

        async public override Task<string> GetRawDeviceDataAsync(List<string> additionalInfo = null)
        {
            return await PureCmd("ioreg -rd1 -c IOPlatformExpertDevice | awk '/IOPlatformUUID/ { print $3; }'", additionalInfo); // "unique" machine id
        }

        private async Task<string> PureCmd(string command, List<string> additionalInfo = null)
        {
            try
            {
                var cmd = new Cmd();

                var k = await cmd.RunAsync("/bin/bash", $"-c \"{command}\"", new CmdOptions
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    RedirectStdOut = true,
                    UseOsShell = false
                }, true);

                additionalInfo?.Add($"Getting {command} resulted: \n {k.Output}\n");
                if (k.ExitCode != 0)
                {
                    additionalInfo?.Add($"Real error while exec {command}: \n {k.Msg}\n");
                }

                return k.Output;
            }
            catch (Exception)
            {
                additionalInfo?.Add($"Error while exec {command}\n");
            }
            return string.Empty;
        }
    }
}
