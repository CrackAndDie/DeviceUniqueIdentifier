
using System.Text;

namespace DeviceUniqueIdentifier.Platforms
{
    internal class Windows : IPlatform
    {
        public string GetRawDeviceData()
        {
            throw new NotImplementedException();
        }

        async public Task<string> GetRawDeviceDataAsync()
        {
            StringBuilder dataStr = new StringBuilder();
            try
            {
                ManagementObjectSearcher searcherBb = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                foreach (var obj in searcherBb.Get())
                {
                    concatStr += (string)obj.Properties["SerialNumber"].Value.ToString().Trim() ?? string.Empty;
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
                    concatStr += (string)obj.Properties["SerialNumber"].Value.ToString().Trim() ?? string.Empty;
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
                    concatStr += obj.Properties["SerialNumber"].Value.ToString().Trim() ?? string.Empty;
                    break;
                }
            }
            catch (Exception)
            {
            }
        }

        public string GetSHA1Identifier()
        {
            throw new NotImplementedException();
        }

        async public Task<string> GetSHA1IdentifierAsync()
        {
            throw new NotImplementedException();
        }

        public string GetSHA1IdentifierFromRawData(string raw)
        {
            throw new NotImplementedException();
        }

        async public Task<string> GetSHA1IdentifierFromRawDataAsync(string raw)
        {
            throw new NotImplementedException();
        }
    }
}
