using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Text;

namespace cb0t
{
    public partial class PersonalSettings : UserControl
    {
        public PersonalSettings()
        {
            InitializeComponent();
        }

        public void UpdateTemplate()
        {
            this.label1.Text = StringTemplate.Get(STType.Settings, 7);
            this.label3.Text = StringTemplate.Get(STType.PersonalSettings, 0) + ":";
            this.label9.Text = StringTemplate.Get(STType.PersonalSettings, 0) + ":";
            this.label6.Text = StringTemplate.Get(STType.PersonalSettings, 1) + ":";
            this.label7.Text = StringTemplate.Get(STType.PersonalSettings, 2) + ":";
            this.label4.Text = StringTemplate.Get(STType.PersonalSettings, 3) + ":";
            this.label5.Text = StringTemplate.Get(STType.PersonalSettings, 4) + ":";
            this.comboBox2.Items[0] = StringTemplate.Get(STType.PersonalSettings, 5);
            this.comboBox3.Items[0] = StringTemplate.Get(STType.PersonalSettings, 5);
            this.comboBox4.Items[0] = StringTemplate.Get(STType.PersonalSettings, 5);
            this.label2.Text = StringTemplate.Get(STType.PersonalSettings, 6) + ":";
            this.button1.Text = StringTemplate.Get(STType.PersonalSettings, 7);
            this.button2.Text = StringTemplate.Get(STType.PersonalSettings, 7);
            this.button4.Text = StringTemplate.Get(STType.PersonalSettings, 7);
            this.label8.Text = StringTemplate.Get(STType.PersonalSettings, 8) + ": [+n]";
            this.checkBox1.Text = StringTemplate.Get(STType.PersonalSettings, 9);
            this.groupBox4.Text = StringTemplate.Get(STType.PersonalSettings, 10);
            this.button3.Text = StringTemplate.Get(STType.PersonalSettings, 11);
            this.groupBox3.Text = StringTemplate.Get(STType.PersonalSettings, 12);
            this.label10.Text = StringTemplate.Get(STType.PersonalSettings, 13) + ":";
            this.label12.Text = StringTemplate.Get(STType.PersonalSettings, 14) + ":";
            this.label11.Text = StringTemplate.Get(STType.PersonalSettings, 15) + ":";
            this.checkBox2.Text = StringTemplate.Get(STType.PersonalSettings, 16);
        }

        public void Populate()
        {
            String str;
            this.textBox2.Text = Settings.GetReg<String>("user_region", String.Empty);
            this.textBox3.Text = Settings.GetReg<String>("username", String.Empty);
            this.comboBox4.Items.Add("Secret");

            for (int i = 1; i < 256; i++)
            {
                str = Helpers.CountryCodeToString((byte)i);

                if (str != "?")
                    this.comboBox4.Items.Add(str);
            }

            this.comboBox4.SelectedIndex = Settings.GetReg<int>("user_country", 0);

            for (int i = 0; i < 100; i++)
                this.comboBox2.Items.Add(i == 0 ? "Secret" : i.ToString());

            this.comboBox2.SelectedIndex = Settings.GetReg<int>("user_age", 0);
            this.comboBox3.SelectedIndex = Settings.GetReg<int>("user_gender", 0);
            this.textBox4.Text = Settings.GetReg<String>("pm_reply", "Hello +n, please leave a message.");
            this.checkBox1.Checked = Settings.GetReg<bool>("can_pm_reply", true);
            this.textBox1.Text = Settings.GetReg<String>("personal_message", String.Empty);

            if (Avatar.Image != null)
            {
                this.button3.Enabled = true;
                this.pictureBox1.Image = (Bitmap)Avatar.Image.Clone();
            }

            SharedUI.ColorPicker.SelectedHex = Settings.GetReg<String>("name_color", "000000");
            this.panel2.BackColor = SharedUI.ColorPicker.SelectedColor;
            SharedUI.ColorPicker.SelectedHex = Settings.GetReg<String>("text_color", "0000FF");
            this.panel1.BackColor = SharedUI.ColorPicker.SelectedColor;
            InstalledFontCollection fonts = new InstalledFontCollection();

            for (int i = 0; i < fonts.Families.Length; i++)
                this.comboBox1.Items.Add(fonts.Families[i].Name);

            str = Settings.GetReg<String>("user_font", "Tahoma");

            for (int i = 0; i < this.comboBox1.Items.Count; i++)
                if ((String)this.comboBox1.Items[i] == str)
                {
                    this.comboBox1.SelectedIndex = i;
                    break;
                }

            this.numericUpDown1.Value = Settings.GetReg<int>("user_font_size", 10);
            this.checkBox2.Checked = Settings.GetReg<bool>("user_font_enabled", false);

            if (this.checkBox2.Checked)
            {
                Settings.MyFont = new AresFont
                {
                    FontName = Settings.GetReg<String>("user_font", "Tahoma"),
                    NameColor = Settings.GetReg<String>("name_color", "000000"),
                    Size = Settings.GetReg<int>("user_font_size", 10),
                    TextColor = Settings.GetReg<String>("text_color", "0000FF")
                };
            }
            else Settings.MyFont = null;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Settings.SetReg("user_region", this.textBox2.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Settings.SetReg("username", this.textBox3.Text);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.SetReg("user_country", this.comboBox4.SelectedIndex);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.SetReg("user_age", this.comboBox2.SelectedIndex);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.SetReg("user_gender", this.comboBox3.SelectedIndex);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Settings.SetReg("pm_reply", this.textBox4.Text);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("can_pm_reply", this.checkBox1.Checked);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Settings.CAN_WRITE_REG)
                this.button4.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Settings.SetReg("personal_message", this.textBox1.Text);
            this.button4.Enabled = false;
            RoomPool.Rooms.ForEach(x => x.UpdatePersonalMessage());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Avatar.Clear();

            if (this.pictureBox1.Image != null)
            {
                this.pictureBox1.Image.Dispose();
                this.pictureBox1.Image = null;
            }

            this.button3.Enabled = false;
            RoomPool.Rooms.ForEach(x => x.UpdateAvatar());
        }

