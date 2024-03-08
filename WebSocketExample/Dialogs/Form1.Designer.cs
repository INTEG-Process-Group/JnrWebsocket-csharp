namespace WebSocketExample
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeOutput1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readClockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setClockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.readRegistryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.getSerialNumberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secureConnectToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(0, 24);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(1291, 446);
            this.textBox1.TabIndex = 1;
            this.textBox1.WordWrap = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionMenuItem,
            this.loginToolStripMenuItem,
            this.closeOutput1ToolStripMenuItem,
            this.readClockToolStripMenuItem,
            this.setClockToolStripMenuItem,
            this.rebootToolStripMenuItem,
            this.readRegistryToolStripMenuItem,
            this.openConsoleToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1291, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Enabled = false;
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.loginToolStripMenuItem_Click);
            // 
            // closeOutput1ToolStripMenuItem
            // 
            this.closeOutput1ToolStripMenuItem.Name = "closeOutput1ToolStripMenuItem";
            this.closeOutput1ToolStripMenuItem.Size = new System.Drawing.Size(104, 20);
            this.closeOutput1ToolStripMenuItem.Text = "Toggle Output 1";
            this.closeOutput1ToolStripMenuItem.Click += new System.EventHandler(this.toggleOutput1ToolStripMenuItem_Click);
            // 
            // readClockToolStripMenuItem
            // 
            this.readClockToolStripMenuItem.Name = "readClockToolStripMenuItem";
            this.readClockToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.readClockToolStripMenuItem.Text = "Read Clock";
            this.readClockToolStripMenuItem.Click += new System.EventHandler(this.readClockToolStripMenuItem_Click);
            // 
            // setClockToolStripMenuItem
            // 
            this.setClockToolStripMenuItem.Name = "setClockToolStripMenuItem";
            this.setClockToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.setClockToolStripMenuItem.Text = "Set Clock";
            this.setClockToolStripMenuItem.Click += new System.EventHandler(this.setClockToolStripMenuItem_Click);
            // 
            // rebootToolStripMenuItem
            // 
            this.rebootToolStripMenuItem.Name = "rebootToolStripMenuItem";
            this.rebootToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.rebootToolStripMenuItem.Text = "Reboot";
            this.rebootToolStripMenuItem.Click += new System.EventHandler(this.rebootToolStripMenuItem_Click);
            // 
            // openConsoleToolStripMenuItem
            // 
            this.openConsoleToolStripMenuItem.Enabled = false;
            this.openConsoleToolStripMenuItem.Name = "openConsoleToolStripMenuItem";
            this.openConsoleToolStripMenuItem.Size = new System.Drawing.Size(94, 20);
            this.openConsoleToolStripMenuItem.Text = "Open Console";
            this.openConsoleToolStripMenuItem.Click += new System.EventHandler(this.openConsoleToolStripMenuItem_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // readRegistryToolStripMenuItem
            // 
            this.readRegistryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.getSerialNumberToolStripMenuItem});
            this.readRegistryToolStripMenuItem.Name = "readRegistryToolStripMenuItem";
            this.readRegistryToolStripMenuItem.Size = new System.Drawing.Size(90, 20);
            this.readRegistryToolStripMenuItem.Text = "Read Registry";
            // 
            // getSerialNumberToolStripMenuItem
            // 
            this.getSerialNumberToolStripMenuItem.Name = "getSerialNumberToolStripMenuItem";
            this.getSerialNumberToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.getSerialNumberToolStripMenuItem.Text = "GetSerialNumber";
            this.getSerialNumberToolStripMenuItem.Click += new System.EventHandler(this.getSerialNumberToolStripMenuItem_Click);
            // 
            // connectionMenuItem
            // 
            this.connectionMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.secureConnectToolStripMenuItem1,
            this.disconnectToolStripMenuItem});
            this.connectionMenuItem.Name = "connectionMenuItem";
            this.connectionMenuItem.Size = new System.Drawing.Size(81, 20);
            this.connectionMenuItem.Text = "Connection";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // secureConnectToolStripMenuItem1
            // 
            this.secureConnectToolStripMenuItem1.Name = "secureConnectToolStripMenuItem1";
            this.secureConnectToolStripMenuItem1.Size = new System.Drawing.Size(180, 22);
            this.secureConnectToolStripMenuItem1.Text = "Secure Connect";
            this.secureConnectToolStripMenuItem1.Click += new System.EventHandler(this.secureConnectToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Enabled = false;
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1291, 470);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "WebSocket Example";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem closeOutput1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readClockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setClockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rebootToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openConsoleToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem readRegistryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem getSerialNumberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secureConnectToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
    }
}

