using SpotiListener.Core.Interfaces;
using SpotiListener.Core.Models;
using SpotiListener.Mpris;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ICurrentTrackProvider, MprisTrackProvider>();

var app = builder.Build();

app.MapGet("/api/track", async (ICurrentTrackProvider trackProvider,CancellationToken cancellationToken) =>
{
    var result = Results.Empty;
    var track = new Track();

    try
    {
        track = await trackProvider.GetCurrentTrackAsync(cancellationToken);    
        result = Results.Ok(track);
    }
    catch (Exception ex)
    {
        result = Results.NotFound(ex.Message);
    }

    return result;
});

app.Run();
