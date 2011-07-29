using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using AxWMPLib;
using WMPLib;

namespace cb0t_chat_client_v2
{
    public partial class AudioPlayer : UserControl
    {
        private List<PlaylistItem> playlist = new List<PlaylistItem>();
        private int playlist_position = -1;
        private IWMPMedia current_media = null;
        private Random rnd = new Random();
        private PlsParser plsparser = new PlsParser();

        public event EventHandler SongTitleChanged;

        public AudioPlayer()
        {
            InitializeComponent();
            AudioSettings.Load();
            this.playListContainer1.LoadOptions();
            this.DoubleBuffered = true;
            this.playListContainer1.AddFileToPlaylist += new AddFileToPlaylistEventHandler(this.AddFileToPlaylist);
            this.playListContainer1.SavePlaylist += new EventHandler(this.SavePlaylist);
            this.playListContainer1.PlayNow += new PlayNowEventHandler(this.PlayNow);
            this.playListContainer1.PlaylistItemMoved += new PlaylistItemMovedEventHandler(this.PlaylistItemMoved);
            this.playListContainer1.StopAndClearPlaylist += new EventHandler(this.StopAndClearPlaylist);
            this.playListContainer1.DownloadPlsFile += new DownloadPlsFileEventHandler(this.DownloadPlsFile);
            this.playListContainer1.ConnectToMostRecentRadio += new EventHandler(this.ConnectToMostRecentRadio);
            this.playListContainer1.PlaylistItemRemoved += new PlaylistItemRemovedEventHandler(this.PlaylistItemRemoved);
            this.axWindowsMediaPlayer1.settings.autoStart = false;
            this.axWindowsMediaPlayer1.settings.volume = 75;
            this.axWindowsMediaPlayer1.PlayStateChange += new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(this.PlayStateChange);
            this.axWindowsMediaPlayer1.MediaChange += new AxWMPLib._WMPOCXEvents_MediaChangeEventHandler(this.RadioMediaChange);
            this.axWindowsMediaPlayer1.MediaError += new AxWMPLib._WMPOCXEvents_MediaErrorEventHandler(this.MediaError);
            this.audioController1.VolumeChanged += new AudioPlayerVolumeChangedHandler(this.VolumeChanged);
            this.audioController1.Stop += new EventHandler(this.Stop);
            this.audioController1.Pause += new EventHandler(this.Pause);
            this.audioController1.Play += new EventHandler(this.Play);
            this.audioController1.Forward += new EventHandler(this.Forward);
            this.audioController1.Back += new EventHandler(this.Back);
            this.plsparser.PlsParsed += new PlsParserEventHandler(this.PlsParsed);
        }

        public void ExternalSongRequest(String path)
        {
            this.StopAndClearPlaylist(this, new EventArgs());
            this.playListContainer1.ClearPlaylist();
            this.albumArtDisplayer1.UpdateArt();

            if (AudioSettings.radio_mode)
            {
                this.axWindowsMediaPlayer1.Ctlcontrols.stop();
                this.axWindowsMediaPlayer1.settings.autoStart = false;
                this.axWindowsMediaPlayer1.URL = "";
                this.Stopped();
                AudioSettings.radio_mode = false;
            }

            this.AddFileToPlaylist(this, new AddFileToPlaylistEventArgs(path));

            if (this.playlist.Count > 0)
            {
                this.audioController1.PlaylistEmpty(false);
                this.audioController1.SetToPlay();
                this.Play(this, new EventArgs());
            }
        }

        public void VoiceClipStarted()
        {
            if (AudioSettings.voice_mute)
                this.axWindowsMediaPlayer1.settings.mute = true;
        }

        public void VoiceClipStopped()
        {
            this.axWindowsMediaPlayer1.settings.mute = false;
        }

