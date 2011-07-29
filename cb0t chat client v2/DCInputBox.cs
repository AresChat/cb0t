using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace cb0t_chat_client_v2
{
    class DCInputBox : TextBox
    {
        public delegate void SendMsgDelegate(String text);
        public event SendMsgDelegate OnMessageSending;

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
            }
            else
            {
                base.OnKeyPress(e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.OnMessageSending(this.Text);
                this.Clear();
            }
            else
            {
                base.OnKeyDown(e);
            }
        }
    }
}
