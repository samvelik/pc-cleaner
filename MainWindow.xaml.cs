using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Cleaner.Models;
using Cleaner.Services;

namespace Cleaner
{
    public sealed partial class MainWindow : Window
    {
        private readonly CleaningService _cleaningService;
        private readonly Dictionary<CleaningType, CheckBox> _checkBoxes;
        private readonly Dictionary<CleaningType, TextBlock> _sizeLabels;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"InitializeComponent failed: {ex.Message}");
                throw;
            }
            
            _cleaningService = new CleaningService();
            _cleaningService.LogUpdated += OnLogUpdated;

            _checkBoxes = new Dictionary<CleaningType, CheckBox>
            {
                { CleaningType.WindowsTemp, WindowsTempCheck },
                { CleaningType.UserTemp, UserTempCheck },
                { CleaningType.RecycleBin, RecycleBinCheck },
                { CleaningType.BrowserCache, BrowserCacheCheck },
                { CleaningType.WindowsUpdate, WindowsUpdateCheck },
                { CleaningType.NvidiaCache, NvidiaCacheCheck },
                { CleaningType.NvidiaInstallers, NvidiaInstallersCheck },
                { CleaningType.AmdCache, AmdCacheCheck },
                { CleaningType.DirectXCache, DirectXCacheCheck },
                { CleaningType.Prefetch, PrefetchCheck },
                { CleaningType.WindowsStore, WindowsStoreCheck },
                { CleaningType.DeliveryOptimization, DeliveryOptimizationCheck },
                { CleaningType.EventLogs, EventLogsCheck },
                { CleaningType.NetworkUsage, NetworkUsageCheck },
                { CleaningType.RegistryHistory, RegistryHistoryCheck },
                { CleaningType.RecentFiles, RecentFilesCheck },
                { CleaningType.JumpLists, JumpListsCheck }
            };

            _sizeLabels = new Dictionary<CleaningType, TextBlock>
            {
                { CleaningType.WindowsTemp, WindowsTempSize },
                { CleaningType.UserTemp, UserTempSize },
                { CleaningType.RecycleBin, RecycleBinSize },
                { CleaningType.BrowserCache, BrowserCacheSize },
                { CleaningType.WindowsUpdate, WindowsUpdateSize },
                { CleaningType.NvidiaCache, NvidiaCacheSize },
                { CleaningType.NvidiaInstallers, NvidiaInstallersSize },
                { CleaningType.AmdCache, AmdCacheSize },
                { CleaningType.DirectXCache, DirectXCacheSize },
                { CleaningType.Prefetch, PrefetchSize },
                { CleaningType.WindowsStore, WindowsStoreSize },
                { CleaningType.DeliveryOptimization, DeliveryOptimizationSize },
                { CleaningType.EventLogs, EventLogsSize },
                { CleaningType.NetworkUsage, NetworkUsageSize },
                { CleaningType.RegistryHistory, RegistryHistorySize },
                { CleaningType.RecentFiles, RecentFilesSize },
                { CleaningType.JumpLists, JumpListsSize }
            };

            Title = "PC Cleaner";
            
            this.Closed += MainWindow_Closed;
            
            // Set window icon
            SetWindowIcon();
        }

        private void SetWindowIcon()
        {
            try
            {
                var appWindow = this.AppWindow;
                if (appWindow != null)
                {
                    var iconPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "app.ico");
                    if (System.IO.File.Exists(iconPath))
                    {
                        appWindow.SetIcon(iconPath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to set icon: {ex.Message}");
            }
        }

        private void Button_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var scaleAnimation = new DoubleAnimation
                {
                    To = 0.95,
                    Duration = TimeSpan.FromMilliseconds(100),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                var storyboard = new Storyboard();
                Storyboard.SetTarget(scaleAnimation, button);
                Storyboard.SetTargetProperty(scaleAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
                storyboard.Children.Add(scaleAnimation);

                var scaleAnimationY = new DoubleAnimation
                {
                    To = 0.95,
                    Duration = TimeSpan.FromMilliseconds(100),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(scaleAnimationY, button);
                Storyboard.SetTargetProperty(scaleAnimationY, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
                storyboard.Children.Add(scaleAnimationY);

                storyboard.Begin();
            }
        }

        private void Button_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var scaleAnimation = new DoubleAnimation
                {
                    To = 1.0,
                    Duration = TimeSpan.FromMilliseconds(100),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                var storyboard = new Storyboard();
                Storyboard.SetTarget(scaleAnimation, button);
                Storyboard.SetTargetProperty(scaleAnimation, "(UIElement.RenderTransform).(ScaleTransform.ScaleX)");
                storyboard.Children.Add(scaleAnimation);

                var scaleAnimationY = new DoubleAnimation
                {
                    To = 1.0,
                    Duration = TimeSpan.FromMilliseconds(100),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };

                Storyboard.SetTarget(scaleAnimationY, button);
                Storyboard.SetTargetProperty(scaleAnimationY, "(UIElement.RenderTransform).(ScaleTransform.ScaleY)");
                storyboard.Children.Add(scaleAnimationY);

                storyboard.Begin();
            }
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            _cleaningService.LogUpdated -= OnLogUpdated;
        }

        private void OnLogUpdated(object? sender, string message)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                LogTextBlock.Text += message + Environment.NewLine;
            });
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var checkbox in _checkBoxes.Values)
            {
                checkbox.IsChecked = true;
            }
        }

        private void DeselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var checkbox in _checkBoxes.Values)
            {
                checkbox.IsChecked = false;
            }
        }

        private async void Calculate_Click(object sender, RoutedEventArgs e)
        {
            CleaningProgress.IsActive = true;
            CleanButton.IsEnabled = false;
            CalculateButton.IsEnabled = false;
            SelectAllButton.IsEnabled = false;
            DeselectAllButton.IsEnabled = false;

            foreach (var kvp in _sizeLabels)
            {
                kvp.Value.Text = "...";
            }

            long totalSize = 0;

            var tasks = _checkBoxes.Select(async kvp =>
            {
                var type = kvp.Key;
                var checkbox = kvp.Value;

                if (checkbox.IsChecked == true)
                {
                    var size = await _cleaningService.CalculateSizeAsync(type);
                    
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        _sizeLabels[type].Text = _cleaningService.FormatSize(size);
                    });

                    return size;
                }
                else
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        _sizeLabels[type].Text = "—";
                    });
                    return 0L;
                }
            });

            var sizes = await Task.WhenAll(tasks);
            totalSize = sizes.Sum();

            TotalSizeText.Text = _cleaningService.FormatSize(totalSize);

            CleaningProgress.IsActive = false;
            CleanButton.IsEnabled = true;
            CalculateButton.IsEnabled = true;
            SelectAllButton.IsEnabled = true;
            DeselectAllButton.IsEnabled = true;
        }

        private async void Clean_Click(object sender, RoutedEventArgs e)
        {
            var selectedTypes = _checkBoxes
                .Where(kvp => kvp.Value.IsChecked == true)
                .Select(kvp => kvp.Key)
                .ToList();

            if (!selectedTypes.Any())
            {
                var dialog = new ContentDialog
                {
                    Title = "Warning",
                    Content = "Please select at least one category to clean",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            CleaningProgress.IsActive = true;
            CleanButton.IsEnabled = false;
            CalculateButton.IsEnabled = false;
            SelectAllButton.IsEnabled = false;
            DeselectAllButton.IsEnabled = false;

            LogTextBlock.Text += $"{Environment.NewLine}=== Cleaning started ==={Environment.NewLine}";

            foreach (var type in selectedTypes)
            {
                await _cleaningService.CleanAsync(type);
            }

            LogTextBlock.Text += $"=== Cleaning completed ==={Environment.NewLine}";

            CleaningProgress.IsActive = false;

            // Show success notification with InfoBar
            SuccessInfoBar.IsOpen = true;
            
            // Auto-hide after 5 seconds
            await Task.Delay(5000);
            SuccessInfoBar.IsOpen = false;

            CleanButton.IsEnabled = true;
            CalculateButton.IsEnabled = true;
            SelectAllButton.IsEnabled = true;
            DeselectAllButton.IsEnabled = true;

            // Recalculate sizes
            await Task.Delay(500);
            Calculate_Click(sender, e);
        }

        private void ClearLog_Click(object sender, RoutedEventArgs e)
        {
            LogTextBlock.Text = string.Empty;
        }
    }
}
