using System;

namespace player.Models
{

    /// 播放列表历史记录项

    public class PlaylistHistoryItem
    {
        public string Name { get; set; }
        public string Path { get; set; } 
        public DateTime LastAccessed { get; set; } 
        public int SongCount { get; set; }

        public override string ToString() 
        {
            return $"{Name} ({SongCount} songs) - {LastAccessed:yyyy-MM-dd}";
        }
    }
}