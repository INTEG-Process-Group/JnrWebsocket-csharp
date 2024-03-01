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
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.secureConnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeOutput1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readClockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setClockToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebootToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.publishProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iPLookupToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.repeatedSnapshotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openConsoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.testFlashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.connectToolStripMenuItem,
            this.secureConnectToolStripMenuItem,
            this.loginToolStripMenuItem,
            this.closeOutput1ToolStripMenuItem,
            this.readClockToolStripMenuItem,
            this.setClockToolStripMenuItem,
            this.rebootToolStripMenuItem,
            this.readFileToolStripMenuItem,
            this.toolStripMenuItem2,
            this.publishProjectToolStripMenuItem,
            this.cancelUpdateToolStripMenuItem,
            this.iPLookupToolStripMenuItem1,
            this.testToolStripMenuItem,
            this.openConsoleToolStripMenuItem,
            this.testFlashToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1291, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // secureConnectToolStripMenuItem
            // 
            this.secureConnectToolStripMenuItem.Name = "secureConnectToolStripMenuItem";
            this.secureConnectToolStripMenuItem.Size = new System.Drawing.Size(102, 20);
            this.secureConnectToolStripMenuItem.Text = "Secure Connect";
            this.secureConnectToolStripMenuItem.Click += new System.EventHandler(this.secureConnectToolStripMenuItem_Click);
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.loginToolStripMenuItem_Click);
            // 
            // closeOutput1ToolStripMenuItem
            // 
            this.closeOutput1ToolStripMenuItem.Name = "closeOutput1ToolStripMenuItem";
            this.closeOutput1ToolStripMenuItem.Size = new System.Drawing.Size(106, 20);
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
            // readFileToolStripMenuItem
            // 
            this.readFileToolStripMenuItem.Name = "readFileToolStripMenuItem";
            this.readFileToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.readFileToolStripMenuItem.Text = "Snapshot";
            this.readFileToolStripMenuItem.Click += new System.EventHandler(this.readFileToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.editToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(97, 20);
            this.toolStripMenuItem2.Text = "Update Project";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.ProjectOpenToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.ProjectEditToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.ProjectCloseToolStripMenuItem_Click);
            // 
            // publishProjectToolStripMenuItem
            // 
            this.publishProjectToolStripMenuItem.Name = "publishProjectToolStripMenuItem";
            this.publishProjectToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.publishProjectToolStripMenuItem.Text = "Publish Project";
            this.publishProjectToolStripMenuItem.Click += new System.EventHandler(this.publishProjectToolStripMenuItem_Click);
            // 
            // cancelUpdateToolStripMenuItem
            // 
            this.cancelUpdateToolStripMenuItem.Name = "cancelUpdateToolStripMenuItem";
            this.cancelUpdateToolStripMenuItem.Size = new System.Drawing.Size(96, 20);
            this.cancelUpdateToolStripMenuItem.Text = "Cancel Update";
            this.cancelUpdateToolStripMenuItem.Click += new System.EventHandler(this.cancelUpdateToolStripMenuItem_Click);
            // 
            // iPLookupToolStripMenuItem1
            // 
            this.iPLookupToolStripMenuItem1.Name = "iPLookupToolStripMenuItem1";
            this.iPLookupToolStripMenuItem1.Size = new System.Drawing.Size(72, 20);
            this.iPLookupToolStripMenuItem1.Text = "IP Lookup";
            this.iPLookupToolStripMenuItem1.Click += new System.EventHandler(this.iPLookupToolStripMenuItem1_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.repeatedSnapshotToolStripMenuItem});
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.testToolStripMenuItem.Text = "Test";
            // 
            // repeatedSnapshotToolStripMenuItem
            // 
            this.repeatedSnapshotToolStripMenuItem.Name = "repeatedSnapshotToolStripMenuItem";
            this.repeatedSnapshotToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.repeatedSnapshotToolStripMenuItem.Text = "Repeated Snapshot";
            this.repeatedSnapshotToolStripMenuItem.Click += new System.EventHandler(this.repeatedSnapshotToolStripMenuItem_Click);
            // 
            // openConsoleToolStripMenuItem
            // 
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
            // testFlashToolStripMenuItem
            // 
            this.testFlashToolStripMenuItem.Name = "testFlashToolStripMenuItem";
            this.testFlashToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.testFlashToolStripMenuItem.Text = "Test Flash";
            this.testFlashToolStripMenuItem.Click += new System.EventHandler(this.testFlashToolStripMenuItem_Click);
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
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readClockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setClockToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem secureConnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rebootToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem publishProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelUpdateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iPLookupToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem repeatedSnapshotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openConsoleToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem testFlashToolStripMenuItem;
    }
}

