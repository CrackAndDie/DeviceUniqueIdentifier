using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceUniqueIdentifier.Platforms
{
    internal class Windows : PlatformBase
    {
        public override string GetRawDeviceData(List<string> additionalInfo = null)
        {
            StringBuilder dataStr = new StringBuilder();

            dataStr.Append(GetBaseBoardSerial());
            dataStr.Append(GetBiosSerial());
            dataStr.Append(GetOsSerial());

            return dataStr.ToString();
        }

        // ─── BaseBoard ──────────────────────────────────────────────────────────────

        private string GetBaseBoardSerial()
        {
            try { return GetWmiSerial("Win32_BaseBoard", "SerialNumber"); }
            catch { }
            try { return GetSmbiosSerial(2); }
            catch { }
            return string.Empty;
        }

        // ─── BIOS ───────────────────────────────────────────────────────────────────

        private string GetBiosSerial()
        {
            try { return GetWmiSerial("Win32_BIOS", "SerialNumber"); }
            catch { }
            try { return GetSmbiosSerial(1); }
            catch { }
            return string.Empty;
        }

        // ─── OS ─────────────────────────────────────────────────────────────────────

        private string GetOsSerial()
        {
            try { return GetWmiSerial("Win32_OperatingSystem", "SerialNumber"); }
            catch { }
            try { return GetRegistryOsSerial(); }
            catch { }
            return string.Empty;
        }

        // ─── WMI via reflection ─────────

        private string GetWmiSerial(string wmiClass, string property)
        {
            var searcherType = Type.GetType("System.Management.ManagementObjectSearcher, System.Management");
            if (searcherType == null)
                throw new PlatformNotSupportedException("WMI недоступен");

            using var searcher = (IDisposable)Activator.CreateInstance(searcherType, $"SELECT * FROM {wmiClass}");

            var getMethod = searcherType.GetMethod("Get", Type.EmptyTypes);
            var collection = (System.Collections.IEnumerable)getMethod.Invoke(searcher, null);

            foreach (var obj in collection)
            {
                var properties = obj.GetType().GetProperty("Properties").GetValue(obj);
                var indexer = properties.GetType().GetProperty("Item", new[] { typeof(string) });
                var prop = indexer.GetValue(properties, new object[] { property });
                var value = prop.GetType().GetProperty("Value").GetValue(prop);
                return value?.ToString()?.Trim() ?? string.Empty;
            }

            return string.Empty;
        }

        // ─── SMBIOS via kernel32 ──────────────────────

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern uint GetSystemFirmwareTable(
            uint firmwareTableProviderSignature,
            uint firmwareTableID,
            System.IntPtr pFirmwareTableBuffer,
            uint bufferSize);

        private string GetSmbiosSerial(byte targetType)
        {
            const uint RSMB = 0x52534D42;

            uint size = GetSystemFirmwareTable(RSMB, 0, System.IntPtr.Zero, 0);
            if (size == 0) return string.Empty;

            System.IntPtr buffer = System.Runtime.InteropServices.Marshal.AllocHGlobal((int)size);
            try
            {
                GetSystemFirmwareTable(RSMB, 0, buffer, size);
                byte[] data = new byte[size];
                System.Runtime.InteropServices.Marshal.Copy(buffer, data, 0, (int)size);
                return ParseSmbiosSerial(data, targetType);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(buffer);
            }
        }

        private string ParseSmbiosSerial(byte[] data, byte targetType)
        {
            int offset = 8;
            while (offset + 4 < data.Length)
            {
                byte type = data[offset];
                byte length = data[offset + 1];
                if (length < 4 || offset + length > data.Length) break;

                var strings = new List<string>();
                int pos = offset + length;
                while (pos < data.Length)
                {
                    if (data[pos] == 0)
                    {
                        pos++;
                        break;
                    }
                    int end = pos;
                    while (end < data.Length && data[end] != 0) end++;
                    if (end == pos) break;
                    strings.Add(System.Text.Encoding.ASCII.GetString(data, pos, end - pos));
                    pos = end + 1;
                }

                if (type == targetType && strings.Count > 0)
                {
                    int serialIndex = (targetType == 0 ? data[offset + 8] : data[offset + 7]) - 1;
                    if (serialIndex >= 0 && serialIndex < strings.Count)
                        return strings[serialIndex].Trim();
                }

                int next = offset + length;
                while (next + 1 < data.Length && !(data[next] == 0 && data[next + 1] == 0))
                    next++;
                offset = next + 2;
            }
            return string.Empty;
        }

        // ─── Registry ────────

        private string GetRegistryOsSerial()
        {
            var registryType = Type.GetType("Microsoft.Win32.Registry, mscorlib") ??
                               Type.GetType("Microsoft.Win32.Registry, Microsoft.Win32.Registry");

            if (registryType == null)
                throw new PlatformNotSupportedException("Registry недоступен");

            var lmField = registryType.GetField("LocalMachine",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            var lm = lmField.GetValue(null);

            var openSubKey = lm.GetType().GetMethod("OpenSubKey", new[] { typeof(string) });
            var key = openSubKey.Invoke(lm, new object[] { @"SOFTWARE\Microsoft\Windows NT\CurrentVersion" });

            var getValue = key.GetType().GetMethod("GetValue", new[] { typeof(string) });
            return getValue.Invoke(key, new object[] { "ProductId" })?.ToString()?.Trim() ?? string.Empty;
        }
    }
}
