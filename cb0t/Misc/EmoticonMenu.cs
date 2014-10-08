using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    public partial class EmoticonMenu : Form
    {
        private ImageList ilist { get; set; }
        private EmojiMenuBar mbar { get; set; }

        private Emoji_People e_people { get; set; }
        private Emoji_Nature e_nature { get; set; }
        private Emoji_Objects e_objects { get; set; }
        private Emoji_Places e_places { get; set; }
        private Emoji_Symbols e_symbols { get; set; }

        public EmoticonMenu()
        {
            this.InitializeComponent();
            this.ClientSize = new Size(232, 267);
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.emRegMenu1.EmoticonClicked += this.RegEmoticonClicked;
            this.VisibleChanged += this.EmoticonMenu_VisibleChanged;

            String path = Path.Combine(Settings.AniEmoticPath, "ext");
            FileInfo[] files = new DirectoryInfo(path).GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                PictureBox pb = new PictureBox();
                pb.BackColor = Color.White;
                pb.Size = new Size(50, 50);
                pb.Location = new Point(1 + ((i % 4) * 50) + (i % 4), 1 + ((i / 4) * 50) + (i / 4));
                pb.ImageLocation = files[i].FullName;
                pb.SizeMode = PictureBoxSizeMode.CenterImage | PictureBoxSizeMode.StretchImage;
                pb.Tag = Path.GetFileNameWithoutExtension(files[i].FullName);
                pb.Click += this.EmoticonClicked;
                pb.Cursor = Cursors.Hand;
                this.emExtMenu1.Controls.Add(pb);
            }

            this.ilist = new ImageList();
            this.ilist.Images.Add((Bitmap)Emoticons.emotic[0].Clone());
            this.ilist.Images.Add((Bitmap)Emoji.EmojiFromFileName("1f43c.png").Image.Clone());
            this.tabControl1.ImageList = this.ilist;
            this.tabControl1.TabPages[0].ImageIndex = 0;
            this.tabControl1.TabPages[1].ImageIndex = 1;
            this.tabControl1.SelectedIndexChanged += this.MenuSelectedIndexChanged;

            this.toolStripButton1.Image = (Bitmap)Emoji.EmojiFromFileName("1f604.png").Image.Clone();
            this.toolStripButton1.Tag = EmojiMenuBarSelectedItem.People;
            this.toolStripButton2.Image = (Bitmap)Emoji.EmojiFromFileName("1f338.png").Image.Clone();
            this.toolStripButton2.Tag = EmojiMenuBarSelectedItem.Nature;
            this.toolStripButton3.Image = (Bitmap)Emoji.EmojiFromFileName("1f514.png").Image.Clone();
            this.toolStripButton3.Tag = EmojiMenuBarSelectedItem.Objects;
            this.toolStripButton4.Image = (Bitmap)Emoji.EmojiFromFileName("1f698.png").Image.Clone();
            this.toolStripButton4.Tag = EmojiMenuBarSelectedItem.Places;
            this.toolStripButton5.Image = (Bitmap)Emoji.EmojiFromFileName("1f523.png").Image.Clone();
            this.toolStripButton5.Tag = EmojiMenuBarSelectedItem.Symbols;

            this.mbar = new EmojiMenuBar();
            this.toolStrip1.Renderer = this.mbar;
            this.toolStrip1.ItemClicked += this.ToolstripItemClicked;

            this.e_people = new Emoji_People();
            this.e_people.Dock = DockStyle.Fill;
            this.e_people.AutoScroll = true;
            this.e_people.BackColor = Color.White;
            this.e_people.Populate(this.EmojiClicked);

            this.e_nature = new Emoji_Nature();
            this.e_nature.Dock = DockStyle.Fill;
            this.e_nature.AutoScroll = true;
            this.e_nature.BackColor = Color.White;
            this.e_nature.Populate(this.EmojiClicked);

            this.e_objects = new Emoji_Objects();
            this.e_objects.Dock = DockStyle.Fill;
            this.e_objects.AutoScroll = true;
            this.e_objects.BackColor = Color.White;
            this.e_objects.Populate(this.EmojiClicked);

            this.e_places = new Emoji_Places();
            this.e_places.Dock = DockStyle.Fill;
            this.e_places.AutoScroll = true;
            this.e_places.BackColor = Color.White;
            this.e_places.Populate(this.EmojiClicked);

            this.e_symbols = new Emoji_Symbols();
            this.e_symbols.Dock = DockStyle.Fill;
            this.e_symbols.AutoScroll = true;
            this.e_symbols.BackColor = Color.White;
            this.e_symbols.Populate(this.EmojiClicked);

            this.panel1.Controls.Add(this.e_people);
        }

        private void EmojiClicked(object sender, EventArgs e)
        {
            String shortcut = ((EmojiMenuShortcutItem)((PictureBox)sender).Tag).Shortcut;

            if (this.callback != null)
            {
                this.callback.EmoticonCallback(shortcut);
                this.callback = null;
            }

            this.Hide();
        }

        private void ToolstripItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is ToolStripButton)
            {
                EmojiMenuBarSelectedItem item = (EmojiMenuBarSelectedItem)e.ClickedItem.Tag;

                if (this.mbar.SelectedItem == item)
                    return;

                this.mbar.SelectedItem = item;
                this.toolStrip1.Invalidate();

                while (this.panel1.Controls.Count > 0)
                    this.panel1.Controls.RemoveAt(0);

                switch (item)
                {
                    case EmojiMenuBarSelectedItem.People:
                        this.panel1.Controls.Add(this.e_people);
                        break;

                    case EmojiMenuBarSelectedItem.Nature:
                        this.panel1.Controls.Add(this.e_nature);
                        break;

                    case EmojiMenuBarSelectedItem.Objects:
                        this.panel1.Controls.Add(this.e_objects);
                        break;

                    case EmojiMenuBarSelectedItem.Places:
                        this.panel1.Controls.Add(this.e_places);
                        break;

                    case EmojiMenuBarSelectedItem.Symbols:
                        this.panel1.Controls.Add(this.e_symbols);
                        break;
                }

                if (this.panel1.Controls.Count > 0)
                    this.panel1.Controls[0].BeginInvoke((Action)(() => this.panel1.Controls[0].Focus()));
            }
        }

        public void UpdateTemplate()
        {
            this.tabControl1.TabPages[0].Text = StringTemplate.Get(STType.ButtonBar, 5);
            this.tabControl1.TabPages[1].Text = "emoji";
        }

        private void MenuSelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.tabControl1.SelectedIndex)
            {
                case 0:
                    this.emExtMenu1.BeginInvoke((Action)(() => this.emExtMenu1.Focus()));
                    break;

                case 1:
                    if (this.panel1.Controls.Count > 0)
                        this.panel1.Controls[0].BeginInvoke((Action)(() => this.panel1.Controls[0].Focus()));
                    break;
            }
        }

        private void EmoticonMenu_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
                this.emExtMenu1.BeginInvoke((Action)(() => this.emExtMenu1.Focus()));
        }

        private RoomPanel callback = null;

        public void SetCallback(RoomPanel cb)
        {
            this.callback = cb;
        }

        private void EmoticonMenu_Deactivate(object sender, EventArgs e)
        {
            this.callback = null;
            this.Hide();
        }

        private void RegEmoticonClicked(object sender, EmoticonShortcutEventArgs e)
        {
            if (this.callback != null)
            {
                this.callback.EmoticonCallback(e.Shortcut);
                this.callback = null;
            }

            this.Hide();
        }

        private void EmoticonClicked(object sender, EventArgs e)
        {
            String shortcut = (String)((PictureBox)sender).Tag;

            if (this.callback != null)
            {
                this.callback.EmoticonCallback("(" + shortcut + ")");
                this.callback = null;
            }

            this.Hide();
        }
    }

    class EmojiMenuShortcutItem
    {
        public String SurrogateSequence { get; set; }
        public String Shortcut { get; set; }
        public String Description { get; set; }
    }
}
