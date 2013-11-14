using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WMPLib;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Xml;
using System.IO;

namespace cb0t
{
    public partial class AudioPanel : UserControl
    {
        private Color column_bg1 = Color.Gainsboro;
        private Color column_bg2 = Color.Silver;
        private Pen column_outline = new Pen(new SolidBrush(Color.FromArgb(109, 115, 123)), 1);
        private SolidBrush column_text_brush = new SolidBrush(Color.Black);
        private Random rnd = new Random();

        private WindowsMediaPlayer Player { get; set; }
        private ImageList icons { get; set; }
        private System.Windows.Forms.Timer timer { get; set; }

        private List<AudioPlayerItem> play_list = new List<AudioPlayerItem>();

        public event EventHandler PlayPauseIconChanged;
        public event EventHandler ShowAudioText;

        public static bool Available { get; set; }
        public static bool DoRepeat { get; set; }
        public static bool DoRandom { get; set; }
        public static String Song = String.Empty;

        public AudioPanel()
        {
            this.InitializeComponent();

            String path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Windows Media Player");
            path = Path.Combine(path, "wmplayer.exe");
            Available = File.Exists(path);

            if (Available)
            {
                this.Player = new WindowsMediaPlayer();
                this.Player.PlayStateChange += this.PlayStateChange;
                this.icons = new ImageList();
                this.icons.Images.Add((Bitmap)Properties.Resources.note.Clone());
                this.icons.Images.Add((Bitmap)Properties.Resources.audio_play.Clone());
                this.audioList1.SmallImageList = this.icons;
                this.timer = new System.Windows.Forms.Timer();
                this.timer.Interval = 1000;
                this.timer.Tick += this.TimerTick;
                this.timer.Start();
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (this.Player.playState == WMPPlayState.wmppsPlaying)
            {
                IWMPMedia m = this.Player.currentMedia;
                int pos = this.play_list.FindIndex(x => x.Playing);

                if (m != null && pos > -1)
                {
                    TimeSpan ts = new TimeSpan(0, 0, (int)this.Player.controls.currentPosition);
                    String text = String.Format("{0:0}:{1:00}", Math.Floor(ts.TotalMinutes), ts.Seconds) + " ";
                    this.ShowAudioText("♫ " + text + this.play_list[pos].ToAudioTextString() + " ♫", EventArgs.Empty);
                }
            }
        }

        private int prepare_for_auto_next_track = 0;

        private void PlayStateChange(int NewState)
        {
            int pos;

            switch ((WMPPlayState)NewState)
            {
                case WMPPlayState.wmppsPlaying:
                    pos = this.play_list.FindIndex(x => x.Playing);

                    if (pos > -1)
                    {
                        Song = this.play_list[pos].ToAudioTextString();
                        this.PlayPauseIconChanged(false, EventArgs.Empty);
                        this.ShowAudioText("♫ 0:00 " + this.play_list[pos].ToAudioTextString() + " ♫", EventArgs.Empty);
                        this.albumArtBox1.UpdateArt(this.play_list[pos]);
                        this.label1.Text = "Now playing:\r\n\r\n" + this.play_list[pos].ToAudioTextString();
                    }

                    break;

                case WMPPlayState.wmppsMediaEnded:
                    this.prepare_for_auto_next_track = 1;
                    break;

                case WMPPlayState.wmppsStopped:
                    if (this.prepare_for_auto_next_track == 1)
                    {
                        pos = this.play_list.FindIndex(x => x.Playing);

                        if (pos > -1)
                        {
                            Song = String.Empty;
                            this.play_list.ForEach(x => x.Playing = false);
                            this.PlayPauseIconChanged(true, EventArgs.Empty);
                            this.ShowAudioText(String.Empty, EventArgs.Empty);
                            this.albumArtBox1.ClearArt();
                            this.label1.Text = String.Empty;

                            if (DoRandom)
                            {
                                this.PlayRandom();
                                this.prepare_for_auto_next_track = 2;
                                return;
                            }

                            pos++;

                            if (pos >= this.play_list.Count && DoRepeat)
                                pos = 0;

                            for (int i = pos; i < this.play_list.Count; i++)
                                if (this.play_list[i].Exists)
                                {
                                    this.play_list[i].Playing = true;
                                    this.Player.URL = this.play_list[i].Path;
                                    break;
                                }
                        }

                        this.prepare_for_auto_next_track = 2;
                    }
                    else if (this.prepare_for_auto_next_track > 1)
                    {
                        this.prepare_for_auto_next_track = 0;
                        this.Player.controls.play();
                    }

                    break;
            }
        }

        public void ClearPlaylist()
        {
            this.StopClicked();
            this.audioList1.Items.Clear();
            this.play_list.Clear();
            this.SavePlaylist();
        }

        public void StopClicked()
        {
            if (this.Player.playState == WMPPlayState.wmppsPlaying ||
                this.Player.playState == WMPPlayState.wmppsPaused)
            {
                this.play_list.ForEach(x => x.Playing = false);
                this.PlayPauseIconChanged(true, EventArgs.Empty);
                this.ShowAudioText(String.Empty, EventArgs.Empty);
                this.albumArtBox1.ClearArt();
                this.label1.Text = String.Empty;
                this.Player.controls.stop();
                Song = String.Empty;
            }
        }

        private void PlayRandom()
        {
            if (this.play_list.Count == 0)
                return;

            int pos = (int)Math.Floor(rnd.NextDouble() * this.play_list.Count);
            int attempts = 0;

            while (!this.play_list[pos].Exists)
            {
                attempts++;

                if (attempts == this.play_list.Count)
                    return;

                pos = (int)Math.Floor(rnd.NextDouble() * this.play_list.Count);
            }

            this.play_list[pos].Playing = true;
            this.Player.URL = this.play_list[pos].Path;
        }

        public void PreviousClicked()
        {
            if (this.Player.playState == WMPPlayState.wmppsPlaying ||
                this.Player.playState == WMPPlayState.wmppsPaused)
            {
                int pos = this.play_list.FindIndex(x => x.Playing);

                if (pos > -1)
                {
                    this.play_list.ForEach(x => x.Playing = false);
                    this.PlayPauseIconChanged(true, EventArgs.Empty);
                    this.ShowAudioText(String.Empty, EventArgs.Empty);
                    this.albumArtBox1.ClearArt();
                    this.label1.Text = String.Empty;
                    this.Player.controls.stop();

                    if (DoRandom)
                    {
                        this.PlayRandom();
                        return;
                    }

                    pos--;

                    if (pos < 0)
                        pos = 0;

                    for (int i = pos; i < this.play_list.Count; i++)
                        if (this.play_list[i].Exists)
                        {
                            this.play_list[i].Playing = true;
                            this.Player.URL = this.play_list[i].Path;
                            break;
                        }
                }
            }
        }

        public void NextClicked()
        {
            if (this.Player.playState == WMPPlayState.wmppsPlaying ||
               this.Player.playState == WMPPlayState.wmppsPaused)
            {
                int pos = this.play_list.FindIndex(x => x.Playing);

                if (pos > -1)
                {
                    this.play_list.ForEach(x => x.Playing = false);
                    this.PlayPauseIconChanged(true, EventArgs.Empty);
                    this.ShowAudioText(String.Empty, EventArgs.Empty);
                    this.albumArtBox1.ClearArt();
                    this.label1.Text = String.Empty;
                    this.Player.controls.stop();
                    Song = String.Empty;

                    if (DoRandom)
                    {
                        this.PlayRandom();
                        return;
                    }

                    pos++;

                    if (pos >= this.play_list.Count && DoRepeat)
                        pos = 0;

                    for (int i = pos; i < this.play_list.Count; i++)
                        if (this.play_list[i].Exists)
                        {
                            this.play_list[i].Playing = true;
                            this.Player.URL = this.play_list[i].Path;
                            break;
                        }
                }
            }
        }

        public void PlayPauseClicked()
        {
            if (this.Player.playState == WMPPlayState.wmppsPlaying)
            {
                this.Player.controls.pause();
                this.PlayPauseIconChanged(true, EventArgs.Empty);
            }
            else if (this.Player.playState == WMPPlayState.wmppsPaused)
            {
                this.Player.controls.play();
                this.PlayPauseIconChanged(false, EventArgs.Empty);
            }
            else if (this.play_list.Count > 0)
            {
                this.play_list.ForEach(x => x.Playing = false);

                if (DoRandom)
                {
                    this.PlayRandom();
                    return;
                }

                for (int i = 0; i < this.play_list.Count; i++)
                    if (this.play_list[i].Exists)
                    {
                        this.play_list[i].Playing = true;
                        this.Player.URL = this.play_list[i].Path;
                        break;
                    }
            }
        }

        private void PlaySong(AudioPlayerItem item)
        {
            this.Player.URL = item.Path;
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            Rectangle r = new Rectangle(0, 0, this.panel2.Width, this.panel2.Height - 1);

            using (LinearGradientBrush lb = new LinearGradientBrush(r, this.column_bg1, this.column_bg2, 90f))
                e.Graphics.FillRectangle(lb, r);

            e.Graphics.DrawRectangle(this.column_outline, r);
            e.Graphics.DrawString("Now playing", this.Font, this.column_text_brush, new PointF(3, 5));
        }

        private void AudioPanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
        }

