using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    public partial class FilterDialog : Form
    {
        public FilterDialog()
        {
            InitializeComponent();
        }

        public void UpdateTemplate()
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool needs_text = true;

            if (this.comboBox4.SelectedIndex > -1)
                if (this.comboBox4.SelectedItem.ToString() == "Block")
                    needs_text = false;

            if (needs_text)
                if (String.IsNullOrEmpty(this.textBox3.Text))
                    if (this.textBox3.Text.Trim(' ').Length == 0)
                    {
                        MessageBox.Show("Missing text in section 7", "cb0t", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

            this.DialogResult = DialogResult.OK;
        }

        public FilterItem Item
        {
            get
            {
                FilterItem item = new FilterItem
                {
                    Argument = this.textBox2.Text,
                    Condition = this.comboBox3.SelectedItem.ToString(),
                    Event = this.comboBox1.SelectedItem.ToString(),
                    Property = this.comboBox2.SelectedItem.ToString(),
                    Room = this.textBox1.Text,
                    Task = this.comboBox4.SelectedItem.ToString(),
                    Text = this.textBox3.Text
                };

                return item;
            }
        }

        public void SetForm(FilterItem item)
        {
            this.comboBox1.SelectedItem = item.Event;
            this.comboBox2.SelectedItem = item.Property;
            this.comboBox3.SelectedItem = item.Condition;
            this.comboBox4.SelectedItem = item.Task;
            this.textBox1.Text = item.Room;
            this.textBox2.Text = item.Argument;
            this.textBox3.Text = item.Text;
        }

        public void ResetForm()
        {
            this.comboBox1.SelectedIndex = 0;
            this.comboBox2.SelectedIndex = 0;
            this.comboBox3.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = 0;
            this.textBox1.Clear();
            this.textBox2.Clear();
            this.textBox3.Clear();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedIndex == 0 || this.comboBox1.SelectedIndex == 1 || this.comboBox1.SelectedIndex == 5)
            {
                if (this.comboBox2.Items.Count == 2)
                {
                    this.comboBox2.Items.RemoveAt(1);
                    this.comboBox2.SelectedIndex = 0;
                }

                if (this.comboBox4.Items.Count == 3)
                {
                    this.comboBox4.Items.RemoveAt(2);
                    this.comboBox4.SelectedIndex = 0;
                }
            }
            else
            {
                if (this.comboBox2.Items.Count == 1)
                    this.comboBox2.Items.Add("Text");

                if (this.comboBox4.Items.Count == 2)
                    this.comboBox4.Items.Add("Block");
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox4.SelectedIndex > -1)
            {
                String str = this.comboBox4.SelectedItem.ToString();

                if (str == "Block")
                {
                    this.textBox3.Clear();
                    this.textBox3.Enabled = false;
                }
                else this.textBox3.Enabled = true;
            }
        }
    }
}
