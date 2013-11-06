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

        private WindowsMediaPlayer Player { get; set; }
        private ImageList icons { get; set; }

        private List<AudioPlayerItem> play_list = new List<AudioPlayerItem>();

        public event EventHandler PlayPauseIconChanged;
        public event EventHandler ShowAudioText;

        public AudioPanel()
        {
            this.InitializeComponent();
            this.Player = new WindowsMediaPlayer();
            this.icons = new ImageList();
            this.icons.Images.Add((Bitmap)Properties.Resources.note.Clone());
            this.icons.Images.Add((Bitmap)Properties.Resources.audio_play.Clone());
            this.audioList1.SmallImageList = this.icons;
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
                for (int i = 0; i < this.play_list.Count; i++)
                    if (this.play_list[i].Exists)
                    {
                        this.play_list[i].Playing = true;
                        this.Player.URL = this.play_list[i].Path;
                        this.PlayPauseIconChanged(false, EventArgs.Empty);
                        this.ShowAudioText("♫ " + this.play_list[i].ToAudioTextString() + " ♫", EventArgs.Empty);
                        this.albumArtBox1.UpdateArt(this.play_list[i]);
                        this.label1.Text = "Now playing:\r\n\r\n" + this.play_list[i].ToAudioTextString();
                        break;
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
                this.audioList1.BeginUpdate();

                foreach (AudioPlayerItem i in this.play_list)
                    this.AddPlaylistItem(i);

                this.audioList1.EndUpdate();
            }
        }
    }
}
