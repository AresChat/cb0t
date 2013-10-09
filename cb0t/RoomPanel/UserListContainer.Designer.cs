namespace cb0t
{
    partial class UserListContainer
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
            this.userListHeader1 = new cb0t.UserListHeader();
            this.userListBox1 = new cb0t.UserListBox();
            this.SuspendLayout();
            // 
            // userListHeader1
            // 
            this.userListHeader1.BackColor = System.Drawing.Color.White;
            this.userListHeader1.Dock = System.Windows.Forms.DockStyle.Top;
            this.userListHeader1.HeaderText = "Users (0)";
            this.userListHeader1.Location = new System.Drawing.Point(0, 0);
            this.userListHeader1.Name = "userListHeader1";
            this.userListHeader1.ServerVersion = null;
            this.userListHeader1.Size = new System.Drawing.Size(288, 22);
            this.userListHeader1.TabIndex = 0;
            // 
            // userListBox1
            // 
            this.userListBox1.BackColor = System.Drawing.Color.White;
            this.userListBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.userListBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userListBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.userListBox1.FormattingEnabled = true;
            this.userListBox1.Location = new System.Drawing.Point(0, 22);
            this.userListBox1.Name = "userListBox1";
            this.userListBox1.ScrollAlwaysVisible = true;
            this.userListBox1.Size = new System.Drawing.Size(288, 377);
            this.userListBox1.TabIndex = 1;
            // 
            // UserListContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.userListBox1);
            this.Controls.Add(this.userListHeader1);
            this.Name = "UserListContainer";
            this.Size = new System.Drawing.Size(288, 399);
            this.ResumeLayout(false);

        }

        #endregion

        private UserListHeader userListHeader1;
        private UserListBox userListBox1;
    }
}
