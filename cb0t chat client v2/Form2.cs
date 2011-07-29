using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Ares.PacketHandlers;

namespace cb0t_chat_client_v2
{
    public partial class Form2 : Form
    {
        [DllImport("user32.dll")]
        private static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        private DCSendFileObject[] file_send = new DCSendFileObject[20];
        private DCReceiveFileObject[] file_receive = new DCReceiveFileObject[20];

        private bool got_avatar = false;
        private List<byte> avatar_bytes = new List<byte>();

        private Random rnd = new Random(Environment.TickCount);
        private bool appearance_set = false;
        private EmoticonMenu emotemenu;
        private ColorMenu colormenu;
        private int tick;

        public bool server_mode;
        public Socket socket;
        public IPAddress ip;
        public String name;
        public ushort port;
        public int ident;

        public bool session_up = false;
        public bool connected = false;
        public bool handshake_complete = false;

        private List<byte> bytes_in = new List<byte>();
        private Queue<byte[]> bytes_out = new Queue<byte[]>();
        private int fails = 0;
        private ushort file_send_ref = 0;
        private int listviewtimer = 0;

        private bool client_received_ack = false;
        private bool client_received_info = false;
        private bool accepting_files = false;

        public delegate void DCExternalDelegate(int ident);
        public event DCExternalDelegate MakeWindowPopUp;
        public event DCExternalDelegate MakeSessionEnd;
        
        internal event ChannelList.ChannelClickedDelegate OnHashlinkClicking;

        public Form2()
        {
            InitializeComponent();
            this.dcInputBox1.OnMessageSending += new DCInputBox.SendMsgDelegate(this.OnMessageSending);
            this.dcOutBox1.OnHashlinkClicked += new ChannelList.ChannelClickedDelegate(this.OnHashlinkClicked);
        }

        private void OnHashlinkClicked(ChannelObject cObj)
        {
            this.OnHashlinkClicking(cObj);
        }

        private void OnMessageSending(String text)
        {
            if (this.session_up)
            {
                if (this.client_received_ack && this.client_received_info)
                {
                    while (Encoding.UTF8.GetByteCount(text) > 200)
                        text = text.Substring(0, text.Length - 1);

                    this.dcOutBox1.Message(Settings.my_username, text);
                    this.bytes_out.Enqueue(Packets.DCTextMessage(text));
                }
            }
        }

