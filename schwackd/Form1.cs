using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Entities;
using schwackd.Helpers;

namespace schwackd
{
    public partial class Form1 : Form
    {
        private Entities.User ThisUser { get; set; }

        public Form1()
        {
            InitializeComponent();
        }




        private void Form1_Load(object sender, EventArgs e)
        {
            var users = new SchwackHelper().GetUsers();
            PopulateUsers(users);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SignOut();

        }

        private void btnSignIn_Click(object sender, EventArgs e)
        {
            SignIn();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            Send();
        }

        private void btnGetMessages_Click(object sender, EventArgs e)
        {
            GetMessages();
        }

        private void txtWho_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SignIn();
            }
        }




        private void PopulateUsers(List<User> users)
        {
            lstUsers.DataSource = users;
            lstUsers.ValueMember = "Name";
        }

        private void SignIn()
        {
            var who = txtWho.Text;

            if (string.IsNullOrEmpty(who.Trim()))
            {
                MessageBox.Show("Who are you?", "Identify Yourself!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var helper = new SchwackHelper();
            ThisUser = helper.SignIn(who);
            var onlineUsers = helper.GetUsers();
            PopulateUsers(onlineUsers);
            btnSignIn.Visible = false;
            txtWho.Enabled = false;
        }

        private void SignOut()
        {
            new SchwackHelper().SignOut(ThisUser.Id.ToString());
        }

        private void Send()
        {
            var message = txtMessage.Text;
            var to = ((User)lstUsers.SelectedItem).Id;
            var from = ThisUser.Id;
            new SchwackHelper().PostMessage(from.ToString(), to.ToString(), message);

            txtMessages.Text = string.Format("{0} {1}:  {2}\r\n {3}\r\n",DateTime.Now.ToString(), ThisUser.Name, txtMessage.Text, txtMessages.Text);
            txtMessage.Text = "";
        }

        private void GetMessages()
        {
            new SchwackHelper().GetMessages(ThisUser.Id.ToString());
        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                Send();
            }
        }




        #region Flashing Stuff

        // To support flashing.
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        //Flash both the window caption and taskbar button.
        //This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags. 
        public const UInt32 FLASHW_ALL = 3;

        // Flash continuously until the window comes to the foreground. 
        public const UInt32 FLASHW_TIMERNOFG = 12;

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            public UInt32 cbSize;
            public IntPtr hwnd;
            public UInt32 dwFlags;
            public UInt32 uCount;
            public UInt32 dwTimeout;
        }

        // Do the flashing - this does not involve a raincoat.
        public static bool FlashWindowEx(Form form)
        {
            IntPtr hWnd = form.Handle;
            FLASHWINFO fInfo = new FLASHWINFO();

            fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hWnd;
            fInfo.dwFlags = FLASHW_ALL | FLASHW_TIMERNOFG;
            fInfo.uCount = UInt32.MaxValue;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }



        #endregion


    }
}
