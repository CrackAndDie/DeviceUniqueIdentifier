namespace DeviceUniqueIdentifier.Platforms
{
    internal interface IPlatform
    {
        string GetRawDeviceData();
        Task<string> GetRawDeviceDataAsync();
        string GetSHA1Identifier();
        Task<string> GetSHA1IdentifierAsync();
        string GetSHA1IdentifierFromRawData(string raw);
        Task<string> GetSHA1IdentifierFromRawDataAsync(string raw);
    }
}
