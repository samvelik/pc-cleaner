using Microsoft.UI.Xaml;
using System;
using System.IO;
using Windows.Storage;
using Cleaner.Services;

namespace Cleaner
{
    public partial class App : Application
    {
        private Window? _window;

        public App()
        {
            try
            {
                InitializeComponent();
                UnhandledException += App_UnhandledException;
            }
            catch (Exception ex)
            {
                LogError($"App constructor failed: {ex.Message}\n{ex.StackTrace}");
                throw;
            }
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            var message = $"Unhandled exception: {e.Message}\n{e.Exception.StackTrace}";
            System.Diagnostics.Debug.WriteLine(message);
            LogError(message);
            
            // Do not mark as Handled so the real error is visible
            e.Handled = false;
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            try
            {
                _window = new MainWindow();
                _window.Activate();
            }
            catch (Exception ex)
            {
                var message = $"Failed to launch: {ex.Message}\n{ex.StackTrace}";
                System.Diagnostics.Debug.WriteLine(message);
                LogError(message);
                throw;
            }
        }

        private void LogError(string message)
        {
            try
            {
                var logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "CleanerError.log"
                );
                File.AppendAllText(logPath, $"\n[{DateTime.Now}] {message}\n");
            }
            catch { }
        }
    }
}
