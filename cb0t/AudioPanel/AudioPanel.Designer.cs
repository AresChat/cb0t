namespace cb0t
{
    partial class AudioPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.audioList1 = new cb0t.AudioList();
            this.albumArtBox1 = new cb0t.AlbumArtBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.albumArtBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.albumArtBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(178, 441);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(178, 23);
            this.panel2.TabIndex = 1;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 214);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 148);
            this.label1.TabIndex = 2;
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // audioList1
            // 
            this.audioList1.BackColor = System.Drawing.Color.White;
            this.audioList1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.audioList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.audioList1.FullRowSelect = true;
            this.audioList1.GridLines = true;
            this.audioList1.Location = new System.Drawing.Point(178, 0);
            this.audioList1.Name = "audioList1";
            this.audioList1.OwnerDraw = true;
            this.audioList1.Size = new System.Drawing.Size(559, 441);
            this.audioList1.TabIndex = 1;
            this.audioList1.UseCompatibleStateImageBehavior = false;
            this.audioList1.View = System.Windows.Forms.View.Details;
            // 
            // albumArtBox1
            // 
            this.albumArtBox1.Location = new System.Drawing.Point(2, 25);
            this.albumArtBox1.Name = "albumArtBox1";
            this.albumArtBox1.Size = new System.Drawing.Size(174, 174);
            this.albumArtBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.albumArtBox1.TabIndex = 0;
            this.albumArtBox1.TabStop = false;
            // 
            // AudioPanel
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.audioList1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AudioPanel";
            this.Size = new System.Drawing.Size(737, 441);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.AudioPanel_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.AudioPanel_DragEnter);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.albumArtBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AlbumArtBox albumArtBox1;
        private AudioList audioList1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;

    }
}
