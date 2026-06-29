using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Win32;
using Cleaner.Models;

namespace Cleaner.Services
{
    public class CleaningService
    {
        [DllImport("Shell32.dll")]
        private static extern int SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);

        private const uint SHERB_NOCONFIRMATION = 0x00000001;
        private const uint SHERB_NOPROGRESSUI = 0x00000002;
        private const uint SHERB_NOSOUND = 0x00000004;

        public event EventHandler<string>? LogUpdated;

        private void Log(string message)
        {
            LogUpdated?.Invoke(this, $"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        public async Task<long> CalculateSizeAsync(CleaningType type)
        {
            return await Task.Run(() => CalculateSize(type));
        }

        private long CalculateSize(CleaningType type)
        {
            try
            {
                return type switch
                {
                    CleaningType.WindowsTemp => GetDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp")),
                    CleaningType.UserTemp => GetDirectorySize(Path.GetTempPath()),
                    CleaningType.RecycleBin => GetRecycleBinSize(),
                    CleaningType.BrowserCache => GetBrowserCacheSize(),
                    CleaningType.WindowsUpdate => GetDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution", "Download")),
                    CleaningType.NvidiaCache => GetNvidiaCacheSize(),
                    CleaningType.NvidiaInstallers => GetNvidiaInstallersSize(),
                    CleaningType.AmdCache => GetAmdCacheSize(),
                    CleaningType.DirectXCache => GetDirectXCacheSize(),
                    CleaningType.Prefetch => GetDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch")),
                    CleaningType.WindowsStore => GetWindowsStoreSize(),
                    CleaningType.DeliveryOptimization => GetDirectorySize(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution", "DeliveryOptimization")),
                    CleaningType.EventLogs => GetEventLogsSize(),
                    CleaningType.NetworkUsage => GetNetworkUsageSize(),
                    CleaningType.RegistryHistory => 0,
                    CleaningType.RecentFiles => GetRecentFilesSize(),
                    CleaningType.JumpLists => GetJumpListsSize(),
                    _ => 0
                };
            }
            catch
            {
                return 0;
            }
        }

        public async Task CleanAsync(CleaningType type)
        {
            await Task.Run(() => Clean(type));
        }

        private void Clean(CleaningType type)
        {
            try
            {
                switch (type)
                {
                    case CleaningType.WindowsTemp:
                        CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp"));
                        Log("? Windows Temp cleaned");
                        break;

                    case CleaningType.UserTemp:
                        CleanDirectory(Path.GetTempPath());
                        Log("? User Temp cleaned");
                        break;

                    case CleaningType.RecycleBin:
                        SHEmptyRecycleBin(IntPtr.Zero, string.Empty, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
                        Log("? Recycle Bin cleaned");
                        break;

                    case CleaningType.BrowserCache:
                        CleanBrowserCache();
                        break;

                    case CleaningType.WindowsUpdate:
                        CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution", "Download"));
                        Log("? Windows Update cleaned");
                        break;

                    case CleaningType.NvidiaCache:
                        CleanNvidiaCache();
                        break;

                    case CleaningType.NvidiaInstallers:
                        CleanNvidiaInstallers();
                        break;

                    case CleaningType.AmdCache:
                        CleanAmdCache();
                        break;

                    case CleaningType.DirectXCache:
                        CleanDirectXCache();
                        break;

                    case CleaningType.Prefetch:
                        CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Prefetch"));
                        Log("? Prefetch cleaned");
                        break;

                    case CleaningType.WindowsStore:
                        CleanWindowsStore();
                        break;

                    case CleaningType.DeliveryOptimization:
                        CleanDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SoftwareDistribution", "DeliveryOptimization"));
                        Log("? Delivery Optimization cleaned");
                        break;

                    case CleaningType.EventLogs:
                        CleanEventLogs();
                        break;

                    case CleaningType.NetworkUsage:
                        CleanNetworkUsage();
                        break;

                    case CleaningType.RegistryHistory:
                        CleanRegistryHistory();
                        break;

                    case CleaningType.RecentFiles:
                        CleanRecentFiles();
                        break;

                    case CleaningType.JumpLists:
                        CleanJumpLists();
                        break;
                }
            }
            catch (Exception ex)
            {
                Log($"? Error cleaning {type}: {ex.Message}");
            }
        }

        private void CleanDirectory(string path)
        {
            if (!Directory.Exists(path)) return;

            foreach (var file in Directory.GetFiles(path))
            {
                try { File.Delete(file); } catch { }
            }

            foreach (var dir in Directory.GetDirectories(path))
            {
                try { Directory.Delete(dir, true); } catch { }
            }
        }

        private long GetDirectorySize(string path)
        {
            if (!Directory.Exists(path)) return 0;

            long size = 0;
            try
            {
                foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
                {
                    try { size += new FileInfo(file).Length; } catch { }
                }
            }
            catch { }
            return size;
        }

        private long GetRecycleBinSize()
        {
            long size = 0;
            try
            {
                string recycleBin = @"C:\$Recycle.Bin";
                if (Directory.Exists(recycleBin))
                {
                    size = GetDirectorySize(recycleBin);
                }
            }
            catch { }
            return size;
        }

        private long GetBrowserCacheSize()
        {
            long size = 0;
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string[] cachePaths = new[]
            {
                Path.Combine(localAppData, "Google", "Chrome", "User Data", "Default", "Cache"),
                Path.Combine(localAppData, "Google", "Chrome", "User Data", "Default", "Code Cache"),
                Path.Combine(localAppData, "Mozilla", "Firefox", "Profiles"),
                Path.Combine(localAppData, "Microsoft", "Edge", "User Data", "Default", "Cache")
            };

            foreach (var path in cachePaths)
            {
                size += GetDirectorySize(path);
            }

            return size;
        }

        private void CleanBrowserCache()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string[] cachePaths = new[]
            {
                Path.Combine(localAppData, "Google", "Chrome", "User Data", "Default", "Cache"),
                Path.Combine(localAppData, "Google", "Chrome", "User Data", "Default", "Code Cache"),
                Path.Combine(localAppData, "Mozilla", "Firefox", "Profiles"),
                Path.Combine(localAppData, "Microsoft", "Edge", "User Data", "Default", "Cache")
            };

            foreach (var path in cachePaths)
            {
                CleanDirectory(path);
            }

            Log("? Browser cache cleaned");
        }

        private long GetNvidiaCacheSize()
        {
            long size = 0;
            string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string[] nvidiaPaths = new[]
            {
                Path.Combine(programData, "NVIDIA Corporation", "NV_Cache"),
                Path.Combine(localAppData, "NVIDIA", "DXCache"),
                Path.Combine(localAppData, "NVIDIA", "GLCache")
            };

            foreach (var path in nvidiaPaths)
            {
                size += GetDirectorySize(path);
            }

            return size;
        }

        private void CleanNvidiaCache()
        {
            KillNvidiaProcesses();

            string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string[] nvidiaPaths = new[]
            {
                Path.Combine(programData, "NVIDIA Corporation", "NV_Cache"),
                Path.Combine(localAppData, "NVIDIA", "DXCache"),
                Path.Combine(localAppData, "NVIDIA", "GLCache")
            };

            foreach (var path in nvidiaPaths)
            {
                CleanDirectory(path);
            }

            Log("? NVIDIA cache cleaned");
        }

        private long GetNvidiaInstallersSize()
        {
            long size = 0;
            string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string nvidiaPath = Path.Combine(programData, "NVIDIA Corporation", "Downloader");

            if (Directory.Exists(nvidiaPath))
            {
                size = GetDirectorySize(nvidiaPath);
            }

            string cDrive = Path.GetPathRoot(Environment.SystemDirectory) ?? "C:\\";
            string nvidiaInstaller = Path.Combine(cDrive, "NVIDIA");
            if (Directory.Exists(nvidiaInstaller))
            {
                size += GetDirectorySize(nvidiaInstaller);
            }

            return size;
        }

        private void CleanNvidiaInstallers()
        {
            string programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            CleanDirectory(Path.Combine(programData, "NVIDIA Corporation", "Downloader"));

            string cDrive = Path.GetPathRoot(Environment.SystemDirectory) ?? "C:\\";
            string nvidiaPath = Path.Combine(cDrive, "NVIDIA");
            if (Directory.Exists(nvidiaPath))
            {
                CleanDirectory(nvidiaPath);
            }

            Log("? NVIDIA installers cleaned");
        }

        private long GetAmdCacheSize()
        {
            long size = 0;
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string amdPath = Path.Combine(localAppData, "AMD", "DxCache");

            if (Directory.Exists(amdPath))
            {
                size = GetDirectorySize(amdPath);
            }

            return size;
        }

        private void CleanAmdCache()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            CleanDirectory(Path.Combine(localAppData, "AMD", "DxCache"));
            Log("? AMD cache cleaned");
        }

        private long GetDirectXCacheSize()
        {
            long size = 0;
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dxCachePath = Path.Combine(localAppData, "D3DSCache");

            if (Directory.Exists(dxCachePath))
            {
                size = GetDirectorySize(dxCachePath);
            }

            return size;
        }

        private void CleanDirectXCache()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            CleanDirectory(Path.Combine(localAppData, "D3DSCache"));
            Log("? DirectX Shader Cache cleaned");
        }

        private long GetWindowsStoreSize()
        {
            long size = 0;
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string storePath = Path.Combine(localAppData, "Packages", "Microsoft.WindowsStore_8wekyb3d8bbwe", "AC", "INetCache");

            if (Directory.Exists(storePath))
            {
                size = GetDirectorySize(storePath);
            }

            return size;
        }

        private void CleanWindowsStore()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "wsreset.exe",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
                Log("? Windows Store cache cleaning");
            }
            catch (Exception ex)
            {
                Log($"? Error cleaning Windows Store: {ex.Message}");
            }
        }

        private long GetEventLogsSize()
        {
            return 0;
        }

        private void CleanEventLogs()
        {
            try
            {
                string[] logs = { "Application", "System", "Security" };
                foreach (var log in logs)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "wevtutil.exe",
                            Arguments = $"cl {log}",
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            Verb = "runas"
                        })?.WaitForExit();
                    }
                    catch { }
                }
                Log("? Event logs cleaned");
            }
            catch (Exception ex)
            {
                Log($"? Error cleaning event logs: {ex.Message}");
            }
        }

        private long GetNetworkUsageSize()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "sru", "SRUDB.dat");
                if (File.Exists(path))
                {
                    return new FileInfo(path).Length;
                }
            }
            catch { }
            return 0;
        }

        private void CleanNetworkUsage()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "sru", "SRUDB.dat");
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Log("? Network usage statistics cleaned");
                }
            }
            catch (Exception ex)
            {
                Log($"? Error cleaning network statistics: {ex.Message}");
            }
        }

        private void CleanRegistryHistory()
        {
            try
            {
                string[] registryPaths = new[]
                {
                    @"Software\Microsoft\Windows\CurrentVersion\Explorer\ComDlg32\OpenSavePidlMRU",
                    @"Software\Microsoft\Windows\CurrentVersion\Explorer\RunMRU",
                    @"Software\Microsoft\Windows\CurrentVersion\Explorer\TypedPaths"
                };

                foreach (var path in registryPaths)
                {
                    try
                    {
                        using var key = Registry.CurrentUser.OpenSubKey(path, true);
                        if (key != null)
                        {
                            foreach (var valueName in key.GetValueNames())
                            {
                                if (valueName != "MRUList")
                                {
                                    key.DeleteValue(valueName, false);
                                }
                            }
                        }
                    }
                    catch { }
                }

                Log("? Registry history cleaned");
            }
            catch (Exception ex)
            {
                Log($"? Error cleaning registry history: {ex.Message}");
            }
        }

        private long GetRecentFilesSize()
        {
            string recentPath = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
            return GetDirectorySize(recentPath);
        }

        private void CleanRecentFiles()
        {
            string recentPath = Environment.GetFolderPath(Environment.SpecialFolder.Recent);
            CleanDirectory(recentPath);
            Log("? Recent files cleaned");
        }

        private long GetJumpListsSize()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string jumpListPath = Path.Combine(appData, "Microsoft", "Windows", "Recent", "AutomaticDestinations");
            return GetDirectorySize(jumpListPath);
        }

        private void CleanJumpLists()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string jumpListPath = Path.Combine(appData, "Microsoft", "Windows", "Recent", "AutomaticDestinations");
            CleanDirectory(jumpListPath);

            jumpListPath = Path.Combine(appData, "Microsoft", "Windows", "Recent", "CustomDestinations");
            CleanDirectory(jumpListPath);

            Log("? Jump Lists cleaned");
        }

        private void KillNvidiaProcesses()
        {
            try
            {
                foreach (var process in Process.GetProcessesByName("NVIDIA Control Panel"))
                {
                    try { process.Kill(); } catch { }
                }
            }
            catch { }
        }

        public string FormatSize(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
            return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
        }
    }
}
