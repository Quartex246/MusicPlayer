using System;
using System.Collections.Generic;
using System.Windows.Forms;
using player.Models;

namespace player
{
    public partial class HistoryForm : Form
    {
        private List<Form1.PlaylistHistoryItem> historyList;
        public event Action<Form1.PlaylistHistoryItem> PlaylistSelected;
        public HistoryForm(List <Form1.PlaylistHistoryItem> history)
        {
            InitializeComponent();
            historyList = history;
            SetupColumns();
            LoadHistory();
        }

        private void SetupColumns()
        {
            listViewHistory.Columns.Clear();
            listViewHistory.Columns.Add("Playlist Name", 150);
            listViewHistory.Columns.Add("Song Count", 100);
            listViewHistory.Columns.Add("Last Accessed", 120);
            listViewHistory.Columns.Add("Path", 250);
        }
        
        private void LoadHistory()
        {
            listViewHistory.Items.Clear();
            
            foreach (var item in historyList)
            {
                var listItem = new ListViewItem(item.Name);
                listItem.SubItems.Add(item.SongCount.ToString());
                listItem.SubItems.Add(item.LastAccessed.ToString("yyyy-MM-dd HH:mm"));
                listItem.SubItems.Add(item.Path);
                listItem.Tag = item;
                listViewHistory.Items.Add(listItem);
            }
        }
        
        private void listViewHistory_DoubleClick(object sender, EventArgs e)
        {
            if (listViewHistory.SelectedItems.Count > 0)
            {
                var selectedItem = listViewHistory.SelectedItems[0];
                var historyItem = selectedItem.Tag as Form1.PlaylistHistoryItem;
                if (historyItem != null)
                {
                    PlaylistSelected?.Invoke(historyItem);
                }
            }
        }

        private void btnClearHistory_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear all history?", "Confirm",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                listViewHistory.Items.Clear();
                historyList.Clear();
            }
        }
    }
}

