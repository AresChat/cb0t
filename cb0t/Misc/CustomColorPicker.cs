using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    public partial class CustomColorPicker : Form
    {
        public CustomColorPicker()
        {
            this.InitializeComponent();
            this.PopulateColorList();
            this.doubleBufferedComboBox1.SelectedIndex = 0;
        }

        public void UpdateTemplate()
        {
            this.label1.Text = StringTemplate.Get(STType.ColorPicker, 0) + ":";
            this.label2.Text = StringTemplate.Get(STType.ColorPicker, 1) + ":";
            this.label7.Text = StringTemplate.Get(STType.ColorPicker, 2) + ":";
        }

        private Pen pen = new Pen(Brushes.Black, 1);
        private SolidBrush text_brush = new SolidBrush(Color.Black);
        private List<Color> list = new List<Color>();

        private void PopulateColorList()
        {
            MethodAttributes attributes = MethodAttributes.Static | MethodAttributes.Public;
            PropertyInfo[] properties = typeof(Color).GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo info = properties[i];

                if (info.PropertyType == typeof(Color))
                {
                    MethodInfo getMethod = info.GetGetMethod();

                    if ((getMethod != null) && ((getMethod.Attributes & attributes) == attributes))
                        this.list.Add((Color)info.GetValue(null, null));
                }
            }

            this.list.Sort(new StandardColorComparer());

            foreach (Color c in this.list)
                this.doubleBufferedComboBox1.Items.Add(c);

            this.doubleBufferedComboBox2.Items.Add(Color.White);
            this.doubleBufferedComboBox2.Items.Add(Color.Black);
            this.doubleBufferedComboBox2.Items.Add(Color.Navy);
            this.doubleBufferedComboBox2.Items.Add(Color.Green);
            this.doubleBufferedComboBox2.Items.Add(Color.Red);
            this.doubleBufferedComboBox2.Items.Add(Color.Maroon);
            this.doubleBufferedComboBox2.Items.Add(Color.Purple);
            this.doubleBufferedComboBox2.Items.Add(Color.Orange);
            this.doubleBufferedComboBox2.Items.Add(Color.Yellow);
            this.doubleBufferedComboBox2.Items.Add(Color.Lime);
            this.doubleBufferedComboBox2.Items.Add(Color.Teal);
            this.doubleBufferedComboBox2.Items.Add(Color.Aqua);
            this.doubleBufferedComboBox2.Items.Add(Color.Blue);
            this.doubleBufferedComboBox2.Items.Add(Color.Fuchsia);
            this.doubleBufferedComboBox2.Items.Add(Color.Gray);
            this.doubleBufferedComboBox2.Items.Add(Color.Silver);
            this.doubleBufferedComboBox1.SelectedIndex = -1;
        }

        private void doubleBufferedComboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index > -1)
            {
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);

                Color color = (Color)this.doubleBufferedComboBox1.Items[e.Index];

                using (SolidBrush sb = new SolidBrush(color))
                    e.Graphics.FillRectangle(sb, new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, 18, 12));

                e.Graphics.DrawRectangle(this.pen, new Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, 17, 11));

                if (e.Index == 0)
                {
                    e.Graphics.DrawString("Custom", this.Font, this.text_brush, new PointF(e.Bounds.X + 24, e.Bounds.Y));
                    e.Graphics.DrawString("??", this.Font, this.text_brush, new PointF(e.Bounds.X + 2, e.Bounds.Y));
                }
                else e.Graphics.DrawString(color.Name, this.Font, this.text_brush, new PointF(e.Bounds.X + 24, e.Bounds.Y));
            }
        }

        private void doubleBufferedComboBox2_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index > -1)
            {
                using (SolidBrush sb = new SolidBrush((Color)this.doubleBufferedComboBox2.Items[e.Index]))
                    e.Graphics.FillRectangle(sb, e.Bounds);
            }
        }

        private bool user_changing = false;

        private void doubleBufferedComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!this.user_changing)
            {
                if (this.doubleBufferedComboBox1.SelectedIndex == 0)
                {
                    this.panel1.BackColor = Color.White;
                    this.trackBar1.Value = 255;
                    this.label3.Text = "R: 255";
                    this.trackBar2.Value = 255;
                    this.label4.Text = "G: 255";
                    this.trackBar3.Value = 255;
                    this.label5.Text = "B: 255";
                    this.textBox1.Text = String.Format("#{0:x2}{1:x2}{2:x2}", (byte)this.trackBar1.Value, (byte)this.trackBar2.Value, (byte)this.trackBar3.Value);
                }
                else
                {
                    this.panel1.BackColor = this.list[this.doubleBufferedComboBox1.SelectedIndex];
                    this.trackBar1.Value = this.panel1.BackColor.R;
                    this.label3.Text = "R: " + this.trackBar1.Value;
                    this.trackBar2.Value = this.panel1.BackColor.G;
                    this.label4.Text = "G: " + this.trackBar2.Value;
                    this.trackBar3.Value = this.panel1.BackColor.B;
                    this.label5.Text = "B: " + this.trackBar3.Value;
                    this.textBox1.Text = String.Format("#{0:x2}{1:x2}{2:x2}", (byte)this.trackBar1.Value, (byte)this.trackBar2.Value, (byte)this.trackBar3.Value);
                }
            }
            else this.user_changing = false;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            Color color = Color.FromArgb(this.trackBar1.Value, this.trackBar2.Value, this.trackBar3.Value);

            for (int i = 1; i < this.list.Count; i++)
                if (this.list[i].RGBEquals(color))
                    if (this.doubleBufferedComboBox1.SelectedIndex != i)
                    {
                        this.user_changing = true;
                        this.doubleBufferedComboBox1.SelectedIndex = i;
                        this.panel1.BackColor = color;
                        this.label3.Text = "R: " + this.trackBar1.Value;
                        this.label4.Text = "G: " + this.trackBar2.Value;
                        this.label5.Text = "B: " + this.trackBar3.Value;
                        this.textBox1.Text = String.Format("#{0:x2}{1:x2}{2:x2}", (byte)this.trackBar1.Value, (byte)this.trackBar2.Value, (byte)this.trackBar3.Value);
                        return;
                    }

            if (this.doubleBufferedComboBox1.SelectedIndex != 0)
            {
                this.user_changing = true;
                this.doubleBufferedComboBox1.SelectedIndex = 0;
            }

            this.panel1.BackColor = color;
            this.label3.Text = "R: " + this.trackBar1.Value;
            this.label4.Text = "G: " + this.trackBar2.Value;
            this.label5.Text = "B: " + this.trackBar3.Value;
            this.textBox1.Text = String.Format("#{0:x2}{1:x2}{2:x2}", (byte)this.trackBar1.Value, (byte)this.trackBar2.Value, (byte)this.trackBar3.Value);
        }

        public Color SelectedColor
        {
            get { return this.panel1.BackColor; }

            set
            {
                this.panel1.BackColor = value;
                this.trackBar1.Value = this.panel1.BackColor.R;
                this.trackBar2.Value = this.panel1.BackColor.G;
                this.trackBar3.Value = this.panel1.BackColor.B;
                this.trackBar1_Scroll(null, null);
            }
        }

        public String SelectedHex
        {
            get
            {
                return String.Format("{0:x2}{1:x2}{2:x2}", (byte)this.trackBar1.Value, (byte)this.trackBar2.Value, (byte)this.trackBar3.Value);
            }
            set
            {
                if (value.Length == 6)
                {
                    String r_str = value.Substring(0, 2);
                    String g_str = value.Substring(2, 2);
                    String b_str = value.Substring(4, 2);

                    byte r, g, b;

                    if (byte.TryParse(r_str, NumberStyles.HexNumber, null, out r))
                        if (byte.TryParse(g_str, NumberStyles.HexNumber, null, out g))
                            if (byte.TryParse(b_str, NumberStyles.HexNumber, null, out b))
                            {
                                this.trackBar1.Value = r;
                                this.trackBar2.Value = g;
                                this.trackBar3.Value = b;
                                this.trackBar1_Scroll(null, null);
                            }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String str = this.textBox1.Text.Replace("#", String.Empty).Replace(" ", String.Empty);

            if (str.Length == 6)
            {
                String r_str = str.Substring(0, 2);
                String g_str = str.Substring(2, 2);
                String b_str = str.Substring(4, 2);

                byte r, g, b;

                if (byte.TryParse(r_str, NumberStyles.HexNumber, null, out r))
                    if (byte.TryParse(g_str, NumberStyles.HexNumber, null, out g))
                        if (byte.TryParse(b_str, NumberStyles.HexNumber, null, out b))
                        {
                            this.trackBar1.Value = r;
                            this.trackBar2.Value = g;
                            this.trackBar3.Value = b;
                            this.trackBar1_Scroll(null, null);
                            return;
                        }

                MessageBox.Show("invalid hex color",
                                "cb0t",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void doubleBufferedComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.doubleBufferedComboBox2.SelectedIndex > -1)
            {
                this.panel1.BackColor = (Color)this.doubleBufferedComboBox2.SelectedItem;
                this.trackBar1.Value = this.panel1.BackColor.R;
                this.label3.Text = "R: " + this.trackBar1.Value;
                this.trackBar2.Value = this.panel1.BackColor.G;
                this.label4.Text = "G: " + this.trackBar2.Value;
                this.trackBar3.Value = this.panel1.BackColor.B;
                this.label5.Text = "B: " + this.trackBar3.Value;
                this.textBox1.Text = String.Format("#{0:x2}{1:x2}{2:x2}", (byte)this.trackBar1.Value, (byte)this.trackBar2.Value, (byte)this.trackBar3.Value);

                for (int i = 1; i < this.list.Count; i++)
                    if (this.list[i].RGBEquals(this.panel1.BackColor))
                        if (this.doubleBufferedComboBox1.SelectedIndex != i)
                        {
                            this.doubleBufferedComboBox1.SelectedIndex = i;
                            break;
                        }

                this.doubleBufferedComboBox2.SelectedIndex = -1;
                this.DialogResult = DialogResult.OK;
            }
        }
    }

    class StandardColorComparer : IComparer<Color>
    {
        public int Compare(Color color, Color color2)
        {
            if (color.A < color2.A)
                return -1;

            if (color.A > color2.A)
                return 1;

            if (color.GetHue() < color2.GetHue())
                return -1;

            if (color.GetHue() > color2.GetHue())
                return 1;

            if (color.GetSaturation() < color2.GetSaturation())
                return -1;

            if (color.GetSaturation() > color2.GetSaturation())
                return 1;

            if (color.GetBrightness() < color2.GetBrightness())
                return -1;

            if (color.GetBrightness() > color2.GetBrightness())
                return 1;

            return 0;
        }
    }
}
