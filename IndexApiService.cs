using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;
using Swan.Parsers;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SpotiListener
{
    public class IndexApiService
    {
        private readonly SpotifyClient spotify;

        private readonly string clientId = Codes.clientId;
        private readonly string clientSecret = Codes.clientSecret;
        private readonly string base64 = Codes.base64;
        private readonly string refreshToken = Codes.refreshToken;

        private string accessToken;
        private DateTime tokenExpiration;

        public IndexApiService()
        {
            var config = SpotifyClientConfig
              .CreateDefault()
              .WithAuthenticator(new ClientCredentialsAuthenticator(clientId, clientSecret));

            spotify = new SpotifyClient(config);

            // Initialize token
            accessToken = string.Empty;
            tokenExpiration = DateTime.MinValue;
        }

        public async Task<string> GetTrack()
        {
            try
            {
                var track = await spotify.Tracks.Get("1s6ux0lNiTziSrd7iUAADH");
                return track.Name;
            }
            catch (Exception)
            {
                return "Track not found!";
            }
        }

        public string GetToken()
        {
            if (string.IsNullOrEmpty(accessToken) || DateTime.Now >= tokenExpiration)
            {
                RefreshToken();
            }

            return accessToken;
        }

        private void RefreshToken()
        {
            string tokenUrl = "https://accounts.spotify.com/api/token";

            using (HttpClient httpClient = new HttpClient())
            {
                // POST request
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + base64);

                // Parameters for request
                var values = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", refreshToken }
                };

                var content = new FormUrlEncodedContent(values);
                var response = client.PostAsync(tokenUrl, content).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    User? user = JsonConvert.DeserializeObject<User>(responseString);

                    if (user != null)
                    {
                        accessToken = user.access_token;
                        tokenExpiration = DateTime.Now.AddSeconds(user.expires_in);
                    }
                }
            }
        }

        public async Task<Track?> GetCurrentPlayingTrack()
        {
            string apiUrl = "https://api.spotify.com/v1/me/player/currently-playing";

            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                string responseBody = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(responseBody))
                {
                    return null;
                }

                dynamic jsonResponse = JsonConvert.DeserializeObject(responseBody);

                string playingType = jsonResponse.currently_playing_type;

                if (playingType != "track")
                {
                    return null;
                }

                dynamic item = jsonResponse.item;
                string trackName = item.name;
                string artistName = item.artists[0].name;
                string songPhoto = item.album.images[0].url;

                Track trackInfo = new Track
                {
                    TrackName = trackName,
                    ArtistName = artistName,
                    SongPhoto = songPhoto
                };

                return trackInfo;
            }
        }
    }
}
