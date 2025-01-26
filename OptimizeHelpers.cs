using Microsoft.Win32;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ComponentModel;

internal static class OptimizeHelpers
{
    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int LsaOpenPolicy(IntPtr systemName, ref LSA_OBJECT_ATTRIBUTES attributes, int access, out IntPtr policyHandle);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int LsaAddAccountRights(IntPtr policyHandle, byte[] accountSid, LSA_UNICODE_STRING[] userRights, int countOfRights);

    [DllImport("advapi32.dll", SetLastError = true)]
    private static extern int LsaClose(IntPtr policyHandle);

    private struct LSA_OBJECT_ATTRIBUTES { public int Length; public IntPtr RootDirectory, ObjectName, SecurityDescriptor, SecurityQualityOfService; public uint Attributes; }
    private struct LSA_UNICODE_STRING { public ushort Length, MaximumLength; public IntPtr Buffer; }

    private static LSA_UNICODE_STRING CreateLsaString(string s) =>
        new LSA_UNICODE_STRING { Length = (ushort)(s.Length * 2), MaximumLength = (ushort)((s.Length + 1) * 2), Buffer = Marshal.StringToHGlobalUni(s) };

    private static byte[] GetAdminSid()
    {
        var sid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
        byte[] sidArray = new byte[sid.BinaryLength];
        sid.GetBinaryForm(sidArray, 0);
        return sidArray;
    }

    public static void EnableLargePages()
    {
        LSA_OBJECT_ATTRIBUTES attributes = new LSA_OBJECT_ATTRIBUTES();
        IntPtr policyHandle = IntPtr.Zero;
        byte[] adminSid = GetAdminSid();
        var privilege = CreateLsaString("SeLockMemoryPrivilege");

        try
        {
            if (LsaOpenPolicy(IntPtr.Zero, ref attributes, 0x00000002 | 0x00000800 | 0x00000001, out policyHandle) != 0 ||
                LsaAddAccountRights(policyHandle, adminSid, new[] { privilege }, 1) != 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Console.WriteLine("Successfully enabled large pages for Administrators.");
        }
        finally
        {
            if (policyHandle != IntPtr.Zero) LsaClose(policyHandle);
            if (privilege.Buffer != IntPtr.Zero) Marshal.FreeHGlobal(privilege.Buffer);
        }
    }

    public static void RegAdd(string fullKey, string valueName, object value, RegistryValueKind valueKind = RegistryValueKind.None)
    {
        // Split fullKey into rootKey and subKey
        int index = fullKey.IndexOf('\\');
        string subKey = fullKey.Substring(index + 1);
        RegistryKey rootKey = GetRootKey(fullKey.Substring(0, index));

        if (valueKind == RegistryValueKind.None)
            rootKey.CreateSubKey(subKey, true).SetValue(valueName, value);
        else
            rootKey.CreateSubKey(subKey, true).SetValue(valueName, value, valueKind);
    }

    public static void RegDelKey(string fullKey)
    {
        // Split fullKey into rootKey and subKey
        int index = fullKey.IndexOf('\\');
        string subKey = fullKey.Substring(index + 1);
        RegistryKey rootKey = GetRootKey(fullKey.Substring(0, index));

        rootKey.DeleteSubKeyTree(subKey, throwOnMissingSubKey: false);
    }

    public static void RegDelValue(string fullKey, string valueName)
    {
        // Split fullKey into rootKey and subKey
        int index = fullKey.IndexOf('\\');
        string subKey = fullKey.Substring(index + 1);
        RegistryKey rootKey = GetRootKey(fullKey.Substring(0, index));

        rootKey.OpenSubKey(subKey, true)?.DeleteValue(valueName, throwOnMissingValue: false);
    }

    public static string[] GetRegistrySubkeys(string keyPath)
    {
        try
        {
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(keyPath, false);
            return key == null ? throw new Exception("Registry key not found.") : key.GetSubKeyNames();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error accessing registry: " + ex.Message);
            return [];
        }
    }

    private static RegistryKey GetRootKey(string rootKey)
    {
        return rootKey switch
        {
            "HKCR" => Registry.ClassesRoot,
            "HKCU" => Registry.CurrentUser,
            "HKLM" => Registry.LocalMachine,
            "HKU" => Registry.Users,
            "HKCC" => Registry.CurrentConfig,
            _ => Registry.LocalMachine,
        };
    }

    public static string ExecuteCommand(string command, string arguments)
    {
        using (var process = Process.Start(new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }))
        {
            string? output = process?.StandardOutput.ReadToEnd();
            string? error = process?.StandardError.ReadToEnd();
            process?.WaitForExit();
            
            if (!string.IsNullOrEmpty(error))
                output += $"\nError: {error}";

            if (!string.IsNullOrEmpty(output))
                return output;
            else
                return string.Empty;
        }
    }

}