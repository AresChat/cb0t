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
    public partial class PrivacySettings : UserControl
    {
        public PrivacySettings()
        {
            InitializeComponent();
        }

        public void UpdateTemplate()
        {
            this.label1.Text = StringTemplate.Get(STType.Settings, 15);
            this.label2.Text = StringTemplate.Get(STType.PrivacySettings, 0) + ":";
            this.dataGridView1.Columns[0].HeaderText = StringTemplate.Get(STType.PrivacySettings, 1);
            this.label3.Text = StringTemplate.Get(STType.PrivacySettings, 1) + ":";
            this.dataGridView1.Columns[1].HeaderText = StringTemplate.Get(STType.PrivacySettings, 2);
            this.label4.Text = StringTemplate.Get(STType.PrivacySettings, 2) + ":";
            this.dataGridView1.Columns[2].HeaderText = StringTemplate.Get(STType.PrivacySettings, 3);
            this.label5.Text = StringTemplate.Get(STType.PrivacySettings, 3) + ":";
            this.button1.Text = StringTemplate.Get(STType.PrivacySettings, 4);
            this.removeToolStripMenuItem.Text = StringTemplate.Get(STType.PrivacySettings, 5);
        }

        public void Populate()
        {
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;

            foreach (AutoIgnoreItem o in AutoIgnores.ToArray())
            {
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[0].Value = o.Condition.ToString();
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[1].Value = o.Name;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[2].Value = o.Action.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String name = this.textBox1.Text;

            if (String.IsNullOrEmpty(name))
            {
                MessageBox.Show("Missing name", "cb0t", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (AutoIgnores.AddItem(name, this.comboBox1.SelectedIndex, this.comboBox2.SelectedIndex))
            {
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[0].Value = ((AutoIgnoreCondition)this.comboBox1.SelectedIndex).ToString();
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[1].Value = this.textBox1.Text;
                this.dataGridView1.Rows[this.dataGridView1.Rows.Count - 1].Cells[2].Value = ((AutoIgnoreType)this.comboBox2.SelectedIndex).ToString();
                this.textBox1.Clear();
                this.comboBox1.SelectedIndex = 0;
                this.comboBox2.SelectedIndex = 0;
            }
            else MessageBox.Show("Name already filtered", "cb0t", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                int index = this.dataGridView1.SelectedRows[0].Index;

                if (index > -1 && index < AutoIgnores.Count)
                {
                    AutoIgnores.RemoveItem(index);
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
    }
}
