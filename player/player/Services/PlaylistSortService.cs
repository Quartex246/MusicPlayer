using System;
using System.Collections.Generic;
using System.Linq;
using player.Models;

namespace player.Services
{

    /// 播放列表排序服务

    public class PlaylistSortService
    {
        private List<Music> originalPlaylistOrder = new List<Music>();
        
    
        /// 按艺术家排序
    
        public List<Music> SortByArtist(List<Music> musicList, bool saveOriginal = true)
        {
            if (saveOriginal && originalPlaylistOrder.Count == 0)
            {
                SaveOriginalOrder(musicList);
            }
            
            return musicList
                .OrderBy(m => m.Artist)
                .ThenBy(m => m.Title)
                .ToList();
        }
        
    
        /// 按专辑排序
    
        public List<Music> SortByAlbum(List<Music> musicList, bool saveOriginal = true)
        {
            if (saveOriginal && originalPlaylistOrder.Count == 0)
            {
                SaveOriginalOrder(musicList);
            }
            
            return musicList
                .OrderBy(m => m.Album)
                .ThenBy(m => m.Title)
                .ToList();
        }
        
    
        /// 按流派排序
    
        public List<Music> SortByGenre(List<Music> musicList, bool saveOriginal = true)
        {
            if (saveOriginal && originalPlaylistOrder.Count == 0)
            {
                SaveOriginalOrder(musicList);
            }
            
            return musicList
                .OrderBy(m => m.Genre)
                .ThenBy(m => m.Title)
                .ToList();
        }
        
    
        /// 恢复原始顺序
    
        public List<Music> RestoreOriginalOrder()
        {
            if (originalPlaylistOrder.Count == 0)
                return new List<Music>();
                
            var result = new List<Music>(originalPlaylistOrder);
            originalPlaylistOrder.Clear();
            return result;
        }
        
    
        /// 是否有保存的原始顺序
    
        public bool HasOriginalOrder => originalPlaylistOrder.Count > 0;
        
    
        /// 保存原始顺序
    
        private void SaveOriginalOrder(List<Music> musicList)
        {
            originalPlaylistOrder.Clear();
            originalPlaylistOrder.AddRange(musicList);
        }
    }
}