using DeviceUniqueIdentifier.Utils;
using System.Diagnostics;
using System.Text;

namespace DeviceUniqueIdentifier.Platforms
{
    internal class Linux : PlatformBase
    {
        public override string GetRawDeviceData(List<string> additionalInfo = null)
        {
            return GetRawDeviceDataAsync(additionalInfo).GetAwaiter().GetResult();
        }

        async public override Task<string> GetRawDeviceDataAsync(List<string> additionalInfo = null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(await CpuInfo("vendor_id")); // intel or other shite
            stringBuilder.AppendLine(await CpuInfo("cpu family")); // https://superuser.com/questions/1154244/what-is-cpu-family
            stringBuilder.AppendLine(await CpuInfo("cpu cores")); // the same: https://superuser.com/questions/1154244/what-is-cpu-family
            stringBuilder.AppendLine(await CpuInfo("model name")); // anime
            stringBuilder.AppendLine(await JournalCtl()); // kernel info
            stringBuilder.AppendLine(await PureCmd("cat /etc/machine-id")); // "unique" machine id
            return stringBuilder.ToString();
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

        private async Task<string> JournalCtl(List<string> additionalInfo = null)
        {
            try
            {
                var cmd = new Cmd();

                var k = await cmd.RunAsync("/bin/bash", $"-c \"journalctl --quiet --system --boot --no-pager -o cat SYSLOG_IDENTIFIER=kernel | head -n 200\"", new CmdOptions
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    RedirectStdOut = true,
                    UseOsShell = false
                }, true);

                // LoggingService.Info($"Getting kernel info with resulted: \n {k.Output}");
                if (k.ExitCode != 0)
                {
                    additionalInfo?.Add($"Real error while getting kernel info: \n {k.Msg}\n");
                }

                var lines = k.Output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                StringBuilder paramAll = new StringBuilder();
                paramAll.AppendLine(GetIncluding(lines, "Linux version"));
                paramAll.AppendLine(GetIncluding(lines, "Command line"));
                paramAll.AppendLine(GetIncluding(lines, "BIOS-e820"));
                paramAll.AppendLine(GetIncluding(lines, "DMI"));

                return paramAll.ToString();
            }
            catch (Exception)
            {
                additionalInfo?.Add($"Error while getting kernel info");
            }
            return string.Empty;

            string GetIncluding(string[] lines, string parm)
            {
                StringBuilder parmAll = new StringBuilder();
                foreach (var l in lines)
                {
                    if (l.Contains(parm))
                        parmAll.AppendLine(l);
                }
                return parmAll.ToString();
            }
        }

        private async Task<string> CpuInfo(string param, List<string> additionalInfo = null)
        {
            try
            {
                var cmd = new Cmd();

                var k = await cmd.RunAsync("/bin/bash", $"-c \"cat /proc/cpuinfo\"", new CmdOptions
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    RedirectStdOut = true,
                    UseOsShell = false
                }, true);

                // LoggingService.Info($"Getting cpuinfo with {param} resulted: \n {k.Output}");
                if (k.ExitCode != 0)
                {
                    additionalInfo?.Add($"Real error while getting param {param} in cpuinfo: \n {k.Msg}");
                }

                var lines = k.Output.Split(new[]
                    {
                    Environment.NewLine
                }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => a.Trim(' ', '\t'));

                var line = lines.First(a => a.StartsWith(param));
                var res = line.Substring(line.IndexOf(param, StringComparison.Ordinal) + param.Length).Trim(' ', '\t');

                return res;
            }
            catch (Exception)
            {
                additionalInfo?.Add($"Error while getting param {param} in cpuinfo");
            }
            return string.Empty;
        }
    }
}
