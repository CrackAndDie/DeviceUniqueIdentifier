using DeviceUniqueIdentifier.Platforms;
using System.Runtime.InteropServices;

namespace DeviceUniqueIdentifier
{
    public class UniqueIdentifierService : IPlatform
    {
        public static IPlatform GetPlatform() => new UniqueIdentifierService()._concretePlatform;

        private readonly IPlatform _concretePlatform;
        public UniqueIdentifierService()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                _concretePlatform = new Windows();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                _concretePlatform = new Linux();
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                _concretePlatform = new MacOs();
            else throw new NotImplementedException("Platform is not supported");
        }

        public string GetRawDeviceData(List<string> additionalInfo = null)
        {
            return _concretePlatform.GetRawDeviceData(additionalInfo);
        }

        public Task<string> GetRawDeviceDataAsync(List<string> additionalInfo = null)
        {
            return _concretePlatform.GetRawDeviceDataAsync(additionalInfo);
        }

        public string GetSHA1Identifier(List<string> additionalInfo = null)
        {
            return _concretePlatform.GetSHA1Identifier(additionalInfo);
        }

        public Task<string> GetSHA1IdentifierAsync(List<string> additionalInfo = null)
        {
            return _concretePlatform.GetSHA1IdentifierAsync(additionalInfo);
        }

        public string GetSHA1IdentifierFromRawData(string raw, List<string> additionalInfo = null)
        {
            return _concretePlatform.GetSHA1IdentifierFromRawData(raw, additionalInfo); 
        }

        public Task<string> GetSHA1IdentifierFromRawDataAsync(string raw, List<string> additionalInfo = null)
        {
            return _concretePlatform.GetSHA1IdentifierFromRawDataAsync(raw, additionalInfo);
        }
    }
}
