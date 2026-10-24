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
    private EndpointNotificationClient? _notificationClient;
    private bool _disposed;

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
                _enumerator.RegisterEndpointNotificationCallback(_notificationClient);
            }
            catch
            {
                PublishUnavailable("دسترسی به خروجی صدای ویندوز در دسترس نیست");
                return;
            }
        }

        BindDefaultDevice();
    }

    public Task SetVolumeAsync(float normalizedVolume)
    {
        lock (_sync)
        {
            if (_disposed || _device is null)
            {
                return Task.CompletedTask;
            }

            try
            {
                _device.AudioEndpointVolume.MasterVolumeLevelScalar = Math.Clamp(normalizedVolume, 0f, 1f);
                PublishFromDevice(_device);
            }
            catch
            {
                PublishUnavailable("تغییر حجم صدا ناموفق بود");
            }
        }

        return Task.CompletedTask;
    }

    public Task SetMutedAsync(bool isMuted)
    {
        lock (_sync)
        {
            if (_disposed || _device is null)
            {
                return Task.CompletedTask;
            }

            try
            {
                _device.AudioEndpointVolume.Mute = isMuted;
                PublishFromDevice(_device);
            }
            catch
            {
                PublishUnavailable("تغییر وضعیت قطع صدا ناموفق بود");
            }
        }

        return Task.CompletedTask;
    }

    private void OnVolumeNotification(AudioVolumeNotificationData notification)
    {
        lock (_sync)
        {
            if (_disposed || _device is null)
            {
                return;
            }

            Publish(new AudioSnapshot(true, Math.Clamp(notification.MasterVolume, 0f, 1f),
                notification.Muted, _device.FriendlyName, "Available"));
        }
    }

    private void BindDefaultDevice()
    {
        lock (_sync)
        {
            if (_disposed || _enumerator is null)
            {
                return;
            }

            MMDevice? next = null;
            try
            {
                next = _enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                if (_device is not null)
                {
                    _device.AudioEndpointVolume.OnVolumeNotification -= OnVolumeNotification;
                    _device.Dispose();
                }

                _device = next;
                next = null;
                _device.AudioEndpointVolume.OnVolumeNotification += OnVolumeNotification;
                PublishFromDevice(_device);
            }
            catch
            {
                next?.Dispose();
                if (_device is not null)
                {
                    _device.AudioEndpointVolume.OnVolumeNotification -= OnVolumeNotification;
                    _device.Dispose();
                    _device = null;
                }

                PublishUnavailable("هیچ خروجی صدای پیش‌فرضی پیدا نشد");
            }
        }
    }

    private void PublishFromDevice(MMDevice device)
    {
        var endpoint = device.AudioEndpointVolume;
        Publish(new AudioSnapshot(true,
            Math.Clamp(endpoint.MasterVolumeLevelScalar, 0f, 1f),
            endpoint.Mute,
            device.FriendlyName,
            "Available"));
    }

    private void QueueRebind()
    {
        ThreadPool.QueueUserWorkItem(_ => BindDefaultDevice());
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

    public void Dispose()
    {
        lock (_sync)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (_device is not null)
            {
                _device.AudioEndpointVolume.OnVolumeNotification -= OnVolumeNotification;
                _device.Dispose();
                _device = null;
            }

            if (_enumerator is not null)
            {
                if (_notificationClient is not null)
                {
                    _enumerator.UnregisterEndpointNotificationCallback(_notificationClient);
                }

                _enumerator.Dispose();
                _enumerator = null;
            }

            _notificationClient = null;
            StateChanged = null;
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