        private void AudioPanel_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Thread thread = new Thread(new ParameterizedThreadStart(this.ImportFiles));
                thread.Start(e.Data.GetData(DataFormats.FileDrop));
            }
        }

        private void ImportFiles(object arg)
        {
            List<String> files = new List<String>((String[])arg);
            files.Sort((x, y) => x.CompareTo(y));

            foreach (String f in files)
                if (this.play_list.Find(x => x.Path == f) == null)
                {
                    AudioPlayerItem item = AudioHelpers.CreateAudioPlayerItem(this.Player, f);

                    if (item.Duration > 0)
                    {
                        this.play_list.Add(item);
                        this.AddPlaylistItem(item);
                    }
                }

            this.SavePlaylist();
        }

        private void AddPlaylistItem(AudioPlayerItem item)
        {
            if (this.audioList1.InvokeRequired)
                this.audioList1.BeginInvoke((Action)(() => this.AddPlaylistItem(item)));
            else
                this.audioList1.Items.Add(item);
        }

        private void SavePlaylist()
        {
            try
            {
                XmlWriterSettings appearance = new XmlWriterSettings();
                appearance.Indent = true;
                XmlWriter xml = XmlWriter.Create(Settings.DataPath + "playlist.xml", appearance);

                xml.WriteStartDocument();
                xml.WriteStartElement("playlist");

                foreach (AudioPlayerItem item in this.play_list)
                {
                    xml.WriteStartElement("item");

                    xml.WriteElementString("Album", Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Album)));
                    xml.WriteElementString("Artist", Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Artist)));
                    xml.WriteElementString("Author", Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Author)));
                    xml.WriteElementString("Length", item.Duration.ToString());
                    xml.WriteElementString("Name", Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Title)));
                    xml.WriteElementString("Path", Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Path)));

                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
                xml.WriteEndDocument();
                xml.Flush();
                xml.Close();
            }
            catch { }
        }

        public void LoadPlaylist()
        {
            Thread thread = new Thread(new ThreadStart((Action)(() =>
            {
                try
                {
                    using (FileStream f = new FileStream(Settings.DataPath + "playlist.xml", FileMode.Open))
                    {
                        XmlReader xml = XmlReader.Create(new StreamReader(f));

                        xml.MoveToContent();
                        xml.ReadSubtree().ReadToFollowing("playlist");

                        while (xml.ReadToFollowing("item"))
                        {
                            AudioPlayerItem item = new AudioPlayerItem();
                            xml.ReadSubtree().ReadToFollowing("Album");
                            item.Album = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                            xml.ReadToFollowing("Artist");
                            item.Artist = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                            xml.ReadToFollowing("Author");
                            item.Author = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                            xml.ReadToFollowing("Length");
                            item.Duration = int.Parse(xml.ReadElementContentAsString());
                            item.SetDurationText(item.Duration);
                            xml.ReadToFollowing("Name");
                            item.Title = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                            xml.ReadToFollowing("Path");
                            item.Path = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                            this.play_list.Add(item);
                        }

                        xml.Close();
                    }
                }
                catch { }

                if (this.play_list.Count > 0)
                {
                    this.LockList(true);

                    foreach (AudioPlayerItem i in this.play_list)
                        this.AddPlaylistItem(i);

                    this.LockList(false);
                }
            })));

            thread.Start();
        }

        private void LockList(bool locked)
        {
            if (this.audioList1.InvokeRequired)
                this.audioList1.BeginInvoke((Action)(() => this.LockList(locked)));
            else
            {
                if (locked)
                    this.audioList1.BeginUpdate();
                else
                    this.audioList1.EndUpdate();
            }
        }

        private void audioList1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                if (this.audioList1.SelectedIndices.Count > 0)
                {
                    this.StopClicked();
                    int i = this.audioList1.SelectedIndices[0];

                    if (this.play_list[i].Exists)
                    {
                        this.play_list[i].Playing = true;
                        this.Player.URL = this.play_list[i].Path;
                    }
                }
        }

        private bool reordering = false;

        private void audioList1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            this.reordering = true;
            this.DoDragDrop(e.Item, DragDropEffects.Link);
        }

        private void audioList1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else if (this.reordering)
                e.Effect = DragDropEffects.Link;
        }

        private void audioList1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Thread thread = new Thread(new ParameterizedThreadStart(this.ImportFiles));
                thread.Start(e.Data.GetData(DataFormats.FileDrop));
                this.reordering = false;
            }
            else if (this.reordering)
            {
                this.reordering = false;
                Point point = this.audioList1.PointToClient(new Point(e.X, e.Y));
                ListViewItem item = this.audioList1.GetItemAt(point.X, point.Y);

                if (this.audioList1.SelectedIndices.Count > 0 && item != null)
                {
                    int dest_index = item.Index;
                    int current_index = this.audioList1.SelectedIndices[0];

                    if (dest_index != current_index)
                    {
                        AudioPlayerItem a = this.play_list[current_index];
                        this.audioList1.Items.RemoveAt(current_index);
                        this.play_list.RemoveAt(current_index);
                        this.play_list.Insert(dest_index, a);
                        this.audioList1.Items.Insert(dest_index, a);
                        this.SavePlaylist();
                    }
                }
            }
        }

        private void audioList1_DragLeave(object sender, EventArgs e)
        {
            this.reordering = false;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.audioList1.SelectedIndices.Count == 0)
                e.Cancel = true;
        }

        private void removeFromPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.audioList1.SelectedIndices.Count > 0)
            {
                int r = this.audioList1.SelectedIndices[0];

                if (r >= 0 && r < this.play_list.Count)
                {
                    AudioPlayerItem item = this.play_list[r];
                    this.audioList1.Items.RemoveAt(r);
                    this.play_list.RemoveAt(r);

                    if (item.Playing)
                        this.StopClicked();

                    this.SavePlaylist();
                }
            }
        }
    }
}
