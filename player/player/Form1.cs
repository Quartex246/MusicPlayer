using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using player.Models;
using player.Services;

namespace player
{
    public partial class Form1 : Form
    {
        // 服务实例
        private MusicPlayerService musicPlayerService;
        private PlaylistService playlistService;
        private PlaylistHistoryService historyService;
        private PlaylistSortService sortService;
        
        // 播放列表状态
        private bool isPlaylistLoaded = false;
        private string currentPlaylistPath = null;
        
        public Form1()
        {
            InitializeComponent();
            
            // 初始化服务
            musicPlayerService = new MusicPlayerService();
            playlistService = new PlaylistService();
            historyService = new PlaylistHistoryService();
            sortService = new PlaylistSortService();
            
            // 绑定事件
            musicPlayerService.NowPlayingUpdated += UpdateNowPlayingInfo;
            musicPlayerService.PlaybackError += ShowErrorMessage;
            playlistService.OperationError += ShowErrorMessage;
            playlistService.OperationSuccess += ShowSuccessMessage;
            
            SetupPlaylistColumns();
            listViewPlaylist.DoubleClick += ListViewPlaylist_DoubleClick;
            
            // 绑定排序复选框事件
            SortArtistBox.CheckedChanged += SortCheckBox_CheckedChanged;
            SortAlbumBox.CheckedChanged += SortCheckBox_CheckedChanged;
            SortGenreBox.CheckedChanged += SortCheckBox_CheckedChanged;
        }
        
        // =============================================
        // 界面初始化
        // =============================================
        
        private void SetupPlaylistColumns()
        {
            listViewPlaylist.Columns.Clear();
            listViewPlaylist.Columns.Add("Title", 75);
            listViewPlaylist.Columns.Add("Artist", 75);
            listViewPlaylist.Columns.Add("Album", 75);
            listViewPlaylist.Columns.Add("Genre", 75);
            listViewPlaylist.Columns.Add("Length", 75);
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            // 窗体加载事件
        }
        
        // =============================================
        // 播放控制
        // =============================================
        
        private void ListViewPlaylist_DoubleClick(object sender, EventArgs e)
        {
            if (listViewPlaylist.SelectedItems.Count > 0)
            {
                var selectedItem = listViewPlaylist.SelectedItems[0];
                var music = selectedItem.Tag as Music;
                if (music != null)
                {
                    musicPlayerService.PlayMusic(music, listViewPlaylist.Items);
                }
            }
        }
        
        private void PlayButton_Click(object sender, EventArgs e)
        {
            musicPlayerService.TogglePlayPause();
            UpdatePlayButtonText();
        }
        
        private void UpdatePlayButtonText()
        {
            PlayButton.Text = musicPlayerService.IsPlaying ? "暂停" : "播放";
        }
        
        private void UpdateNowPlayingInfo(Music music)
        {
            if (music == null)
            {
                SongName.Text = "无歌曲";
                Artist.Text = "";
                return;
            }

            SongName.Text = music.Title ?? "未知标题";
            Artist.Text = music.Artist ?? "未知艺术家";
            UpdatePlayButtonText();
        }
        
        private void PrevButton_Click(object sender, EventArgs e)
        {
            musicPlayerService.PlayPrevious(listViewPlaylist.Items);
        }
        
        private void NextButton_Click(object sender, EventArgs e)
        {
            musicPlayerService.PlayNext(listViewPlaylist.Items);
        }
        
        // =============================================
        // 播放列表管理
        // =============================================
        
        private void PLICreateButton_Click(object sender, EventArgs e)
        {
            clearPlaylist();
            
            var newMusicList = playlistService.OpenAddMusicDialog();
            playlistService.AddMusicToListView(listViewPlaylist, newMusicList);
            
            currentPlaylistPath = null;
            SaveCurrentPlaylist();
        }
        
        private void PLIEditButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你想做什么？\n是：添加音乐\n否：删除选中的音乐",
                "编辑播放列表", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                var newMusicList = playlistService.OpenAddMusicDialog();
                playlistService.AddMusicToListView(listViewPlaylist, newMusicList);
            }
            else if (MessageBox.Show("确定要从播放列表中删除选中的音乐吗？",
                "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                playlistService.RemoveSelectedMusic(listViewPlaylist);
            }
        }
        
        private void PLISelectButton_Click(object sender, EventArgs e)
        {
            LoadPlaylistFromFile();
        }
        
