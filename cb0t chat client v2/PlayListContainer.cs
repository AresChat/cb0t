using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace cb0t_chat_client_v2
{
    public partial class PlayListContainer : UserControl
    {
        internal event AddFileToPlaylistEventHandler AddFileToPlaylist;
        internal event PlayNowEventHandler PlayNow;
        internal event PlaylistItemMovedEventHandler PlaylistItemMoved;
        internal event DownloadPlsFileEventHandler DownloadPlsFile;
        internal event PlaylistItemRemovedEventHandler PlaylistItemRemoved;
        public event EventHandler SavePlaylist;
        public event EventHandler StopAndClearPlaylist;
        public event EventHandler ConnectToMostRecentRadio;

        private AudioOptionsScreen options = new AudioOptionsScreen();

        public PlayListContainer()
        {
            InitializeComponent();
            this.toolStrip1.Renderer = new PlayListToolStrip();
            this.BackColor = Color.White;
            this.DoubleBuffered = true;
            this.playlistUpDown1.UpClicked += new EventHandler(this.OrderingUpClicked);
            this.playlistUpDown1.DownClicked += new EventHandler(this.OrderingDownClicked);
        }

        public void LoadOptions()
        {
            this.options.SetupValues();
        }

        private void OrderingDownClicked(object sender, EventArgs e)
        {
            int index = -1;

            for (int i = 0; i < this.playListBox1.Items.Count; i++)
                if (this.playListBox1.Items[i].BackColor.Equals(Color.Silver))
                    index = i;

            if (index > -1)
            {
                int pos = this.playListBox1.MoveItem(index, false);

                if (this.PlaylistItemMoved != null)
                    this.PlaylistItemMoved(this, new PlaylistItemMovedEventArgs(pos, index, false));
            }
        }

        private void OrderingUpClicked(object sender, EventArgs e)
        {
            int index = -1;

            for (int i = 0; i < this.playListBox1.Items.Count; i++)
                if (this.playListBox1.Items[i].BackColor.Equals(Color.Silver))
                    index = i;

            if (index > -1)
            {
                int pos = this.playListBox1.MoveItem(index, true);

                if (this.PlaylistItemMoved != null)
                    this.PlaylistItemMoved(this, new PlaylistItemMovedEventArgs(pos, index, true));
            }
        }

        public void ClearPlaylist()
        {
            this.playListBox1.Items.Clear();
        }

        internal void AddPlaylistItem(PlaylistItem item)
        {
            this.playListBox1.AddItem(item);
        }

        internal void AddPlaylist(PlaylistItem[] items)
        {
            this.playListBox1.BeginUpdate();

            foreach (PlaylistItem i in items)
                this.playListBox1.AddItem(i);

            this.playListBox1.EndUpdate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.White);

            Rectangle rec = new Rectangle(0, 0, this.Width, 24);

            using (LinearGradientBrush brush = new LinearGradientBrush(rec, Color.Black, Color.White, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(brush, rec);

            rec = new Rectangle(0, this.Height - 24, this.Width, 24);

            using (LinearGradientBrush brush = new LinearGradientBrush(rec, Color.White, Color.Black, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(brush, rec);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }

        private void playListBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void playListBox1_DragDrop(object sender, DragEventArgs e)
        {
            String[] files = null;

            if (e.Data.GetDataPresent(DataFormats.Text))
                files = new String[] { (String)e.Data.GetData(DataFormats.Text) };
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                files = (String[])e.Data.GetData(DataFormats.FileDrop);

            if (files != null)
                if (this.AddFileToPlaylist != null)
                {
                    foreach (String str in files)
                        this.AddFileToPlaylist(this, new AddFileToPlaylistEventArgs(str));

                    if (this.SavePlaylist != null)
                        this.SavePlaylist(this, new EventArgs());
                }
        }

        private void toolStripButton1_Click(object sender, MouseEventArgs e)
        {
            this.contextMenuStrip1.Show(this.toolStrip1, e.Location);
        }

        private void playListBox1_MouseDoubleClick(object sender, MouseEventArgs e) // change and play a new song
        {
            if (this.playListBox1.SelectedIndices.Count > 0)
            {
                if (this.PlayNow != null)
                {
                    int index = this.playListBox1.SelectedIndices[0];
                    this.playListBox1.SetActiveItem(index);
                    this.PlayNow(this, new PlayNowEventArgs(index));
                }
            }
        }

        public void SetActiveItem(int index)
        {
            this.playListBox1.SetActiveItem(index);
        }

        private void addFilesToolStripMenuItem_Click(object sender, EventArgs e) // add files
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Supported file types (*.wav;*.mp3;*.wma)|*.wav;*.mp3;*.wma|WAV files (*.wav)|*.wav|MP3 files (*.mp3)|*.mp3|WMA files (*.wma)|*.wma";
                ofd.Multiselect = true;
                ofd.InitialDirectory = AudioSettings.last_folder_path;
                String last_dir = null;

                if (ofd.ShowDialog() == DialogResult.OK)
                    if (this.AddFileToPlaylist != null)
                    {
                        foreach (String str in ofd.FileNames)
                        {
                            this.AddFileToPlaylist(this, new AddFileToPlaylistEventArgs(str));
                            last_dir = str;
                        }

                        if (this.SavePlaylist != null)
                            this.SavePlaylist(this, new EventArgs());

                        if (last_dir != null)
                        {
                            AudioSettings.last_folder_path = new FileInfo(last_dir).DirectoryName;
                            AudioSettings.Save();
                        }
                    }
            }
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e) // add folder
        {
            using (FolderBrowserDialog f = new FolderBrowserDialog())
            {
                f.SelectedPath = AudioSettings.last_folder_path;
                
                if (f.ShowDialog() == DialogResult.OK)
                    new Thread(new ParameterizedThreadStart(this.FolderThread)).Start(f.SelectedPath);
            }
        }

        private void FolderThread(object args)
        {
            String[] files = this.FilesFromFolder((String)args);

            foreach (String str in files)
                this.AddFileToPlaylist(this, new AddFileToPlaylistEventArgs(str));

            if (this.SavePlaylist != null)
                this.SavePlaylist(this, new EventArgs());

            AudioSettings.last_folder_path = (String)args;
            AudioSettings.Save();
        }

        private String[] FilesFromFolder(String path)
        {
            List<String> files = new List<String>();
            DirectoryInfo d1 = new DirectoryInfo(path);

            foreach (FileInfo file in d1.GetFiles())
                files.Add(file.FullName);

            foreach (DirectoryInfo d2 in d1.GetDirectories())
            {
                foreach (FileInfo file in d2.GetFiles())
                    files.Add(file.FullName);

                foreach (DirectoryInfo d3 in d2.GetDirectories())
                {
                    foreach (FileInfo file in d3.GetFiles())
                        files.Add(file.FullName);

                    foreach (DirectoryInfo d4 in d3.GetDirectories())
                    {
                        foreach (FileInfo file in d4.GetFiles())
                            files.Add(file.FullName);

                        foreach (DirectoryInfo d5 in d4.GetDirectories())
                            foreach (FileInfo file in d5.GetFiles())
                                files.Add(file.FullName);
                    }
                }
            }

            return files.ToArray();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e) // add playlist
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Playlist files (*.m3u)|*.m3u";
                ofd.Multiselect = false;
                ofd.InitialDirectory = AudioSettings.last_folder_path;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    AudioSettings.last_folder_path = new FileInfo(ofd.FileName).DirectoryName;
                    AudioSettings.Save();

                    if (this.AddFileToPlaylist != null)
                    {
                        try
                        {
                            String[] files = File.ReadAllLines(ofd.FileName);

                            foreach (String str in files)
                            {
                                if (str.Trim().Length > 0)
                                {
                                    try
                                    {
                                        if (File.Exists(str))
                                            this.AddFileToPlaylist(this, new AddFileToPlaylistEventArgs(str));
                                        else if (File.Exists(AudioSettings.last_folder_path + "\\" + str))
                                            this.AddFileToPlaylist(this, new AddFileToPlaylistEventArgs(AudioSettings.last_folder_path + "\\" + str));
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }

                        if (this.SavePlaylist != null)
                            this.SavePlaylist(this, new EventArgs());
                    }
                }
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e) // load playlist
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Playlist files (*.m3u)|*.m3u";
                ofd.Multiselect = false;
                ofd.InitialDirectory = AudioSettings.last_folder_path;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    AudioSettings.last_folder_path = new FileInfo(ofd.FileName).DirectoryName;
                    AudioSettings.Save();

                    if (this.AddFileToPlaylist != null)
                    {
                        this.playListBox1.Items.Clear();
                        this.StopAndClearPlaylist(this, new EventArgs());

                        try
                        {
                            String[] files = File.ReadAllLines(ofd.FileName);

                            foreach (String str in files)
                            {
                                if (str.Trim().Length > 0)
                                {
                                    try
                                    {
                                        if (File.Exists(str))
                                            this.AddFileToPlaylist(this, new AddFileToPlaylistEventArgs(str));
                                        else if (File.Exists(AudioSettings.last_folder_path + "\\" + str))
                                            this.AddFileToPlaylist(this, new AddFileToPlaylistEventArgs(AudioSettings.last_folder_path + "\\" + str));
                                    }
                                    catch { }
                                }
                            }
                        }
                        catch { }

                        if (this.SavePlaylist != null)
                            this.SavePlaylist(this, new EventArgs());
                    }
                }
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e) // clear playlist
        {
            this.playListBox1.Items.Clear();
            this.StopAndClearPlaylist(this, new EventArgs());
            this.SavePlaylist(this, new EventArgs());
        }

        private void toolStripButton3_Click(object sender, MouseEventArgs e)
        {
            this.contextMenuStrip2.Show(this.toolStrip1, e.Location);
        }

        private void browseShoutcastToolStripMenuItem_Click(object sender, EventArgs e) // browse shoutcast
        {
            try
            {
                System.Diagnostics.Process.Start("ShoutcastBrowser.exe");
            }
            catch { }
        }

        private void connectToPLSLinkToolStripMenuItem_Click(object sender, EventArgs e) // connect to pls link
        {
            String str = String.Empty;
            
            using (PlsLink l = new PlsLink())
                if (l.ShowDialog() == DialogResult.OK)
                    str = l.Link;

            if (String.IsNullOrEmpty(str))
            {
                MessageBox.Show("invalid or missing pls link", "cb0t", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (str.StartsWith("http://") && str.Contains(".pls"))
            {
                if (this.DownloadPlsFile != null)
                    this.DownloadPlsFile(this, new DownloadPlsFileEventArgs(str));
            }
            else MessageBox.Show("invalid or missing pls link", "cb0t", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void connectToMostRecentRadioToolStripMenuItem_Click(object sender, EventArgs e) // most recent radio
        {
            if (this.ConnectToMostRecentRadio != null)
                this.ConnectToMostRecentRadio(this, new EventArgs());
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e) // remove song from playlist
        {
            if (this.playListBox1.SelectedIndices.Count > 0)
            {
                int index = this.playListBox1.SelectedIndices[0];
                this.playListBox1.Items.RemoveAt(index);

                int pos = -1;

                for (int i = 0; i < this.playListBox1.Items.Count; i++)
                    if (this.playListBox1.Items[i].ForeColor.Equals(Color.Red))
                    {
                        pos = i;
                        break;
                    }

                if (this.PlaylistItemRemoved != null)
                    this.PlaylistItemRemoved(this, new PlaylistItemRemovedEventArgs(pos, index));
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            this.options.ShowDialog();
            this.options.UpdateNPText();
        }

    }

    class PlayListToolStrip : ToolStripProfessionalRenderer
    {
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            Color col1 = Color.FromArgb(227, 239, 255);
            Color col2 = Color.FromArgb(177, 211, 255);

            using (LinearGradientBrush brush = new LinearGradientBrush(e.AffectedBounds, col1, col2, LinearGradientMode.Vertical))
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
        }
    }

    class AddFileToPlaylistEventArgs : EventArgs
    {
        private String filename;

        public String Filename
        {
            get { return this.filename; }
        }

        public AddFileToPlaylistEventArgs(String filename)
        {
            this.filename = filename;
        }
    }

    internal delegate void AddFileToPlaylistEventHandler(object sender, AddFileToPlaylistEventArgs e);

    class PlaylistOrderingChangedEventArgs : EventArgs
    {
        public int old_position;
        public int new_position;

        public PlaylistOrderingChangedEventArgs(int o, int n)
        {
            this.old_position = o;
            this.new_position = n;
        }
    }

    internal delegate void PlaylistOrderingChangedEventHandler(object sender, PlaylistOrderingChangedEventArgs e);

    class PlayNowEventArgs : EventArgs
    {
        public int index;

        public PlayNowEventArgs(int index)
        {
            this.index = index;
        }
    }

    internal delegate void PlayNowEventHandler(object sender, PlayNowEventArgs e);

    class PlaylistItemMovedEventArgs : EventArgs
    {
        public bool up;
        public int playlist_position;
        public int index;

        public PlaylistItemMovedEventArgs(int playlist_position, int index, bool up)
        {
            this.playlist_position = playlist_position;
            this.index = index;
            this.up = up;
        }
    }

    internal delegate void PlaylistItemMovedEventHandler(object sender, PlaylistItemMovedEventArgs e);

    class DownloadPlsFileEventArgs : EventArgs
    {
        public String url;

        public DownloadPlsFileEventArgs(String url)
        {
            this.url = url;
        }
    }

    internal delegate void DownloadPlsFileEventHandler(object sender, DownloadPlsFileEventArgs e);

    class PlaylistItemRemovedEventArgs : EventArgs
    {
        public int playlist_position;
        public int index;

        public PlaylistItemRemovedEventArgs(int playlist_position, int index)
        {
            this.playlist_position = playlist_position;
            this.index = index;
        }
    }

    internal delegate void PlaylistItemRemovedEventHandler(object sender, PlaylistItemRemovedEventArgs e);
}
