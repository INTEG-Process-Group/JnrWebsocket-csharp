namespace MultipleJniors
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.addJNIORToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tvwJniors = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pulseROUT1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readJniorsyslogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.menuStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addJNIORToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1315, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // addJNIORToolStripMenuItem
            // 
            this.addJNIORToolStripMenuItem.Name = "addJNIORToolStripMenuItem";
            this.addJNIORToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.addJNIORToolStripMenuItem.Text = "Add JNIOR";
            this.addJNIORToolStripMenuItem.Click += new System.EventHandler(this.addJNIORToolStripMenuItem_Click);
            // 
            // tvwJniors
            // 
            this.tvwJniors.ContextMenuStrip = this.contextMenuStrip1;
            this.tvwJniors.Dock = System.Windows.Forms.DockStyle.Left;
            this.tvwJniors.Location = new System.Drawing.Point(0, 24);
            this.tvwJniors.Name = "tvwJniors";
            this.tvwJniors.Size = new System.Drawing.Size(256, 589);
            this.tvwJniors.TabIndex = 1;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.removeToolStripMenuItem,
            this.pulseROUT1ToolStripMenuItem,
            this.readJniorsyslogToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(164, 114);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // pulseROUT1ToolStripMenuItem
            // 
            this.pulseROUT1ToolStripMenuItem.Name = "pulseROUT1ToolStripMenuItem";
            this.pulseROUT1ToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.pulseROUT1ToolStripMenuItem.Text = "Pulse ROUT 1";
            this.pulseROUT1ToolStripMenuItem.Click += new System.EventHandler(this.pulseROUT1ToolStripMenuItem_Click);
            // 
            // readJniorsyslogToolStripMenuItem
            // 
            this.readJniorsyslogToolStripMenuItem.Name = "readJniorsyslogToolStripMenuItem";
            this.readJniorsyslogToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.readJniorsyslogToolStripMenuItem.Text = "Read jniorsys.log";
            this.readJniorsyslogToolStripMenuItem.Click += new System.EventHandler(this.readJniorsyslogToolStripMenuItem_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(256, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1059, 589);
            this.tabControl1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1315, 613);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.tvwJniors);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addJNIORToolStripMenuItem;
        private System.Windows.Forms.TreeView tvwJniors;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pulseROUT1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readJniorsyslogToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
    }
}

