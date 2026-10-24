using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;

namespace GlassIsland.App.Services;

public sealed record NotificationItem(
    uint Id,
    string AppName,
    DateTimeOffset CreatedAt,
    string Title,
    string Body);

public sealed record NotificationSnapshot(
    bool HasAccess,
    string Status,
    IReadOnlyList<NotificationItem> Items);

/// <summary>
/// Reads Windows toast notifications through the documented UserNotificationListener API.
/// Requires package identity and the userNotificationListener manifest capability.
/// Notification content is kept in memory only and is never logged or persisted.
/// </summary>
public sealed class NotificationService : IDisposable
{
    private readonly SemaphoreSlim _refreshGate = new(1, 1);
    private UserNotificationListener? _listener;
    private bool _disposed;

    public event Action<NotificationSnapshot>? StateChanged;

    public NotificationSnapshot Current { get; private set; } =
        new(false, "Notifications aren't connected yet.", Array.Empty<NotificationItem>());

    public async Task InitializeAsync(Action? beforeAccessRequest = null)
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            _listener = UserNotificationListener.Current;
            var access = _listener.GetAccessStatus();

            if (access == UserNotificationListenerAccessStatus.Unspecified)
            {
                // Explain the request before Windows shows its system permission dialog.
                // This is informational only; Windows remains the sole permission gate.
                beforeAccessRequest?.Invoke();

                // This method is called by the WPF window on its UI thread. Windows requires
                // RequestAccessAsync to be invoked from the UI thread.
                access = await _listener.RequestAccessAsync();
            }

            if (access != UserNotificationListenerAccessStatus.Allowed)
            {
                Publish(new NotificationSnapshot(
                    false,
                    access == UserNotificationListenerAccessStatus.Denied
                        ? "Notification access is off. You can turn it on in Windows Settings."
                        : "Windows hasn't granted notification access yet.",
                    Array.Empty<NotificationItem>()));
                return;
            }

            _listener.NotificationChanged += Listener_NotificationChanged;
            await RefreshAsync();
        }
        catch (COMException)
        {
            Publish(new NotificationSnapshot(
                false,
                "Notifications need a packaged Windows build with the notification capability enabled.",
                Array.Empty<NotificationItem>()));
        }
        catch
        {
            Publish(new NotificationSnapshot(
                false,
                "We couldn't connect to Windows notifications. GlassIsland will keep running.",
                Array.Empty<NotificationItem>()));
        }
    }

    private async void Listener_NotificationChanged(
        UserNotificationListener sender,
        UserNotificationChangedEventArgs args)
    {
        await RefreshAsync();
    }

    /// <summary>
    /// Re-checks the permission state without polling notification contents.
    /// This lets the UI clear previously displayed notification text after access is revoked.
    /// </summary>
    public async Task CheckAccessStatusAsync()
    {
        var listener = _listener;
        if (_disposed || listener is null)
        {
            return;
        }

        try
        {
            var access = listener.GetAccessStatus();
            if (access != UserNotificationListenerAccessStatus.Allowed)
            {
                Publish(new NotificationSnapshot(
                    false,
                    access == UserNotificationListenerAccessStatus.Denied
                        ? "Notification access is off. You can turn it on in Windows Settings."
                        : "Windows hasn't granted notification access yet.",
                    Array.Empty<NotificationItem>()));
                return;
            }

            if (!Current.HasAccess)
            {
                await RefreshAsync();
            }
        }
        catch (COMException)
        {
            Publish(new NotificationSnapshot(
                false,
                "Notification access is unavailable. Check Windows Settings.",
                Array.Empty<NotificationItem>()));
        }
        catch
        {
            Publish(new NotificationSnapshot(
                false,
                "We couldn't check notification access just now.",
                Array.Empty<NotificationItem>()));
        }
    }

    private async Task RefreshAsync()
    {
        var listener = _listener;
        if (_disposed || listener is null)
        {
            return;
        }

        await _refreshGate.WaitAsync();
        try
        {
            if (_disposed || !ReferenceEquals(listener, _listener))
            {
                return;
            }

            if (listener.GetAccessStatus() != UserNotificationListenerAccessStatus.Allowed)
            {
                Publish(new NotificationSnapshot(
                    false,
                    "Notification access is no longer available. Check Windows Settings.",
                    Array.Empty<NotificationItem>()));
                return;
            }

            var notifications = await listener.GetNotificationsAsync(NotificationKinds.Toast);
            var items = new List<NotificationItem>(notifications.Count);

            foreach (var notification in notifications)
            {
                var appName = "An app";
                var title = "Notification";
                var body = string.Empty;

                try
                {
                    var binding = notification.Notification.Visual.GetBinding(KnownNotificationBindings.ToastGeneric);
                    var textElements = binding?.GetTextElements();
                    var texts = textElements?
                        .Select(element => element.Text?.Trim())
                        .Where(text => !string.IsNullOrWhiteSpace(text))
                        .Cast<string>()
                        .ToArray() ?? Array.Empty<string>();

                    var displayName = notification.AppInfo?.DisplayInfo?.DisplayName;
                    if (!string.IsNullOrWhiteSpace(displayName))
                    {
                        appName = displayName;
                    }

                    title = texts.FirstOrDefault() ?? "Notification";
                    body = texts.Length > 1 ? string.Join(Environment.NewLine, texts.Skip(1)) : string.Empty;
                }
                catch
                {
                    // Preserve a safe fallback for this item without dropping other notifications.
                }

                items.Add(new NotificationItem(
                    notification.Id,
                    appName,
                    notification.CreationTime,
                    title,
                    body));
            }

            Publish(new NotificationSnapshot(
                true,
                items.Count == 0 ? "You're all caught up. No notifications right now." : $"{items.Count} notification(s)",
                items.OrderByDescending(item => item.CreatedAt).ToArray()));
        }
        catch (COMException)
        {
            Publish(new NotificationSnapshot(
                false,
                "Notification access is unavailable. Check Windows Settings.",
                Array.Empty<NotificationItem>()));
        }
        catch
        {
            Publish(new NotificationSnapshot(
                false,
                "We couldn't refresh notifications just now.",
                Array.Empty<NotificationItem>()));
        }
        finally
        {
            _refreshGate.Release();
        }
    }

    private void Publish(NotificationSnapshot snapshot)
    {
        if (_disposed)
        {
            return;
        }

        Current = snapshot;
        StateChanged?.Invoke(snapshot);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        if (_listener is not null)
        {
            _listener.NotificationChanged -= Listener_NotificationChanged;
            _listener = null;
        }

        Current = new(false, "Notifications are disconnected.", Array.Empty<NotificationItem>());
        StateChanged = null;
    }
}
