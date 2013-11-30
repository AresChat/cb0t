using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    public partial class MenuSettings : UserControl
    {
        public MenuSettings()
        {
            InitializeComponent();
        }

        public void UpdateTemplate()
        {
            this.label1.Text = StringTemplate.Get(STType.Settings, 13);
            this.label2.Text = StringTemplate.Get(STType.MenuSettings, 0) + ":";
            this.label3.Text = StringTemplate.Get(STType.MenuSettings, 1) + ":";
            this.dataGridView1.Columns[0].HeaderText = StringTemplate.Get(STType.MenuSettings, 2);
            this.dataGridView2.Columns[0].HeaderText = StringTemplate.Get(STType.MenuSettings, 2);
            this.dataGridView1.Columns[1].HeaderText = StringTemplate.Get(STType.MenuSettings, 3);
            this.dataGridView2.Columns[1].HeaderText = StringTemplate.Get(STType.MenuSettings, 3);
            this.removeToolStripMenuItem.Text = StringTemplate.Get(STType.MenuSettings, 4);
            this.removeToolStripMenuItem1.Text = StringTemplate.Get(STType.MenuSettings, 4);
            this.groupBox1.Text = StringTemplate.Get(STType.MenuSettings, 5);
            this.label4.Text = StringTemplate.Get(STType.MenuSettings, 6) + ":";
            this.label5.Text = StringTemplate.Get(STType.MenuSettings, 7) + ": [+n]";
            this.button1.Text = StringTemplate.Get(STType.MenuSettings, 8);
        }

        public void Populate()
        {
            this.comboBox1.SelectedIndex = 0;

            foreach (CustomMenuOption o in Menus.UserList)
            {
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[0].Value = o.Name;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[1].Value = o.Text;
            }

            foreach (CustomMenuOption o in Menus.Room)
            {
                this.dataGridView2.Rows.Add();
                this.dataGridView2.Rows[this.dataGridView2.Rows.Count - 1].Cells[0].Value = o.Name;
                this.dataGridView2.Rows[this.dataGridView2.Rows.Count - 1].Cells[1].Value = o.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String name = this.textBox1.Text;
            String text = this.textBox2.Text;

            if (String.IsNullOrEmpty(name))
            {
                MessageBox.Show("Missing option name", "cb0t", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (String.IsNullOrEmpty(text))
            {
                MessageBox.Show("Missing action text", "cb0t", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (this.comboBox1.SelectedIndex == 0)
            {
                if (Menus.UserList.Find(x => x.Name == name) != null)
                {
                    MessageBox.Show("This menu option already exists", "cb0t", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Menus.UserList.Add(new CustomMenuOption { Name = name, Text = text });
                Menus.UpdateUL();
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[0].Value = name;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[1].Value = text;
            }
            else if (this.comboBox1.SelectedIndex == 1)
            {
                if (Menus.Room.Find(x => x.Name == name) != null)
                {
                    MessageBox.Show("This menu option already exists", "cb0t", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Menus.Room.Add(new CustomMenuOption { Name = name, Text = text });
                Menus.UpdateR();
                this.dataGridView2.Rows.Add();
                this.dataGridView2.Rows[this.dataGridView2.Rows.Count - 1].Cells[0].Value = name;
                this.dataGridView2.Rows[this.dataGridView2.Rows.Count - 1].Cells[1].Value = text;
            }

            this.textBox1.Clear();
            this.textBox2.Clear();
            this.comboBox1.SelectedIndex = 0;
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e) // remove1
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                int index = this.dataGridView1.SelectedRows[0].Index;

                if (index > -1 && index < Menus.UserList.Count)
                {
                    Menus.UserList.RemoveAt(index);
                    Menus.UpdateUL();
                    DataGridViewRow row = this.dataGridView1.Rows[index];
                    this.dataGridView1.Rows.RemoveAt(index);

                    foreach (DataGridViewCell d in row.Cells)
                        d.Dispose();

                    row.Dispose();
                }
            }
        }

        private void removeToolStripMenuItem1_Click(object sender, EventArgs e) // remove2
        {
            if (this.dataGridView2.SelectedRows.Count > 0)
            {
                int index = this.dataGridView2.SelectedRows[0].Index;

                if (index > -1 && index < Menus.Room.Count)
                {
                    Menus.Room.RemoveAt(index);
                    Menus.UpdateR();
                    DataGridViewRow row = this.dataGridView2.Rows[index];
                    this.dataGridView2.Rows.RemoveAt(index);

                    foreach (DataGridViewCell d in row.Cells)
                        d.Dispose();

                    row.Dispose();
                }
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e) // 1
        {
            if (e.Button == MouseButtons.Right)
                this.contextMenuStrip1.Show(this.dataGridView1, e.Location);
        }

        private void dataGridView2_MouseClick(object sender, MouseEventArgs e) // 2
        {
            if (e.Button == MouseButtons.Right)
                this.contextMenuStrip2.Show(this.dataGridView2, e.Location);
        }
    }
}
