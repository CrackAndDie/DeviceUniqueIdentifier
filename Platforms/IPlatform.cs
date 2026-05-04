namespace DeviceUniqueIdentifier.Platforms
{
    public interface IPlatform
    {
        string GetRawDeviceData(List<string> additionalInfo = null);
        Task<string> GetRawDeviceDataAsync(List<string> additionalInfo = null);
        string GetSHA1Identifier(List<string> additionalInfo = null);
        Task<string> GetSHA1IdentifierAsync(List<string> additionalInfo = null);
        string GetSHA1IdentifierFromRawData(string raw, List<string> additionalInfo = null);
        Task<string> GetSHA1IdentifierFromRawDataAsync(string raw, List<string> additionalInfo = null);
    }
}
