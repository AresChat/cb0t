using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    public partial class Scribble : Form
    {
        public Scribble()
        {
            InitializeComponent();
            this.scribbleScreen1.Init();
        }

        public void SetFormText(String text)
        {
            this.Text = text;
        }

        internal ScribbleObject GetScribble()
        {
            return this.scribbleScreen1.ExportPicture();
        }

        private void Scribble_Move(object sender, EventArgs e)
        {
            this.scribbleScreen1.Invalidate();
            this.colorButton1.Invalidate();
        }

        private void colorButton1_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();

            if (cd.ShowDialog() == DialogResult.OK)
            {
                this.scribbleScreen1.pen = cd.Color;
                this.colorButton1.SelectedColor = cd.Color;
            }

            this.scribbleScreen1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.scribbleScreen1.brush = ScribbleScreen.ScribbleType.Thin;
            this.button1.BackColor = Color.DarkGray;
            this.button2.BackColor = Color.Gainsboro;
            this.button3.BackColor = Color.Gainsboro;
            this.scribbleScreen1.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.scribbleScreen1.brush = ScribbleScreen.ScribbleType.Medium;
            this.button1.BackColor = Color.Gainsboro;
            this.button2.BackColor = Color.DarkGray;
            this.button3.BackColor = Color.Gainsboro;
            this.scribbleScreen1.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.scribbleScreen1.brush = ScribbleScreen.ScribbleType.Thick;
            this.button1.BackColor = Color.Gainsboro;
            this.button2.BackColor = Color.Gainsboro;
            this.button3.BackColor = Color.DarkGray;
            this.scribbleScreen1.Focus();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.scribbleScreen1.Init();
        }
    }
}