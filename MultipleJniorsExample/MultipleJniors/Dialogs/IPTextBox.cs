using System;
using System.Text;
using System.Windows.Forms;

public partial class IPTextBox : UserControl
{
    private TextBox m_controlJustReceivedFocus = null;

    public IPTextBox()
    {
        InitializeComponent();
    }

    public event EventHandler IpAddressChanged;

    public string IPAddress
    {
        get
        {
            return txtOctet1.Text + "." + txtOctet2.Text + "." + txtOctet3.Text + "." + txtOctet4.Text;
        }
        set
        {
            string[] octets = value.Split(new char[] { '.' });
            if (octets.Length == 4)
            {
                txtOctet1.Text = octets[0];
                txtOctet2.Text = octets[1];
                txtOctet3.Text = octets[2];
                txtOctet4.Text = octets[3];
            }
        }
    }

    protected override void OnResize(EventArgs e)
    {
        //base.OnResize(e);

        //if (!Name.Equals("iptextbox", StringComparison.CurrentCultureIgnoreCase)) AppLog.Write("IPTextBox::OnResize() for control: " + Name);

        IPTextBox_Resize(this, e);
    }



    public new void Focus()
    {
        txtOctet1.Focus();
    }



    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (m_controlJustReceivedFocus != null)
        {
            if (keyData == Keys.Right && !string.IsNullOrEmpty(m_controlJustReceivedFocus.Text) && m_controlJustReceivedFocus.SelectionStart >= m_controlJustReceivedFocus.Text.Length)
            {
                if (m_controlJustReceivedFocus == txtOctet1)
                {
                    txtOctet2.SelectAll();
                    txtOctet2.Focus();
                }
                if (m_controlJustReceivedFocus == txtOctet2)
                {
                    txtOctet3.SelectAll();
                    txtOctet3.Focus();
                }
                if (m_controlJustReceivedFocus == txtOctet3)
                {
                    txtOctet4.SelectAll();
                    txtOctet4.Focus();
                }
            } // left 
            else if (keyData == Keys.Left && m_controlJustReceivedFocus.SelectionStart == 0)
            {
                if (m_controlJustReceivedFocus == txtOctet2)
                {
                    txtOctet1.SelectAll();
                    txtOctet1.Focus();
                }
                if (m_controlJustReceivedFocus == txtOctet3)
                {
                    txtOctet2.SelectAll();
                    txtOctet2.Focus();
                }
                if (m_controlJustReceivedFocus == txtOctet4)
                {
                    txtOctet3.SelectAll();
                    txtOctet3.Focus();
                }
            } // right
        } // m_controlJustReceivedFocus not null

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void IPTextBox_Resize(object sender, EventArgs e)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("IPTextBox::Resize() for control: " + Name);
        sb.AppendLine(Name + ".Width=" + this.DisplayRectangle.Width);
        sb.AppendLine(Name + ".label1.Width=" + label1.Width);

        /** when resizing the text box we need to determine how wide each octet 
         * text box can be and where the dots should be */
        int txtWidth = (int)((this.DisplayRectangle.Width - (label1.Width * 3)) / 4.0);
        int txtTop = (int)((this.DisplayRectangle.Height - txtOctet1.Height) / 2.0);
        sb.AppendLine(Name + ".txtWidth=" + txtWidth + " = (int)((this.DisplayRectangle.Width - (label1.Width * 3)) / 4.0)");

        txtOctet1.Left = 1;
        txtOctet1.Top = txtTop;
        txtOctet1.Width = txtWidth;
        sb.AppendLine(Name + ".textOctet1.Left=" + txtOctet1.Left);

        txtOctet2.Left = txtWidth + label1.Width;
        txtOctet2.Width = txtWidth;
        txtOctet2.Top = txtTop;
        sb.AppendLine(Name + ".textOctet2.Left=" + txtOctet2.Left);

        txtOctet3.Left = (txtWidth + label1.Width) * 2;
        txtOctet3.Width = txtWidth;
        txtOctet3.Top = txtTop;
        sb.AppendLine(Name + ".textOctet3.Left=" + txtOctet3.Left);

        txtOctet4.Left = (txtWidth + label1.Width) * 3;
        txtOctet4.Width = txtWidth;
        txtOctet4.Top = txtTop;
        sb.AppendLine(Name + ".textOctet4.Left=" + txtOctet4.Left);

        label1.Top = txtTop;
        label2.Top = txtTop;
        label3.Top = txtTop;

