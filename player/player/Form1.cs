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
using Microsoft.VisualBasic;

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
        }
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
            currentMusic = music;

            // 加载并播放音乐
            axWindowsMediaPlayer1.URL = music.FilePath;
            axWindowsMediaPlayer1.Ctlcontrols.play();

            isPlaying = true;
            UpdatePlayButtonText();


        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                axWindowsMediaPlayer1.Ctlcontrols.pause();
                isPlaying = false;
            }
            else
            {
                axWindowsMediaPlayer1.Ctlcontrols.play();
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
            axWindowsMediaPlayer1.Ctlcontrols.next();
        }

        private void PlayPrevSong()
        {
            axWindowsMediaPlayer1.Ctlcontrols.previous();
        }

        private void SortArtistBox_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void PLICreateButton_Click(object sender, EventArgs e)
        {

            // Open the file manager and select music
            // Using List function

        }
    }
}
