using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace player.Models
{
    internal class Playlist
    {
        public string Name { get; set; } //The name of the playlist
        public List<Music> Songs { get; set; } //Created a list called "Music"
        public DateTime CreatedDate { get; set; } // Created date

        public Playlist(string name)
        {
            Name = name;
            Songs = new List<Music>();
            CreatedDate = DateTime.Now;
        }

        public void AddSong(Music song)
        {
            Songs.Add(song); //Add and remove function always in the list.
        }

        public void RemoveSong(Music song)
        {
            Songs.Remove(song);
        }
    }
}