        #region appearance
        private void Form2_Load(object sender, EventArgs e)
        {
            if (!this.appearance_set)
            {
                this.appearance_set = true;
                this.toolStripButton1.Text = "B";
                this.toolStripButton1.Font = new Font(this.Font, FontStyle.Bold);
                this.toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
                this.toolStripButton1.ToolTipText = "bold";
                this.toolStripButton2.Text = "I";
                this.toolStripButton2.Font = new Font(this.Font, FontStyle.Italic);
                this.toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Text;
                this.toolStripButton2.ToolTipText = "italic";
                this.toolStripButton3.Text = "U";
                this.toolStripButton3.Font = new Font(this.Font, FontStyle.Underline);
                this.toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Text;
                this.toolStripButton3.ToolTipText = "underline";
                this.toolStripButton4.Image = AresImages.TransparentEmoticons[47];
                this.toolStripButton4.ToolTipText = "fore color";
                this.toolStripButton5.Image = AresImages.TransparentEmoticons[48];
                this.toolStripButton5.ToolTipText = "back color";
                this.toolStripButton6.Image = AresImages.TransparentEmoticons[0];
                this.toolStripButton6.ToolTipText = "emoticons";
                this.toolStripButton7.Image = AresImages.Files;
                this.toolStripButton7.ToolTipText = "send file";
                this.toolStripButton8.Image = AresImages.DCBlock;
                this.toolStripButton8.ToolTipText = "block user";

                this.emotemenu = new EmoticonMenu(this.dcInputBox1);
                this.colormenu = new ColorMenu(this.dcInputBox1);

                this.checkBox1.Checked = Settings.dc_auto_accept;
                this.accepting_files = Settings.dc_auto_accept;

                this.dcOutBox1.Announce("\x000314Connecting to host, please wait...");
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e) // bold
        {
            this.dcInputBox1.Text += "\x00026";
            this.dcInputBox1.SelectionStart = this.dcInputBox1.Text.Length;
        }

        private void toolStripButton2_Click(object sender, EventArgs e) // italic
        {
            this.dcInputBox1.Text += "\x00029";
            this.dcInputBox1.SelectionStart = this.dcInputBox1.Text.Length;
        }

        private void toolStripButton3_Click(object sender, EventArgs e) // underline
        {
            this.dcInputBox1.Text += "\x00027";
            this.dcInputBox1.SelectionStart = this.dcInputBox1.Text.Length;
        }

        private void toolStripButton4_Click(object sender, EventArgs e) // fore
        {
            this.colormenu.foreground = true;
            this.colormenu.Show(MousePosition.X - 16, MousePosition.Y - (16 * 23));
        }

        private void toolStripButton5_Click(object sender, EventArgs e) // back
        {
            this.colormenu.foreground = false;
            this.colormenu.Show(MousePosition.X - 16, MousePosition.Y - (16 * 23));
        }

        private void toolStripButton6_Click(object sender, EventArgs e) // emoticon
        {
            this.emotemenu.Show();
        }

        private void toolStripButton8_Click(object sender, EventArgs e) // block user
        {
            if (!Settings.blocked_dcs.Contains(this.ip))
            {
                Settings.blocked_dcs.Add(this.ip);

                try
                {
                    this.socket.Disconnect(false);
                    this.session_up = false;
                    this.dcOutBox1.Announce("\x000314Connection closed");
                }
                catch { }

                this.dcOutBox1.Announce("\x000314User is now blocked");
            }
        }
        #endregion

        public void MakeClientConnect()
        {
            this.tick = (int)Math.Round((double)Environment.TickCount / 1000);
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.Blocking = false;

            try
            {
                this.socket.Connect(new IPEndPoint(this.ip, this.port));
            }
            catch { }
        }

        public void MakeServerConnect()
        {
            this.tick = (int)Math.Round((double)Environment.TickCount / 1000);
        }

        public void ResetOutputWindow()
        {
            this.dcOutBox1.Invalidate();
        }

        #region SocketReader
        public void ServiceSocketConnection(int time)
        {
            if (!this.handshake_complete)
            {
                if (!this.server_mode) // we are connecting to someone
                {
                    if (!this.connected) // not connected to server yet
                    {
                        if ((this.tick + 15) < time)
                        {
                            this.dcOutBox1.Announce("\x000314Remote host unreachable");

                            try
                            {
                                this.socket.Disconnect(false);
                            }
                            catch { }

                            this.session_up = false;
                        }

                        if (this.socket.Poll(0, SelectMode.SelectWrite))
                        {
                            this.bytes_out.Enqueue(Packets.DCSessionRequestString());
                            this.tick = time;
                            this.connected = true;
                        }
                    }
                    else // connected
                    {
                        if (!this.client_received_ack || !this.client_received_info)
                        {
                            if ((this.tick + 15) < time)
                            {
                                this.dcOutBox1.Announce("\x000314Connection closed by remote peer");

                                try
                                {
                                    this.socket.Disconnect(false);
                                }
                                catch { }

                                this.session_up = false;
                            }
                        }

                        while (this.bytes_out.Count > 0) // bytes out
                        {
                            try
                            {
                                this.socket.Send(this.bytes_out.Peek());
                                this.bytes_out.Dequeue();
                            }
                            catch { break; }
                        }
                        
                        byte[] buf1 = new byte[8192];
                        SocketError e1 = SocketError.Success;
                        int rec1 = 0;

                        try
                        {
                            rec1 = this.socket.Receive(buf1, 0, this.socket.Available, SocketFlags.None, out e1);
                        }
                        catch { }

                        if (rec1 == 0)
                        {
                            if (e1 == SocketError.WouldBlock)
                            {
                                this.fails = 0;
                            }
                            else
                            {
                                if (this.fails++ > 3)
                                {
                                    this.dcOutBox1.Announce("\x000314Connection closed by remote peer");

                                    try
                                    {
                                        this.socket.Disconnect(false);
                                    }
                                    catch { }

                                    for (int i = 0; i < this.file_send.Length; i++)
                                    {
                                        if (this.file_send[i] != null)
                                        {
                                            this.file_send[i].Dispose();
                                            this.file_send[i] = null;
                                        }
                                    }

                                    this.session_up = false;
                                }
                            }
                        }
                        else
                        {
                            byte[] buf2 = new byte[rec1];
                            Array.Copy(buf1, 0, buf2, 0, buf2.Length);
                            this.bytes_in.AddRange(buf2);
                            this.fails = 0;
                        }

                        if (!this.client_received_ack)
                        {
                            String str1 = "CHAT/0.1 200 OK";

                            if (this.bytes_in.Count >= 19)
                            {
                                String str2 = Encoding.UTF8.GetString(this.bytes_in.GetRange(0, 15).ToArray());
                                this.bytes_in.RemoveRange(0, 19);

                                if (str2 == str1)
                                {
                                    this.bytes_out.Enqueue(Packets.DCVersionString());
                                    this.bytes_out.Enqueue(Packets.DCMyInfo());
                                    this.client_received_ack = true;
                                }
                            }
                        }

                        if (!this.client_received_info && this.client_received_ack)
                        {
                            if (this.bytes_in.Count > 0)
                            {
                                byte last_byte = this.bytes_in[this.bytes_in.Count - 1];

                                if (last_byte == 1 || last_byte == 15)
                                {
                                    int split = this.bytes_in.IndexOf(0);

                                    if (split > -1)
                                    {
                                        String[] str1 = Encoding.UTF8.GetString(this.bytes_in.GetRange(0,
                                            split).ToArray()).Replace("\r\n", "").Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                                        if (str1.Length > 1)
                                        {
                                            if (str1[1].StartsWith("|02:"))
                                            {
                                                this.name = str1[1].Substring(4);
                                                this.client_received_info = true;
                                                this.bytes_in.Clear();
                                                this.dcOutBox1.Announce("\x000302Connection established [" + this.ip.ToString() + "]");
                                                this.SetWindowText();
                                                this.tick = time;
                                                this.listviewtimer = time;
                                                this.StartAvatarTransfer();
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (this.client_received_info && this.client_received_ack)
                        {
                            if (this.listviewtimer < time)
                            {
                                this.listviewtimer = time;
                                this.dcListView1.UpdateItems();
                            }

                            for (int i = 0; i < this.file_send.Length; i++)
                            {
                                if (this.file_send[i] != null)
                                {
                                    if (this.file_send[i].Authorised)
                                    {
                                        byte[] send_data = this.file_send[i].current_chunk;

                                        if (send_data == null)
                                        {
                                            this.dcListView1.AmendState(this.file_send[i].referal, "complete");
                                            this.dcOutBox1.Announce("\x000314Completed sending: " + this.file_send[i].displayname);
                                            this.file_send[i].Dispose();
                                            this.file_send[i] = null;
                                        }
                                        else
                                        {
                                            int send_amount = send_data.Length;

                                            try
                                            {
                                                this.socket.Send(Packets.DCFileChunk(this.file_send[i].referal, send_data));
                                                this.file_send[i].NextChunk();
                                                this.dcListView1.AmendSoFar(this.file_send[i].referal, (ulong)send_amount);
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }

                            if ((this.tick + 60) < time) // ping now!
                            {
                                this.tick = time;
                                this.bytes_out.Enqueue(Packets.DCPing());
                            }

                            while (this.bytes_in.Count > 3)
                            {
                                buf1 = this.bytes_in.ToArray();
                                ushort packet_len = BitConverter.ToUInt16(buf1, 1);
                                byte packet_id = buf1[3];

                                if (buf1.Length >= (packet_len + 4))
                                {
                                    byte[] buf2 = new byte[packet_len];
                                    Array.Copy(buf1, 4, buf2, 0, packet_len);
                                    this.bytes_in.RemoveRange(0, (packet_len + 4));
                                    try
                                    {
                                        this.ProcessPacket(packet_id, new AresDataPacket(buf2));
                                    }
                                    catch { }
                                }
                                else { break; }
                            }
                        }
                    }
                }
                else // we are hosting
                {
                    if (!this.client_received_ack || !this.client_received_info)
                    {
                        if ((this.tick + 15) < time)
                        {
                            try
                            {
                                this.socket.Disconnect(false);
                            }
                            catch { }

                            this.session_up = false;
                            this.MakeSessionEnd(this.ident);
                        }
                    }

                    while (this.bytes_out.Count > 0) // bytes out
                    {
                        try
                        {
                            this.socket.Send(this.bytes_out.Peek());
                            this.bytes_out.Dequeue();
                        }
                        catch { break; }
                    }

                    byte[] buf1 = new byte[8192];
                    SocketError e1 = SocketError.Success;
                    int rec1 = 0;

                    try
                    {
                        rec1 = this.socket.Receive(buf1, 0, this.socket.Available, SocketFlags.None, out e1);
                    }
                    catch { }

                    if (rec1 == 0)
                    {
                        if (e1 == SocketError.WouldBlock)
                        {
                            this.fails = 0;
                        }
                        else
                        {
                            if (this.fails++ > 3)
                            {
                                this.dcOutBox1.Announce("\x000314Connection closed by remote peer");

                                try
                                {
                                    this.socket.Disconnect(false);
                                }
                                catch { }

                                for (int i = 0; i < this.file_send.Length; i++)
                                {
                                    if (this.file_send[i] != null)
                                    {
                                        this.file_send[i].Dispose();
                                        this.file_send[i] = null;
                                    }
                                }

                                this.session_up = false;
                            }
                        }
                    }
                    else
                    {
                        byte[] buf2 = new byte[rec1];
                        Array.Copy(buf1, 0, buf2, 0, buf2.Length);
                        this.bytes_in.AddRange(buf2);
                        this.fails = 0;
                    }

                    if (!this.client_received_ack)
                    {
                        String str1 = "CHAT CONNECT/0.1";

                        if (this.bytes_in.Count >= 20)
                        {
                            String str2 = Encoding.UTF8.GetString(this.bytes_in.GetRange(0, 16).ToArray());
                            this.bytes_in.RemoveRange(0, 20);

                            if (str2 == str1)
                            {
                                this.bytes_out.Enqueue(Packets.DCVersionString());
                                this.bytes_out.Enqueue(Packets.DCMyInfo());
                                this.client_received_ack = true;
                            }
                        }
                    }

                    if (!this.client_received_info && this.client_received_ack)
                    {
                        if (this.bytes_in.Count > 0)
                        {
                            byte last_byte = this.bytes_in[this.bytes_in.Count - 1];

                            if (last_byte == 1 || last_byte == 15)
                            {
                                int split = this.bytes_in.IndexOf(0);

                                if (split > -1)
                                {
                                    String[] str1 = Encoding.UTF8.GetString(this.bytes_in.GetRange(0,
                                        split).ToArray()).Replace("\r\n", "").Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                                    if (str1.Length > 1)
                                    {
                                        if (str1[1].StartsWith("|02:"))
                                        {
                                            this.name = str1[1].Substring(4);
                                            this.client_received_info = true;
                                            this.bytes_in.Clear();
                                            this.MakeWindowPopUp(this.ident);
                                            System.Threading.Thread.Sleep(500);
                                            this.dcOutBox1.Announce("\x000302Connection established [" + this.ip.ToString() + "]");
                                            this.SetWindowText();
                                            this.tick = time;
                                            this.listviewtimer = time;

                                            if (Settings.enable_dc_reply)
                                            {
                                                String[] lines = Settings.dc_reply.Split(new String[] { "\r\n" }, StringSplitOptions.None);

                                                foreach (String l in lines)
                                                {
                                                    this.dcOutBox1.Message(Settings.my_username, Helpers.FormatAresColorCodes(l).Replace("+n", this.name).Replace("+ip", this.ip.ToString()));
                                                    this.bytes_out.Enqueue(Packets.DCTextMessage(Helpers.FormatAresColorCodes(l).Replace("+n", this.name).Replace("+ip", this.ip.ToString())));
                                                }
                                            }

                                            this.StartAvatarTransfer();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (this.client_received_info && this.client_received_ack)
                    {
                        if (this.listviewtimer < time)
                        {
                            this.listviewtimer = time;
                            this.dcListView1.UpdateItems();
                        }

                        for (int i = 0; i < this.file_send.Length; i++)
                        {
                            if (this.file_send[i] != null)
                            {
                                if (this.file_send[i].Authorised)
                                {
                                    byte[] send_data = this.file_send[i].current_chunk;

                                    if (send_data == null)
                                    {
                                        this.dcListView1.AmendState(this.file_send[i].referal, "complete");
                                        this.dcOutBox1.Announce("\x000314Completed sending: " + this.file_send[i].displayname);
                                        this.file_send[i].Dispose();
                                        this.file_send[i] = null;
                                    }
                                    else
                                    {
                                        int send_amount = send_data.Length;

                                        try
                                        {
                                            this.socket.Send(Packets.DCFileChunk(this.file_send[i].referal, send_data));
                                            this.file_send[i].NextChunk();
                                            this.dcListView1.AmendSoFar(this.file_send[i].referal, (ulong)send_amount);
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }

                        if ((this.tick + 60) < time) // ping now!
                        {
                            this.tick = time;
                            this.bytes_out.Enqueue(Packets.DCPing());
                        }

                        while (this.bytes_in.Count > 3)
                        {
                            buf1 = this.bytes_in.ToArray();
                            ushort packet_len = BitConverter.ToUInt16(buf1, 1);
                            byte packet_id = buf1[3];

                            if (buf1.Length >= (packet_len + 4))
                            {
                                byte[] buf2 = new byte[packet_len];
                                Array.Copy(buf1, 4, buf2, 0, packet_len);
                                this.bytes_in.RemoveRange(0, (packet_len + 4));

                                try
                                {
                                    this.ProcessPacket(packet_id, new AresDataPacket(buf2));
                                }
                                catch { }
                            }
                            else { break; }
                        }
                    }
                }
            }
        }
        #endregion

        private void StartAvatarTransfer()
        {
            foreach (byte[] av_packet in Avatar.avatar_dc)
                this.bytes_out.Enqueue(Packets.DCAvatarChunkPacket(av_packet));

            this.bytes_out.Enqueue(Packets.DCAvatarEndPacket());

            this.dcAvatar2.UpdateImage(Avatar.avatar_big);
        }

        private delegate void SetWindowTextDelegate();

        private void SetWindowText()
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new SetWindowTextDelegate(this.SetWindowText));
            else
                this.Text = this.name + " - " + this.Text + " [dcref #" + this.ident + "]";
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.session_up = false;

            try
            {
                this.socket.Disconnect(false);
            }
            catch { }

            foreach (DCSendFileObject s in this.file_send)
                if (s != null)
                    s.Dispose();

            foreach (DCReceiveFileObject r in this.file_receive)
                if (r != null)
                    r.Dispose(true);

            this.MakeSessionEnd(this.ident);
        }

        private void CheckFocus()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new SetWindowTextDelegate(this.CheckFocus));
            }
            else
            {
                bool flash = true;

                if (Form.ActiveForm != null)
                    if (Form.ActiveForm.Text == this.Text)
                        flash = false;

                if (flash)
                {
                    FlashWindow(this.Handle, true);
                }
            }
        }

        private void ProcessPacket(byte id, AresDataPacket packet)
        {
            ulong filesize;
            ushort file_reference;
            String filename;
            int i;

            switch (id)
            {
                case 1: // message
                    this.dcOutBox1.Message(this.name, packet.ReadString());
                    this.CheckFocus();
                    break;

                case 2: // inbound file asking for permission
                    filesize = packet.ReadInt64();
                    file_reference = packet.ReadInt16();
                    packet.SkipBytes(4);
                    filename = packet.ReadString();

                    if (!this.accepting_files)
                    {
                        this.dcOutBox1.Announce("\x000314Inbound file was blocked: " + Helpers.ExtractDCFilename(filename));
                        this.bytes_out.Enqueue(Packets.DCDeclineFileRequest(filename));
                        return;
                    }
                    else
                    {
                        i = this.NextReceiveObject();

                        if (i > -1) // allow new file receive - set up
                        {
                            this.file_receive[i] = new DCReceiveFileObject(filename, file_reference, filesize);
                            this.file_receive[i].Prepare();
                            this.dcListView1.AddNewFileRec(this.file_receive[i]);
                            this.dcOutBox1.Announce("\x000314Inbound file receiving: " + this.file_receive[i].displayname);
                            this.bytes_out.Enqueue(Packets.DCAcceptFileRequest(filename));
                        }
                        else
                        {
                            this.dcOutBox1.Announce("\x000314Inbound file was blocked (too many files being received): " + Helpers.ExtractDCFilename(filename));
                            this.bytes_out.Enqueue(Packets.DCDeclineFileRequest(filename));
                            return;
                        }
                    }

                    break;

                case 3: // target accepting your file
                    packet.SkipBytes(8);
                    i = this.FindSendFileSessionByFilename(packet.ReadString());

                    if (i > -1)
                    {
                        this.file_send[i].NextChunk();
                        this.file_send[i].Authorised = true;
                        this.dcOutBox1.Announce("\x000314Permission granted to send: " + this.file_send[i].displayname);
                        this.dcListView1.AmendState(this.file_send[i].referal, "sending");
                    }

                    break;

                case 4: // target rejecting your file
                    i = this.FindSendFileSessionByFilename(packet.ReadString());

                    if (i > -1)
                    {
                        this.dcOutBox1.Announce("\x000314Permission blocked to send: " + this.file_send[i].displayname);
                        this.dcListView1.AmendState(this.file_send[i].referal, "cancelled");
                        this.file_send[i].Dispose();
                        this.file_send[i] = null;
                    }

                    break;

                case 5: // received a chunk
                    i = this.FindReceiveFileSessionByRef(packet.ReadInt16());

                    if (i > -1)
                    {
                        byte[] _rec_chunk = packet.ReadBytes();
                        this.file_receive[i].AddChunk(_rec_chunk);
                        this.dcListView1.AmendSoFarRec(this.file_receive[i].referal, (ulong)_rec_chunk.Length);

                        if (this.file_receive[i].UploadComplete())
                        {
                            this.dcListView1.AmendStateRec(this.file_receive[i].referal, "complete");
                            this.dcOutBox1.Announce("\x000314Finished receiving: " + this.file_receive[i].displayname);
                            this.file_receive[i].Dispose();
                            this.file_receive[i] = null;
                        }
                    }
                        
                    break;

                case 6: // avatar
                    if (this.got_avatar)
                        return;

                    if (packet.Remaining() == 0) // end of avatar
                    {
                        this.dcAvatar1.UpdateImage(this.avatar_bytes.ToArray());
                        this.got_avatar = true;
                        return;
                    }

                    this.avatar_bytes.AddRange(packet.ReadBytes());
                    break;

                case 10: // ping
                    this.bytes_out.Enqueue(Packets.DCPong());
                    break;
            }
        }

        private int FindSendFileSessionByFilename(String filename)
        {
            for (int i = 0; i < this.file_send.Length; i++)
                if (this.file_send[i] != null)
                    if (this.file_send[i].filename == filename)
                        return i;

            return -1;
        }

        private int FindReceiveFileSessionByRef(ushort referal)
        {
            for (int i = 0; i < this.file_receive.Length; i++)
                if (this.file_receive != null)
                    if (this.file_receive[i].referal == referal)
                        return i;

            return -1;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e) // auto accept files inbound
        {
            this.accepting_files = this.checkBox1.Checked;
        }

        private void toolStripButton7_Click(object sender, EventArgs e) // send file
        {
            if (!this.session_up ||
                !this.client_received_ack ||
                !this.client_received_info)
                return;

            OpenFileDialog file = new OpenFileDialog();
            file.Multiselect = false;

            if (file.ShowDialog() == DialogResult.OK)
            {
                int i = this.NextSendObject();
                
                if (i > -1)
                {
                    this.file_send[i] = new DCSendFileObject(file.FileName);
                    this.file_send[i].Prepare();
                    this.file_send[i].referal = this.file_send_ref++;
                    this.dcListView1.AddNewFile(this.file_send[i], file.FileName);
                    this.bytes_out.Enqueue(Packets.DCSendFileRequest(this.file_send[i].filesize,
                        this.file_send[i].referal, Helpers.UnixTime(), file.FileName));
                }
            }
        }

        private int NextSendObject()
        {
            for (int i = 0; i < this.file_send.Length; i++)
                if (this.file_send[i] == null)
                    return i;

            return -1;
        }

        private int NextReceiveObject()
        {
            for (int i = 0; i < this.file_receive.Length; i++)
                if (this.file_receive[i] == null)
                    return i;

            return -1;
        }

        private void Form2_Move(object sender, EventArgs e)
        {
            this.dcAvatar1.Invalidate();
            this.dcAvatar2.Invalidate();
        }

    }
}