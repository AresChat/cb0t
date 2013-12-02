using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using NHunspell;

namespace cb0t
{
    class AccuTextBox : RichTextBox
    {
        private ContextMenuStrip ctx { get; set; }

        public AccuTextBox()
        {
            this.WordWrap = false;
            this.ScrollBars = RichTextBoxScrollBars.None;
            this.BorderStyle = BorderStyle.None;
            this.BackColor = Color.White;
            this.Multiline = false;
            this.HideSelection = false;
            this.DetectUrls = false;
            this.ctx = new ContextMenuStrip();
            this.ctx.ShowImageMargin = false;
            this.ctx.ShowCheckMargin = false;
            this.ctx.Items.Add("Cut");//0
            this.ctx.Items.Add("Copy");//1
            this.ctx.Items.Add("Paste");//2
            this.ctx.Items.Add(new ToolStripSeparator());
            this.ctx.Items.Add("Add to dictionary");//4
            this.ctx.Items.Add(new ToolStripSeparator());
            this.ctx.Items[3].Visible = false;
            this.ctx.Items[4].Visible = false;
            this.ctx.Items[5].Visible = false;
            this.ctx.Opening += this.CTXOpening;
            this.ctx.ItemClicked += this.CTXItemClicked;
            this.ContextMenuStrip = this.ctx;
        }

        public void UpdateTemplate()
        {
            this.ctx.Items[0].Text = StringTemplate.Get(STType.InBox, 0);
            this.ctx.Items[1].Text = StringTemplate.Get(STType.InBox, 1);
            this.ctx.Items[2].Text = StringTemplate.Get(STType.InBox, 2);
            this.ctx.Items[4].Text = StringTemplate.Get(STType.InBox, 3);
        }

