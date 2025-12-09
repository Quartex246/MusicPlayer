using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using player.Models;

namespace player.Services
{

    /// 播放列表服务

    public class PlaylistService
    {
        public event Action<string> OperationError;
        public event Action<string> OperationSuccess;
        
    
        /// 从 ListView 项获取音乐列表
    
        public List<Music> GetMusicListFromListView(ListView.ListViewItemCollection items)
        {
            List<Music> musicList = new List<Music>();
            
            foreach (ListViewItem item in items)
            {
                var music = item.Tag as Music;
                if (music != null)
                {
                    musicList.Add(music);
                }
            }
            
            return musicList;
        }
        
    
        /// 保存播放列表到文件
    
        public string SavePlaylist(List<Music> musicList, string playlistName = "CurrentPlaylist")
        {
            try
            {
                var playlist = new Playlist(playlistName);
                playlist.Songs.AddRange(musicList);

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                    saveFileDialog.FilterIndex = 1;
                    saveFileDialog.FileName = "Playlist.json";
                    saveFileDialog.Title = "保存播放列表";
                    saveFileDialog.DefaultExt = "json";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string json = JsonConvert.SerializeObject(playlist, Formatting.Indented);
                        string playlistPath = saveFileDialog.FileName;
                        
                        File.WriteAllText(playlistPath, json);
                        OperationSuccess?.Invoke("播放列表保存成功！");
                        return playlistPath;
                    }
                }
            }
            catch (Exception ex)
            {
                OperationError?.Invoke($"保存播放列表失败：{ex.Message}");
            }
            
            return null;
        }
        
    
        /// 从文件加载播放列表
    
        public Playlist LoadPlaylist(out string filePath)
        {
            filePath = null;
            
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.FileName = "Playlist.json";
                    openFileDialog.Title = "加载播放列表";
                    openFileDialog.DefaultExt = "json";

                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                        return null;

                    filePath = openFileDialog.FileName;
                    
                    if (!File.Exists(filePath))
                    {
                        OperationError?.Invoke("未找到保存的播放列表文件。");
                        return null;
                    }

                    string json = File.ReadAllText(filePath);
                    var playlist = JsonConvert.DeserializeObject<Playlist>(json);

                    if (playlist != null)
                    {
                        OperationSuccess?.Invoke("播放列表加载成功！");
                        return playlist;
                    }
                }
            }
            catch (Exception ex)
            {
                OperationError?.Invoke($"加载播放列表失败：{ex.Message}");
            }
            
            return null;
        }
        
    
        /// 向 ListView 添加音乐
    
        public void AddMusicToListView(ListView listView, Music music)
        {
            var item = new ListViewItem(music.Title);
            item.SubItems.Add(music.Artist);
            item.SubItems.Add(music.Album);
            item.SubItems.Add(music.Genre);
            item.SubItems.Add(music.Duration);
            item.Tag = music;
            
            listView.Items.Add(item);
        }
        
    
        /// 批量向 ListView 添加音乐
    
        public void AddMusicToListView(ListView listView, IEnumerable<Music> musicList)
        {
            foreach (var music in musicList)
            {
                AddMusicToListView(listView, music);
            }
        }
        
    
        /// 打开文件对话框添加音乐
    
        public List<Music> OpenAddMusicDialog()
        {
            List<Music> newMusicList = new List<Music>();
            
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "音频文件|*.mp3;*.wav;*.wma;*.aac|所有文件|*.*";
                openFileDialog.Multiselect = true;
                openFileDialog.Title = "选择音乐文件";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        var music = MusicPlayerService.CreateMusicFromFile(filePath);
                        if (music != null)
                        {
                            newMusicList.Add(music);
                        }
                    }
                }
                else
                {
                    OperationError?.Invoke("未选择任何音乐。");
                }
            }
            
            return newMusicList;
        }
        
    
        /// 删除选中的音乐
    
        public void RemoveSelectedMusic(ListView listView)
        {
            // 从后往前删除，避免索引变化问题
            var selectedIndices = new List<int>();
            foreach (ListViewItem selectedItem in listView.SelectedItems)
            {
                selectedIndices.Add(selectedItem.Index);
            }
            
            // 按索引降序排序，然后删除
            selectedIndices.Sort((a, b) => b.CompareTo(a));
            foreach (int index in selectedIndices)
            {
                if (index >= 0 && index < listView.Items.Count)
                {
                    listView.Items.RemoveAt(index);
                }
            }
        }
    }
}