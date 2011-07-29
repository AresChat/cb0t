using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class FileList : ListView
    {
        public FileList()
        {
            this.FullRowSelect = true;
            this.MultiSelect = false;

            this.Columns.Add(new ColumnHeader());
            this.Columns[0].Text = "Title";
            this.Columns[0].Width = 200;
            this.Columns.Add(new ColumnHeader());
            this.Columns[1].Text = "Artist";
            this.Columns[1].Width = 100;
            this.Columns.Add(new ColumnHeader());
            this.Columns[2].Text = "Media";
            this.Columns[2].Width = 75;
            this.Columns.Add(new ColumnHeader());
            this.Columns[3].Text = "Catagory";
            this.Columns[3].Width = 75;
            this.Columns.Add(new ColumnHeader());
            this.Columns[4].Text = "Size";
            this.Columns[4].Width = 75;
            this.Columns.Add(new ColumnHeader());
            this.Columns[5].Text = "Filename";
            this.Columns[5].Width = 200;

            this.Items.Add("Receiving file list...");
            this.SmallImageList = new ImageList();
            this.SmallImageList.TransparentColor = Color.Magenta;
            this.SmallImageList.Images.Add(AresImages.Mime_Audio);
            this.SmallImageList.Images.Add(AresImages.Mime_Software);
            this.SmallImageList.Images.Add(AresImages.Mime_Video);
            this.SmallImageList.Images.Add(AresImages.Mime_Document);
            this.SmallImageList.Images.Add(AresImages.Mime_Picture);
            this.SmallImageList.Images.Add(AresImages.Mime_Other);
        }

        protected override void OnResize(EventArgs e)
        {
            if (this.Columns.Count == 6)
            {
                this.Columns[0].Width = 200;
                this.Columns[1].Width = 100;
                this.Columns[2].Width = 75;
                this.Columns[3].Width = 75;
                this.Columns[4].Width = 75;
                
                if (this.Width < 550)
                    this.Columns[5].Width = 200;
                else
                    this.Columns[5].Width = (this.Width - 550);
            }
        }

        protected override void OnColumnWidthChanging(ColumnWidthChangingEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 0:
                    e.NewWidth = 200;
                    break;

                case 1:
                    e.NewWidth = 100;
                    break;

                case 2:
                    e.NewWidth = 75;
                    break;

                case 3:
                    e.NewWidth = 75;
                    break;

                case 4:
                    e.NewWidth = 75;
                    break;

                case 5:
                    e.NewWidth = this.Columns[5].Width;
                    break;
            }

            e.Cancel = true;
        }
    }
}
