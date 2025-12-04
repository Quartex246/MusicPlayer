using System;
using player.Models;

namespace player
{
    public class MusicInfoReader
    {
        public Music GetMusicInfo(string filePath)
        {
            try
                {
                    // Using Windows Media Player to fetch info
                        var tempPlayer = new WMPLib.WindowsMediaPlayer();
                        var media = tempPlayer.newMedia(filePath);

                        // Create Music Object
                        var music = new Music
                        {
                            FilePath = filePath,
                            Title = string.IsNullOrEmpty(media.getItemInfo("Title")) ?
                                System.IO.Path.GetFileNameWithoutExtension(filePath) : media.getItemInfo("Title"),

                            Artist = string.IsNullOrEmpty(media.getItemInfo("Artist")) ?
                                    "Unknown Artist" : media.getItemInfo("Artist"),

                            Album = string.IsNullOrEmpty(media.getItemInfo("Album")) ?
                                "Unknown Album" : media.getItemInfo("Album"),

                            Genre = string.IsNullOrEmpty(media.getItemInfo("Genre")) ?
                                "Unknown Genre" : media.getItemInfo("Genre"),

                            Duration = FormatTime(media.duration)
                        };

                        return music;
                }
                catch (Exception ex)
                { 
                throw new Exception($"Unable to get music info：{ex.Message}");
                }
        } 

        private string FormatTime(double duration)
        {
            TimeSpan time = TimeSpan.FromSeconds(duration);
            return time.ToString(@"mm\:ss");
        }
    }    
}
        
                