        private void PlaylistItemRemoved(object sender, PlaylistItemRemovedEventArgs e)
        {
            if (this.playlist_position == e.index)
            {
                if (this.axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
                    this.axWindowsMediaPlayer1.Ctlcontrols.stop();

                this.Stopped();
            }

            this.playlist.RemoveAt(e.index);
            this.playlist_position = e.playlist_position;
            this.SavePlaylist(this, new EventArgs());

            if (this.playlist.Count == 0)
                this.audioController1.PlaylistEmpty(true);
        }

        private void ConnectToMostRecentRadio(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(AudioSettings.last_radio))
            {
                if (this.axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
                    this.axWindowsMediaPlayer1.Ctlcontrols.stop();

                AudioSettings.current_song = String.Empty;
                this.pending_play = false;
                this.end_of_current_song = false;
                this.Stopped();
                this.audioController1.SetRadioMode();
                AudioSettings.radio_mode = true;
                this.axWindowsMediaPlayer1.settings.autoStart = true;
                this.axWindowsMediaPlayer1.URL = AudioSettings.last_radio;
                this.label2.Text = "Loading...";
                this.albumArtDisplayer1.UpdateArt();
            }
        }

        private void RadioMediaChange(object sender, _WMPOCXEvents_MediaChangeEvent e)
        {
            if (AudioSettings.radio_mode)
            {
                IWMPMedia item = (IWMPMedia)e.item;

                if (item != null)
                {
                    if (item.name != AudioSettings.current_song)
                    {
                        AudioSettings.current_song = item.name;
                        this.label1.Text = item.name;

                        if (this.SongTitleChanged != null)
                            this.SongTitleChanged(this, new EventArgs());
                    }
                }
            }
        }

        private void PlsParsed(object sender, PlsParserEventArgs e)
        {
            if (e.success)
            {
                this.audioController1.SetRadioMode();
                AudioSettings.radio_mode = true;
                AudioSettings.last_radio = e.url;
                AudioSettings.current_song = String.Empty;
                AudioSettings.Save();

                this.axWindowsMediaPlayer1.BeginInvoke(new MethodInvoker(delegate
                {
                    this.axWindowsMediaPlayer1.settings.autoStart = true;
                    this.axWindowsMediaPlayer1.URL = e.url;
                }));

                this.label2.BeginInvoke(new MethodInvoker(delegate
                {
                    this.label2.Text = "Loading...";
                }));

                this.albumArtDisplayer1.UpdateArt();
            }
            else
            {
                MessageBox.Show("Could not connect to radio", "cb0t", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AudioSettings.radio_mode = false;
            }
        }

        public void RadioHashlink(String url)
        {
            if (this.axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
                this.axWindowsMediaPlayer1.Ctlcontrols.stop();

            AudioSettings.current_song = String.Empty;
            this.pending_play = false;
            this.end_of_current_song = false;
            this.Stopped();
            this.audioController1.SetRadioMode();
            AudioSettings.radio_mode = true;
            AudioSettings.last_radio = url;
            AudioSettings.Save();
            this.axWindowsMediaPlayer1.settings.autoStart = true;
            this.axWindowsMediaPlayer1.URL = url;
            this.label2.Text = "Loading...";
            this.albumArtDisplayer1.UpdateArt();
        }

        internal void DownloadPlsFile(object sender, DownloadPlsFileEventArgs e)
        {
            if (this.axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
                this.axWindowsMediaPlayer1.Ctlcontrols.stop();

            this.pending_play = false;
            this.end_of_current_song = false;
            this.Stopped();
            this.plsparser.Parse(e.url);
        }

        private void StopAndClearPlaylist(object sender, EventArgs e)
        {
            if (!AudioSettings.radio_mode)
            {
                if (this.axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
                    this.axWindowsMediaPlayer1.Ctlcontrols.stop();

                this.Stopped();
            }

            this.pending_play = false;
            this.end_of_current_song = false;
            this.playlist.Clear();
            this.audioController1.PlaylistEmpty(true);
            this.SavePlaylist(sender, e);
        }

        private int timer_tick = 0;

        public void SecondsTick()
        {
            if (this.timer_tick++ > 120)
            {
                this.timer_tick = 0;
                this.SavePlaylist(this, new EventArgs());
            }

            if (AudioSettings.radio_mode)
                return;

            try
            {
                this.axWindowsMediaPlayer1.BeginInvoke(new MethodInvoker(delegate
                {
                    if (this.axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
                    {
                        int i = (int)this.axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
                        int minutes = (i / 60);
                        int seconds = i;

                        if (minutes > 0)
                            seconds = i - (minutes * 60);

                        String time = String.Format("{0:00}", minutes) + ":" + String.Format("{0:00}", seconds);

                        this.label2.BeginInvoke(new MethodInvoker(delegate
                        {
                            this.label2.Text = time;
                        }));

                        if (i == 5)
                        {
                            PlaylistItem item = this.playlist[this.playlist_position];

                            if (!String.IsNullOrEmpty(item.Artist) && (!String.IsNullOrEmpty(item.Album) || !String.IsNullOrEmpty(item.Name)))
                                this.albumArtDisplayer1.UpdateArt(this.playlist[this.playlist_position].Artist,
                                    this.playlist[this.playlist_position].Album, this.playlist[this.playlist_position].Name);
                            else
                                this.albumArtDisplayer1.UpdateArt();
                        }
                    }
                }));
            }
            catch { }

        }

        private void PlaylistItemMoved(object sender, PlaylistItemMovedEventArgs e)
        {
            if (e.up)
            {
                if (e.index == 0)
                    return;

                PlaylistItem p1 = (PlaylistItem)this.playlist[e.index];
                PlaylistItem p2 = (PlaylistItem)this.playlist[e.index - 1];

                this.playlist.RemoveAt(e.index);
                this.playlist.RemoveAt(e.index - 1);
                this.playlist.Insert(e.index - 1, p1);
                this.playlist.Insert(e.index, p2);
                this.playlist_position = e.playlist_position;
            }
            else
            {
                if (e.index == (this.playlist.Count - 1))
                    return;

                PlaylistItem p1 = (PlaylistItem)this.playlist[e.index];
                PlaylistItem p2 = (PlaylistItem)this.playlist[e.index + 1];

                this.playlist.RemoveAt(e.index + 1);
                this.playlist.RemoveAt(e.index);
                this.playlist.Insert(e.index, p2);
                this.playlist.Insert(e.index + 1, p1);
                this.playlist_position = e.playlist_position;
            }
        }

        private void PlayNow(object sender, PlayNowEventArgs e)
        {
            if (this.axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
                this.axWindowsMediaPlayer1.Ctlcontrols.stop();

            if (AudioSettings.radio_mode)
            {
                AudioSettings.radio_mode = false;
                this.axWindowsMediaPlayer1.settings.autoStart = false;
                this.axWindowsMediaPlayer1.URL = "";
                this.audioController1.PlaylistEmpty(this.playlist.Count == 0);
            }

            this.playlist_position = e.index;

            if (this.playlist_position < this.playlist.Count)
            {
                PlaylistItem item = this.playlist[this.playlist_position];
                this.current_media = this.axWindowsMediaPlayer1.newMedia(item.Path);
                this.axWindowsMediaPlayer1.currentMedia = this.current_media;
                this.axWindowsMediaPlayer1.Ctlcontrols.play();
                this.audioController1.SetToPlay();
            }
        }

        private void Back(object sender, EventArgs e)
        {
            if (this.axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
            {
                this.axWindowsMediaPlayer1.Ctlcontrols.stop();

                if (AudioSettings.shuffle)
                {
                    if (this.playlist.Count > 1)
                    {
                        List<int> ids = new List<int>();

                        for (int i = 0; i < this.playlist.Count; i++)
                            if (i != this.playlist_position)
                                ids.Add(i);

                        this.playlist_position = ids[(int)Math.Floor(this.rnd.NextDouble() * ids.Count)];
                        PlaylistItem item = this.playlist[this.playlist_position];
                        this.current_media = this.axWindowsMediaPlayer1.newMedia(item.Path);
                        this.axWindowsMediaPlayer1.currentMedia = this.current_media;
                        this.axWindowsMediaPlayer1.Ctlcontrols.play();
                        this.playListContainer1.SetActiveItem(this.playlist_position);
                        return;
                    }
                }

                this.playlist_position--;

                if (this.playlist_position < this.playlist.Count && this.playlist_position > -1)
                {
                    PlaylistItem item = this.playlist[this.playlist_position];
                    this.current_media = this.axWindowsMediaPlayer1.newMedia(item.Path);
                    this.axWindowsMediaPlayer1.currentMedia = this.current_media;
                    this.axWindowsMediaPlayer1.Ctlcontrols.play();
                    this.playListContainer1.SetActiveItem(this.playlist_position);
                }
                else this.Stopped();
            }
        }

        private void Forward(object sender, EventArgs e)
        {
            if (this.axWindowsMediaPlayer1.playState == WMPPlayState.wmppsPlaying)
            {
                this.axWindowsMediaPlayer1.Ctlcontrols.stop();

                if (AudioSettings.shuffle)
                {
                    if (this.playlist.Count > 1)
                    {
                        List<int> ids = new List<int>();

                        for (int i = 0; i < this.playlist.Count; i++)
                            if (i != this.playlist_position)
                                ids.Add(i);

                        this.playlist_position = ids[(int)Math.Floor(this.rnd.NextDouble() * ids.Count)];
                        PlaylistItem item = this.playlist[this.playlist_position];
                        this.current_media = this.axWindowsMediaPlayer1.newMedia(item.Path);
                        this.axWindowsMediaPlayer1.currentMedia = this.current_media;
                        this.axWindowsMediaPlayer1.Ctlcontrols.play();
                        this.playListContainer1.SetActiveItem(this.playlist_position);
                        return;
                    }
                }

                this.playlist_position++;

                if (this.playlist_position < this.playlist.Count)
                {
                    PlaylistItem item = this.playlist[this.playlist_position];
                    this.current_media = this.axWindowsMediaPlayer1.newMedia(item.Path);
                    this.axWindowsMediaPlayer1.currentMedia = this.current_media;
                    this.axWindowsMediaPlayer1.Ctlcontrols.play();
                    this.playListContainer1.SetActiveItem(this.playlist_position);
                }
                else this.Stopped();
            }
        }

        private void Stopped()
        {
            this.playlist_position = -1;
            this.current_media = null;
            this.audioController1.SetToStopped();
            this.playListContainer1.SetActiveItem(-1);
            this.label2.Text = "00:00";
            this.albumArtDisplayer1.UpdateArt();

            if (this.label1.Text != "Stopped")
                this.label1.Text = "Stopped";
        }

        private bool pending_play = false;
        private bool end_of_current_song = false;
        private void PlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch (e.newState)
            {
                case 1: // stop
                    if (AudioSettings.radio_mode)
                    {
                        AudioSettings.current_song = String.Empty;

                        if (this.SongTitleChanged != null)
                            this.SongTitleChanged(this, new EventArgs());

                        return;
                    }

                    if (this.end_of_current_song)
                    {
                        this.end_of_current_song = false;
                        PlaylistItem item = this.playlist[this.playlist_position];
                        this.current_media = this.axWindowsMediaPlayer1.newMedia(item.Path);
                        this.axWindowsMediaPlayer1.currentMedia = this.current_media;
                        this.axWindowsMediaPlayer1.Ctlcontrols.play();
                        this.playListContainer1.SetActiveItem(this.playlist_position);
                        this.pending_play = true;
                    }
                    else if (this.pending_play)
                    {
                        this.pending_play = false;
                        this.axWindowsMediaPlayer1.Ctlcontrols.play();
                        this.playListContainer1.SetActiveItem(this.playlist_position);
                    }
                    else
                    {
                        AudioSettings.current_song = String.Empty;

                        if (this.SongTitleChanged != null)
                            this.SongTitleChanged(this, new EventArgs());
                    }
                    

                    break;

                case 3: // play
                    if (AudioSettings.radio_mode)
                    {
                        if (this.label2.Text != "Radio")
                            this.label2.Text = "Radio";
                    }
                    else
                    {
                        this.end_of_current_song = false;
                        this.pending_play = false;
                        String np = "Now playing ";
                        PlaylistItem np_item = this.playlist[this.playlist_position];

                        if (np_item.Artist.Length > 0)
                            np += np_item.Artist + " - ";

                        np += np_item.Name;

                        if (this.label1.Text != np)
                            this.label1.Text = np;

                        AudioSettings.current_song = np.Substring(12);

                        if (this.SongTitleChanged != null)
                            this.SongTitleChanged(this, new EventArgs());
                    }
                    break;

                case 8: // end of media
                    if (AudioSettings.radio_mode)
                        return;

                    if (AudioSettings.shuffle)
                    {
                        if (this.playlist.Count > 1)
                        {
                            List<int> ids = new List<int>();

                            for (int i = 0; i < this.playlist.Count; i++)
                                if (i != this.playlist_position)
                                    ids.Add(i);

                            this.playlist_position = ids[(int)Math.Floor(this.rnd.NextDouble() * ids.Count)];
                            this.end_of_current_song = true;
                            this.pending_play = false;
                            return;
                        }
                    }

                    this.playlist_position++;

                    if (this.playlist_position < this.playlist.Count)
                    {
                        this.end_of_current_song = true;
                        this.pending_play = false;
                    }
                    else if (AudioSettings.repeat)
                    {
                        this.playlist_position = 0;

                        if (this.playlist_position < this.playlist.Count)
                        {
                            this.end_of_current_song = true;
                            this.pending_play = false;
                        }
                    }
                    else
                    {
                        this.end_of_current_song = false;
                        this.pending_play = false;
                        this.Stopped();
                        this.playListContainer1.SetActiveItem(-1);
                    }
                    break;

                case 9: // buffering
                    if (!AudioSettings.radio_mode)
                        return;

                    if (this.label2.Text != "Loading...")
                        this.label2.Text = "Loading...";
                    break;
            }
        }

        private void MediaError(object sender, _WMPOCXEvents_MediaErrorEvent e)
        {
            if (AudioSettings.radio_mode)
            {
                this.axWindowsMediaPlayer1.Ctlcontrols.stop();
                this.axWindowsMediaPlayer1.settings.autoStart = false;
                this.axWindowsMediaPlayer1.URL = "";
                this.Stopped();
                AudioSettings.radio_mode = false;
                this.audioController1.PlaylistEmpty(this.playlist.Count == 0);
                this.albumArtDisplayer1.UpdateArt();
                MessageBox.Show("Connection to radio was unsuccessful", "cb0t", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Play(object sender, EventArgs e)
        {
            if (this.current_media == null)
            {
                if (this.playlist.Count == 0)
                    return;

                if (AudioSettings.shuffle)
                {
                    if (this.playlist.Count > 1)
                    {
                        List<int> ids = new List<int>();

                        for (int i = 0; i < this.playlist.Count; i++)
                            ids.Add(i);

                        this.playlist_position = ids[(int)Math.Floor(this.rnd.NextDouble() * ids.Count)];
                    }
                    else this.playlist_position = 0;
                }
                else this.playlist_position = 0;

                PlaylistItem item = this.playlist[this.playlist_position];
                this.current_media = this.axWindowsMediaPlayer1.newMedia(item.Path);
                this.axWindowsMediaPlayer1.currentMedia = this.current_media;
                this.playListContainer1.SetActiveItem(this.playlist_position);
            }
            
            this.axWindowsMediaPlayer1.Ctlcontrols.play();
        }

        private void Pause(object sender, EventArgs e)
        {
            this.axWindowsMediaPlayer1.Ctlcontrols.pause();
        }

        private void Stop(object sender, EventArgs e)
        {
            if (AudioSettings.radio_mode)
            {
                this.axWindowsMediaPlayer1.Ctlcontrols.stop();
                this.axWindowsMediaPlayer1.settings.autoStart = false;
                this.axWindowsMediaPlayer1.URL = "";
                this.Stopped();
                AudioSettings.radio_mode = false;
                this.audioController1.PlaylistEmpty(this.playlist.Count == 0);
                this.albumArtDisplayer1.UpdateArt();
            }
            else
            {
                this.axWindowsMediaPlayer1.Ctlcontrols.stop();
                this.Stopped();
            }
        }

        private void VolumeChanged(object sender, AudioPlayerVolumeChanged e)
        {
            this.axWindowsMediaPlayer1.settings.volume = e.volume;
        }

        private void SavePlaylist(object sender, EventArgs e)
        {
            try
            {
                XmlWriterSettings appearance = new XmlWriterSettings();
                appearance.Indent = true;
                XmlWriter xml = XmlWriter.Create(Settings.folder_path + "playlist.xml", appearance);

                xml.WriteStartDocument();
                xml.WriteStartElement("playlist");

                foreach (PlaylistItem item in this.playlist)
                {
                    xml.WriteStartElement("item");

                    xml.WriteElementString("Album", Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Album)));
                    xml.WriteElementString("Artist", Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Artist)));
                    xml.WriteElementString("Author", Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Author)));
                    xml.WriteElementString("Length", Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Length)));
                    xml.WriteElementString("Name", Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Name)));
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
                using (FileStream f = new FileStream(Settings.folder_path + "playlist.xml", FileMode.Open))
                {
                    XmlReader xml = XmlReader.Create(new StreamReader(f));

                    xml.MoveToContent();
                    xml.ReadSubtree().ReadToFollowing("playlist");

                    while (xml.ReadToFollowing("item"))
                    {
                        PlaylistItem item = new PlaylistItem();
                        xml.ReadSubtree().ReadToFollowing("Album");
                        item.Album = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                        xml.ReadToFollowing("Artist");
                        item.Artist = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                        xml.ReadToFollowing("Author");
                        item.Author = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                        xml.ReadToFollowing("Length");
                        item.Length = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                        xml.ReadToFollowing("Name");
                        item.Name = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                        xml.ReadToFollowing("Path");
                        item.Path = Encoding.UTF8.GetString(Convert.FromBase64String(xml.ReadElementContentAsString()));
                        this.playlist.Add(item);
                    }

                    xml.Close();
                }
            }
            catch { }

            if (this.playlist.Count > 0)
            {
                this.playListContainer1.AddPlaylist(this.playlist.ToArray());
                this.audioController1.PlaylistEmpty(false);
            }
        }

        private void AddFileToPlaylist(object sender, AddFileToPlaylistEventArgs e)
        {
            FileInfo info = new FileInfo(e.Filename);

            if (!(info.Extension == ".mp3" || info.Extension == ".wma" || info.Extension == ".wav"))
                return;

            try
            {
                IWMPMedia media = this.axWindowsMediaPlayer1.newMedia(e.Filename);

                if (media.duration > 0)
                {
                    PlaylistItem item = new PlaylistItem();
                    item.Length = media.durationString;
                    item.Name = media.name.Trim();
                    item.Path = e.Filename;
                    item.Artist = String.Empty;
                    item.Album = String.Empty;
                    item.Author = String.Empty;

                    for (int i = 0; i < media.attributeCount; i++)
                    {
                        switch (media.getAttributeName(i).ToUpper())
                        {
                            case "ALBUMIDALBUMARTIST":
                                String[] strs = media.getItemInfo(media.getAttributeName(i)).Split(new String[] { "*;*" }, StringSplitOptions.None);

                                if (strs.Length > 0)
                                    item.Album = strs[0].Trim();
                                if (strs.Length > 1)
                                    item.Artist = strs[1].Trim();
                                break;

                            case "AUTHOR":
                                item.Author = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;

                            case "DISPLAYARTIST":
                                if (item.Artist.Length == 0)
                                    item.Artist = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;

                            case "TITLE":
                                if (item.Name.Length == 0)
                                    item.Name = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;

                            case "WM/ALBUMARTIST":
                                if (item.Artist.Length == 0)
                                    item.Artist = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;

                            case "WM/ALBUMTITLE":
                                if (item.Album.Length == 0)
                                    item.Album = media.getItemInfo(media.getAttributeName(i)).Trim();
                                break;
                        }
                    }

                    if (String.IsNullOrEmpty(item.Artist))
                        item.Artist = item.Author;

                    this.playlist.Add(item);
                    this.playListContainer1.AddPlaylistItem(item);
                    this.audioController1.PlaylistEmpty(false);
                }
            }
            catch { }
        }

        private void AudioPlayer_Resize(object sender, EventArgs e)
        {
            int x = (this.Width / 2) - 194;

            if (x < 0)
                x = 0;

            int y = (this.Height - 53);

            if (y < 0)
                y = 0;

            this.audioController1.Location = new Point(x, y);

            x = this.audioController1.Location.X - 81;

            if (x < 0)
                x = 0;

            y = (this.Height - 37);

            if (y < 0)
                y = 0;

            this.label2.Location = new Point(x, y);
        }
    }

    class PlaylistItem
    {
        public String Artist;
        public String Album;
        public String Name;
        public String Length;
        public String Path;
        public String Author;
    }
}
