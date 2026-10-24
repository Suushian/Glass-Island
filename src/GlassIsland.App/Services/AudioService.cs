using System.Runtime.InteropServices;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace GlassIsland.App.Services;

public sealed record AudioSnapshot(
    bool IsAvailable,
    float Volume,
    bool IsMuted,
    string DeviceName,
    string Status);

/// <summary>
/// Controls the default multimedia render endpoint and listens for system volume/device changes.
/// </summary>
public sealed class AudioService : IDisposable
{
    private readonly object _sync = new();
    private MMDeviceEnumerator? _enumerator;
    private MMDevice? _device;
    private AudioEndpointVolumeNotificationDelegate? _deviceVolumeNotificationHandler;
    private EndpointNotificationClient? _notificationClient;
    private volatile bool _disposed;

    public event Action<AudioSnapshot>? StateChanged;

    public AudioSnapshot Current { get; private set; } =
        new(false, 0, false, "خروجی صدا در دسترس نیست", "Unavailable");

    public void Initialize()
    {
        lock (_sync)
        {
            if (_disposed || _enumerator is not null)
            {
                return;
            }

            try
            {
                _enumerator = new MMDeviceEnumerator();
                _notificationClient = new EndpointNotificationClient(this);
                Marshal.ThrowExceptionForHR(_enumerator.RegisterEndpointNotificationCallback(_notificationClient));
            }
            catch
            {
                try
                {
                    _enumerator?.Dispose();
                }
                catch
                {
                    // Initialization failure must leave the shell usable.
                }

                _enumerator = null;
                _notificationClient = null;
                PublishUnavailable("دسترسی به خروجی صدای ویندوز در دسترس نیست");
                return;
            }
        }

        BindDefaultDevice();
    }

    public Task SetVolumeAsync(float normalizedVolume)
    {
        MMDevice? device;
        lock (_sync)
        {
            if (_disposed)
            {
                return Task.CompletedTask;
            }

            device = _device;
        }

        if (device is null)
        {
            return Task.CompletedTask;
        }

        try
        {
            device.AudioEndpointVolume.MasterVolumeLevelScalar = Math.Clamp(normalizedVolume, 0f, 1f);
            PublishFromDevice(device);
        }
        catch
        {
            // An endpoint may disappear during a write. Do not mark a newly rebound endpoint
            // unavailable because an operation on the previous endpoint failed.
            if (ReferenceEquals(Volatile.Read(ref _device), device))
            {
                PublishUnavailable("تغییر حجم صدا ناموفق بود");
            }
        }

        return Task.CompletedTask;
    }

    public Task SetMutedAsync(bool isMuted)
    {
        MMDevice? device;
        lock (_sync)
        {
            if (_disposed)
            {
                return Task.CompletedTask;
            }

            device = _device;
        }

        if (device is null)
        {
            return Task.CompletedTask;
        }

        try
        {
            device.AudioEndpointVolume.Mute = isMuted;
            PublishFromDevice(device);
        }
        catch
        {
            if (ReferenceEquals(Volatile.Read(ref _device), device))
            {
                PublishUnavailable("تغییر وضعیت قطع صدا ناموفق بود");
            }
        }

        return Task.CompletedTask;
    }

    private void OnVolumeNotification(MMDevice sourceDevice, AudioVolumeNotificationData notification)
    {
        // Core Audio invokes this callback on a COM-owned thread. Do not hold _sync here:
        // Dispose/endpoint rebinding must not block waiting for a callback that is waiting on us.
        if (_disposed || !ReferenceEquals(Volatile.Read(ref _device), sourceDevice))
        {
            return;
        }

        try
        {
            var snapshot = new AudioSnapshot(
                true,
                Math.Clamp(notification.MasterVolume, 0f, 1f),
                notification.Muted,
                sourceDevice.FriendlyName,
                "Available");

            // Ignore notifications that raced with a default-endpoint rebind.
            if (!_disposed && ReferenceEquals(Volatile.Read(ref _device), sourceDevice))
            {
                Publish(snapshot);
            }
        }
        catch
        {
            // Endpoint may have been removed while the callback was in flight.
        }
    }