        private int char_index = -1;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.X))
            {
                String str;
                int ss, sl;
                StringBuilder sb;

                try
                {
                    ss = this.SelectionStart;
                    sl = this.SelectionLength;
                    str = this.Text.Substring(ss, sl);
                    sb = new StringBuilder();
                    sb.Append(this.Text.Substring(0, ss));
                    sb.Append(this.Text.Substring(ss + sl));
                    this.Text = sb.ToString();
                    this.SelectionStart = ss;
                    Clipboard.SetText(str);
                }
                catch { }

                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }
            else if (e.KeyData == (Keys.Control | Keys.C))
            {
                String str;
                int ss, sl;

                try
                {
                    ss = this.SelectionStart;
                    sl = this.SelectionLength;
                    str = this.Text.Substring(ss, sl);
                    Clipboard.SetText(str);
                }
                catch { }

                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }
            else if (e.KeyData == (Keys.Control | Keys.V))
            {
                String str;
                int ss, sl;
                StringBuilder sb;

                try
                {
                    if (Clipboard.ContainsText())
                    {
                        str = Clipboard.GetText();

                        if (!String.IsNullOrEmpty(str))
                        {
                            ss = this.SelectionStart;
                            sl = this.SelectionLength;
                            sb = new StringBuilder();
                            sb.Append(this.Text.Substring(0, ss));
                            sb.Append(str);
                            sb.Append(this.Text.Substring(ss + sl));
                            this.Text = sb.ToString();
                            this.SelectionStart = ss + str.Length;
                        }
                    }
                }
                catch { }

                e.Handled = true;
                e.SuppressKeyPress = true;
                return;
            }

            base.OnKeyDown(e);
        }

        private void CTXItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            String str;
            int ss, sl;
            StringBuilder sb;

            if (e.ClickedItem.Equals(this.ctx.Items[0]))
            {
                try
                {
                    ss = this.SelectionStart;
                    sl = this.SelectionLength;
                    str = this.Text.Substring(ss, sl);
                    sb = new StringBuilder();
                    sb.Append(this.Text.Substring(0, ss));
                    sb.Append(this.Text.Substring(ss + sl));
                    this.Text = sb.ToString();
                    this.SelectionStart = ss;
                    Clipboard.SetText(str);
                }
                catch { }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[1]))
            {
                try
                {
                    ss = this.SelectionStart;
                    sl = this.SelectionLength;
                    str = this.Text.Substring(ss, sl);
                    Clipboard.SetText(str);
                }
                catch { }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[2]))
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        str = Clipboard.GetText();

                        if (!String.IsNullOrEmpty(str))
                        {
                            ss = this.SelectionStart;
                            sl = this.SelectionLength;
                            sb = new StringBuilder();
                            sb.Append(this.Text.Substring(0, ss));
                            sb.Append(str);
                            sb.Append(this.Text.Substring(ss + sl));
                            this.Text = sb.ToString();
                            this.SelectionStart = ss + str.Length;
                        }
                    }
                }
                catch { }
            }
            else if (e.ClickedItem.Equals(this.ctx.Items[4]))
            {
                String[] words = this.Text.Split(' ');
                int counter = 0;

                for (int i = 0; i < words.Length; i++)
                    if (this.char_index >= counter && this.char_index <= (counter + words[i].Length))
                    {
                        SpellChecker.AddAllowedWord(words[i]);
                        break;
                    }
                    else counter += words[i].Length + 1;

                this.needs_checking = true;
            }
            else
            {
                String replacement = null;

                for (int i = 4; i < this.ctx.Items.Count; i++)
                    if (e.ClickedItem.Equals(this.ctx.Items[i]))
                    {
                        replacement = e.ClickedItem.Text;
                        break;
                    }

                if (!String.IsNullOrEmpty(replacement))
                {
                    String[] words = this.Text.Split(' ');
                    int counter = 0;

                    for (int i = 0; i < words.Length; i++)
                        if (this.char_index >= counter && this.char_index <= (counter + words[i].Length))
                        {
                            sb = new StringBuilder();
                            sb.Append(this.Text.Substring(0, counter));
                            sb.Append(replacement);
                            sb.Append(this.Text.Substring(counter + words[i].Length));
                            this.Text = sb.ToString();
                            this.SelectionStart = counter + words[i].Length + 1;
                            break;
                        }
                        else counter += words[i].Length + 1;
                }
            }
        }

        private void CTXOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            while (this.ctx.Items.Count > 6)
                this.ctx.Items[6].Dispose();

            this.ctx.Items[0].Enabled = this.SelectionLength > 0;
            this.ctx.Items[1].Enabled = this.SelectionLength > 0;
            bool can_paste = false;

            try
            {
                if (Clipboard.ContainsText())
                    can_paste = !String.IsNullOrEmpty(Clipboard.GetText());
            }
            catch { }

            this.ctx.Items[2].Enabled = can_paste;
            this.ctx.Items[3].Visible = false;
            this.ctx.Items[4].Visible = false;
            this.ctx.Items[5].Visible = false;

            if (this.char_index > -1)
                if (Settings.GetReg<int>("spell_checker", 0) > 0)
                {
                    String[] words = this.Text.Split(' ');
                    int counter = 0;
                    String found = null;

                    for (int i = 0; i < words.Length; i++)
                        if (this.char_index >= counter && this.char_index <= (counter + words[i].Length))
                        {
                            found = words[i];
                            break;
                        }
                        else counter += words[i].Length + 1;

                    if (!String.IsNullOrEmpty(found))
                        using (Hunspell sc = new Hunspell(SpellChecker.AFF, SpellChecker.DIC))
                        {
                            SpellChecker.AllowedWords.ForEach(x => sc.Add(x));

                            if (!found.StartsWith("http://") && !found.StartsWith("www.") && !found.Contains("arlnk"))
                                if (!sc.Spell(new String(found.Where(x => Char.IsLetter(x) || Char.IsNumber(x)).ToArray())))
                                {
                                    List<String> suggestions = sc.Suggest(found);
                                    this.ctx.Items[3].Visible = true;
                                    this.ctx.Items[4].Visible = true;
                                    this.ctx.Items[5].Visible = true;

                                    if (suggestions.Count > 0)
                                    {
                                        for (int i = 0; i < suggestions.Count; i++)
                                        {
                                            if (i == 10)
                                                break;

                                            this.ctx.Items.Add(suggestions[i]);
                                        }
                                    }
                                    else
                                    {
                                        this.ctx.Items.Add("no suggestions");
                                        this.ctx.Items[this.ctx.Items.Count - 1].Enabled = false;
                                    }
                                }
                        }
                }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                this.char_index = this.GetCharIndexFromPosition(e.Location);

            base.OnMouseDown(e);
        }

        public void Free()
        {
            this.ContextMenuStrip = null;
            this.ctx.Opening -= this.CTXOpening;
            this.ctx.ItemClicked -= this.CTXItemClicked;

            while (this.ctx.Items.Count > 0)
                this.ctx.Items[0].Dispose();

            this.ctx.Dispose();
            this.ctx = null;
        }

        private bool needs_checking = false;

        public void SpellCheck()
        {
            if (this.needs_checking)
            {
                this.needs_checking = false;
                StringBuilder rtf = new StringBuilder();
                rtf.Append("{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang1040{\\fonttbl{\\f0\\fswiss\\fprq2\\fcharset0Tahoma;}}");
                rtf.Append("{\\colortbl ;\\red0\\green0\\blue0; \\red255\\green0\\blue0; }");
                rtf.Append("\\fs20");

                using (Hunspell sc = new Hunspell(SpellChecker.AFF, SpellChecker.DIC))
                {
                    SpellChecker.AllowedWords.ForEach(x => sc.Add(x));
                    String[] words = this.Text.Split(' ');
                    bool correct = false;

                    for (int i = 0; i < words.Length; i++)
                    {
                        if (words[i].Length > 0)
                        {
                            correct = (words[i].StartsWith("http://") || words[i].StartsWith("www.") || words[i].Contains("arlnk"));

                            if (!correct)
                                correct = sc.Spell(new String(words[i].Where(x => Char.IsLetter(x) || Char.IsNumber(x)).ToArray()));

                            if (correct)
                                rtf.Append("\\cf0\\cf1");
                            else
                                rtf.Append("\\cf0\\cf2");

                            foreach (char c in words[i].ToCharArray())
                                rtf.Append("\\u" + ((int)c) + "?");
                        }
                        
                        if (i < (words.Length - 1))
                            rtf.Append("\\u" + ((int)' ') + "?");
                    }
                }

                rtf.Append("\\cf0\\cf1}");
                int ss = this.SelectionStart;
                int sl = this.SelectionLength;
                this.Rtf = rtf.ToString();
                this.SelectionStart = ss;
                this.SelectionLength = sl;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            this.needs_checking = true;

            if (String.IsNullOrEmpty(this.Text))
                this.needs_checking = false;

            base.OnTextChanged(e);
        }
    }
}
