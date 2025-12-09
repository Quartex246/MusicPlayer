using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms; // 添加这个引用
using player.Models;
using WMPLib;

namespace player.Services
{

    /// 音乐播放服务

    public class MusicPlayerService
    {
        private WindowsMediaPlayer wplayer = new WindowsMediaPlayer();
        private Music currentMusic;
        private bool isPlaying = false;
        private int currentSongIndex = -1;
        
        public event Action<Music> NowPlayingUpdated;
        public event Action<string> PlaybackError;
        
        // 播放状态
        public bool IsPlaying => isPlaying;
        public Music CurrentMusic => currentMusic;
        public int CurrentSongIndex => currentSongIndex;
        
    
        /// 播放音乐
    
        public void PlayMusic(Music music, ListView.ListViewItemCollection playlistItems = null)
        {
            if (music == null || string.IsNullOrEmpty(music.FilePath))
            {
                PlaybackError?.Invoke("无法播放：音乐文件路径无效");
                return;
            }

            currentMusic = music;

            // 如果有播放列表项，查找当前索引
            if (playlistItems != null && playlistItems.Count > 0)
            {
                for (int i = 0; i < playlistItems.Count; i++)
                {
                    if (playlistItems[i].Tag == music)
                    {
                        currentSongIndex = i;
                        break;
                    }
                }
            }

            try
            {
                wplayer.URL = music.FilePath; // 通过Music中的FilePath属性设置URL
                wplayer.controls.play();
                isPlaying = true;
                NowPlayingUpdated?.Invoke(music);
            }
            catch (Exception ex)
            {
                PlaybackError?.Invoke($"无法播放：{ex.Message}");
            }
        }
        
    
        /// 播放音乐（通过索引）
    
        public void PlayMusicByIndex(int index, ListView.ListViewItemCollection playlistItems)
        {
            if (playlistItems == null || index < 0 || index >= playlistItems.Count)
            {
                PlaybackError?.Invoke("无效的歌曲索引");
                return;
            }
            
            var music = playlistItems[index].Tag as Music;
            if (music != null)
            {
                currentSongIndex = index;
                PlayMusic(music, playlistItems);
            }
        }
        
    
        /// 播放/暂停
    
        public void TogglePlayPause()
        {
            if (isPlaying)
            {
                wplayer.controls.pause();
                isPlaying = false;
            }
            else
            {
                wplayer.controls.play();
                isPlaying = true;
            }
        }
        
    
        /// 播放下一首
    
        public void PlayNext(ListView.ListViewItemCollection playlistItems)
        {
            if (playlistItems == null || playlistItems.Count == 0)
            {
                PlaybackError?.Invoke("播放列表为空");
                return;
            }

            // 如果没有歌曲在播放，从第一首开始
            if (currentSongIndex < 0 || currentSongIndex >= playlistItems.Count)
            {
                currentSongIndex = 0;
            }
            else
            {
                // 移动到下一首，循环到开头
                currentSongIndex++;
                if (currentSongIndex >= playlistItems.Count)
                {
                    currentSongIndex = 0;
                }
            }

            PlayMusicByIndex(currentSongIndex, playlistItems);
        }
        
    
        /// 播放上一首
    
        public void PlayPrevious(ListView.ListViewItemCollection playlistItems)
        {
            if (playlistItems == null || playlistItems.Count == 0)
            {
                PlaybackError?.Invoke("播放列表为空");
                return;
            }

            // 如果没有歌曲在播放，从最后一首开始
            if (currentSongIndex < 0 || currentSongIndex >= playlistItems.Count)
            {
                currentSongIndex = playlistItems.Count - 1;
            }
            else
            {
                // 移动到上一首，循环到末尾
                currentSongIndex--;
                if (currentSongIndex < 0)
                {
                    currentSongIndex = playlistItems.Count - 1;
                }
            }

            PlayMusicByIndex(currentSongIndex, playlistItems);
        }
        
    
        /// 停止播放
    
        public void Stop()
        {
            wplayer.controls.stop();
            isPlaying = false;
            currentMusic = null;
            currentSongIndex = -1;
        }
        
    
        /// 格式化时间
    
        public static string FormatTime(double duration)
        {
            TimeSpan time = TimeSpan.FromSeconds(duration);
            return time.ToString(@"mm\:ss");
        }
        
    
        /// 从文件创建 Music 对象
    
        public static Music CreateMusicFromFile(string filePath)
        {
            try
            {
                var tempPlayer = new WindowsMediaPlayer();
                var media = tempPlayer.newMedia(filePath);

                return new Music
                {
                    FilePath = filePath,
                    Title = string.IsNullOrEmpty(media.getItemInfo("Title")) ?
                           System.IO.Path.GetFileNameWithoutExtension(filePath) : media.getItemInfo("Title"),
                    Artist = string.IsNullOrEmpty(media.getItemInfo("Artist")) ?
                            "未知艺术家" : media.getItemInfo("Artist"),
                    Album = string.IsNullOrEmpty(media.getItemInfo("Album")) ?
                           "未知专辑" : media.getItemInfo("Album"),
                    Genre = string.IsNullOrEmpty(media.getItemInfo("Genre")) ?
                           "未知流派" : media.getItemInfo("Genre"),
                    Duration = FormatTime(media.duration)
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}