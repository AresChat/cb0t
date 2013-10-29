namespace cb0t
{
    partial class BrowseLeftContainer
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("All");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Audio");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Image");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Video");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Document");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Software");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Other");
            this.browseLeftHeader1 = new cb0t.BrowseLeftHeader();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // browseLeftHeader1
            // 
            this.browseLeftHeader1.BackColor = System.Drawing.Color.White;
            this.browseLeftHeader1.Dock = System.Windows.Forms.DockStyle.Top;
            this.browseLeftHeader1.HeaderText = "Loading...";
            this.browseLeftHeader1.Location = new System.Drawing.Point(0, 0);
            this.browseLeftHeader1.Name = "browseLeftHeader1";
            this.browseLeftHeader1.Size = new System.Drawing.Size(236, 22);
            this.browseLeftHeader1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.White;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Location = new System.Drawing.Point(0, 22);
            this.treeView1.Name = "treeView1";
            treeNode1.Name = "Node0";
            treeNode1.Text = "All";
            treeNode2.Name = "Node1";
            treeNode2.Text = "Audio";
            treeNode3.Name = "Node2";
            treeNode3.Text = "Image";
            treeNode4.Name = "Node3";
            treeNode4.Text = "Video";
            treeNode5.Name = "Node4";
            treeNode5.Text = "Document";
            treeNode6.Name = "Node5";
            treeNode6.Text = "Software";
            treeNode7.Name = "Node6";
            treeNode7.Text = "Other";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7});
            this.treeView1.ShowRootLines = false;
            this.treeView1.Size = new System.Drawing.Size(236, 362);
            this.treeView1.TabIndex = 1;
            // 
            // BrowseLeftContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.browseLeftHeader1);
            this.Name = "BrowseLeftContainer";
            this.Size = new System.Drawing.Size(236, 384);
            this.ResumeLayout(false);

        }

        #endregion

        private BrowseLeftHeader browseLeftHeader1;
        private System.Windows.Forms.TreeView treeView1;
    }
}
