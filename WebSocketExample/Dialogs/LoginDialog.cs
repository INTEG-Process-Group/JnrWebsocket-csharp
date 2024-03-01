using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;



namespace WebSocketExample
{
    public partial class LoginDialog : Form
    {
        // here we will store our network credentials
        private static Dictionary<string, NetworkCredential> RememberedCredentials
            = new Dictionary<string, NetworkCredential>();


        private string m_destination;



        public LoginDialog(string dest)
        {
            InitializeComponent();

            m_destination = dest;

            // see if there is a remembered credential for this destination.  
            //  if so we need to display it.
            if (RememberedCredentials.ContainsKey(m_destination))
            {
                NetworkCredential creds = RememberedCredentials[m_destination];

                UserName = creds.UserName;

                string encrypted = creds.Password;
                byte[] ciphertext = StringToByteArray(encrypted);

                byte[] plaintext2 = ProtectedData.Unprotect(ciphertext, null, DataProtectionScope.CurrentUser);
                Password = Encoding.ASCII.GetString(plaintext2);

                chbRemember.Enabled = true;
                chbRemember.Checked = true;
            }
        }



        private string m_userName = "";
        public string UserName
        {
            get { return m_userName; }
            set
            {
                if (value == null || value.Equals(string.Empty))
                {
                    txtUserName.Text = "jnior";
                }
                else
                {
                    m_userName = value;
                    txtUserName.Text = value;
                }
            }
        }



        private string m_password = "";
        public string Password
        {
            get { return m_password; }
            set
            {
                if (value == null || value.Equals(string.Empty))
                {
                    txtPassword.Text = "jnior";
                }
                else
                {
                    m_password = value;
                    txtPassword.Text = value;
                }
            }
        }



        private void btnOK_Click(object sender, EventArgs e)
        {
            UserName = txtUserName.Text;
            Password = txtPassword.Text;


            // if the user wants to remember the credentials then we should 
            //  store them.  we must store them securely.
            if (chbRemember.Checked)
            {
                // Data to protect. Convert a string to a byte[] using Encoding.UTF8.GetBytes().
                byte[] plaintext = Encoding.ASCII.GetBytes(Password);


                byte[] ciphertext = ProtectedData.Protect(plaintext, null, DataProtectionScope.CurrentUser);
                string encrypted = ByteArrayToString(ciphertext);
                Console.WriteLine(encrypted);

                NetworkCredential creds = new NetworkCredential(UserName, encrypted);

                RememberedCredentials[m_destination] = creds;
            }
            // if there was a remembered credential for this destination and 
            //  the remember box is no longer shecked we must remove it from 
            //  memory.
            else
            {
                if (RememberedCredentials.ContainsKey(m_destination))
                {
                    RememberedCredentials.Remove(m_destination);
                }
            }
        }



        private void LoginDialog_Load(object sender, EventArgs e)
        {
            txtDest.Text = m_destination;
        }



        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
            chbRemember.Enabled = true;
        }



        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            chbRemember.Enabled = true;
        }



        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }



        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }
    }
}