        label1.Left = txtWidth;
        sb.AppendLine(Name + ".label1.Left=" + label1.Left);
        label2.Left = ((txtWidth + label1.Width) * 2) - label1.Width;
        sb.AppendLine(Name + ".label2.Left=" + label2.Left);
        label3.Left = ((txtWidth + label1.Width) * 3) - label1.Width;
        sb.AppendLine(Name + ".label3.Left=" + label3.Left);

        // write to the log
        //if (!Name.Equals("iptextbox", StringComparison.CurrentCultureIgnoreCase)) AppLog.Write(sb.ToString());
    }

    private void txtOctet_KeyPress(object sender, KeyPressEventArgs e)
    {
        TextBox txtOctet = (TextBox)sender;

        // make sure the typed character is valid
        if (e.KeyChar == '.') // . advance to next octet
        {
            if (txtOctet == txtOctet1)
            {
                txtOctet2.SelectAll();
                txtOctet2.Focus();
            }
            if (txtOctet == txtOctet2)
            {
                txtOctet3.SelectAll();
                txtOctet3.Focus();
            }
            if (txtOctet == txtOctet3)
            {
                txtOctet4.SelectAll();
                txtOctet4.Focus();
            }
            e.Handled = true;
            return;
        }
        else if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != '\b')
        {
            e.Handled = true;
            return;
        }

        int selectionStart = txtOctet.SelectionStart;
        string c = e.KeyChar == '\b' ? "" : e.KeyChar.ToString();
        string newValue;
        // is the selected text empty?
        if (txtOctet.SelectedText == null || txtOctet.SelectedText.Equals(string.Empty))
        {
            if (e.KeyChar == '\b')
            {
                if (selectionStart > 0)
                {
                    selectionStart--;
                    newValue = txtOctet.Text.Substring(0, selectionStart) + c + txtOctet.Text.Substring(selectionStart + 1);
                }
                else
                {
                    if (txtOctet == txtOctet2)
                    {
                        txtOctet1.SelectionStart = txtOctet1.Text.Length;
                        txtOctet_KeyPress(txtOctet1, e);
                        txtOctet1.Focus();
                    }
                    if (txtOctet == txtOctet3)
                    {
                        txtOctet2.SelectionStart = txtOctet1.Text.Length;
                        txtOctet_KeyPress(txtOctet2, e);
                        txtOctet2.Focus();
                    }
                    if (txtOctet == txtOctet4)
                    {
                        txtOctet3.SelectionStart = txtOctet1.Text.Length;
                        txtOctet_KeyPress(txtOctet3, e);
                        txtOctet3.Focus();
                    }

                }
            } // backspace pressed
            newValue = txtOctet.Text.Substring(0, selectionStart) + c + txtOctet.Text.Substring(txtOctet.SelectionStart);
        }
        // replace the selected text
        else
        {
            newValue = txtOctet.Text.Replace(txtOctet.SelectedText, c);
        }
        selectionStart += c.Length;

        // strip off any leading zeros
        while (newValue.StartsWith("0") && newValue.Length > 1)
        {
            newValue = newValue.Substring(1);
            selectionStart--;
        }


        int octetValue = 0;
        int.TryParse(newValue, out octetValue);
        if (octetValue >= 0 && octetValue <= 255)
        {
            txtOctet.Text = newValue;

            // validate the selection start
            if (selectionStart >= 0 && selectionStart <= newValue.Length)
            {
                txtOctet.SelectionStart = selectionStart;
            }
        }
        else
        {
        }

        // alert any listeners that the ip address has changed
        IpAddressChanged?.Invoke(this, EventArgs.Empty);

        e.Handled = true;
    }

    private void txtOctet_Leave(object sender, EventArgs e)
    {
        TextBox txtOctet = (TextBox)sender;

        if (txtOctet.Text.Equals(string.Empty))
        {
            txtOctet.Text = "0";
        }
    }

    private void txtOctet_Enter(object sender, EventArgs e)
    {
        TextBox txtOctet = (TextBox)sender;
        m_controlJustReceivedFocus = txtOctet;
    }

    private void txtOctet_MouseUp(object sender, MouseEventArgs e)
    {
        TextBox txtOctet = (TextBox)sender;
        if (txtOctet == m_controlJustReceivedFocus)
        {
            txtOctet.SelectAll();
        }
        m_controlJustReceivedFocus = null;
    }

}