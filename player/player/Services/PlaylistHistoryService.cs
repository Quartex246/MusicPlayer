using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using player.Models;

namespace player.Services
{

    /// 播放列表历史记录服务

    public class PlaylistHistoryService
    {
        private List<PlaylistHistoryItem> playlistHistory = new List<PlaylistHistoryItem>();
        private const int MAX_PLAYLIST_HISTORY_SIZE = 20;
        private readonly string historyFilePath;
        
        public PlaylistHistoryService()
        {
            historyFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "playlist_history.json");
            LoadPlaylistHistory();
        }
        
    
        /// 获取历史记录列表
    
        public List<PlaylistHistoryItem> GetHistory() => playlistHistory;
        
    
        /// 添加到历史记录
    
        public void AddToHistory(string playlistPath, Playlist playlist)
        {
            try
            {
                string playlistName = playlist.Name ?? Path.GetFileNameWithoutExtension(playlistPath);
                
                // 检查是否已存在
                var existing = playlistHistory.FirstOrDefault(p => 
                    p.Path.Equals(playlistPath, StringComparison.OrdinalIgnoreCase));
                    
                if (existing != null)
                {
                    // 更新现有记录
                    existing.LastAccessed = DateTime.Now;
                    existing.SongCount = playlist.Songs?.Count ?? 0;
                    // 移动到列表开头（最近使用的）
                    playlistHistory.Remove(existing);
                    playlistHistory.Insert(0, existing);
                }
                else
                {
                    // 添加新记录
                    var historyItem = new PlaylistHistoryItem
                    {
                        Name = playlistName,
                        Path = playlistPath,
                        LastAccessed = DateTime.Now,
                        SongCount = playlist.Songs?.Count ?? 0
                    };
                    
                    playlistHistory.Insert(0, historyItem);
                    
                    // 限制历史记录大小
                    if (playlistHistory.Count > MAX_PLAYLIST_HISTORY_SIZE)
                    {
                        playlistHistory.RemoveAt(MAX_PLAYLIST_HISTORY_SIZE);
                    }
                }
                
                SavePlaylistHistory();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"添加到播放列表历史记录时出错：{ex.Message}");
            }
        }
        
    
        /// 清除历史记录
    
        public void ClearHistory()
        {
            playlistHistory.Clear();
            SavePlaylistHistory();
        }
        
    
        /// 保存历史记录到文件
    
        private void SavePlaylistHistory()
        {
            try
            {
                string json = JsonConvert.SerializeObject(playlistHistory, Formatting.Indented);
                File.WriteAllText(historyFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存播放列表历史记录时出错：{ex.Message}");
            }
        }
        
    
        /// 从文件加载历史记录
    
        private void LoadPlaylistHistory()
        {
            try
            {
                if (File.Exists(historyFilePath))
                {
                    string json = File.ReadAllText(historyFilePath);
                    playlistHistory = JsonConvert.DeserializeObject<List<PlaylistHistoryItem>>(json) 
                        ?? new List<PlaylistHistoryItem>();
                    
                    // 确保不超过最大数量
                    if (playlistHistory.Count > MAX_PLAYLIST_HISTORY_SIZE)
                    {
                        playlistHistory = playlistHistory.Take(MAX_PLAYLIST_HISTORY_SIZE).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载播放列表历史记录时出错：{ex.Message}");
                playlistHistory = new List<PlaylistHistoryItem>();
            }
        }
    }
}