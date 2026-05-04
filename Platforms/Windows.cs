
using System.Data;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace DeviceUniqueIdentifier.Platforms
{
    internal class Windows : PlatformBase
    {
        public override string GetRawDeviceData(List<string> additionalInfo = null)
        {
            StringBuilder dataStr = new StringBuilder();
            try
            {
                ManagementObjectSearcher searcherBb = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                foreach (var obj in searcherBb.Get())
                {
                    dataStr.Append((string)obj.Properties["SerialNumber"].Value.ToString().Trim() ?? string.Empty);
                    break;
                }
            }
            catch (Exception)
            {
            }

            try
            {
                ManagementObjectSearcher searcherBios = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
                foreach (var obj in searcherBios.Get())
                {
                    dataStr.Append((string)obj.Properties["SerialNumber"].Value.ToString().Trim() ?? string.Empty);
                    break;
                }
            }
            catch (Exception)
            {
            }

            try
            {
                ManagementObjectSearcher searcherOs = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                foreach (var obj in searcherOs.Get())
                {
                    dataStr.Append(obj.Properties["SerialNumber"].Value.ToString().Trim() ?? string.Empty);
                    break;
                }
            }
            catch (Exception)
            {
            }
            return dataStr.ToString();
        }

        async public override Task<string> GetRawDeviceDataAsync(List<string> additionalInfo = null)
        {
            string dataStr = "";
            await Task.Run(() =>
            {
                dataStr = GetRawDeviceData(additionalInfo);
            });
            return dataStr;
        }
    }
}
