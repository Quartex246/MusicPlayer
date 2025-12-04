using Newtonsoft.Json; // With JSON extension to make playlist
using player.Models; // With "Music" & "Playlist" function included
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
        private Music currentMusic;   
        private bool isPlaying = false; // Music playing status
        private int currentSongIndex = -1; // Track current song position in playlist
        private string currentPlaylistPath = null;

        // Related with history playlist
        private List<PlaylistHistoryItem> playlistHistory = new List<PlaylistHistoryItem>();
        private const int MAX_PLAYLIST_HISTORY_SIZE = 20;  // Max history size

        // 用于保存原始顺序的列表
        private List<Music> originalPlaylistOrder = new List<Music>();
        private bool isPlaylistLoaded = false;

        public Form1()
        {
            InitializeComponent();
            SetupPlaylistColumns();
            listViewPlaylist.DoubleClick += ListViewPlaylist_DoubleClick; // Double click to play function

            LoadPlaylistHistory();

            SortArtistBox.CheckedChanged += SortCheckBox_CheckedChanged;
            SortAlbumBox.CheckedChanged += SortCheckBox_CheckedChanged;
            SortGenreBox.CheckedChanged += SortCheckBox_CheckedChanged;
        }

        private void ListViewPlaylist_DoubleClick(object sender, EventArgs e)
        { // Double click to play the item
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

        WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer(); // Adding Windows Media Player

        // It means we can use WMP functions in this program, and we name it "wplayer"
        // Using "new" means we create a new instance of it

        private void SetupPlaylistColumns() //initialize playlist section
        {
            // 清空现有列
            listViewPlaylist.Columns.Clear();

            // 添加列
            listViewPlaylist.Columns.Add("Title", 75);
            listViewPlaylist.Columns.Add("Artist", 75);
            listViewPlaylist.Columns.Add("Album", 75);
            listViewPlaylist.Columns.Add("Genre", 75);
            listViewPlaylist.Columns.Add("Length", 75);
        }
        public class PlaylistHistoryItem
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public DateTime LastAccessed { get; set; }
            public int SongCount { get; set; }
            
            // 用于ListView显示的友好名称
            public override string ToString()
            {
                return $"{Name} ({SongCount} songs) - {LastAccessed:yyyy-MM-dd}";
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void PlayMusic(Music music)

        // The function to play music

        {
            if (music == null || string.IsNullOrEmpty(music.FilePath))
            {
                MessageBox.Show("Cannot play: Music file directory invalid");
                return;
            }

            currentMusic = music;

            // Find and set the current song index in the playlist
            for (int i = 0; i < listViewPlaylist.Items.Count; i++)
            {
                if (listViewPlaylist.Items[i].Tag == music)
                {
                    currentSongIndex = i;
                    listViewPlaylist.Items[i].Selected = true; // Highlight current song
                    break;
                }
            }

            try
            {
                wplayer.URL = music.FilePath;
                wplayer.controls.play();

                isPlaying = true;
                UpdatePlayButtonText();
                UpdateNowPlayingInfo(music);
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Cannot play：{ex.Message}");
            }

        }

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
            /* If the music is playing, do pause *
             * If not, do play. */

            UpdatePlayButtonText();
        }

        private void UpdatePlayButtonText()
        {
            PlayButton.Text = isPlaying ? "Pause" : "Play";
            /* Originally I use a "if-else" method to change the text. 
             * This looks so much better */
        }

        private void UpdateNowPlayingInfo(Music music)
        {
            if (music == null)
            {
                SongName.Text = "No song";
                Artist.Text = "";
                return;
            }

            SongName.Text = music.Title ?? "Unknown Title";
            Artist.Text = music.Artist ?? "Unknown Artist";
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            PlayPrevSong(); 
            
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            PlayNextSong();
        }

        private void PlayNextSong()
        {
            // If no song is currently playing, start from the first one
            if (currentSongIndex < 0 || currentSongIndex >= listViewPlaylist.Items.Count)
            {
                if (listViewPlaylist.Items.Count > 0)
                {
                    currentSongIndex = 0;
                }
                else
                {
                    MessageBox.Show("Playlist is empty");
                    return;
                }
            }
            // Move to next song, loop back to start if at the end
            else
            {
                currentSongIndex++;
                if (currentSongIndex >= listViewPlaylist.Items.Count)
                {
                    currentSongIndex = 0; // Loop to beginning
                }
            }

            var nextItem = listViewPlaylist.Items[currentSongIndex];
            var music = nextItem.Tag as Music;
            if (music != null)
            {
                PlayMusic(music);
            }
        }

        private void PlayPrevSong()
        {
            // If no song is currently playing, start from the last one
            if (currentSongIndex < 0 || currentSongIndex >= listViewPlaylist.Items.Count)
            {
                if (listViewPlaylist.Items.Count > 0)
                {
                    currentSongIndex = listViewPlaylist.Items.Count - 1;
                }
                else
                {
                    MessageBox.Show("Playlist is empty");
                    return;
                }
            }
            // Move to previous song, loop to end if at the start
            else
            {
                currentSongIndex--;
                if (currentSongIndex < 0)
                {
                    currentSongIndex = listViewPlaylist.Items.Count - 1; // Loop to end
                }
            }

            var prevItem = listViewPlaylist.Items[currentSongIndex];
            var music = prevItem.Tag as Music;
            if (music != null)
            {
                PlayMusic(music);
            }
        }

        private void SortArtistBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void PLICreateButton_Click(object sender, EventArgs e)
        {
            clearPlaylist();

            AddMusicDialog();

            currentPlaylistPath = null;
            
            SavePlaylist();
        }

        private void AddMusicDialog()
        {
            // Open the file manager and select music
            // Using List function
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "音频文件|*.mp3;*.wav;*.wma;*.aac|所有文件|*.*"; // Filter
                openFileDialog.Multiselect = true; // Allow Multi-select
                openFileDialog.Title = "Select Music File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in openFileDialog.FileNames)
                    {
                        AddMusicToPlaylist(filePath);
                    }
                }
                else
                {
                    MessageBox.Show("No music selected.");
                }
            }
        }
        private void AddMusicToPlaylist(string filePath)
        {
            try
            {
                // Using Windows Media Player to fetch info
                var tempPlayer = new WMPLib.WindowsMediaPlayer();
                var media = tempPlayer.newMedia(filePath);

                // Create Music Object
                var music = new Music
                {
                    FilePath = filePath,
                    Title = string.IsNullOrEmpty(media.getItemInfo("Title")) ?
                           System.IO.Path.GetFileNameWithoutExtension(filePath) : media.getItemInfo("Title"),

                    Artist = string.IsNullOrEmpty(media.getItemInfo("Artist")) ?
                            "Unknown Artist" : media.getItemInfo("Artist"),

                    Album = string.IsNullOrEmpty(media.getItemInfo("Album")) ?
                           "Unknown Album" : media.getItemInfo("Album"),

                    Genre = string.IsNullOrEmpty(media.getItemInfo("Genre")) ?
                           "Unknown Genre" : media.getItemInfo("Genre"),

                    Duration = FormatTime(media.duration)
                };

                // 添加到 ListView
                var item = new ListViewItem(music.Title);
                item.SubItems.Add(music.Artist);
                item.SubItems.Add(music.Album);
                item.SubItems.Add(music.Genre);
                item.SubItems.Add(music.Duration);
                item.Tag = music; // 保存 Music 对象引用

                listViewPlaylist.Items.Add(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add file：{ex.Message}");
            }
        }

        // Note to self: The function above is to add music to playlist
        // It is not only needed when creating playlist, but also when editing playlist

        private string FormatTime(double duration)
        {
            TimeSpan time = TimeSpan.FromSeconds(duration);
            return time.ToString(@"mm\:ss");
        }

        private void PLIEditButton_Click(object sender, EventArgs e)
        {
            EditPlaylist();
        }

        private void SavePlaylist()
        {
            try
            {
                // Create a playlist from current ListView items
                var playlist = new Playlist("CurrentPlaylist");
                
                foreach (ListViewItem item in listViewPlaylist.Items)
                {
                    var music = item.Tag as Music;
                    if (music != null)
                    {
                        playlist.AddSong(music);
                    }
                }

                // Serialize to JSON and save to file



                SaveFileDialog saveFileDialog = new SaveFileDialog();

                // 设置对话框属性
                saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1; // 默认选择 JSON 格式
                saveFileDialog.FileName = "Playlist.json"; // 默认文件名
                saveFileDialog.Title = "Save Playlist"; // 对话框标题
                saveFileDialog.DefaultExt = "json"; // 默认扩展名

                // 显示对话框并检查用户是否点击了"保存"
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 序列化对象
                    string json = JsonConvert.SerializeObject(playlist, Formatting.Indented);
                    
                    // 获取用户选择的路径
                    string playlistPath = saveFileDialog.FileName;
                    
                    // 保存文件
                    System.IO.File.WriteAllText(playlistPath, json);
                    MessageBox.Show("Playlist saved successfully!");
                }            

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save playlist: {ex.Message}");
            }
        }


        private void PLISelectButton_Click(object sender, EventArgs e)
        {
            LoadPlaylist();
        }

        private void LoadPlaylist()
        {
           try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.FileName = "Playlist.json";
                openFileDialog.Title = "Load Playlist";
                openFileDialog.DefaultExt = "json";

                // 如果用户没有选择文件或点击取消，直接返回
                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                        return;

                    string playlistPath = openFileDialog.FileName;
                    currentPlaylistPath = playlistPath;
                
                    if (!System.IO.File.Exists(playlistPath))
                    {
                        MessageBox.Show("No saved playlist found.");
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

                    MessageBox.Show("Playlist loaded successfully!");
                }

                // 设置播放列表已加载标志
                    isPlaylistLoaded = true;
                    
                // 清除任何选中的排序选项
                SortArtistBox.Checked = false;
                SortAlbumBox.Checked = false;
                SortGenreBox.Checked = false;
                    
                // 清除保存的原始顺序
                originalPlaylistOrder.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load playlist: {ex.Message}");
            }
        }

        private void AddToPlaylistHistory(string playlistPath, Playlist playlist)
        {
            try
            {
                string playlistName = playlist.Name ?? Path.GetFileNameWithoutExtension(playlistPath);
                
                // 检查是否已存在
                var existing = playlistHistory.FirstOrDefault(p => p.Path.Equals(playlistPath, StringComparison.OrdinalIgnoreCase));
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
                
                // 保存历史记录到文件
                SavePlaylistHistory();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding to playlist history: {ex.Message}");
            }
        }
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
                Console.WriteLine($"Error saving playlist history: {ex.Message}");
            }
        }

        private void LoadPlaylistHistory()
        {
            try
            {
                string historyPath = Path.Combine(Application.StartupPath, "playlist_history.json");
                if (File.Exists(historyPath))
                {
                    string json = File.ReadAllText(historyPath);
                    playlistHistory = JsonConvert.DeserializeObject<List<PlaylistHistoryItem>>(json) ?? new List<PlaylistHistoryItem>();
                    
                    // 确保不超过最大数量
                    if (playlistHistory.Count > MAX_PLAYLIST_HISTORY_SIZE)
                    {
                        playlistHistory = playlistHistory.Take(MAX_PLAYLIST_HISTORY_SIZE).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading playlist history: {ex.Message}");
                playlistHistory = new List<PlaylistHistoryItem>();
            }
        }


        private void EditPlaylist()
        {
            // Open a dialog to edit the playlist
            if(MessageBox.Show("What would you like to do?\nYes: Add Music\nNo: Remove Selected Music",
            "Edit Playlist", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
            {
                AddMusicDialog();
            }
            // TODO: Edit function should be disabled when there is no playlist loaded

            else if (MessageBox.Show("Are you sure you want to remove the selected music from the playlist?",
            "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            // Even when the closing the dialog, it will still pop up.
            {
                // Remove selected music
                foreach (ListViewItem selectedItem in listViewPlaylist.SelectedItems)
                {
                    listViewPlaylist.Items.Remove(selectedItem);
                }
                // It's removing the selected music from the listview instead of the playlist object
            }
        }
        private void PLIDeleteButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPlaylistPath))
            {
                MessageBox.Show("No playlist file is currently loaded or the playlist hasn't been saved yet.");
                return;
            }

            // Delete the playlist
            if (MessageBox.Show("Are you sure you want to delete the entire playlist?", 
            "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                System.IO.File.Delete(currentPlaylistPath);
                clearPlaylist();
                MessageBox.Show("Playlist deleted successfully!");
            }
        }

        public void clearPlaylist()
        {
            listViewPlaylist.Items.Clear();
            currentSongIndex = -1;
            isPlaying = false;
            wplayer.controls.stop();
            UpdatePlayButtonText();
            UpdateNowPlayingInfo(null);

            // 重置播放列表加载标志
            isPlaylistLoaded = false;
            
            // 清除排序选项
            SortArtistBox.Checked = false;
            SortAlbumBox.Checked = false;
            SortGenreBox.Checked = false;
            
            // 清除保存的原始顺序
            originalPlaylistOrder.Clear();
        }

        private void PLIHistoryButton_Click(object sender, EventArgs e)
        {
            ShowHistoryPlaylist();
        }

        private void ShowHistoryPlaylist()
        {
            using (var historyForm = new HistoryForm(playlistHistory))
            {
                historyForm.PlaylistSelected += (historyItem) =>
                {
                    // 加载选中的播放列表
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
                MessageBox.Show($"Playlist file not found:\n{historyItem.Path}");
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
                    
                    // 更新访问时间
                    historyItem.LastAccessed = DateTime.Now;
                    SavePlaylistHistory();
                    
                    MessageBox.Show($"Playlist '{historyItem.Name}' loaded successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load playlist: {ex.Message}");
            }
        }

                private void SortCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox clickedCheckBox = sender as CheckBox;
            
            // 如果当前CheckBox被勾选，确保其他CheckBox被取消勾选
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
            // 如果所有CheckBox都被取消勾选，恢复原始顺序
            else if (!SortArtistBox.Checked && !SortAlbumBox.Checked && !SortGenreBox.Checked)
            {
                RestoreOriginalOrder();
            }
        }
        
        private void SortByArtist()
        {
            if (!isPlaylistLoaded || listViewPlaylist.Items.Count == 0) return;
            
            // 保存当前顺序（如果尚未保存）
            if (originalPlaylistOrder.Count == 0)
            {
                SaveOriginalOrder();
            }
            
            // 获取所有音乐并按艺术家排序
            var sortedMusic = GetMusicListFromListView()
                .OrderBy(m => m.Artist)
                .ThenBy(m => m.Title)
                .ToList();
            
            // 更新ListView
            UpdateListViewWithMusic(sortedMusic);
        }
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

        private void RestoreOriginalOrder()
        {
            if (!isPlaylistLoaded || originalPlaylistOrder.Count == 0) return;
            
            // 恢复原始顺序
            UpdateListViewWithMusic(originalPlaylistOrder);
            
            // 清除保存的原始顺序（以便下次可以重新保存）
            originalPlaylistOrder.Clear();
        }

        private void SaveOriginalOrder()
        {
            originalPlaylistOrder.Clear();
            originalPlaylistOrder.AddRange(GetMusicListFromListView());
        }

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

        private void UpdateListViewWithMusic(List<Music> musicList)
        {
            // 清除当前ListView
            listViewPlaylist.Items.Clear();
            
            // 添加排序后的音乐
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

    }
}
