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
    public partial class FilterSettings : UserControl
    {
        private FilterDialog filter_dialog = new FilterDialog();

        public FilterSettings()
        {
            InitializeComponent();
        }

        public void Populate()
        {
            foreach (FilterItem f in Filter.Items)
            {
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[0].Value = f.Room;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[1].Value = f.Event;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[2].Value = f.Property;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[3].Value = f.Condition;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[4].Value = f.Argument;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[5].Value = f.Task;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[6].Value = f.Text;
            }

            this.checkBox1.Checked = Settings.GetReg<bool>("filter_on", false);
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.filter_dialog.ResetForm();
            this.filter_dialog.StartPosition = FormStartPosition.CenterParent;

            if (this.filter_dialog.ShowDialog() == DialogResult.OK)
            {
                FilterItem f = this.filter_dialog.Item;
                Filter.Items.Add(f);
                Filter.Update();
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[0].Value = f.Room;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[1].Value = f.Event;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[2].Value = f.Property;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[3].Value = f.Condition;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[4].Value = f.Argument;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[5].Value = f.Task;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[6].Value = f.Text;
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                int index = this.dataGridView1.SelectedRows[0].Index;

                if (index > -1 && index < Filter.Items.Count)
                {
                    FilterItem org = Filter.Items[index];
                    this.filter_dialog.SetForm(org);
                    this.filter_dialog.StartPosition = FormStartPosition.CenterParent;

                    if (this.filter_dialog.ShowDialog() == DialogResult.OK)
                    {
                        FilterItem f = this.filter_dialog.Item;
                        Filter.Items[index] = f;
                        Filter.Update();
                        this.dataGridView1.Rows[index].Cells[0].Value = f.Room;
                        this.dataGridView1.Rows[index].Cells[1].Value = f.Event;
                        this.dataGridView1.Rows[index].Cells[2].Value = f.Property;
                        this.dataGridView1.Rows[index].Cells[3].Value = f.Condition;
                        this.dataGridView1.Rows[index].Cells[4].Value = f.Argument;
                        this.dataGridView1.Rows[index].Cells[5].Value = f.Task;
                        this.dataGridView1.Rows[index].Cells[6].Value = f.Text;
                    }
                }
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                int index = this.dataGridView1.SelectedRows[0].Index;

                if (index > -1 && index < Filter.Items.Count)
                {
                    Filter.Items.RemoveAt(index);
                    Filter.Update();
                    DataGridViewRow row = this.dataGridView1.Rows[index];
                    this.dataGridView1.Rows.RemoveAt(index);

                    foreach (DataGridViewCell d in row.Cells)
                        d.Dispose();

                    row.Dispose();
                }
            }
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                this.contextMenuStrip1.Show(this.dataGridView1, e.Location);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Settings.SetReg("filter_on", this.checkBox1.Checked);
        }
    }
}
