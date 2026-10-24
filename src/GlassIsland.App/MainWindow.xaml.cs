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
    private const double ExpandedHeight = 480;
    private static readonly TimeSpan MotionDuration = TimeSpan.FromMilliseconds(240);

    private readonly Forms.ContextMenuStrip _trayMenu;
    private readonly Forms.NotifyIcon _trayIcon;
    private readonly MediaSessionService _mediaService = new();
    private readonly AudioService _audioService = new();
    private readonly NotificationService _notificationService = new();
    private readonly DispatcherTimer _notificationAccessTimer = new() { Interval = TimeSpan.FromSeconds(30) };
    private bool _updatingAudioSlider;
    private bool _isExpanded;
    private bool _allowClose;
    private bool _disposed;

    public MainWindow()
    {
        InitializeComponent();

        _trayMenu = new Forms.ContextMenuStrip();
        _trayMenu.Items.Add("Show GlassIsland", null, (_, _) => ShowFromTray());
        _trayMenu.Items.Add("Hide", null, (_, _) => Hide());
        _trayMenu.Items.Add(new Forms.ToolStripSeparator());
        _trayMenu.Items.Add("Quit GlassIsland", null, (_, _) => ExitApplication());

        _trayIcon = new Forms.NotifyIcon
        {
            Text = "GlassIsland",
            Icon = SystemIcons.Application,
            ContextMenuStrip = _trayMenu,
            Visible = true
        };
        _trayIcon.DoubleClick += TrayIcon_DoubleClick;

        _mediaService.StateChanged += MediaService_StateChanged;
        _audioService.StateChanged += AudioService_StateChanged;
        _notificationService.StateChanged += NotificationService_StateChanged;

        Loaded += MainWindow_Loaded;
        Activated += MainWindow_Activated;
        _notificationAccessTimer.Tick += NotificationAccessTimer_Tick;
        Closing += MainWindow_Closing;
        SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        PositionWindow();
        Height = CollapsedHeight;
        _audioService.Initialize();
        await _mediaService.InitializeAsync();
        await _notificationService.InitializeAsync(() =>
            System.Windows.MessageBox.Show(
                this,
                "Glass Island can bring your notifications together in one place. Windows will ask whether you want to allow access. You can change this permission later in Windows Settings.",
                "Connect your notifications",
                MessageBoxButton.OK,
                MessageBoxImage.Information));
        _notificationAccessTimer.Start();
    }

    private async void MainWindow_Activated(object? sender, EventArgs e)
    {
        await _notificationService.CheckAccessStatusAsync();
    }

    private async void NotificationAccessTimer_Tick(object? sender, EventArgs e)
    {
        await _notificationService.CheckAccessStatusAsync();
    }

    private void MediaService_StateChanged(MediaSnapshot snapshot)
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

    private void AudioService_StateChanged(AudioSnapshot snapshot)
    {
        if (Dispatcher.HasShutdownStarted)
        {
            return;
        }

        if (Dispatcher.CheckAccess())
        {
            ApplyAudioSnapshot(snapshot);
        }
        else
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ApplyAudioSnapshot(snapshot)));
        }
    }

    private void ApplyAudioSnapshot(AudioSnapshot snapshot)
    {
        _updatingAudioSlider = true;
        try
        {
            AudioVolumeSlider.Value = snapshot.Volume * 100d;
        }
        finally
        {
            _updatingAudioSlider = false;
        }

        AudioVolumeSlider.IsEnabled = snapshot.IsAvailable;
        AudioMuteButton.IsEnabled = snapshot.IsAvailable;
        AudioMuteButton.Content = snapshot.IsMuted ? "Unmute" : "Mute";
        AudioDeviceText.Text = snapshot.IsAvailable ? snapshot.DeviceName : snapshot.Status;
    }

    private void NotificationService_StateChanged(NotificationSnapshot snapshot)
    {
        if (Dispatcher.HasShutdownStarted)
        {
            return;
        }

        if (Dispatcher.CheckAccess())
        {
            ApplyNotificationSnapshot(snapshot);
        }
        else
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => ApplyNotificationSnapshot(snapshot)));
        }
    }

    private void ApplyNotificationSnapshot(NotificationSnapshot snapshot)
    {
        NotificationStatusText.Text = snapshot.Status;
        NotificationCountText.Text = snapshot.HasAccess
            ? snapshot.Items.Count == 1 ? "1 notification" : $"{snapshot.Items.Count} notifications"
            : "Not connected yet";
        NotificationItemsControl.ItemsSource = snapshot.Items;
    }

    private async void AudioVolumeSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (!_updatingAudioSlider)
        {
            await _audioService.SetVolumeAsync((float)(AudioVolumeSlider.Value / 100d));
        }
    }

    private async void AudioVolumeSlider_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key is Key.Left or Key.Right or Key.Up or Key.Down or Key.Home or Key.End or Key.PageUp or Key.PageDown)
        {
            await _audioService.SetVolumeAsync((float)(AudioVolumeSlider.Value / 100d));
        }
    }

    private async void AudioMuteButton_Click(object sender, RoutedEventArgs e)
    {
        if (_audioService.Current.IsAvailable)
        {
            await _audioService.SetMutedAsync(!_audioService.Current.IsMuted);
        }
    }

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
        _audioService.StateChanged -= AudioService_StateChanged;
        _notificationService.StateChanged -= NotificationService_StateChanged;
        Activated -= MainWindow_Activated;
        _notificationAccessTimer.Stop();
        _notificationAccessTimer.Tick -= NotificationAccessTimer_Tick;
        _ = _mediaService.DisposeAsync();
        _audioService.Dispose();
        _notificationService.Dispose();
        SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
        _trayIcon.DoubleClick -= TrayIcon_DoubleClick;
        _trayIcon.Visible = false;
        _trayIcon.Dispose();
        _trayMenu.Dispose();
    }
}