    private void BindDefaultDevice()
    {
        MMDevice? next = null;
        AudioEndpointVolumeNotificationDelegate? nextHandler = null;
        MMDevice? previous = null;
        AudioEndpointVolumeNotificationDelegate? previousHandler = null;
        AudioSnapshot snapshot;

        lock (_sync)
        {
            if (_disposed || _enumerator is null)
            {
                return;
            }

            try
            {
                next = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                var boundDevice = next;
                nextHandler = notification => OnVolumeNotification(boundDevice, notification);
                boundDevice.AudioEndpointVolume.OnVolumeNotification += nextHandler;
                snapshot = CreateSnapshot(boundDevice);

                previous = _device;
                previousHandler = _deviceVolumeNotificationHandler;
                _device = boundDevice;
                _deviceVolumeNotificationHandler = nextHandler;
                next = null;
                nextHandler = null;
            }
            catch
            {
                DisposeDevice(next, nextHandler);
                next = null;
                nextHandler = null;

                previous = _device;
                previousHandler = _deviceVolumeNotificationHandler;
                _device = null;
                _deviceVolumeNotificationHandler = null;
                snapshot = new AudioSnapshot(false, 0, false,
                    "خروجی صدا در دسترس نیست", "هیچ خروجی صدای پیش‌فرضی پیدا نشد");
            }
        }

        DisposeDevice(previous, previousHandler);
        Publish(snapshot);
    }

    private static AudioSnapshot CreateSnapshot(MMDevice device)
    {
        var endpoint = device.AudioEndpointVolume;
        return new AudioSnapshot(true,
            Math.Clamp(endpoint.MasterVolumeLevelScalar, 0f, 1f),
            endpoint.Mute,
            device.FriendlyName,
            "Available");
    }

    private void PublishFromDevice(MMDevice device)
    {
        try
        {
            var snapshot = CreateSnapshot(device);
            if (!_disposed && ReferenceEquals(Volatile.Read(ref _device), device))
            {
                Publish(snapshot);
            }
        }
        catch
        {
            if (ReferenceEquals(Volatile.Read(ref _device), device))
            {
                PublishUnavailable("وضعیت خروجی صدا در دسترس نیست");
            }
        }
    }

    private void QueueRebind()
    {
        if (!_disposed)
        {
            ThreadPool.QueueUserWorkItem(_ => BindDefaultDevice());
        }
    }

    private void PublishUnavailable(string status) =>
        Publish(new AudioSnapshot(false, 0, false, "خروجی صدا در دسترس نیست", status));

    private void Publish(AudioSnapshot snapshot)
    {
        if (_disposed)
        {
            return;
        }

        Current = snapshot;
        StateChanged?.Invoke(snapshot);
    }

    private static void DisposeDevice(
        MMDevice? device,
        AudioEndpointVolumeNotificationDelegate? notificationHandler)
    {
        if (device is null)
        {
            return;
        }

        try
        {
            if (notificationHandler is not null)
            {
                device.AudioEndpointVolume.OnVolumeNotification -= notificationHandler;
            }
        }
        catch
        {
            // The endpoint may already have been removed.
        }

        try
        {
            device.Dispose();
        }
        catch
        {
            // Best-effort release of a device that may already be disconnected.
        }
    }

    public void Dispose()
    {
        MMDevice? device;
        AudioEndpointVolumeNotificationDelegate? notificationHandler;
        MMDeviceEnumerator? enumerator;
        EndpointNotificationClient? notificationClient;

        lock (_sync)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            device = _device;
            _device = null;
            notificationHandler = _deviceVolumeNotificationHandler;
            _deviceVolumeNotificationHandler = null;
            enumerator = _enumerator;
            _enumerator = null;
            notificationClient = _notificationClient;
            _notificationClient = null;
            StateChanged = null;
        }

        DisposeDevice(device, notificationHandler);

        if (enumerator is not null)
        {
            try
            {
                if (notificationClient is not null)
                {
                    Marshal.ThrowExceptionForHR(enumerator.UnregisterEndpointNotificationCallback(notificationClient));
                }
            }
            catch
            {
                // Continue disposing the enumerator even if callback unregistration fails.
            }

            try
            {
                enumerator.Dispose();
            }
            catch
            {
                // Shutdown must not fail because a Windows audio endpoint disappeared.
            }
        }
    }

    private sealed class EndpointNotificationClient : IMMNotificationClient
    {
        private readonly AudioService _owner;

        public EndpointNotificationClient(AudioService owner) => _owner = owner;

        public void OnDeviceStateChanged(string deviceId, DeviceState newState) => _owner.QueueRebind();
        public void OnDeviceAdded(string pwstrDeviceId) => _owner.QueueRebind();
        public void OnDeviceRemoved(string deviceId) => _owner.QueueRebind();

        public void OnDefaultDeviceChanged(DataFlow flow, Role role, string defaultDeviceId)
        {
            if (flow == DataFlow.Render && role == Role.Multimedia)
            {
                _owner.QueueRebind();
            }
        }

        public void OnPropertyValueChanged(string pwstrDeviceId, PropertyKey key)
        {
        }
    }
}
