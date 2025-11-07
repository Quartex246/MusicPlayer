namespace player
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.PlayButton = new System.Windows.Forms.Button();
            this.NextButton = new System.Windows.Forms.Button();
            this.PrevButton = new System.Windows.Forms.Button();
            this.SongName = new System.Windows.Forms.Label();
            this.Artist = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.SortArtistBox = new System.Windows.Forms.CheckBox();
            this.SortAlbumBox = new System.Windows.Forms.CheckBox();
            this.SortGenreBox = new System.Windows.Forms.CheckBox();
            this.TextSortBy = new System.Windows.Forms.Label();
            this.listViewPlaylist = new System.Windows.Forms.ListView();
            this.PLICreateButton = new System.Windows.Forms.Button();
            this.PLIHistoryButton = new System.Windows.Forms.Button();
            this.SortLengthBox = new System.Windows.Forms.CheckBox();
            this.PLIEditButton = new System.Windows.Forms.Button();
            this.PLISelectButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.PLIDeleteButton = new System.Windows.Forms.Button();
            this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
            this.SuspendLayout();
            // 
            // PlayButton
            // 
            this.PlayButton.Location = new System.Drawing.Point(355, 375);
            this.PlayButton.Name = "PlayButton";
            this.PlayButton.Size = new System.Drawing.Size(75, 34);
            this.PlayButton.TabIndex = 1;
            this.PlayButton.Text = "Play";
            this.PlayButton.UseVisualStyleBackColor = true;
            this.PlayButton.Click += new System.EventHandler(this.PlayButton_Click);
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(436, 375);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(75, 34);
            this.NextButton.TabIndex = 2;
            this.NextButton.Text = "Next";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // PrevButton
            // 
            this.PrevButton.Location = new System.Drawing.Point(274, 375);
            this.PrevButton.Name = "PrevButton";
            this.PrevButton.Size = new System.Drawing.Size(75, 34);
            this.PrevButton.TabIndex = 3;
            this.PrevButton.Text = "Prev";
            this.PrevButton.UseVisualStyleBackColor = true;
            this.PrevButton.Click += new System.EventHandler(this.PrevButton_Click);
            // 
            // SongName
            // 
            this.SongName.AutoSize = true;
            this.SongName.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SongName.Location = new System.Drawing.Point(7, 134);
            this.SongName.Name = "SongName";
            this.SongName.Size = new System.Drawing.Size(328, 69);
            this.SongName.TabIndex = 4;
            this.SongName.Text = "SongName";
            // 
            // Artist
            // 
            this.Artist.AutoSize = true;
            this.Artist.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.Artist.Location = new System.Drawing.Point(12, 102);
            this.Artist.Name = "Artist";
            this.Artist.Size = new System.Drawing.Size(84, 36);
            this.Artist.TabIndex = 5;
            this.Artist.Text = "Artist";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(12, 269);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(104, 56);
            this.trackBar1.TabIndex = 7;
            // 
            // SortArtistBox
            // 
            this.SortArtistBox.AutoSize = true;
            this.SortArtistBox.Location = new System.Drawing.Point(472, 21);
            this.SortArtistBox.Name = "SortArtistBox";
            this.SortArtistBox.Size = new System.Drawing.Size(63, 21);
            this.SortArtistBox.TabIndex = 11;
            this.SortArtistBox.Text = "Artist";
            this.SortArtistBox.UseVisualStyleBackColor = true;
            this.SortArtistBox.CheckedChanged += new System.EventHandler(this.SortArtistBox_CheckedChanged);
            // 
            // SortAlbumBox
            // 
            this.SortAlbumBox.AutoSize = true;
            this.SortAlbumBox.Location = new System.Drawing.Point(547, 21);
            this.SortAlbumBox.Name = "SortAlbumBox";
            this.SortAlbumBox.Size = new System.Drawing.Size(69, 21);
            this.SortAlbumBox.TabIndex = 12;
            this.SortAlbumBox.Text = "Album";
            this.SortAlbumBox.UseVisualStyleBackColor = true;
            // 
            // SortGenreBox
            // 
            this.SortGenreBox.AutoSize = true;
            this.SortGenreBox.Location = new System.Drawing.Point(629, 21);
            this.SortGenreBox.Name = "SortGenreBox";
            this.SortGenreBox.Size = new System.Drawing.Size(66, 21);
            this.SortGenreBox.TabIndex = 13;
            this.SortGenreBox.Text = "Genre";
            this.SortGenreBox.UseVisualStyleBackColor = true;
            // 
            // TextSortBy
            // 
            this.TextSortBy.AutoSize = true;
            this.TextSortBy.Location = new System.Drawing.Point(398, 22);
            this.TextSortBy.Name = "TextSortBy";
            this.TextSortBy.Size = new System.Drawing.Size(53, 17);
            this.TextSortBy.TabIndex = 14;
            this.TextSortBy.Text = "Sort by:";
            // 
            // listViewPlaylist
            // 
            this.listViewPlaylist.FullRowSelect = true;
            this.listViewPlaylist.GridLines = true;
            this.listViewPlaylist.HideSelection = false;
            this.listViewPlaylist.Location = new System.Drawing.Point(393, 41);
            this.listViewPlaylist.Name = "listViewPlaylist";
            this.listViewPlaylist.Size = new System.Drawing.Size(371, 257);
            this.listViewPlaylist.TabIndex = 15;
            this.listViewPlaylist.UseCompatibleStateImageBehavior = false;
            this.listViewPlaylist.View = System.Windows.Forms.View.Details;
            // 
            // PLICreateButton
            // 
            this.PLICreateButton.Location = new System.Drawing.Point(393, 332);
            this.PLICreateButton.Name = "PLICreateButton";
            this.PLICreateButton.Size = new System.Drawing.Size(61, 24);
            this.PLICreateButton.TabIndex = 17;
            this.PLICreateButton.Text = "Create";
            this.PLICreateButton.UseVisualStyleBackColor = true;
            this.PLICreateButton.Click += new System.EventHandler(this.PLICreateButton_Click);
            // 
            // PLIHistoryButton
            // 
            this.PLIHistoryButton.Location = new System.Drawing.Point(681, 332);
            this.PLIHistoryButton.Name = "PLIHistoryButton";
            this.PLIHistoryButton.Size = new System.Drawing.Size(83, 24);
            this.PLIHistoryButton.TabIndex = 16;
            this.PLIHistoryButton.Text = "History";
            this.PLIHistoryButton.UseVisualStyleBackColor = true;
            // 
            // SortLengthBox
            // 
            this.SortLengthBox.AutoSize = true;
            this.SortLengthBox.Location = new System.Drawing.Point(705, 22);
            this.SortLengthBox.Name = "SortLengthBox";
            this.SortLengthBox.Size = new System.Drawing.Size(71, 21);
            this.SortLengthBox.TabIndex = 18;
            this.SortLengthBox.Text = "Length";
            this.SortLengthBox.UseVisualStyleBackColor = true;
            // 
            // PLIEditButton
            // 
            this.PLIEditButton.Location = new System.Drawing.Point(460, 332);
            this.PLIEditButton.Name = "PLIEditButton";
            this.PLIEditButton.Size = new System.Drawing.Size(61, 24);
            this.PLIEditButton.TabIndex = 19;
            this.PLIEditButton.Text = "Edit";
            this.PLIEditButton.UseVisualStyleBackColor = true;
            // 
            // PLISelectButton
            // 
            this.PLISelectButton.Location = new System.Drawing.Point(527, 332);
            this.PLISelectButton.Name = "PLISelectButton";
            this.PLISelectButton.Size = new System.Drawing.Size(61, 24);
            this.PLISelectButton.TabIndex = 20;
            this.PLISelectButton.Text = "Select";
            this.PLISelectButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(390, 309);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 16);
            this.label1.TabIndex = 21;
            this.label1.Text = "Playlist Settings";
            // 
            // PLIDeleteButton
            // 
            this.PLIDeleteButton.Location = new System.Drawing.Point(594, 332);
            this.PLIDeleteButton.Name = "PLIDeleteButton";
            this.PLIDeleteButton.Size = new System.Drawing.Size(61, 24);
            this.PLIDeleteButton.TabIndex = 22;
            this.PLIDeleteButton.Text = "Delete";
            this.PLIDeleteButton.UseVisualStyleBackColor = true;
            // 
            // axWindowsMediaPlayer1
            // 
            this.axWindowsMediaPlayer1.Enabled = true;
            this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(724, 375);
            this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
            this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
            this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(40, 45);
            this.axWindowsMediaPlayer1.TabIndex = 9;
            this.axWindowsMediaPlayer1.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 421);
            this.Controls.Add(this.PLIDeleteButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PLISelectButton);
            this.Controls.Add(this.PLIEditButton);
            this.Controls.Add(this.SortLengthBox);
            this.Controls.Add(this.PLICreateButton);
            this.Controls.Add(this.PLIHistoryButton);
            this.Controls.Add(this.listViewPlaylist);
            this.Controls.Add(this.TextSortBy);
            this.Controls.Add(this.SortGenreBox);
            this.Controls.Add(this.SortAlbumBox);
            this.Controls.Add(this.SortArtistBox);
            this.Controls.Add(this.axWindowsMediaPlayer1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.Artist);
            this.Controls.Add(this.SongName);
            this.Controls.Add(this.PrevButton);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.PlayButton);
            this.Font = new System.Drawing.Font("Times New Roman", 9F);
            this.Name = "Form1";
            this.Text = "Music Player";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button PlayButton;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.Button PrevButton;
        private System.Windows.Forms.Label SongName;
        private System.Windows.Forms.Label Artist;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.CheckBox SortArtistBox;
        private System.Windows.Forms.CheckBox SortAlbumBox;
        private System.Windows.Forms.CheckBox SortGenreBox;
        private System.Windows.Forms.Label TextSortBy;
        private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
        private System.Windows.Forms.ListView listViewPlaylist;
        private System.Windows.Forms.Button PLICreateButton;
        private System.Windows.Forms.Button PLIHistoryButton;
        private System.Windows.Forms.CheckBox SortLengthBox;
        private System.Windows.Forms.Button PLIEditButton;
        private System.Windows.Forms.Button PLISelectButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button PLIDeleteButton;
    }
}

