namespace cb0t
{
    partial class CustomColorPicker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label7 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.doubleBufferedComboBox2 = new cb0t.DoubleBufferedComboBox();
            this.doubleBufferedComboBox1 = new cb0t.DoubleBufferedComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 179);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 13);
            this.label7.TabIndex = 44;
            this.label7.Text = "Pick an ares color:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(95, 107);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(72, 20);
            this.textBox1.TabIndex = 43;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(63, 111);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 42;
            this.label6.Text = "hex:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(170, 106);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(30, 23);
            this.button2.TabIndex = 41;
            this.button2.Text = " ⇒";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(245, 193);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 40;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(169, 85);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 13);
            this.label5.TabIndex = 39;
            this.label5.Text = "B: 0";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(169, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 13);
            this.label4.TabIndex = 38;
            this.label4.Text = "G: 0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(169, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 37;
            this.label3.Text = "R: 0";
            // 
            // trackBar3
            // 
            this.trackBar3.Location = new System.Drawing.Point(10, 83);
            this.trackBar3.Maximum = 255;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(164, 45);
            this.trackBar3.TabIndex = 36;
            this.trackBar3.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar3.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(10, 53);
            this.trackBar2.Maximum = 255;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(164, 45);
            this.trackBar2.TabIndex = 35;
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar2.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(10, 23);
            this.trackBar1.Maximum = 255;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(164, 45);
            this.trackBar1.TabIndex = 34;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 33;
            this.label2.Text = "Pick a web color:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(233, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Chosen color:";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(236, 26);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(82, 82);
            this.panel1.TabIndex = 31;
            // 
            // doubleBufferedComboBox2
            // 
            this.doubleBufferedComboBox2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.doubleBufferedComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.doubleBufferedComboBox2.FormattingEnabled = true;
            this.doubleBufferedComboBox2.Location = new System.Drawing.Point(13, 195);
            this.doubleBufferedComboBox2.Name = "doubleBufferedComboBox2";
            this.doubleBufferedComboBox2.Size = new System.Drawing.Size(110, 21);
            this.doubleBufferedComboBox2.TabIndex = 46;
            this.doubleBufferedComboBox2.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.doubleBufferedComboBox2_DrawItem);
            this.doubleBufferedComboBox2.SelectedIndexChanged += new System.EventHandler(this.doubleBufferedComboBox2_SelectedIndexChanged);
            // 
            // doubleBufferedComboBox1
            // 
            this.doubleBufferedComboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.doubleBufferedComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.doubleBufferedComboBox1.FormattingEnabled = true;
            this.doubleBufferedComboBox1.Location = new System.Drawing.Point(13, 151);
            this.doubleBufferedComboBox1.Name = "doubleBufferedComboBox1";
            this.doubleBufferedComboBox1.Size = new System.Drawing.Size(187, 21);
            this.doubleBufferedComboBox1.TabIndex = 45;
            this.doubleBufferedComboBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.doubleBufferedComboBox1_DrawItem);
            this.doubleBufferedComboBox1.SelectedIndexChanged += new System.EventHandler(this.doubleBufferedComboBox1_SelectedIndexChanged);
            // 
            // CustomColorPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(331, 227);
            this.Controls.Add(this.doubleBufferedComboBox2);
            this.Controls.Add(this.doubleBufferedComboBox1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.trackBar3);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(347, 266);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(347, 266);
            this.Name = "CustomColorPicker";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private DoubleBufferedComboBox doubleBufferedComboBox1;
        private DoubleBufferedComboBox doubleBufferedComboBox2;
    }
}