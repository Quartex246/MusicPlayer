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

namespace player
{
    public partial class Form1 : Form
    {
        private Music currentMusic;   
        private bool isPlaying = false; // Music playing status
        private int currentSongIndex = -1; // Track current song position in playlist
        private string currentPlaylistPath = null;

        public Form1()
        {
            InitializeComponent();
            SetupPlaylistColumns();
            listViewPlaylist.DoubleClick += ListViewPlaylist_DoubleClick; // Double click to play function (? why is it written like that)
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
            }

            SavePlaylist();
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
            // Edit playlist function
            // Open a dialog with current playlist items
            // Allow adding/removing songs
            EditPlaylist();
            // TODO: How to create a dialog with buttons to add/remove songs?
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

                // Note to self: No matter if the playlist is empty or not, it is still pop out a window to save the playlist
                // Multiple playlists should be supported.

                // Bug: The song will be added whether if the music is already in the playlist or not.
                // Bug: Add song function should be related to the edit button, not the create button.
                // Detect if the playlist is loaded or not. If so, create a new playlist when clicking create button.
                
            

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
                    MessageBox.Show("Playlist loaded successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load playlist: {ex.Message}");
            }
        }

        private void EditPlaylist()
        {
            // Open a dialog to edit the playlist
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
            // It end up with a "clear playlist" function
            // could do.
        }

        public void clearPlaylist()
        {
            listViewPlaylist.Items.Clear();
            currentSongIndex = -1;
            isPlaying = false;
            wplayer.controls.stop();
            UpdatePlayButtonText();
            UpdateNowPlayingInfo(null);
        }
    }
}
