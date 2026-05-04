using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace DeviceUniqueIdentifier.Platforms
{
    internal abstract class PlatformBase : IPlatform
    {
        public virtual string GetRawDeviceData(List<string> additionalInfo = null)
        {
            throw new NotImplementedException();
        }

        public virtual Task<string> GetRawDeviceDataAsync(List<string> additionalInfo = null)
        {
            throw new NotImplementedException();
        }

        public virtual string GetSHA1Identifier(List<string> additionalInfo = null)
        {
            var dataStr = GetRawDeviceData(additionalInfo);
            return GetSHA1IdentifierFromRawData(dataStr, additionalInfo);
        }

        async public virtual Task<string> GetSHA1IdentifierAsync(List<string> additionalInfo = null)
        {
            var dataStr = await GetRawDeviceDataAsync(additionalInfo);
            return await GetSHA1IdentifierFromRawDataAsync(dataStr, additionalInfo);
        }

        public virtual string GetSHA1IdentifierFromRawData(string raw, List<string> additionalInfo = null)
        {
            return string.Join("", SHA1.HashData(Encoding.UTF8.GetBytes(raw)).Select(b => b.ToString("x2")));
        }

        async public virtual Task<string> GetSHA1IdentifierFromRawDataAsync(string raw, List<string> additionalInfo = null)
        {
            await Task.CompletedTask; // cringe
            return string.Join("", SHA1.HashData(Encoding.UTF8.GetBytes(raw)).Select(b => b.ToString("x2")));
        }
    }
}
