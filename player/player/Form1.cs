using Newtonsoft.Json; // 使用 Newtonsoft.Json 库处理 JSON 数据（用于播放列表的保存和加载）
using player.Models; // 引用自定义的 Music 和 Playlist 模型类
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace player
{
    public partial class Form1 : Form
    {
        // =============================================
        // 1. 类定义区（内部类和字段）
        // =============================================
        
        // 播放列表历史记录项的类（放在最前面，方便理解）
        public class PlaylistHistoryItem
        {
            public string Name { get; set; } // 播放列表名称
            public string Path { get; set; } // 播放列表文件路径
            public DateTime LastAccessed { get; set; } // 最后访问时间
            public int SongCount { get; set; } // 歌曲数量

            // 重写 ToString() 方法，用于在 ListView 中显示友好格式
            public override string ToString()
            {
                return $"{Name} ({SongCount} songs) - {LastAccessed:yyyy-MM-dd}";
            }
        }

        // 核心字段（播放器状态）
        private Music currentMusic;   // 当前正在播放的音乐对象
        private bool isPlaying = false; // 记录当前播放状态（true=正在播放，false=暂停/停止）
        private int currentSongIndex = -1; // 当前播放歌曲在播放列表中的索引（-1 表示没有歌曲在播放）
        private string currentPlaylistPath = null; // 当前加载的播放列表文件的路径

        // 播放列表历史记录相关
        private List<PlaylistHistoryItem> playlistHistory = new List<PlaylistHistoryItem>();
        private const int MAX_PLAYLIST_HISTORY_SIZE = 20;  // 历史记录的最大数量限制

        // 排序功能相关
        private List<Music> originalPlaylistOrder = new List<Music>(); // 用于保存原始顺序
        private bool isPlaylistLoaded = false; // 标记是否已加载播放列表

        // Windows Media Player 实例
        WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();

        // =============================================
        // 2. 构造函数和初始化
        // =============================================
        
        public Form1()
        {
            InitializeComponent();
            SetupPlaylistColumns(); // 初始化播放列表列
            listViewPlaylist.DoubleClick += ListViewPlaylist_DoubleClick; // 绑定双击播放功能

            LoadPlaylistHistory(); // 加载历史记录

            // 绑定排序复选框事件
            SortArtistBox.CheckedChanged += SortCheckBox_CheckedChanged;
            SortAlbumBox.CheckedChanged += SortCheckBox_CheckedChanged;
            SortGenreBox.CheckedChanged += SortCheckBox_CheckedChanged;
        }

        // =============================================
        // 3. 界面初始化方法
        // =============================================
        
        // 设置播放列表 ListView 的列标题
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
            // 窗体加载事件（可在此添加初始化逻辑）
        }

        // =============================================
        // 4. 核心音乐播放功能
        // =============================================
        
        // 播放指定的音乐（核心播放逻辑）
        private void PlayMusic(Music music)
        {
            if (music == null || string.IsNullOrEmpty(music.FilePath))
            {
                MessageBox.Show("无法播放：音乐文件路径无效");
                return;
            }

            currentMusic = music;

            // 查找并高亮当前歌曲
            for (int i = 0; i < listViewPlaylist.Items.Count; i++)
            {
                if (listViewPlaylist.Items[i].Tag == music)
                {
                    currentSongIndex = i;
                    listViewPlaylist.Items[i].Selected = true;
                    break;
                }
            }

            try
            {
                wplayer.URL = music.FilePath; // 寻找音乐路径
                wplayer.controls.play(); // 控制播放
                isPlaying = true; // 将播放状态改为true
                UpdatePlayButtonText(); //更新播放按钮文字
                UpdateNowPlayingInfo(music); //更改播放音乐文本框文字
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法播放：{ex.Message}");
            }
        }

        // 播放/暂停按钮点击事件
        private void PlayButton_Click(object sender, EventArgs e)
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
            UpdatePlayButtonText();
        }

        // 更新播放按钮文本
        private void UpdatePlayButtonText()
        {
            PlayButton.Text = isPlaying ? "Pause" : "Play";
        }

        // 更新正在播放信息显示
        private void UpdateNowPlayingInfo(Music music)
        {
            if (music == null)
            {
                SongName.Text = "No Song";
                Artist.Text = "";
                return;
            }

            SongName.Text = music.Title ?? "Unknown Title";
            Artist.Text = music.Artist ?? "Unknown Artist";
        }

        // =============================================
        // 5. 播放控制（上一曲/下一曲）
        // =============================================
        
        private void PrevButton_Click(object sender, EventArgs e)
        {
            PlayPrevSong();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            PlayNextSong();
        }

        // 播放下一首歌曲
        private void PlayNextSong()
        {
            if (currentSongIndex < 0 || currentSongIndex >= listViewPlaylist.Items.Count)
            {
                if (listViewPlaylist.Items.Count > 0)
                {
                    currentSongIndex = 0;
                }
                else
                {
                    MessageBox.Show("播放列表为空");
                    return;
                }
            }
            else
            {
                currentSongIndex++;
                if (currentSongIndex >= listViewPlaylist.Items.Count)
                {
                    currentSongIndex = 0; // 循环到开头
                }
            }

            var nextItem = listViewPlaylist.Items[currentSongIndex];
            var music = nextItem.Tag as Music;
            if (music != null)
            {
                PlayMusic(music);
            }
        }

        // 播放上一首歌曲
        private void PlayPrevSong()
        {
            if (currentSongIndex < 0 || currentSongIndex >= listViewPlaylist.Items.Count)
            {
                if (listViewPlaylist.Items.Count > 0)
                {
                    currentSongIndex = listViewPlaylist.Items.Count - 1;
                }
                else
                {
                    MessageBox.Show("播放列表为空");
                    return;
                }
            }
            else
            {
                currentSongIndex--;
                if (currentSongIndex < 0)
                {
                    currentSongIndex = listViewPlaylist.Items.Count - 1; // 循环到末尾
                }
            }

            var prevItem = listViewPlaylist.Items[currentSongIndex];
            var music = prevItem.Tag as Music;
            if (music != null)
            {
                PlayMusic(music);
            }
        }

        // =============================================
        // 6. 播放列表管理（创建、编辑、保存、加载）
        // =============================================
        
        // 创建播放列表按钮点击事件
        private void PLICreateButton_Click(object sender, EventArgs e)
        {
            clearPlaylist();
            AddMusicDialog();
            currentPlaylistPath = null;
            SavePlaylist();
        }

        // 编辑播放列表按钮点击事件
        private void PLIEditButton_Click(object sender, EventArgs e)
        {
            EditPlaylist();
        }

        // 选择播放列表按钮点击事件
        private void PLISelectButton_Click(object sender, EventArgs e)
        {
            LoadPlaylist();
        }

        // 删除播放列表按钮点击事件
        private void PLIDeleteButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPlaylistPath))
            {
                MessageBox.Show("当前未加载播放列表文件，或播放列表尚未保存。");
                return;
            }

            if (MessageBox.Show("确定要删除整个播放列表吗？",
                "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                System.IO.File.Delete(currentPlaylistPath);
                clearPlaylist();
                MessageBox.Show("播放列表删除成功！");
            }
        }

        // 历史记录按钮点击事件
        private void PLIHistoryButton_Click(object sender, EventArgs e)
        {
            ShowHistoryPlaylist();
        }

        // =============================================
        // 7. 播放列表操作的具体实现
        // =============================================
        
        // 打开文件对话框添加音乐
        private void AddMusicDialog()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "音频文件|*.mp3;*.wav;*.wma;*.aac|所有文件|*.*";
                openFileDialog.Multiselect = true;
                openFileDialog.Title = "选择音乐文件";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        AddMusicToPlaylist(filePath);
                    }
                }
                else
                {
                    MessageBox.Show("未选择任何音乐。");
                }
            }
        }

        // 添加单个音乐文件到播放列表
        private void AddMusicToPlaylist(string filePath)
        {
            try
            {
                var tempPlayer = new WMPLib.WindowsMediaPlayer();
                var media = tempPlayer.newMedia(filePath);

                var music = new Music
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

                var item = new ListViewItem(music.Title);
                item.SubItems.Add(music.Artist);
                item.SubItems.Add(music.Album);
                item.SubItems.Add(music.Genre);
                item.SubItems.Add(music.Duration);
                item.Tag = music;

                listViewPlaylist.Items.Add(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"添加文件失败：{ex.Message}");
            }
        }

        // 保存播放列表到文件
        private void SavePlaylist()
        {
            try
            {
                var playlist = new Playlist("CurrentPlaylist");
                
                foreach (ListViewItem item in listViewPlaylist.Items)
                {
                    var music = item.Tag as Music;
                    if (music != null)
                    {
                        playlist.AddSong(music);
                    }
                }

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.FileName = "Playlist.json";
                saveFileDialog.Title = "保存播放列表";
                saveFileDialog.DefaultExt = "json";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string json = JsonConvert.SerializeObject(playlist, Formatting.Indented);
                    string playlistPath = saveFileDialog.FileName;
                    
                    System.IO.File.WriteAllText(playlistPath, json);
                    MessageBox.Show("播放列表保存成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存播放列表失败：{ex.Message}");
            }
        }

        // 从文件加载播放列表
        private void LoadPlaylist()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.FileName = "Playlist.json";
                openFileDialog.Title = "加载播放列表";
                openFileDialog.DefaultExt = "json";

                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;

                string playlistPath = openFileDialog.FileName;
                currentPlaylistPath = playlistPath;
                
                if (!System.IO.File.Exists(playlistPath))
                {
                    MessageBox.Show("未找到保存的播放列表文件。");
                    return;
                }

                string json = System.IO.File.ReadAllText(playlistPath);
                var playlist = JsonConvert.DeserializeObject<Playlist>(json);

                if (playlist != null)
                {
                    listViewPlaylist.Items.Clear();
                    foreach (var music in playlist.Songs)
                    {
                        var item = new ListViewItem(music.Title);
                        item.SubItems.Add(music.Artist);
                        item.SubItems.Add(music.Album);
                        item.SubItems.Add(music.Genre);
                        item.SubItems.Add(music.Duration);
                        item.Tag = music;
                        listViewPlaylist.Items.Add(item);
                    }

                    AddToPlaylistHistory(playlistPath, playlist);
                    MessageBox.Show("播放列表加载成功！");
                }

                isPlaylistLoaded = true;
                SortArtistBox.Checked = false;
                SortAlbumBox.Checked = false;
                SortGenreBox.Checked = false;
                originalPlaylistOrder.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载播放列表失败：{ex.Message}");
            }
        }

        // 编辑播放列表（添加/删除歌曲）
        private void EditPlaylist()
        {
            if (MessageBox.Show("你想做什么？\n是：添加音乐\n否：删除选中的音乐",
                "编辑播放列表", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                AddMusicDialog();
            }
            else if (MessageBox.Show("确定要从播放列表中删除选中的音乐吗？",
                "确认删除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (ListViewItem selectedItem in listViewPlaylist.SelectedItems)
                {
                    listViewPlaylist.Items.Remove(selectedItem);
                }
            }
        }

        // 清空播放列表
        public void clearPlaylist()
        {
            listViewPlaylist.Items.Clear();
            currentSongIndex = -1;
            isPlaying = false;
            wplayer.controls.stop();
            UpdatePlayButtonText();
            UpdateNowPlayingInfo(null);

            isPlaylistLoaded = false;
            SortArtistBox.Checked = false;
            SortAlbumBox.Checked = false;
            SortGenreBox.Checked = false;
            originalPlaylistOrder.Clear();
        }

        // =============================================
        // 8. 播放列表历史记录功能
        // =============================================
        
        // 显示历史记录窗体
        private void ShowHistoryPlaylist()
        {
            using (var historyForm = new HistoryForm(playlistHistory))
            {
                historyForm.PlaylistSelected += (historyItem) =>
                {
                    LoadPlaylistFromHistory(historyItem);
                    historyForm.Close();
                };
                
                historyForm.ShowDialog(this);
            }
        }

        // 从历史记录加载播放列表
        private void LoadPlaylistFromHistory(PlaylistHistoryItem historyItem)
        {
            if (!File.Exists(historyItem.Path))
            {
                MessageBox.Show($"播放列表文件未找到：\n{historyItem.Path}");
                return;
            }

            try
            {
                string json = File.ReadAllText(historyItem.Path);
                var playlist = JsonConvert.DeserializeObject<Playlist>(json);

                if (playlist != null)
                {
                    listViewPlaylist.Items.Clear();
                    foreach (var music in playlist.Songs)
                    {
                        var item = new ListViewItem(music.Title);
                        item.SubItems.Add(music.Artist);
                        item.SubItems.Add(music.Album);
                        item.SubItems.Add(music.Genre);
                        item.SubItems.Add(music.Duration);
                        item.Tag = music;
                        listViewPlaylist.Items.Add(item);
                    }
                    
                    currentPlaylistPath = historyItem.Path;
                    historyItem.LastAccessed = DateTime.Now;
                    SavePlaylistHistory();
                    MessageBox.Show($"播放列表 '{historyItem.Name}' 加载成功！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载播放列表失败：{ex.Message}");
            }
        }

        // 添加到历史记录
        private void AddToPlaylistHistory(string playlistPath, Playlist playlist)
        {
            try
            {
                string playlistName = playlist.Name ?? Path.GetFileNameWithoutExtension(playlistPath);
                
                var existing = playlistHistory.FirstOrDefault(p => p.Path.Equals(playlistPath, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    existing.LastAccessed = DateTime.Now;
                    existing.SongCount = playlist.Songs?.Count ?? 0;
                    playlistHistory.Remove(existing);
                    playlistHistory.Insert(0, existing);
                }
                else
                {
                    var historyItem = new PlaylistHistoryItem
                    {
                        Name = playlistName,
                        Path = playlistPath,
                        LastAccessed = DateTime.Now,
                        SongCount = playlist.Songs?.Count ?? 0
                    };
                    
                    playlistHistory.Insert(0, historyItem);
                    
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

        // 保存历史记录到文件
        private void SavePlaylistHistory()
        {
            try
            {
                string historyPath = Path.Combine(Application.StartupPath, "playlist_history.json");
                string json = JsonConvert.SerializeObject(playlistHistory, Formatting.Indented);
                File.WriteAllText(historyPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存播放列表历史记录时出错：{ex.Message}");
            }
        }

        // 从文件加载历史记录
        private void LoadPlaylistHistory()
        {
            try
            {
                string historyPath = Path.Combine(Application.StartupPath, "playlist_history.json");
                if (File.Exists(historyPath))
                {
                    string json = File.ReadAllText(historyPath);
                    playlistHistory = JsonConvert.DeserializeObject<List<PlaylistHistoryItem>>(json) ?? new List<PlaylistHistoryItem>();
                    
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

        // =============================================
        // 9. 播放列表排序功能
        // =============================================
        
        // 排序复选框事件处理（单选逻辑）
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

        // 按艺术家排序
        private void SortByArtist()
        {
            if (!isPlaylistLoaded || listViewPlaylist.Items.Count == 0) return;
            
            if (originalPlaylistOrder.Count == 0)
            {
                SaveOriginalOrder();
            }
            
            var sortedMusic = GetMusicListFromListView()
                .OrderBy(m => m.Artist)
                .ThenBy(m => m.Title)
                .ToList();
            
            UpdateListViewWithMusic(sortedMusic);
        }

        // 按专辑排序
        private void SortByAlbum()
        {
            if (!isPlaylistLoaded || listViewPlaylist.Items.Count == 0) return;
            
            if (originalPlaylistOrder.Count == 0)
            {
                SaveOriginalOrder();
            }
            
            var sortedMusic = GetMusicListFromListView()
                .OrderBy(m => m.Album)
                .ThenBy(m => m.Title)
                .ToList();
            
            UpdateListViewWithMusic(sortedMusic);
        }

        // 按流派排序
        private void SortByGenre()
        {
            if (!isPlaylistLoaded || listViewPlaylist.Items.Count == 0) return;
            
            if (originalPlaylistOrder.Count == 0)
            {
                SaveOriginalOrder();
            }
            
            var sortedMusic = GetMusicListFromListView()
                .OrderBy(m => m.Genre)
                .ThenBy(m => m.Title)
                .ToList();
            
            UpdateListViewWithMusic(sortedMusic);
        }

        // 恢复原始顺序
        private void RestoreOriginalOrder()
        {
            if (!isPlaylistLoaded || originalPlaylistOrder.Count == 0) return;
            
            UpdateListViewWithMusic(originalPlaylistOrder);
            originalPlaylistOrder.Clear();
        }

        // 保存原始顺序
        private void SaveOriginalOrder()
        {
            originalPlaylistOrder.Clear();
            originalPlaylistOrder.AddRange(GetMusicListFromListView());
        }

        // 从 ListView 获取 Music 列表
        private List<Music> GetMusicListFromListView()
        {
            List<Music> musicList = new List<Music>();
            
            foreach (ListViewItem item in listViewPlaylist.Items)
            {
                var music = item.Tag as Music;
                if (music != null)
                {
                    musicList.Add(music);
                }
            }
            
            return musicList;
        }

        // 用 Music 列表更新 ListView
        private void UpdateListViewWithMusic(List<Music> musicList)
        {
            listViewPlaylist.Items.Clear();
            
            foreach (var music in musicList)
            {
                var item = new ListViewItem(music.Title);
                item.SubItems.Add(music.Artist);
                item.SubItems.Add(music.Album);
                item.SubItems.Add(music.Genre);
                item.SubItems.Add(music.Duration);
                item.Tag = music;
                listViewPlaylist.Items.Add(item);
            }
        }

        // =============================================
        // 10. 辅助方法和事件处理
        // =============================================
        
        // 双击播放列表项事件
        private void ListViewPlaylist_DoubleClick(object sender, EventArgs e)
        {
            if (listViewPlaylist.SelectedItems.Count > 0)
            {
                var selectedItem = listViewPlaylist.SelectedItems[0];
                var music = selectedItem.Tag as Music;
                if (music != null)
                {
                    PlayMusic(music);
                }
            }
        }

        // 格式化工具体方法
        private string FormatTime(double duration)
        {
            TimeSpan time = TimeSpan.FromSeconds(duration);
            return time.ToString(@"mm\:ss");
        }

        // 空的事件处理方法（保持设计器兼容性）
        private void SortArtistBox_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}