        private OpenFileDialog file_dialog = new OpenFileDialog();

        private void button2_Click(object sender, EventArgs e)
        {
            this.file_dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            this.file_dialog.Filter = "Supported Types|*.bmp;*.jpg;*.png|BMP (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png";
            this.file_dialog.Multiselect = false;

            if (this.file_dialog.ShowDialog() == DialogResult.OK)
            {
                String path = this.file_dialog.FileName;
                Avatar.Update(path);

                if (this.pictureBox1.Image != null)
                {
                    this.pictureBox1.Image.Dispose();
                    this.pictureBox1.Image = null;
                }

                if (Avatar.Image == null)
                {
                    this.button3.Enabled = false;
                    RoomPool.Rooms.ForEach(x => x.UpdateAvatar());
                }
                else
                {
                    this.button3.Enabled = true;
                    this.pictureBox1.Image = (Bitmap)Avatar.Image.Clone();
                    RoomPool.Rooms.ForEach(x => x.UpdateAvatar());
                }
            }
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            SharedUI.ColorPicker.SelectedColor = this.panel2.BackColor;
            SharedUI.ColorPicker.StartPosition = FormStartPosition.CenterParent;

            if (SharedUI.ColorPicker.ShowDialog() == DialogResult.OK)
            {
                this.panel2.BackColor = SharedUI.ColorPicker.SelectedColor;

                if (Settings.CAN_WRITE_REG)
                    this.button1.Enabled = true;
            }
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            SharedUI.ColorPicker.SelectedColor = this.panel1.BackColor;
            SharedUI.ColorPicker.StartPosition = FormStartPosition.CenterParent;

            if (SharedUI.ColorPicker.ShowDialog() == DialogResult.OK)
            {
                this.panel1.BackColor = SharedUI.ColorPicker.SelectedColor;

                if (Settings.CAN_WRITE_REG)
                    this.button1.Enabled = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex > -1)
                if (Settings.CAN_WRITE_REG)
                    this.button1.Enabled = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (Settings.CAN_WRITE_REG)
                this.button1.Enabled = true;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("user_font_enabled", this.checkBox2.Checked);

            if (Settings.CAN_WRITE_REG)
                if (this.checkBox2.Checked)
                {
                    Settings.MyFont = new AresFont
                    {
                        FontName = Settings.GetReg<String>("user_font", "Tahoma"),
                        NameColor = Settings.GetReg<String>("name_color", "000000"),
                        Size = Settings.GetReg<int>("user_font_size", 10),
                        TextColor = Settings.GetReg<String>("text_color", "0000FF")
                    };

                    RoomPool.Rooms.ForEach(x => x.UpdateFont());
                }
                else Settings.MyFont = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.SetReg("user_font_size", (int)this.numericUpDown1.Value);
            Settings.SetReg("user_font", (String)this.comboBox1.SelectedItem);
            SharedUI.ColorPicker.SelectedColor = this.panel1.BackColor;
            Settings.SetReg("text_color", SharedUI.ColorPicker.SelectedHex);
            SharedUI.ColorPicker.SelectedColor = this.panel2.BackColor;
            Settings.SetReg("name_color", SharedUI.ColorPicker.SelectedHex);

            if (this.checkBox2.Checked)
            {
                Settings.MyFont = new AresFont
                {
                    FontName = Settings.GetReg<String>("user_font", "Tahoma"),
                    NameColor = Settings.GetReg<String>("name_color", "000000"),
                    Size = Settings.GetReg<int>("user_font_size", 10),
                    TextColor = Settings.GetReg<String>("text_color", "0000FF")
                };

                RoomPool.Rooms.ForEach(x => x.UpdateFont());
            }

            this.button1.Enabled = false;
        }
    }
}
