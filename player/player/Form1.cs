using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using player.Models;

namespace player
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetupPlaylistColumns();
        }
        private void SetupPlaylistColumns()
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

        private void PlayButton_Click(object sender, EventArgs e)
        {

        }

        private void PauseButton_Click(object sender, EventArgs e)
        {

        }

        private void PrevButton_Click(object sender, EventArgs e)
        {

        }

        private void NextButton_Click(object sender, EventArgs e)
        {

        }

        private void SortArtistBox_CheckedChanged(object sender, EventArgs e)
        {

        }


    }
}
