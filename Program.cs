using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;

namespace Resonant
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>

        private const string UpdateUrl = "http://github.com/UnLovedCookie/Resonant/releases/latest/download/Resonant.exe";
        private const string VersionInfoUrl = "https://raw.githubusercontent.com/UnLovedCookie/Resonant/refs/heads/main/version";

        [STAThread]
        static void Main()
        {
            CheckForUpdates();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Resonant());
        }

        private static void CheckForUpdates()
        {
            if (NetworkInterface.GetIsNetworkAvailable()) {
                try
                {
                    string currentVersion = GetCurrentVersion();
                    string latestVersion = new WebClient().DownloadString(VersionInfoUrl).Trim();

                    if (new Version(latestVersion) > new Version(currentVersion))
                    {
                        DialogResult result = MessageBox.Show($"Update available: {latestVersion}. Do you want to update?", "Update Available", MessageBoxButtons.YesNo);

                        if (result == DialogResult.Yes)
                        {
                            PerformUpdate();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Update check failed: {ex.Message}", "Error", MessageBoxButtons.OK);
                }
            }
        }

        private static string GetCurrentVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            return version ?? Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.3";
        }

        private static void PerformUpdate()
        {
            try
            {
                string tempPath = Path.Combine(Path.GetTempPath(), "Resonant.exe");
                new WebClient().DownloadFile(UpdateUrl, tempPath);
                                
                string? currentExePath = Environment.ProcessPath;
                if (string.IsNullOrEmpty(currentExePath))
                {
                    throw new InvalidOperationException("Current executable path could not be determined.");
                }

                string updaterScript = Path.Combine(Path.GetTempPath(), "updater.bat");
                File.WriteAllText(updaterScript, $@"
                @echo off
                :waitForExit
                tasklist /fi \""imagename eq {{Path.GetFileName(currentExePath)}}\"" | findstr /i {{Path.GetFileName(currentExePath)}} > nul
                if not errorlevel 1 (
                    timeout /t 1 > nul
                    goto waitForExit
                )
                move /y ""{tempPath}"" ""{currentExePath}""
                start """" ""{currentExePath}""
                del ""%~f0"" > nul
                ");

                Process.Start(new ProcessStartInfo
                {
                    FileName = updaterScript,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Update failed: {ex.Message}", "Error", MessageBoxButtons.OK);
            }
        }
    }
}