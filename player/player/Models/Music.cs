using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace player.Models
{
    internal class Music
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public TimeSpan Duration { get; set; }
        public string FilePath { get; set; }
        public string Lyrics { get; set; }

        public Music(string title, string artist, string album, string genre, TimeSpan duration, string filePath)
        {
            Title = title;
            Artist = artist;
            Album = album;
            Genre = genre;
            Duration = duration;
            FilePath = filePath;
        }

    }