        private void PLIDeleteButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPlaylistPath))
            {
                ShowErrorMessage("当前未加载播放列表文件，或播放列表尚未保存。");
                return;
            }

            if (MessageBox.Show("确定要删除整个播放列表吗？",
                "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                File.Delete(currentPlaylistPath);
                clearPlaylist();
                ShowSuccessMessage("播放列表删除成功！");
            }
        }
        
        private void PLIHistoryButton_Click(object sender, EventArgs e)
        {
            ShowHistoryPlaylist();
        }
        
        // =============================================
        // 播放列表操作实现
        // =============================================
        
        private void SaveCurrentPlaylist()
        {
            var musicList = playlistService.GetMusicListFromListView(listViewPlaylist.Items);
            string savedPath = playlistService.SavePlaylist(musicList, "CurrentPlaylist");
            
            if (!string.IsNullOrEmpty(savedPath))
            {
                currentPlaylistPath = savedPath;
                
                // 添加到历史记录
                var playlist = new Playlist("CurrentPlaylist");
                playlist.Songs.AddRange(musicList);
                historyService.AddToHistory(savedPath, playlist);
            }
        }
        
        private void LoadPlaylistFromFile()
        {
            string filePath;
            var playlist = playlistService.LoadPlaylist(out filePath);
            
            if (playlist != null)
            {
                listViewPlaylist.Items.Clear();
                playlistService.AddMusicToListView(listViewPlaylist, playlist.Songs);
                
                currentPlaylistPath = filePath;
                isPlaylistLoaded = true;
                
                // 添加到历史记录
                historyService.AddToHistory(filePath, playlist);
                
                // 清除排序状态
                ClearSortState();
            }
        }
        
        private void ShowHistoryPlaylist()
        {
            using (var historyForm = new HistoryForm(historyService.GetHistory()))
            {
                historyForm.PlaylistSelected += (historyItem) =>
                {
                    LoadPlaylistFromHistory(historyItem);
                    historyForm.Close();
                };
                
                historyForm.ShowDialog(this);
            }
        }
        
        private void LoadPlaylistFromHistory(PlaylistHistoryItem historyItem)
        {
            if (!File.Exists(historyItem.Path))
            {
                ShowErrorMessage($"播放列表文件未找到：\n{historyItem.Path}");
                return;
            }

            try
            {
                string json = File.ReadAllText(historyItem.Path);
                var playlist = JsonConvert.DeserializeObject<Playlist>(json);

                if (playlist != null)
                {
                    listViewPlaylist.Items.Clear();
                    playlistService.AddMusicToListView(listViewPlaylist, playlist.Songs);
                    
                    currentPlaylistPath = historyItem.Path;
                    isPlaylistLoaded = true;
                    
                    // 更新历史记录访问时间
                    historyItem.LastAccessed = DateTime.Now;
                    historyService.AddToHistory(historyItem.Path, playlist);
                    
                    ShowSuccessMessage($"播放列表 '{historyItem.Name}' 加载成功！");
                    
                    // 清除排序状态
                    ClearSortState();
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"加载播放列表失败：{ex.Message}");
            }
        }
        
        // =============================================
        // 排序功能
        // =============================================
        
        private void SortCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox clickedCheckBox = sender as CheckBox;
            
            if (clickedCheckBox != null && clickedCheckBox.Checked)
            {
                if (clickedCheckBox == SortArtistBox)
                {
                    SortAlbumBox.Checked = false;
                    SortGenreBox.Checked = false;
                    SortByArtist();
                }
                else if (clickedCheckBox == SortAlbumBox)
                {
                    SortArtistBox.Checked = false;
                    SortGenreBox.Checked = false;
                    SortByAlbum();
                }
                else if (clickedCheckBox == SortGenreBox)
                {
                    SortArtistBox.Checked = false;
                    SortAlbumBox.Checked = false;
                    SortByGenre();
                }
            }
            else if (!SortArtistBox.Checked && !SortAlbumBox.Checked && !SortGenreBox.Checked)
            {
                RestoreOriginalOrder();
            }
        }
        
        private void SortByArtist()
        {
            if (!isPlaylistLoaded || listViewPlaylist.Items.Count == 0) return;
            
            var musicList = playlistService.GetMusicListFromListView(listViewPlaylist.Items);
            var sortedMusic = sortService.SortByArtist(musicList, !sortService.HasOriginalOrder);
            UpdatePlaylistView(sortedMusic);
        }
        
        private void SortByAlbum()
        {
            if (!isPlaylistLoaded || listViewPlaylist.Items.Count == 0) return;
            
            var musicList = playlistService.GetMusicListFromListView(listViewPlaylist.Items);
            var sortedMusic = sortService.SortByAlbum(musicList, !sortService.HasOriginalOrder);
            UpdatePlaylistView(sortedMusic);
        }
        
        private void SortByGenre()
        {
            if (!isPlaylistLoaded || listViewPlaylist.Items.Count == 0) return;
            
            var musicList = playlistService.GetMusicListFromListView(listViewPlaylist.Items);
            var sortedMusic = sortService.SortByGenre(musicList, !sortService.HasOriginalOrder);
            UpdatePlaylistView(sortedMusic);
        }
        
        private void RestoreOriginalOrder()
        {
            if (!isPlaylistLoaded) return;
            
            var originalMusic = sortService.RestoreOriginalOrder();
            if (originalMusic.Count > 0)
            {
                UpdatePlaylistView(originalMusic);
            }
        }
        
        private void UpdatePlaylistView(List<Music> musicList)
        {
            listViewPlaylist.Items.Clear();
            playlistService.AddMusicToListView(listViewPlaylist, musicList);
        }
        
        private void ClearSortState()
        {
            SortArtistBox.Checked = false;
            SortAlbumBox.Checked = false;
            SortGenreBox.Checked = false;
        }
        
        // =============================================
        // 辅助方法
        // =============================================
        
        private List<ListViewItem> GetPlaylistItems()
        {
            var items = new List<ListViewItem>();
            foreach (ListViewItem item in listViewPlaylist.Items)
            {
                items.Add(item);
            }
            return items;
        }
        
        private void clearPlaylist()
        {
            listViewPlaylist.Items.Clear();
            musicPlayerService.Stop();
            UpdatePlayButtonText();
            UpdateNowPlayingInfo(null);
            
            isPlaylistLoaded = false;
            ClearSortState();
        }
        
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        // =============================================
        // 空的事件处理方法（保持设计器兼容性）
        // =============================================
        
        private void SortArtistBox_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}