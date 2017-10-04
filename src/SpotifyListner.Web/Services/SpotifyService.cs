﻿using System;
using System.Linq;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyListner.Web.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;

namespace SpotifyListner.Web.Services
{
    public class SpotifyService : ISpotifyService
    {
        private readonly IYouTubeGoogleService m_youTubeGoogleService;
        private SpotifyWebAPI m_spotifyWebApi;

        public SpotifyService(IYouTubeGoogleService youTubeGoogleService)
        {
            m_youTubeGoogleService = youTubeGoogleService;
            //RunAuthentication();
        }

        public async Task<SpotifySong> GetCurrentSong()
        {
            var track = await  Task.Run(() => m_spotifyWebApi.GetPlayingTrack() );
            return new SpotifySong { Artist = track.Item.Artists.First().Name, Genre = track.Item.Type, Song = track.Item.Name, Time = track.Item.DurationMs };
        }

        public async Task PauseSong(string id)
        {
            var currentSong = await GetCurrentSong();
            var videoDuration = await m_youTubeGoogleService.GetSongLength(id);

            // Get youtube video info
           
            var durationDiffrence = videoDuration - new TimeSpan(0, 0, 0, 0, currentSong.Time) + TimeSpan.FromSeconds(2);

            // If diffrence is more than 0 ms
            if (durationDiffrence.TotalMilliseconds > 0)
            {
                // Pause spotify untill youtube and spotify is synchronized.
                m_spotifyWebApi.PausePlayback();
                await Task.Delay(durationDiffrence);
                m_spotifyWebApi.ResumePlayback();
            }
            else
            {
                // Skip spotify to match youtube video.
                var progress = m_spotifyWebApi.GetPlayback().ProgressMs;
                progress = progress - int.Parse(durationDiffrence.TotalMilliseconds.ToString());
                m_spotifyWebApi.SeekPlayback(progress);
            }
        }

        public async Task<SpotifySong> GetCurrentSong(string token)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var responeMessage = await httpClient.GetAsync("https://api.spotify.com/v1/me/player/currently-playing");
            var result = await responeMessage.Content.ReadAsStringAsync();

            var spotify = new SpotifyWebAPI { AccessToken = token };
            var track = await Task.Run(() => spotify.GetPlayingTrack());
            return new SpotifySong { Artist = track.Item.Artists.First().Name, Genre = track.Item.Type, Song = track.Item.Name, Time = track.Item.DurationMs };
        }

        private async void RunAuthentication()
        {
            var webApiFactory = new WebAPIFactory(
                "http://localhost",
                8000,
                "26d287105e31491889f3cd293d85bfea",
                Scope.UserReadPrivate | Scope.UserReadEmail | Scope.PlaylistReadPrivate | Scope.UserLibraryRead |
                Scope.UserReadPrivate | Scope.UserFollowRead | Scope.UserReadBirthdate | Scope.UserTopRead | Scope.PlaylistReadCollaborative |
                Scope.UserReadRecentlyPlayed | Scope.UserReadPlaybackState | Scope.UserModifyPlaybackState);

            try
            {
                m_spotifyWebApi = await webApiFactory.GetWebApi();
            }
            catch (Exception ex)
            {
                RunAuthentication();
            }
        }
    }
}