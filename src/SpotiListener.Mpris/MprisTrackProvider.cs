using SpotiListener.Core.Interfaces;
using SpotiListener.Core.Models;
using SpotiListener.Mpris.DBus;
using Tmds.DBus.Protocol;

namespace SpotiListener.Mpris;

public sealed class MprisTrackProvider : ICurrentTrackProvider
{
    private const string SpotifyService = "org.mpris.MediaPlayer2.spotify";
    private const string PlayerPath = "/org/mpris/MediaPlayer2";
    public async Task<Track?> GetCurrentTrackAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var address = DBusAddress.Session ??
            throw new InvalidOperationException("Session D-Bus address is not available.");

        using var connection = new DBusConnection(address);
        await connection.ConnectAsync();

        var services = await connection.ListServicesAsync();

        if (!services.Contains(SpotifyService))
            throw new InvalidOperationException($"SpotifyService \"{SpotifyService}\" is not present on the list.");

        var player =  new Player(connection, SpotifyService, PlayerPath);

        var metadata = await player.GetMetadataAsync();

        return new Track
        {
            TrackName = GetString(metadata, "xesam:title") ?? string.Empty,
            ArtistName = GetArtist(metadata, "xesam:artist") ?? string.Empty,
            AlbumName = GetString(metadata, "xesam:album") ?? string.Empty,
            SongPhoto = GetString(metadata, "mpris:artUrl") 
        };
    }

    private static string? GetString(Dictionary<string, VariantValue> metadata, string key)
    {
        if (!metadata.TryGetValue(key, out var value))
            return null;

        return value.GetString();
    }

    private static string GetArtist(Dictionary<string, VariantValue> metadata, string key)
    {
        if (!metadata.TryGetValue(key, out var value))
            return string.Empty;

        if (value.ItemType == VariantValueType.String)
            return string.Join(", ", value.GetArray<string>());

        return string.Empty;
    }
}