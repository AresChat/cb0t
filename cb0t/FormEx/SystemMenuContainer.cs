using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace FormEx.SystemMenuEx
{
    public class SystemMenuContainer
    {
        public static IntPtr ImageToHandle(Bitmap img)
        {
            IntPtr result = IntPtr.Zero;

            using (Bitmap bmp = new Bitmap(16, 16))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Rectangle rec = new Rectangle(0, 0, 16, 16);

                using (SolidBrush brush = new SolidBrush(SystemColors.Control))
                    g.FillRectangle(brush, rec);

                g.DrawImage(img, new Point(0, 0));
                result = bmp.GetHbitmap();
            }

            return result;
        }

        private Form Owner { get; set; }
        private List<ISystemMenuItem> items = new List<ISystemMenuItem>();
        private uint ident = 100;

        public SystemMenuContainer(Form owner)
        {
            this.Owner = owner;
        }

        public int Count
        {
            get { return this.items.Count; }
        }

        public event EventHandler<SystemMenuItem> ItemClicked;

        public bool ProcMsg(IntPtr ptr)
        {
            uint ident = (uint)ptr.ToInt32();

            for (int i = 0; i < this.items.Count; i++)
                if (this.items[i] is SystemMenuItem)
                {
                    SystemMenuItem smi = (SystemMenuItem)this.items[i];

                    if (smi.Ident == ident)
                    {
                        smi.Index = i;

                        if (this.ItemClicked != null)
                            this.ItemClicked(null, smi);

                        return true;
                    }
                }

            return false;
        }

        public void Add(String text)
        {
            this.Insert(this.Count, text);
        }

        public void Insert(int index, String text)
        {
            this.Insert(index, new SystemMenuItem(text, null));
        }

        public void Add(SystemMenuSeperator item)
        {
            this.Insert(this.Count, item);
        }

        public void Insert(int index, SystemMenuSeperator item)
        {
            IntPtr ptr = GetSystemMenu(this.Owner.Handle, false);

            if (!ptr.Equals(IntPtr.Zero))
            {
                this.items.Insert(index, item);
                InsertMenu(ptr, (uint)index, MenuFlags.MF_SEPARATOR | MenuFlags.MF_BYPOSITION, 0, null);

                if (this.Owner.IsHandleCreated)
                    DrawMenuBar(ptr);
            }
        }

        public void Add(SystemMenuItem item)
        {
            this.Insert(this.Count, item);
        }

        public void Insert(int index, SystemMenuItem item)
        {
            IntPtr ptr = GetSystemMenu(this.Owner.Handle, false);

            if (!ptr.Equals(IntPtr.Zero))
            {
                this.items.Insert(index, item);
                item.Ident = this.ident++;
                InsertMenu(ptr, (uint)index, MenuFlags.MF_STRING | MenuFlags.MF_BYPOSITION, item.Ident, item.Text);
                SetMenuItemBitmaps(ptr, (uint)index, MenuFlags.MF_BYPOSITION, item.Image, IntPtr.Zero);

                if (this.Owner.IsHandleCreated)
                    DrawMenuBar(ptr);
            }
        }

        public void RemoveAt(int index)
        {
            this.RemoveAt(index, false);
        }

        public void RemoveAt(int index, bool release)
        {
            IntPtr ptr = GetSystemMenu(this.Owner.Handle, false);

            if (!ptr.Equals(IntPtr.Zero))
            {
                this.items.RemoveAt(index);
                RemoveMenu(ptr, (uint)index, MenuFlags.MF_REMOVE | MenuFlags.MF_BYPOSITION);

                if (release)
                    if (this.items[index] is SystemMenuItem)
                        ((SystemMenuItem)this.items[index]).Dispose();

                if (this.Owner.IsHandleCreated)
                    DrawMenuBar(ptr);
            }
        }

        public void Clear()
        {
            this.Clear(false);
        }

        public void Clear(bool release)
        {
            IntPtr ptr = GetSystemMenu(this.Owner.Handle, false);

            if (!ptr.Equals(IntPtr.Zero))
            {
                for (int i = 0; i < this.Count; i++)
                {
                    RemoveMenu(ptr, (uint)0, MenuFlags.MF_REMOVE | MenuFlags.MF_BYPOSITION);

                    if (release)
                        if (this.items[i] is SystemMenuItem)
                            ((SystemMenuItem)this.items[i]).Dispose();
                }

                this.items.Clear();

                if (this.Owner.IsHandleCreated)
                    DrawMenuBar(ptr);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        private static extern bool InsertMenu(IntPtr hMenu, uint uPosition, MenuFlags uFlags, uint uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        private static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, MenuFlags uFlags);

        [DllImport("user32.dll")]
        private static extern bool DrawMenuBar(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetMenuItemBitmaps(IntPtr hMenu, uint uPosition, MenuFlags uFlags, IntPtr hBitmapUnchecked, IntPtr hBitmapChecked);

        [Flags]
        public enum MenuFlags : uint
        {
            MF_STRING = 0,
            MF_BYPOSITION = 0x400,
            MF_SEPARATOR = 0x800,
            MF_REMOVE = 0x1000,
        }
    }
}
