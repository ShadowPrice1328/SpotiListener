namespace SpotiListener.Core.Models;

public sealed class Track
{
    public string TrackName { get; init; } = string.Empty;
    public string ArtistName { get; init; } = string.Empty;
    public string AlbumName {get; init; } = string.Empty;
    public string? SongPhoto { get; init; }
}

