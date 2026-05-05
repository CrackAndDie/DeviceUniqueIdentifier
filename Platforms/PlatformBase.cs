using System;
using System.Linq;
using System.Collections.Generic;
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

        public virtual string GetSHA1Identifier(List<string> additionalInfo = null)
        {
            var dataStr = GetRawDeviceData(additionalInfo);
            return GetSHA1IdentifierFromRawData(dataStr, additionalInfo);
        }

        public virtual string GetSHA1IdentifierFromRawData(string raw, List<string> additionalInfo = null)
        {
            return string.Join("", SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(raw)).Select(b => b.ToString("x2")));
        }
    }
}
