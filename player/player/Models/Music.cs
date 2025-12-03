using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace player.Models
{
    public class Music
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Genre { get; set; }
        public string Duration { get; set; }
        public string FilePath { get; set; }
        public int SongCount { get; set; } // Number of songs in the playlist
        public DateTime LastAccessed { get; set; } // Last accessed date
        public string Path { get; set; } // Path of the playlist or music file

        
        }
    }
