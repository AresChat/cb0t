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
            this.components = new System.ComponentModel.Container();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.audioList1 = new cb0t.AudioList();
            this.albumArtBox1 = new cb0t.AlbumArtBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeFromPlaylistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.albumArtBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
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
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 214);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 148);
            this.label1.TabIndex = 2;
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
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
            // audioList1
            // 
            this.audioList1.AllowDrop = true;
            this.audioList1.BackColor = System.Drawing.Color.White;
            this.audioList1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.audioList1.ContextMenuStrip = this.contextMenuStrip1;
            this.audioList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.audioList1.FullRowSelect = true;
            this.audioList1.GridLines = true;
            this.audioList1.Location = new System.Drawing.Point(178, 0);
            this.audioList1.MultiSelect = false;
            this.audioList1.Name = "audioList1";
            this.audioList1.OwnerDraw = true;
            this.audioList1.Size = new System.Drawing.Size(559, 441);
            this.audioList1.TabIndex = 1;
            this.audioList1.UseCompatibleStateImageBehavior = false;
            this.audioList1.View = System.Windows.Forms.View.Details;
            this.audioList1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.audioList1_ItemDrag);
            this.audioList1.DragDrop += new System.Windows.Forms.DragEventHandler(this.audioList1_DragDrop);
            this.audioList1.DragEnter += new System.Windows.Forms.DragEventHandler(this.audioList1_DragEnter);
            this.audioList1.DragLeave += new System.EventHandler(this.audioList1_DragLeave);
            this.audioList1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.audioList1_MouseDoubleClick);
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
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeFromPlaylistToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(184, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // removeFromPlaylistToolStripMenuItem
            // 
            this.removeFromPlaylistToolStripMenuItem.Name = "removeFromPlaylistToolStripMenuItem";
            this.removeFromPlaylistToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.removeFromPlaylistToolStripMenuItem.Text = "remove from playlist";
            this.removeFromPlaylistToolStripMenuItem.Click += new System.EventHandler(this.removeFromPlaylistToolStripMenuItem_Click);
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
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AlbumArtBox albumArtBox1;
        private AudioList audioList1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeFromPlaylistToolStripMenuItem;

    }
}
