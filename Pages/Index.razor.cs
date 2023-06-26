using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SpotiListener.Pages
{
    public partial class Index : IDisposable
    {
        [Inject]
        public IndexApiService ApiService { get; set; }

        private string trackName = "nothing...";
        private string artistName = "nobody...";
        private string songPhoto;

        private Timer timer;

        protected override async Task OnInitializedAsync()
        {
            await UpdateTrackName();

            timer = new Timer(3000);
            timer.Elapsed += TimerElapsed;
            timer.Start();
        }

        private async void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            await UpdateTrackName();
        }

        private async Task UpdateTrackName()
        {
            var trackInfo = await ApiService.GetCurrentPlayingTrack();
            if (trackInfo != null)
            {
                trackName = trackInfo.TrackName;
                artistName = trackInfo.ArtistName;
                songPhoto = trackInfo.SongPhoto;
            }
            else
            {
                trackName = "nothing...";
                artistName = "nobody...";
                songPhoto = "";
            }
            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}