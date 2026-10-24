using Windows.Media.Control;

namespace GlassIsland.App.Services;

public sealed record MediaSnapshot(
    string Title,
    string Artist,
    bool IsPlaying,
    bool IsAvailable,
    bool CanTogglePlayback,
    bool CanSkipNext,
    bool CanSkipPrevious,
    string Status);

/// <summary>
/// Event-driven adapter for the active Windows Global System Media Transport Controls session.
/// Metadata and transport support depend on the media player.
/// </summary>
public sealed class MediaSessionService : IAsyncDisposable
{
    private readonly SemaphoreSlim _bindGate = new(1, 1);
    private GlobalSystemMediaTransportControlsSessionManager? _manager;
    private GlobalSystemMediaTransportControlsSession? _session;
    private bool _disposed;

    public event Action<MediaSnapshot>? StateChanged;

    public MediaSnapshot Current { get; private set; } =
        new("هیچ رسانه‌ای پخش نمی‌شود", "یک برنامهٔ رسانه‌ای را اجرا کنید", false, false, false, false, false, "Unavailable");

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            _manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            cancellationToken.ThrowIfCancellationRequested();

            if (_disposed)
            {
                return;
            }

            _manager.CurrentSessionChanged += Manager_CurrentSessionChanged;
            _manager.SessionsChanged += Manager_SessionsChanged;
            await BindCurrentSessionAsync();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            PublishUnavailable("دسترسی به نشست رسانه‌ای ویندوز در دسترس نیست");
        }
    }

    public Task PlayPauseAsync() => RunTransportCommandAsync(session => session.TryTogglePlayPauseAsync().AsTask());
    public Task NextAsync() => RunTransportCommandAsync(session => session.TrySkipNextAsync().AsTask());
    public Task PreviousAsync() => RunTransportCommandAsync(session => session.TrySkipPreviousAsync().AsTask());

    private async Task RunTransportCommandAsync(Func<GlobalSystemMediaTransportControlsSession, Task<bool>> command)
    {
        var session = _session;
        if (_disposed || session is null)
        {
            return;
        }

        try
        {
            // A false result means this player declined the command; it does not mean the whole
            // session is unavailable, so refresh state instead of disabling every control.
            _ = await command(session);
            await RefreshSnapshotAsync();
        }
        catch
        {
            await RefreshSnapshotAsync();
        }
    }

    private void Manager_CurrentSessionChanged(
        GlobalSystemMediaTransportControlsSessionManager sender,
        CurrentSessionChangedEventArgs args) =>
        _ = BindCurrentSessionAsync();

    private void Manager_SessionsChanged(
        GlobalSystemMediaTransportControlsSessionManager sender,
        SessionsChangedEventArgs args) =>
        _ = BindCurrentSessionAsync();

    private async Task BindCurrentSessionAsync()
    {
        if (_disposed || _manager is null)
        {
            return;
        }

        await _bindGate.WaitAsync();
        try
        {
            if (_disposed || _manager is null)
            {
                return;
            }

            var nextSession = _manager.GetCurrentSession();
            if (!ReferenceEquals(_session, nextSession))
            {
                UnsubscribeSession();
                _session = nextSession;
                if (_session is not null)
                {
                    _session.MediaPropertiesChanged += Session_MediaPropertiesChanged;
                    _session.PlaybackInfoChanged += Session_PlaybackInfoChanged;
                }
            }

            await RefreshSnapshotAsync();
        }
        catch
        {
            PublishUnavailable("خواندن وضعیت رسانه با خطا مواجه شد");
        }
        finally
        {
            _bindGate.Release();
        }
    }

    private void Session_MediaPropertiesChanged(
        GlobalSystemMediaTransportControlsSession sender,
        MediaPropertiesChangedEventArgs args) =>
        _ = RefreshSnapshotAsync();

    private void Session_PlaybackInfoChanged(
        GlobalSystemMediaTransportControlsSession sender,
        PlaybackInfoChangedEventArgs args) =>
        _ = RefreshSnapshotAsync();

    private async Task RefreshSnapshotAsync()
    {
        if (_disposed)
        {
            return;
        }

        var session = _session;
        if (session is null)
        {
            PublishUnavailable("هیچ نشست رسانه‌ای فعالی پیدا نشد");
            return;
        }

        try
        {
            var properties = await session.TryGetMediaPropertiesAsync();
            var playback = session.GetPlaybackInfo();

            // A slow response from an old session must not overwrite the current session's UI.
            if (_disposed || !ReferenceEquals(session, _session))
            {
                return;
            }

            var controls = playback.Controls;
            var title = string.IsNullOrWhiteSpace(properties.Title) ? "عنوان نامشخص" : properties.Title;
            var artist = string.IsNullOrWhiteSpace(properties.Artist) ? "هنرمند نامشخص" : properties.Artist;

            Publish(new MediaSnapshot(
                title,
                artist,
                playback.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing,
                true,
                controls.IsPlayPauseToggleEnabled,
                controls.IsNextEnabled,
                controls.IsPreviousEnabled,
                "Available"));
        }
        catch
        {
            if (!_disposed && ReferenceEquals(session, _session))
            {
                PublishUnavailable("اطلاعات رسانه در دسترس نیست");
            }
        }
    }

    private void PublishUnavailable(string status) =>
        Publish(new MediaSnapshot("هیچ رسانه‌ای پخش نمی‌شود", status, false, false, false, false, false, "Unavailable"));

    private void Publish(MediaSnapshot snapshot)
    {
        if (_disposed)
        {
            return;
        }

        Current = snapshot;
        StateChanged?.Invoke(snapshot);
    }

    private void UnsubscribeSession()
    {
        if (_session is null)
        {
            return;
        }

        _session.MediaPropertiesChanged -= Session_MediaPropertiesChanged;
        _session.PlaybackInfoChanged -= Session_PlaybackInfoChanged;
        _session = null;
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return ValueTask.CompletedTask;
        }

        _disposed = true;
        if (_manager is not null)
        {
            _manager.CurrentSessionChanged -= Manager_CurrentSessionChanged;
            _manager.SessionsChanged -= Manager_SessionsChanged;
        }

        UnsubscribeSession();
        StateChanged = null;
        // Keep the semaphore alive: a queued session callback may still be unwinding.
        return ValueTask.CompletedTask;
    }
}
