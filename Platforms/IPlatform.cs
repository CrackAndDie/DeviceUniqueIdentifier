using System.Collections.Generic;
using System.Threading.Tasks;

namespace DeviceUniqueIdentifier.Platforms
{
    public interface IPlatform
    {
        string GetRawDeviceData(List<string> additionalInfo = null);
        string GetSHA1Identifier(List<string> additionalInfo = null);
        string GetSHA1IdentifierFromRawData(string raw, List<string> additionalInfo = null);
    }
}
