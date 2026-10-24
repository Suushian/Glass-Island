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
    private volatile bool _disposed;

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

    public Task PlayPauseAsync() =>
        RunTransportCommandAsync(
            async session => await session.TryTogglePlayPauseAsync(),
            snapshot => snapshot with { CanTogglePlayback = false });

    public Task NextAsync() =>
        RunTransportCommandAsync(
            async session => await session.TrySkipNextAsync(),
            snapshot => snapshot with { CanSkipNext = false });

    public Task PreviousAsync() =>
        RunTransportCommandAsync(
            async session => await session.TrySkipPreviousAsync(),
            snapshot => snapshot with { CanSkipPrevious = false });

    private async Task RunTransportCommandAsync(
        Func<GlobalSystemMediaTransportControlsSession, Task<bool>> command,
        Func<MediaSnapshot, MediaSnapshot> markUnsupported)
    {
        var session = _session;
        if (_disposed || session is null)
        {
            return;
        }

        try
        {
            var accepted = await command(session);
            if (_disposed || !ReferenceEquals(session, _session))
            {
                return;
            }

            if (!accepted)
            {
                // The player declined this command. Disable only that command for this snapshot.
                if (Current.IsAvailable)
                {
                    Publish(markUnsupported(Current));
                }

                return;
            }

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
        MediaPropertiesChangedEventArgs args)
    {
        if (ReferenceEquals(sender, _session))
        {
            _ = RefreshSnapshotAsync();
        }
    }

    private void Session_PlaybackInfoChanged(
        GlobalSystemMediaTransportControlsSession sender,
        PlaybackInfoChangedEventArgs args)
    {
        if (ReferenceEquals(sender, _session))
        {
            _ = RefreshSnapshotAsync();
        }
    }

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
            var title = NormalizeMetadata(properties.Title);
            var artist = NormalizeMetadata(properties.Artist);

            // Some players expose the album artist even when the track artist is empty.
            if (artist is null)
            {
                artist = NormalizeMetadata(properties.AlbumArtist);
            }

            if (title is null)
            {
                var sourceName = GetSourceDisplayName(session.SourceAppUserModelId);
                title = sourceName is null ? "رسانهٔ فعال" : $"رسانهٔ فعال از {sourceName}";
            }

            if (artist is null)
            {
                artist = "اطلاعات آهنگ ارائه نشده";
            }

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

    private static string? NormalizeMetadata(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        var placeholders = new[]
        {
            "unknown",
            "unknown artist",
            "unknown title",
            "unknown track",
            "n/a",
            "not available",
            "نامشخص",
            "عنوان نامشخص",
            "هنرمند نامشخص"
        };

        return placeholders.Contains(normalized, StringComparer.OrdinalIgnoreCase)
            ? null
            : normalized;
    }

    private static string? GetSourceDisplayName(string? appUserModelId)
    {
        if (string.IsNullOrWhiteSpace(appUserModelId))
        {
            return null;
        }

        var source = appUserModelId.ToLowerInvariant();
        if (source.Contains("telegram")) return "Telegram";
        if (source.Contains("spotify")) return "Spotify";
        if (source.Contains("msedge") || source.Contains("microsoftedge")) return "Microsoft Edge";
        if (source.Contains("chrome")) return "Chrome";
        if (source.Contains("firefox")) return "Firefox";
        if (source.Contains("vlc")) return "VLC";
        if (source.Contains("zunemusic") || source.Contains("windowsmediaplayer")) return "Media Player";

        return null;
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
