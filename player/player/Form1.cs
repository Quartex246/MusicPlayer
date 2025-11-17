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

        WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer(); // Adding Windows Media Player (? why is it written like that)

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
        {
            if (music == null || string.IsNullOrEmpty(music.FilePath))
            {
                MessageBox.Show("Cannot play: Music file directory invalid");
                return;
            }

            currentMusic = music;

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
            SongName.Text = music.Title;
            Artist.Text = music.Artist;
        }

        private void PrevButton_Click(object sender, EventArgs e)
        {
            PlayNextSong();
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            PlayPrevSong();
        }

        private void PlayNextSong()
        {
            wplayer.controls.next();
        }

        private void PlayPrevSong()
        {
            wplayer.controls.previous();
        }

        private void SortArtistBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void PLICreateButton_Click(object sender, EventArgs e)
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

        private string FormatTime(double duration)
        {
            TimeSpan time = TimeSpan.FromSeconds(duration);
            return time.ToString(@"mm\:ss");
        }
    }
}
