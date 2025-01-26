using System.Management;
using Microsoft.Win32;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

using NvAPIWrapper;
using NvAPIWrapper.Native;
using NvAPIWrapper.Native.DRS.Structures;
using NvAPIWrapper.Native.DRS;
using NvAPIWrapper.Native.GPU;
using NvAPIWrapper.Native.GPU.Structures;
using NvAPIWrapper.GPU;

namespace Resonant
{
    internal partial class Optimize
    {
        public static void OptimizeScript()
        {
            // Get Graphics Card
            ManagementObjectSearcher searcher = new("SELECT * FROM Win32_VideoController");
            List<string> graphicsCard = [];
            foreach (ManagementObject obj in searcher.Get())
                foreach (PropertyData prop in obj.Properties)
                    if (prop.Name == "Description")
                    {
                        if (prop.Value.ToString()?.Contains("NVIDIA", StringComparison.CurrentCultureIgnoreCase) == true)
                            graphicsCard.Add("NVIDIA");
                        else if (prop.Value.ToString()?.Contains("INTEL", StringComparison.CurrentCultureIgnoreCase) == true)
                            graphicsCard.Add("INTEL");
                        else if (prop.Value.ToString()?.Contains("AMD", StringComparison.CurrentCultureIgnoreCase) == true)
                            graphicsCard.Add("AMD");
                    }

            if (graphicsCard.Contains("NVIDIA"))
            {
                Dashboard.Instance.UpdateProg("Optimizing Nvidia Settings...");

                // Create Session
                NVIDIA.Initialize();
                DRSSessionHandle hSession = DRSApi.CreateSession();
                DRSApi.LoadSettings(hSession);
                DRSProfileHandle hProfile = DRSApi.GetBaseProfile(hSession);

                // Enable G-Sync
                DRSSettingV1 drsSetting = new(0x1094F157, DRSSettingType.Integer, 1);
                DRSApi.SetSetting(hSession, hProfile, drsSetting);
                drsSetting = new(0x10A879CE, DRSSettingType.Integer, 1);
                DRSApi.SetSetting(hSession, hProfile, drsSetting);

                // Enable Fullscreen & Windowed G-Sync
                drsSetting = new(0x1094F1F7, DRSSettingType.Integer, 2);
                DRSApi.SetSetting(hSession, hProfile, drsSetting);
                drsSetting = new(0x1194F158, DRSSettingType.Integer, 2);
                DRSApi.SetSetting(hSession, hProfile, drsSetting);

                // Allow G-Sync
                drsSetting = new(0x10A879CF, DRSSettingType.Integer, 0);
                DRSApi.SetSetting(hSession, hProfile, drsSetting);
                drsSetting = new(0x10A879AC, DRSSettingType.Integer, 0);
                DRSApi.SetSetting(hSession, hProfile, drsSetting);

                // Enable ReBar
                drsSetting = new(0x000F00BA, DRSSettingType.Integer, 1);
                DRSApi.SetSetting(hSession, hProfile, drsSetting);

                // Disable Force P2 State
                drsSetting = new(0x50166C5E, DRSSettingType.Integer, 0);
                DRSApi.SetSetting(hSession, hProfile, drsSetting);

                // Disallow Ansel
                drsSetting = new(0x1035DB89, DRSSettingType.Integer, 0);
                DRSApi.SetSetting(hSession, hProfile, drsSetting);

                // Texture Filtering Quality: Performance
                drsSetting = new(0x20797D6C, DRSSettingType.Integer, 0xA);
                DRSApi.SetSetting(hSession, hProfile, drsSetting);

                // Clean Session
                DRSApi.SaveSettings(hSession);
                DRSApi.DestroySession(hSession);

                // PhysicalGPU? gpu = PhysicalGPU.GetPhysicalGPUs().FirstOrDefault();
                // PrivatePowerPoliciesInfoV1 PowerPolicies = GPUApi.ClientPowerPoliciesGetInfo(PhysicalGPUHandle.DefaultHandle);

                // Get Nvidia Registry Key
                string keyPath = @"System\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";
                using var baseKey = Registry.LocalMachine.OpenSubKey(keyPath, writable: true);
                if (baseKey == null) return;
                foreach (string subKeyName in baseKey.GetSubKeyNames().Where(name => FourDigitRegex().IsMatch(name)))
                {
                    using var subKey = baseKey.OpenSubKey(subKeyName, writable: true);
                    if (subKey?.GetValueNames().Any(name => subKey.GetValue(name)?.ToString()?.Contains("NVIDIA", StringComparison.CurrentCultureIgnoreCase) == true) == true)
                    {
                        // Disable Power Gating
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "RMElcg", 0x55555555, RegistryValueKind.DWord);
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "RMBlcg", 0x11111111, RegistryValueKind.DWord);
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "RMElpg", 0x00000FFF, RegistryValueKind.DWord);
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "RMSlcg", 0x0003FFFF, RegistryValueKind.DWord);
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "RMFspg", 0x0000000F, RegistryValueKind.DWord);
                    }
                }
            }

            if (graphicsCard.Contains("INTEL"))
            {
                Dashboard.Instance.UpdateProg("Optimizing Intel GPU Settings...");

                // Get iGPU Registry Key
                string keyPath = @"System\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";
                using var baseKey = Registry.LocalMachine.OpenSubKey(keyPath, writable: true);
                if (baseKey == null) return;

                foreach (string subKeyName in baseKey.GetSubKeyNames().Where(name => FourDigitRegex().IsMatch(name)))
                {
                    using var subKey = baseKey.OpenSubKey(subKeyName, writable: true);
                    if (subKey?.GetValueNames().Any(name => subKey.GetValue(name)?.ToString()?.Contains("Intel", StringComparison.CurrentCultureIgnoreCase) == true) == true)
                    {
                        // Disable C-States
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "AllowDeepCStates", 0, RegistryValueKind.DWord);

                        // Optimizations
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "Disable_OverlayDSQualityEnhancement", 1, RegistryValueKind.DWord);
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "IncreaseFixedSegment", 1, RegistryValueKind.DWord);
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "AdaptiveVsyncEnable", 0, RegistryValueKind.DWord);
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "DisablePFonDP", 1, RegistryValueKind.DWord);
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "EnableCompensationForDVI", 1, RegistryValueKind.DWord);
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "NoFastLinkTrainingForeDP", 0, RegistryValueKind.DWord);

                        // Disable Power Throttling
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "ACPowerPolicyVersion", 16898, RegistryValueKind.DWord);
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "DCPowerPolicyVersion", 16642, RegistryValueKind.DWord);

                        // Increase iGPU VRAM
                        OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\GMM", "DedicatedSegmentSize", 1298, RegistryValueKind.DWord);
                    }
                }
            }

            if (graphicsCard.Contains("AMD"))
            {
                Dashboard.Instance.UpdateProg("Optimizing AMD Settings...");

                // Get AMD Registry Key
                string keyPath = @"System\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}";
                try
                {
                    using var baseKey = Registry.LocalMachine.OpenSubKey(keyPath, writable: true);
                    if (baseKey == null) return;

                    foreach (string subKeyName in baseKey.GetSubKeyNames().Where(name => FourDigitRegex().IsMatch(name)))
                    {
                        using var subKey = baseKey.OpenSubKey(subKeyName, writable: true);
                        if (subKey?.GetValueNames().Any(name => subKey.GetValue(name)?.ToString()?.Contains("AMD", StringComparison.CurrentCultureIgnoreCase) == true) == true)
                        {
                            // WIP
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "DisableDrmdmaPowerGating", 1, RegistryValueKind.DWord);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "EnableUlps", 0, RegistryValueKind.DWord);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "DisableDMACopy", 1, RegistryValueKind.DWord);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "DisableBlockWrite", 0, RegistryValueKind.DWord);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "StutterMode", 0, RegistryValueKind.DWord);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "PP_SclkDeepSleepDisable", 1, RegistryValueKind.DWord);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "PP_ThermalAutoThrottlingEnable", 0, RegistryValueKind.DWord);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "KMD_EnableComputePreemption", 0, RegistryValueKind.DWord);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}", "KMD_DeLagEnabled", 0, RegistryValueKind.DWord);

                            // UMD
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "AreaAniso", 0, RegistryValueKind.String);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "AdaptiveAAMethod", 0, RegistryValueKind.String);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "MLF", Convert.FromHexString("3000"), RegistryValueKind.Binary);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "EQAA", Convert.FromHexString("3000"), RegistryValueKind.Binary);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "PowerState", Convert.FromHexString("3000"), RegistryValueKind.Binary);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "AreaAniso_DEF", 0, RegistryValueKind.String);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "ForceTripleBuffering", Convert.FromHexString("3000"), RegistryValueKind.Binary);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "ForceTripleBuffering_DEF", 0, RegistryValueKind.String);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "TextureOpt_DEF", 1, RegistryValueKind.String);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "TextureLod_DEF", 2, RegistryValueKind.String);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "ForceZBufferDepth_DEF", 0, RegistryValueKind.String);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "Tessellation_OPTION_DEF", 2, RegistryValueKind.String);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "Tessellation_DEF", 0, RegistryValueKind.String);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "TrilinearOptimise", Convert.FromHexString("3100"), RegistryValueKind.Binary);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "TextureOpt", Convert.FromHexString("30000000"), RegistryValueKind.Binary);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "Tessellation", Convert.FromHexString("3100"), RegistryValueKind.Binary);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "Tessellation_OPTION", Convert.FromHexString("3200"), RegistryValueKind.Binary);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "ForceZBufferDepth_SET", Convert.FromHexString("3020313620323400"), RegistryValueKind.Binary);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "SurfaceFormatReplacements", 1, RegistryValueKind.String);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "TFQ", Convert.FromHexString("3200"), RegistryValueKind.Binary);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "TFQ_DEF", 1, RegistryValueKind.String);
                            OptimizeHelpers.RegAdd($"HKLM\\{keyPath}\\{subKeyName}\\UMD", "ShaderCache", Convert.FromHexString("3100"), RegistryValueKind.Binary);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred during the AMD optimizations: {ex.Message}",
                        "Optimization Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            // Enable GPU MSI Mode
            // Set Undefined Priority
            // Set Affinity Policy
            foreach (ManagementObject obj in new ManagementObjectSearcher("SELECT PNPDeviceID FROM Win32_VideoController").Get().Cast<ManagementObject>())
            {
                string? pnpDeviceID = obj["PNPDeviceID"]?.ToString();
                if (pnpDeviceID != null && pnpDeviceID.Contains("VEN_"))
                {
                    string basePath = $"HKLM\\SYSTEM\\CurrentControlSet\\Enum\\{pnpDeviceID}\\Device Parameters\\Interrupt Management";
                    OptimizeHelpers.RegAdd($"{basePath}\\MessageSignaledInterruptProperties", "MSISupported", 1);
                    OptimizeHelpers.RegAdd($"{basePath}\\Affinity Policy", "DevicePriority", 0);
                    OptimizeHelpers.RegAdd($"{basePath}\\Affinity Policy", "DevicePolicy", 3);
                }
            }

            // Thread.Sleep(500);
            Dashboard.Instance.UpdateProg("Optimizing the Swapchain...");

            // Remove Potential GameDVR and FSO Overrides
            OptimizeHelpers.RegDelValue(@"HKLM\System\CurrentControlSet\Control\Session Manager\Environment", "__COMPAT_LAYER");
            OptimizeHelpers.RegDelKey(@"HKLM\System\GameConfigStore");
            OptimizeHelpers.RegDelKey(@"HKU\.Default\System\GameConfigStore");
            OptimizeHelpers.RegDelKey(@"HKU\S-1-5-19\System\GameConfigStore");
            OptimizeHelpers.RegDelKey(@"HKU\S-1-5-20\System\GameConfigStore");

            // Re-Enable Multi Plane Overlays (MPO)
            OptimizeHelpers.RegDelValue(@"HKLM\Software\Microsoft\Windows\Dwm", "OverlayTestMode");

            // Disable GameDVR
            OptimizeHelpers.RegAdd(@"HKCU\System\GameConfigStore", "GameDVR_Enabled", 0);
            OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\GameDVR", "AllowGameDVR", 0);
            OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\PolicyManager\default\ApplicationManagement\AllowGameDVR", "value", 0);

            // Disable GameDVR Capture
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\GameDVR", "AudioCaptureEnabled", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\GameDVR", "CursorCaptureEnabled", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\GameDVR", "MicrophoneCaptureEnabled", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\GameDVR", "HistoricalCaptureEnabled", 0);

            // Disable Game Bar Shortcuts
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\GameBar", "UseNexusForGameBarEnabled", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\GameBar", "GamepadDoublePressIntervalMs", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\GameBar", "ShowStartupPanel", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\GameBar", "GamePanelStartupTipIndex", 0);

            // Disable GameDVR FSE Overrides
            OptimizeHelpers.RegAdd(@"HKCU\System\GameConfigStore", "GameDVR_DSEBehavior", 2);
            OptimizeHelpers.RegAdd(@"HKCU\System\GameConfigStore", "GameDVR_FSEBehavior", 2);
            OptimizeHelpers.RegAdd(@"HKCU\System\GameConfigStore", "GameDVR_FSEBehaviorMode", 2);
            OptimizeHelpers.RegAdd(@"HKCU\System\GameConfigStore", "GameDVR_EFSEFeatureFlags", 0);
            OptimizeHelpers.RegAdd(@"HKCU\System\GameConfigStore", "GameDVR_DXGIHonorFSEWindowsCompatible", 1);
            OptimizeHelpers.RegAdd(@"HKCU\System\GameConfigStore", "GameDVR_HonorUserFSEBehaviorMode", 1);

            // Enable VRR and Windowed Optimizations
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\DirectX\UserGpuPreferences", "DirectXUserGlobalSettings", "SwapEffectUpgradeEnable=1;VRROptimizeEnable=1;", RegistryValueKind.String);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\DirectX\GraphicsSettings", "SwapEffectUpgradeCache", 1);

            Thread.Sleep(500);
            Dashboard.Instance.UpdateProg("Optimizing Windows Settings...");

            // Wallpaper Quality
            OptimizeHelpers.RegAdd(@"HKCU\Control Panel\Desktop", "JPEGImportQuality", 100);

            // Enable Hardware Accelerated GPU Scheduling (Requires Restart)
            OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "HwSchMode", 2);
            OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "DisableHWAcceleration", 0);

            // Enable Game Mode
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\GameBar", "AutoGameModeEnabled", 1);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\GameBar", "AllowAutoGameMode", 1);

            // Adjust processor scheduling to allocate processor resources to programs
            // 2A Hex / 42 Dec = Short, Fixed, High foreground boost.
            // 16 Hex / 22 Dec = Long, Variable, High foreground boost.
            OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\PriorityControl", "Win32PrioritySeparation", 22);

            // MMCSS
            OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "Latency Sensitive", "True", RegistryValueKind.String);
            OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "Scheduling Category", "High", RegistryValueKind.String);
            OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "SFIO Priority", "High", RegistryValueKind.String);
            OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "Priority", 8);
            OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile\Tasks\Games", "GPU Priority", 8);
            OptimizeHelpers.RegAdd(@"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "AlwaysOn", 1);
            OptimizeHelpers.RegAdd(@"HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "SystemResponsiveness", 0);

            // Quick Boot
            OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows\CurrentVersion\Policies\System", "DelayedDesktopSwitchTimeout", 5);
            OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows\CurrentVersion\Explorer\Serialize", "StartupDelayInMSec", 0);
            OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows\CurrentVersion\Explorer\Serialize", "WaitforIdleState", 0);
            OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows\CurrentVersion\Policies\System", "RunStartupScriptSync", 0);
            OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows\CurrentVersion\Policies\System", "RunLogonScriptSync", 1);
            OptimizeHelpers.ExecuteCommand("bcdedit", "/set bootuxdisabled on");
            OptimizeHelpers.ExecuteCommand("bcdedit", "/set bootmenupolicy standard");
            OptimizeHelpers.ExecuteCommand("bcdedit", "/set quietboot yes");

            // Quick Shutdown
            // Quickly kill apps during shutdown
            OptimizeHelpers.RegAdd(@"HKCU\Control Panel\Desktop", "WaitToKillAppTimeout", 2000, RegistryValueKind.String);
            // Quickly end services at shutdown
            OptimizeHelpers.RegAdd(@"HKLM\System\CurrentControlSet\Control", "WaitToKillServiceTimeout", 2000, RegistryValueKind.String);
            // Kill apps at shutdown
            OptimizeHelpers.RegAdd(@"HKCU\Control Panel\Desktop", "AutoEndTasks", 1, RegistryValueKind.String);
            // Quickly show hung apps at shutdown
            OptimizeHelpers.RegAdd(@"HKCU\Control Panel\Desktop", "HungAppTimeout", 1000, RegistryValueKind.String);

            // Speed-up Windows
            // Quickly show menus
            OptimizeHelpers.RegAdd(@"HKCU\Control Panel\Desktop", "MenuShowDelay", 20, RegistryValueKind.String);
            // Decrease Mouse Hover Time
            OptimizeHelpers.RegAdd(@"HKCU\Control Panel\Mouse", "MouseHoverTime", 400, RegistryValueKind.String);

            // Disable Background Apps
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications", "GlobalUserDisabled", 1);
            OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\AppPrivacy", "LetAppsRunInBackground", 2);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Search", "BackgroundAppGlobalToggle", 0);

            // Disable Animations
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", 3);
            OptimizeHelpers.RegAdd(@"HKCU\Control Panel\Desktop", "DragFullWindows", 1, RegistryValueKind.String);
            OptimizeHelpers.RegAdd(@"HKCU\Control Panel\Desktop", "FontSmoothing", 2, RegistryValueKind.String);
            OptimizeHelpers.RegAdd(@"HKCU\Control Panel\Desktop\WindowMetrics", "MinAnimate", 1, RegistryValueKind.String);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\DWM", "EnableAeroPeek", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\DWM", "AlwaysHibernateThumbnails", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\DWM", "ListviewShadow", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\DWM", "ColorizationOpaqueBlend", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "IconsOnly", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ListviewAlphaSelect", 1);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "TaskbarAnimations", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "ListviewShadow", 0);
            OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "DisableThumbnails", 0);
            var data = "90,12,03,80,12,00,00,00".Split(',').Select(x => Convert.ToByte(x, 16)).ToArray();
            OptimizeHelpers.RegAdd(@"HKCU\Control Panel\Desktop", "UserPreferencesMask", data, RegistryValueKind.Binary);

            Thread.Sleep(500);
            Dashboard.Instance.UpdateProg("Disabling Telemetry...");

            try
            {
                Console.WriteLine("Applying optimizations and telemetry settings...");

                // Section: Bypass Windows 11 Checks
                OptimizeHelpers.RegAdd(@"HKLM\System\Setup\LabConfig", "BypassTPMCheck", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\System\Setup\LabConfig", "BypassRAMCheck", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\System\Setup\LabConfig", "BypassSecureBootCheck", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\System\Setup\MoSetup", "AllowUpgradesWithUnsupportedTPMOrCPU", 1, RegistryValueKind.DWord);

                // Section: Disable Inventory Collector
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\AppCompat", "DisableInventory", 1, RegistryValueKind.DWord);

                // Section: Disable Windows Error Reporting
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\Windows Error Reporting", "Disabled", 1, RegistryValueKind.DWord);

                // Section: Disable Application Telemetry
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\AppCompat", "AITEnable", 0, RegistryValueKind.DWord);

                // Section: Disable Customer Experience Improvement Program
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Internet Explorer\SQM", "DisableCustomerImprovementProgram", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\SQMClient\Windows", "CEIPEnable", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\AppV\CEIP", "CEIPEnable", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Messenger\Client", "CEIP", 2, RegistryValueKind.DWord);

                // Section: Disable Telemetry
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\MSDeploy\3", "EnableTelemetry", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\DataCollection", "MaxTelemetryAllowed", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\DataCollection", "DisableTelemetryOptInChangeNotification", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\DataCollection", "DisableTelemetryOptInSettingsUx", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\DataCollection", "AllowCommercialDataPipeline", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\DataCollection", "AllowDeviceNameInTelemetry", 0, RegistryValueKind.DWord);

                // Section: Disable Desktop Analytics
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\DataCollection", "DisableEnterpriseAuthProxy", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\DataCollection", "AllowDesktopAnalyticsProcessing", 0, RegistryValueKind.DWord);

                // Section: Disable Edge Telemetry
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\DataCollection", "MicrosoftEdgeDataOptIn", 0, RegistryValueKind.DWord);

                // Section: Disable Diagnostics
                OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack", "ShowedToastAtLevel", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack", "DiagTrackAuthorization", 775, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack", "DiagTrackStatus", 2, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack", "UploadPermissionReceived", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack\TraceManager", "MiniTraceSlotContentPermitted", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows\CurrentVersion\Diagnostics\DiagTrack\TraceManager", "MiniTraceSlotEnabled", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Privacy", "TailoredExperiencesWithDiagnosticDataEnabled", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKCU\Software\Policies\Microsoft\Windows\CloudContent", "disabletailoredexperiencesWithDiagnosticData", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\DataCollection", "DisableDiagnosticDataViewer", 1, RegistryValueKind.DWord);

                // Section: Disable Text/Ink/Handwriting Telemetry
                OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Input\TIPC", "Enabled", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\InputPersonalization", "RestrictImplicitTextCollection", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\InputPersonalization", "RestrictImplicitInkCollection", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\TabletPC", "PreventHandwritingDataSharing", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\HandwritingErrorReports", "PreventHandwritingErrorReports", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Personalization\Settings", "AcceptedPrivacyPolicy", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Microsoft\Windows\CurrentVersion\Policies\TextInput", "AllowLinguisticDataCollection", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\InputPersonalization\TrainedDataStore", "HarvestContacts", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\InputPersonalization\TrainedDataStore", "InsightsEnabled", 0, RegistryValueKind.DWord);

                // Section: Disable Advertising ID
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\AdvertisingInfo", "DisabledByGroupPolicy", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled", 0, RegistryValueKind.DWord);

                // Section: Disable Tagged Energy Logging
                OptimizeHelpers.RegAdd(@"HKLM\System\CurrentControlSet\Control\Power\EnergyEstimation\TaggedEnergy", "DisableTaggedEnergyLogging", 1, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\System\CurrentControlSet\Control\Power\EnergyEstimation\TaggedEnergy", "TelemetryMaxApplication", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\System\CurrentControlSet\Control\Power\EnergyEstimation\TaggedEnergy", "TelemetryMaxTagPerApplication", 0, RegistryValueKind.DWord);

                // Section: Disable Automatic Installation of Suggested Windows 11 Apps
                OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SilentInstalledAppsEnabled", 0, RegistryValueKind.DWord);

                // Section: Disable Background Map Updates
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\Maps", "AutoDownloadAndUpdateMapData", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows\Maps", "AllowUntriggeredNetworkTrafficOnSettingsPage", 0, RegistryValueKind.DWord);
                // OptimizeHelpers.ExecuteCommand("sc", "config MapsBroker start=disabled");

                // Section: Disable Cortana
                OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Search", "CortanaEnabled", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Search", "CortanaConsent", 0, RegistryValueKind.DWord);
                OptimizeHelpers.RegAdd(@"HKCU\Software\Microsoft\Windows\CurrentVersion\Search", "AllowCortana", 0, RegistryValueKind.DWord);

                // Section: Disable Biometrics
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Biometrics", "Enabled", 0, RegistryValueKind.DWord);
                // OptimizeHelpers.ExecuteCommand("sc", "config WbioSrvc start=disabled");

                // Section: Disable .NET CLI and Powershell Telemetry
                OptimizeHelpers.ExecuteCommand("setx", "DOTNET_CLI_TELEMETRY_OPTOUT 1");
                OptimizeHelpers.ExecuteCommand("setx", "POWERSHELL_TELEMETRY_OPTOUT 1");

                // Section: Disable Key Management System Telemetry
                OptimizeHelpers.RegAdd(@"HKLM\Software\Policies\Microsoft\Windows NT\CurrentVersion\Software Protection Platform", "NoGenTicket", 1, RegistryValueKind.DWord);

                // Section: Disable Usage Statistics
                OptimizeHelpers.ExecuteCommand("schtasks", "/change /tn \"\\Microsoft\\Windows\\Feedback\\Siuf\\DmClient\" /disable");

                string hostsPath = @"C:\Windows\System32\drivers\etc\hosts";
                string backupPath = @"C:\Windows\System32\drivers\etc\hosts.bak";
                string downloadUrl = "https://winhelp2002.mvps.org/hosts.txt";

                try
                {
                    if (NetworkInterface.GetIsNetworkAvailable())
                    {
                        // Backup existing hosts file if not already backed up
                        if (File.Exists(hostsPath) && !File.Exists(backupPath))
                            File.Move(hostsPath, backupPath);
                        else if (File.Exists(hostsPath))
                            File.Delete(hostsPath);

                        // Download new hosts file
                        using WebClient client = new();
                        client.DownloadFile(downloadUrl, hostsPath);
                    }
                }
                catch (Exception ex)
                {
                    if (!File.Exists(hostsPath) && File.Exists(backupPath))
                        File.Move(backupPath, hostsPath);
                    MessageBox.Show($"An error occurred during updating the hosts file: {ex.Message}",
                        "Optimization Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during disabling telemetry: {ex.Message}",
                    "Optimization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            // Thread.Sleep(500);
            Dashboard.Instance.UpdateProg("Optimizing Power Settings...");

            string schemeGuid = "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee";

            // Restore Power Schemes
            OptimizeHelpers.RegAdd(@"HKLM\System\CurrentControlSet\Control\Power", "PlatformAoAcOverride", 0);
            OptimizeHelpers.RegAdd(@"HKLM\System\CurrentControlSet\Control\Power", "CsEnabled", 0);

            // Remove Potential Override
            OptimizeHelpers.ExecuteCommand("powercfg", $"/D {schemeGuid}");

            // Import Ultimate Performance Power Plan
            OptimizeHelpers.ExecuteCommand("powercfg", $"/duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61 {schemeGuid}");

            // Disable Throttle States
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_processor THROTTLING 0");

            // Device Idle Policy: Performance
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_none DEVICEIDLE 0");

            // Enable Hardware P-states
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_processor PERFAUTONOMOUS 1");
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_processor PERFAUTONOMOUSWINDOW 30000");

            // Disable Hardware P-states Energy Saving
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_processor PERFEPP 0");

            // Enable Aggressive Turbo Boost
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_processor PERFBOOSTMODE 2");
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_processor PERFBOOSTPOL 100");

            // Disable Sleep States
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_SLEEP AWAYMODE 0");
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_SLEEP ALLOWSTANDBY 0");
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_SLEEP HYBRIDSLEEP 0");
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_SLEEP UNATTENDSLEEP 0");
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_IR DEEPSLEEP 0");

            // Disable Frequency Scaling
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_processor PROCTHROTTLEMIN 100");

            // Prefer Performant Processors
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_processor SHORTSCHEDPOLICY 2");
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_processor SCHEDPOLICY 2");

            // Don't turn off display when plugged in
            OptimizeHelpers.ExecuteCommand("powercfg", $"/change standby-timeout-ac 0");
            OptimizeHelpers.ExecuteCommand("powercfg", $"/change monitor-timeout-ac 0");
            OptimizeHelpers.ExecuteCommand("powercfg", $"/change hibernate-timeout-ac 0");

            // Determine Processor
            Console.WriteLine(Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER"));

            if (Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")?.Contains("Intel", StringComparison.OrdinalIgnoreCase) == true)
            {
                // Disable Core Parking
                OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_processor CPMINCORES 100");
                OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_processor CPMINCORES1 100");
                OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} sub_processor PERFCHECK 1000");
                // Interrupt Steering: Processor 1
                OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_INTSTEER MODE 6");
            }
            else
            {
                // Disable Core Parking
                OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_INTSTEER UNPARKTIME 0");
                OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_INTSTEER PERPROCLOAD 10000");
            }

            // Disable Selective USB Suspension
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} 2a737441-1930-4402-8d77-b2bebba308a3 d4e98f31-5ffe-4ce1-be31-1b38b384c009 0");
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} 2a737441-1930-4402-8d77-b2bebba308a3 48e6b7a6-50f5-4782-a5d4-53bb8f07e226 0");
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} 2a737441-1930-4402-8d77-b2bebba308a3 0853a681-27c8-4100-a2fd-82013e970683 0");

            // Name Power Plan
            OptimizeHelpers.ExecuteCommand("powercfg", $"-changename {schemeGuid} \"Resonant Ultimate Performance\" \"For Resonant (dsc.gg/resonant) By UnLovedCookie\"");

            // Apply Power Plan
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setactive {schemeGuid}");

            // Enable Hyberboot/Fast Startup
            OptimizeHelpers.ExecuteCommand("powercfg", $"-h on");
            OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\Power", "HibernateEnabled", 1);
            OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\Power", "HiberbootEnabled", 1);

            // Disable GpuEnergyDrv
            OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Services\GpuEnergyDrv", "Start", 4);

            // Enable PPM Driver
            OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Services\AmdPPM", "Start", 3);
            OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Services\IntelPPM", "Start", 3);

            // Disable Power Throttling
            OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\Power\PowerThrottling", "PowerThrottlingOff", 1);

            Dashboard.Instance.UpdateProg("Optimizing Memory...");

            // Enable Large Pages
            OptimizeHelpers.EnableLargePages();

            // Disk
            searcher = new("SELECT * FROM Win32_OperatingSystem");
            UInt64 totalMemory = 0;
            foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
                totalMemory = Convert.ToUInt64(obj.Properties["TotalVisibleMemorySize"].Value);

            // IOPageLockLimit
            OptimizeHelpers.RegAdd(@"HKLM\System\CurrentControlSet\Control\Session Manager\Memory Management", "SvcHostSplitThresholdInKB", Convert.ToUInt64(totalMemory), RegistryValueKind.QWord);

            // SvcSplitThreshold
            OptimizeHelpers.RegAdd(@"HKLM\System\CurrentControlSet\Control", "IOPageLockLimit", Convert.ToUInt64(totalMemory * 128), RegistryValueKind.QWord);

            // Increase Decommitting Memory Threshold
            OptimizeHelpers.RegAdd(@"HKLM\System\CurrentControlSet\Control\Session Manager", "HeapDeCommitFreeBlockThreshold", 262144);

            // Raise the limit of paged pool memory
            OptimizeHelpers.ExecuteCommand("fsutil", "behavior set memoryusage 2");

            // Optimize The Mft Zone
            OptimizeHelpers.ExecuteCommand("fsutil", "behavior set mftzone 2");

            // Enable Trim
            OptimizeHelpers.ExecuteCommand("fsutil", "behavior set disabledeletenotify 0");

            // Disable Page File Encryption
            OptimizeHelpers.ExecuteCommand("fsutil", "behavior set encryptpagingfile 0");

            // Disable 8dot3
            OptimizeHelpers.ExecuteCommand("fsutil", "behavior set disable8dot3 1");
            OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\FileSystem", "NtfsDisable8dot3NameCreation", 1);

            // Disable NTFS Compression
            OptimizeHelpers.ExecuteCommand("fsutil", "behavior set disablecompression 1");

            // Disable Last Access
            OptimizeHelpers.ExecuteCommand("fsutil", "behavior set disableLastAccess 1");
            OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\FileSystem", "NtfsDisableLastAccessUpdate", 2147483649, RegistryValueKind.QWord);

            // Get Storage Device Type
            bool isSSD = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk WHERE DeviceID = 'C:' AND DriveType = 3").Get().Count > 0;
            if (isSSD)
            {
                // Enable Large System Cache
                OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "LargeSystemCache", 1);

                // Disable Superfetch
                OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters", "EnableSuperfetch", 0);
                OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters", "EnablePrefetcher", 0);
            }
            else
            {
                // Disable Large System Cache
                OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "LargeSystemCache", 0);

                // Enable Superfetch
                OptimizeHelpers.RegDelValue(@"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\PrefetchParameters", "EnableSuperfetch");
            }

            // Disable Storage Device Idle
            OptimizeHelpers.RegAdd(@"HKLM\System\CurrentControlSet\Services\stornvme\Parameters\Device", "IdlePowerMode", 0);

            // Disable NVME Power Savings
            // NVMe Power State Transition Latency Tolerance: 0
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_DISK dbc9e238-6de9-49e3-92cd-8c2b4946b472 0");
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_DISK fc95af4d-40e7-4b6d-835a-56d131dbc80e 0");
            // Disable NVMe Idle Timeout
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_DISK d3d55efd-c1ff-424e-9dc3-441be7833010 0");
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_DISK d639518a-e56d-4345-8af2-b9f32fb26109 0");
            // Enable NVME NOPPME
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_DISK DISKNVMENOPPME 1");

            // Disable Link State Power Management
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_PCIEXPRESS ASPM 0");

            // Disable AHCI Link Power Management
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_DISK 0b2d69d7-a2a1-449c-9680-f91c70521c60 0");
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setacvalueindex {schemeGuid} SUB_DISK dab60367-53fe-4fbc-825e-521d069d2456 0");

            // Apply Changes
            OptimizeHelpers.ExecuteCommand("powercfg", $"-setactive {schemeGuid}");

            Dashboard.Instance.UpdateProg("Optimizating Kernel...");

            // Disable HPET (Stock)
            OptimizeHelpers.ExecuteCommand("bcdedit", "/deletevalue useplatformclock");

            // Disable Synthetic Timers
            OptimizeHelpers.ExecuteCommand("bcdedit", "/set useplatformtick yes");

            // Disable Dynamic Tick
            OptimizeHelpers.ExecuteCommand("bcdedit", "/set disabledynamictick yes");
            OptimizeHelpers.RegAdd(@"HKLM\System\CurrentControlSet\Control\Session Manager\kernel", "MaxDynamicTickDuration", 1000);

            // Enable x2APIC
            OptimizeHelpers.ExecuteCommand("bcdedit", "/set x2apicpolicy enable");
            OptimizeHelpers.ExecuteCommand("bcdedit", "/set uselegacyapicmode no");

            // Enable Physical Address Extension (PAE)
            OptimizeHelpers.ExecuteCommand("bcdedit", "/set pae ForceEnable");

            // Disable Kernel Paging
            OptimizeHelpers.RegAdd(@"HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "DisablePagingExecutive", 1);

            Thread.Sleep(500);
            Dashboard.Instance.UpdateProg("Optimizations Complete");
        }

        [GeneratedRegex(@"^\d{4}$")]
        private static partial Regex FourDigitRegex();
    }
}