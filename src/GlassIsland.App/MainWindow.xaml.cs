using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using Microsoft.Win32;
using GlassIsland.App.Services;
using Forms = System.Windows.Forms;

namespace GlassIsland.App;

public partial class MainWindow : Window
{
    private const double CollapsedHeight = 88;
    private const double ExpandedHeight = 264;
    private static readonly TimeSpan MotionDuration = TimeSpan.FromMilliseconds(240);

    private readonly Forms.ContextMenuStrip _trayMenu;
    private readonly Forms.NotifyIcon _trayIcon;
    private readonly MediaSessionService _mediaService = new();
    private bool _isExpanded;
    private bool _allowClose;
    private bool _disposed;

    public MainWindow()
    {
        InitializeComponent();

        _trayMenu = new Forms.ContextMenuStrip();
        _trayMenu.Items.Add("نمایش GlassIsland", null, (_, _) => ShowFromTray());
        _trayMenu.Items.Add("مخفی‌کردن", null, (_, _) => Hide());
        _trayMenu.Items.Add(new Forms.ToolStripSeparator());
        _trayMenu.Items.Add("خروج", null, (_, _) => ExitApplication());

        _trayIcon = new Forms.NotifyIcon
        {
            Text = "GlassIsland",
            Icon = SystemIcons.Application,
            ContextMenuStrip = _trayMenu,
            Visible = true
        };
        _trayIcon.DoubleClick += TrayIcon_DoubleClick;

        _mediaService.StateChanged += MediaService_StateChanged;
        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
        SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        PositionWindow();
        Height = CollapsedHeight;
        await _mediaService.InitializeAsync();
    }

    private void MediaService_StateChanged(object? sender, MediaSnapshot snapshot)
    {
        if (Dispatcher.HasShutdownStarted)
        {
            return;
        }

        if (Dispatcher.CheckAccess())
        {
            ApplyMediaSnapshot(snapshot);
        }
        else
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ApplyMediaSnapshot(snapshot)));
        }
    }

    private void ApplyMediaSnapshot(MediaSnapshot snapshot)
    {
        MediaTitleText.Text = snapshot.Title;
        MediaArtistText.Text = snapshot.Artist;
        MediaPlayPauseButton.Content = snapshot.IsPlaying ? "Ⅱ" : "▶";
        MediaPlayPauseButton.IsEnabled = snapshot.IsAvailable && snapshot.CanTogglePlayback;
        MediaPreviousButton.IsEnabled = snapshot.IsAvailable && snapshot.CanSkipPrevious;
        MediaNextButton.IsEnabled = snapshot.IsAvailable && snapshot.CanSkipNext;
    }

    private async void MediaPlayPause_Click(object sender, RoutedEventArgs e) => await _mediaService.PlayPauseAsync();

    private async void MediaPrevious_Click(object sender, RoutedEventArgs e) => await _mediaService.PreviousAsync();

    private async void MediaNext_Click(object sender, RoutedEventArgs e) => await _mediaService.NextAsync();

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isExpanded)
        {
            Collapse();
        }
        else
        {
            Expand();
        }
    }

    private void Expand()
    {
        _isExpanded = true;
        ExpandedPanel.Visibility = Visibility.Visible;
        Chevron.Text = "⌃";
        AnimatePanelOpacity(ExpandedPanel.Opacity, 1, null);
        AnimateWindowHeight(ExpandedHeight);
    }

    private void Collapse()
    {
        _isExpanded = false;
        Chevron.Text = "⌄";
        AnimatePanelOpacity(ExpandedPanel.Opacity, 0, () =>
        {
            if (!_isExpanded)
            {
                ExpandedPanel.Visibility = Visibility.Collapsed;
            }
        });
        AnimateWindowHeight(CollapsedHeight);
    }

    private void AnimatePanelOpacity(double from, double to, Action? completed)
    {
        var animation = new DoubleAnimation(from, to, MotionDuration)
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        if (completed is not null)
        {
            animation.Completed += (_, _) => completed();
        }

        ExpandedPanel.BeginAnimation(OpacityProperty, animation);
    }

    private void AnimateWindowHeight(double targetHeight)
    {
        var currentHeight = ActualHeight;
        BeginAnimation(HeightProperty, null);
        Height = currentHeight;

        var animation = new DoubleAnimation(currentHeight, targetHeight, MotionDuration)
        {
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
        };
        animation.Completed += (_, _) =>
        {
            BeginAnimation(HeightProperty, null);
            Height = targetHeight;
        };
        BeginAnimation(HeightProperty, animation);
    }

    private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.Escape && _isExpanded)
        {
            Collapse();
            e.Handled = true;
        }
    }

    private void PositionWindow()
    {
        if (!IsLoaded || _disposed)
        {
            return;
        }

        var workArea = Forms.Screen.PrimaryScreen?.WorkingArea;
        if (workArea is null)
        {
            var fallback = SystemParameters.WorkArea;
            Left = fallback.Left + (fallback.Width - Width) / 2;
            Top = fallback.Top + 12;
            return;
        }

        // Screen.WorkingArea is in physical pixels; WPF window coordinates use DIPs.
        var dpi = System.Windows.Media.VisualTreeHelper.GetDpi(this);
        var area = workArea.Value;
        Left = (area.Left + area.Width / 2d) / dpi.DpiScaleX - Width / 2d;
        Top = area.Top / dpi.DpiScaleY + 12;
    }

    private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
    {
        if (_disposed || Dispatcher.HasShutdownStarted)
        {
            return;
        }

        Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(PositionWindow));
    }

    private void TrayIcon_DoubleClick(object? sender, EventArgs e) => ShowFromTray();

    private void ShowFromTray()
    {
        if (!IsVisible)
        {
            ShowActivated = true;
            Show();
            PositionWindow();
            Activate();
            ShowActivated = false;
        }
        else
        {
            Activate();
        }
    }

    private void ExitApplication()
    {
        _allowClose = true;
        Close();
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        if (!_allowClose)
        {
            // Alt+F4 hides the shell to the tray; the tray menu provides an explicit Exit action.
            e.Cancel = true;
            Hide();
            return;
        }

        DisposeResources();
    }

    private void DisposeResources()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _mediaService.StateChanged -= MediaService_StateChanged;
        _ = _mediaService.DisposeAsync();
        SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        _trayIcon.DoubleClick -= TrayIcon_DoubleClick;
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        _trayMenu.Dispose();
    }
}
