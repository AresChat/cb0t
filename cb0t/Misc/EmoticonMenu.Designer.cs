namespace cb0t
{
    partial class EmoticonMenu
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
            this.emExtMenu1 = new cb0t.EmExtMenu();
            this.emRegMenu1 = new cb0t.EmRegMenu();
            this.SuspendLayout();
            // 
            // emExtMenu1
            // 
            this.emExtMenu1.AutoScroll = true;
            this.emExtMenu1.Location = new System.Drawing.Point(0, 69);
            this.emExtMenu1.Name = "emExtMenu1";
            this.emExtMenu1.Size = new System.Drawing.Size(224, 177);
            this.emExtMenu1.TabIndex = 1;
            // 
            // emRegMenu1
            // 
            this.emRegMenu1.BackColor = System.Drawing.Color.White;
            this.emRegMenu1.Location = new System.Drawing.Point(0, 0);
            this.emRegMenu1.Name = "emRegMenu1";
            this.emRegMenu1.Size = new System.Drawing.Size(224, 64);
            this.emRegMenu1.TabIndex = 0;
            // 
            // EmoticonMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(224, 246);
            this.ControlBox = false;
            this.Controls.Add(this.emExtMenu1);
            this.Controls.Add(this.emRegMenu1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(240, 262);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(240, 262);
            this.Name = "EmoticonMenu";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Deactivate += new System.EventHandler(this.EmoticonMenu_Deactivate);
            this.ResumeLayout(false);

        }

        #endregion

        private EmRegMenu emRegMenu1;
        private EmExtMenu emExtMenu1;
    }
}