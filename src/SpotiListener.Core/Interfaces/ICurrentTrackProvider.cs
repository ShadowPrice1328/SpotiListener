using SpotiListener.Core.Models;

namespace SpotiListener.Core.Interfaces;

public interface ICurrentTrackProvider
{
    Task<Track?> GetCurrentTrackAsync(CancellationToken cancellationToken = default);
}