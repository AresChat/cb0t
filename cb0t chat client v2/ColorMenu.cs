using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class ColorMenu : ContextMenuStrip
    {
        private TextBox sendto = new TextBox();

        public bool foreground = true;

        public ColorMenu(TextBox sendto)
        {
            this.sendto = sendto;
            this.Opacity = 0.8;

            for (int i = 0; i < 16; i++) this.Items.Add("");

            this.Items[0].BackColor = Color.Black;
            this.Items[1].BackColor = Color.Maroon;
            this.Items[2].BackColor = Color.Green;
            this.Items[3].BackColor = Color.Orange;
            this.Items[4].BackColor = Color.Navy;
            this.Items[5].BackColor = Color.Purple;
            this.Items[6].BackColor = Color.Teal;
            this.Items[7].BackColor = Color.Gray;
            this.Items[8].BackColor = Color.Silver;
            this.Items[9].BackColor = Color.Red;
            this.Items[10].BackColor = Color.Lime;
            this.Items[11].BackColor = Color.Yellow;
            this.Items[12].BackColor = Color.Blue;
            this.Items[13].BackColor = Color.Fuchsia;
            this.Items[14].BackColor = Color.Aqua;
            this.Items[15].BackColor = Color.White;
        }

        protected override void OnItemClicked(ToolStripItemClickedEventArgs e)
        {
            base.OnItemClicked(e);

            int selected = this.ColorCode(e.ClickedItem.BackColor.Name);

            if (selected > -1)
            {
                String str = foreground ? "3" : "5";
                str += selected < 10 ? ("0" + selected) : selected.ToString();
                this.sendto.Text += str;
                this.sendto.SelectionStart = this.sendto.Text.Length;
                this.sendto.Focus();
            }
        }

        private int ColorCode(String name)
        {
            switch (name)
            {
                case "Black": return 1;
                case "Maroon": return 5;
                case "Green": return 3;
                case "Orange": return 7;
                case "Navy": return 2;
                case "Purple": return 6;
                case "Teal": return 10;
                case "Gray": return 14;
                case "Silver": return 15;
                case "Red": return 4;
                case "Lime": return 9;
                case "Yellow": return 8;
                case "Blue": return 12;
                case "Fuchsia": return 13;
                case "Aqua": return 11;
                case "White": return 0;
            }

            return -1;
        }
    }
}
