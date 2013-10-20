using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace cb0t
{
    class Popups
    {
        public static Popups Available { get; private set; }

        private Popups()
        {
            this.popups = new PopupDialog[5];

            for (int i = 0; i < this.popups.Length; i++)
            {
                this.popups[i] = new PopupDialog();
                this.popups[i].Size = new Size(1, 1);
                this.popups[i].Location = new Point(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                this.popups[i].Visible = false;
                this.popups[i].PopupClicked += this.PopupClicked;
            }
        }

        public event EventHandler PopupMouseClicked;

        private PopupDialog[] popups;

        public static void Create()
        {
            Available = new Popups();
        }

        private void PopupClicked(object sender, EventArgs e)
        {
            this.PopupMouseClicked(sender, e);
        }

        public void ShowPopup(String title, String msg, IPEndPoint room, PopupSound sound)
        {
            int x = 1;

            for (int i = 0; i < this.popups.Length; i++)
            {
                if (this.popups[i].Busy)
                {
                    if (this.popups[i].OffsetBase >= x)
                        x = (this.popups[i].OffsetBase + 1);
                }
            }

            if (x < 6)
            {
                for (int i = 0; i < this.popups.Length; i++)
                {
                    if (!this.popups[i].Busy)
                    {
                        String[] words = msg.Split(new String[] { " " }, StringSplitOptions.None);
                        int char_count = 0;
                        String text = String.Empty;
                        PopupSettings sets = new PopupSettings { Room = room, Title = title };
                        sets.Message = new List<String>();

                        for (int w = 0; w < words.Length; w++)
                        {
                            if ((words[w].Length + char_count) < 26)
                            {
                                text += words[w] + " ";
                                char_count = text.Length;
                            }
                            else if (char_count == 0)
                            {
                                text += words[w] + " ";
                                char_count = text.Length;
                            }
                            else
                            {
                                sets.Message.Add(text);

                                if (sets.Message.Count < 3)
                                {
                                    text = words[w] + " ";
                                    char_count = text.Length;
                                }
                                else
                                {
                                    text = String.Empty;
                                    char_count = 0;
                                    break;
                                }
                            }
                        }

                        if (text.Length > 0)
                            sets.Message.Add(text);

                        for (int w = 0; w < sets.Message.Count; w++)
                            sets.Message[w] = sets.Message[w].TrimEnd();

                        if (sets.Message.Count > 0)
                            this.popups[i].ShowPopup(sets, x);

                        if (sound == PopupSound.Notify)
                            InternalSounds.Notify();
                        else if (sound == PopupSound.Friend)
                            InternalSounds.Friend();

                        break;
                    }
                }
            }
        }
    }
}
